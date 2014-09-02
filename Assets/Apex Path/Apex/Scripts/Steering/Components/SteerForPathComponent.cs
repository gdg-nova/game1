/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering.Components
{
    using System;
    using System.Collections.Generic;
    using Apex.DataStructures;
    using Apex.LoadBalancing;
    using Apex.Messages;
    using Apex.PathFinding;
    using Apex.Services;
    using Apex.Utilities;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// A steering component that enables the unit to issue <see cref="IPathRequest"/>s and move along the resulting path.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Steering/Steer for Path")]
    public class SteerForPathComponent : SteeringComponent, ISteerable, INeedPath, IAdjustUpdateInterval
    {
        /// <summary>
        /// The priority with which this unit's path requests should be processed.
        /// </summary>
        public int pathingPriority = 0;

        /// <summary>
        /// Whether to use path smoothing.
        /// Path smoothing creates more natural routes at a small cost to performance.
        /// </summary>
        public bool usePathSmoothing = true;

        /// <summary>
        /// Controls whether to allow the path to cut corners. Corner cutting has slightly better performance, but produces less natural routes.
        /// </summary>
        public bool allowCornerCutting = false;

        /// <summary>
        /// Controls whether navigation off-grid is prohibited.
        /// </summary>
        public bool preventOffGridNavigation = false;

        /// <summary>
        /// Controls whether the unit is allowed to move to diagonal neighbours.
        /// </summary>
        public bool preventDiagonalMoves = false;

        /// <summary>
        /// Controls whether the unit will navigate to the nearest possible position if the actual destination is blocked or otherwise inaccessible.
        /// </summary>
        public bool navigateToNearestIfBlocked = false;

        /// <summary>
        /// The distance within which the unit will start to slow down for arrival
        /// </summary>
        [MinCheck(0f)]
        public float slowingDistance = 1.5f;

        /// <summary>
        /// The algorithm used to slow the unit for arrival.
        /// Linear works fine with short slowing distances, but logarithmic shows its worth at longer ones.
        /// </summary>
        public SlowingAlgorithm slowingAlgorithm = SlowingAlgorithm.Logarithmic;

        /// <summary>
        /// The distance from the current way point at which the next way point will be requested
        /// </summary>
        [MinCheck(0f)]
        public float requestNextWaypointDistance = 2.0f;

        /// <summary>
        /// The angle at which the end target must be approached before a proximity check is deemed valid. This applies to slowingDistance and requestNextWaypointDistance
        /// </summary>
        [Range(0.0f, 90.0f)]
        public int proximityEvaluationMinAngle = 10;

        /// <summary>
        /// Gets the maximum escape cell distance if origin blocked.
        /// This means that when starting a path and the origin (from position) is blocked, this determines how far away the pather will look for a free cell to escape to, before resuming the planned path.
        /// </summary>
        [MinCheck(0)]
        public int maxEscapeCellDistanceIfOriginBlocked = 3;

        /// <summary>
        /// The distance from the current destination node on the path at which the unit will switch to the next node.
        /// </summary>
        [MinCheck(0.1f)]
        public float nextNodeDistance = 1f;

        /// <summary>
        /// The distance from the final destination where the unit will stop
        /// Any value greater than half the cell size of the grid is treated as half a step size.
        /// </summary>
        [MinCheck(0.1f)]
        public float arrivalDistance = 0.2f;

        /// <summary>
        /// Controls whether a <see cref="Apex.Messages.UnitNavigationEventMessage"/> is raised each time a node is reached.
        /// </summary>
        public bool announceAllNodes = false;

        /// <summary>
        /// The replan mode
        /// </summary>
        public ReplanMode replanMode = ReplanMode.Dynamic;

        /// <summary>
        /// The replan interval
        /// When <see cref="replanMode"/> is <see cref="ReplanMode.AtInterval"/> the replan interval is the fixed interval in seconds between replanning.
        /// When <see cref="replanMode"/> is <see cref="ReplanMode.Dynamic"/> the replan interval is the minimum required time between each replan. 
        /// </summary>
        [MinCheck(0.1f)]
        public float replanInterval = 0.5f;

        private object _syncLock = new object();
        private IPathRequest _pendingPathRequest;
        private PathResult _pendingResult;
        private StackWithLookAhead<IPositioned> _currentPath;
        private ManualPath _manualPath;
        private IPositioned _currentDestination;
        private float _currentDestinationDistance;
        private IGrid _currentGrid;
        private Vector3 _endOfResolvedPath;
        private Vector3 _endOfPath;

        private bool _stop;
        private bool _wait;
        private bool _blockMoveOrders;
        private bool _stopped;
        private bool _onFinalApproach;
        private bool _isPortaling;
        private float _lastPathRequestTime;
        private float _slowingDistanceSquared;
        private float _requestNextWaypointDistanceSquared;
        private UnitNavigationEventMessage _navMessage;
        private IProcessPathResults[] _resultProcessors;

        private Queue<Vector3> _wayPoints;
        private Queue<Vector3> _pathboundWayPoints;
        private float _proximityAngleCos;

        /// <summary>
        /// The algorithms for slowing movement to a halt
        /// </summary>
        public enum SlowingAlgorithm
        {
            /// <summary>
            /// Linear slow down
            /// </summary>
            Linear,

            /// <summary>
            /// Logarithmic slow down
            /// </summary>
            Logarithmic
        }

        /// <summary>
        /// Controls the way replanning is done
        /// </summary>
        public enum ReplanMode
        {
            /// <summary>
            /// Replanning is done a a set interval
            /// </summary>
            AtInterval,

            /// <summary>
            /// Replanning is done when changes occur in the units immediate surroundings. Immediate surroundings are defined by the grid's <see cref="IGrid.gridSections"/>
            /// </summary>
            Dynamic,

            /// <summary>
            /// No replanning
            /// </summary>
            NoReplan
        }

        /// <summary>
        /// Gets the current path.
        /// </summary>
        /// <value>
        /// The current path.
        /// </value>
        public IEnumerable<IPositioned> currentPath
        {
            get { return _currentPath; }
        }

        /// <summary>
        /// Gets the current way points.
        /// </summary>
        /// <value>
        /// The current way points.
        /// </value>
        public IEnumerable<Vector3> currentWaypoints
        {
            get { return _wayPoints; }
        }

        /// <summary>
        /// Gets the current destination.
        /// </summary>
        /// <value>
        /// The current destination.
        /// </value>
        public IPositioned currentDestination
        {
            get { return _currentDestination; }
        }

        /// <summary>
        /// Called on Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            this.WarnIfMultipleInstances();

            if (this.arrivalDistance > this.nextNodeDistance)
            {
                Debug.LogError("The Arrival Distance must be equal to or less that the Next Node Distance.");
            }

            _wayPoints = new Queue<Vector3>();
            _pathboundWayPoints = new Queue<Vector3>();
            _navMessage = new UnitNavigationEventMessage(this.gameObject);

            _resultProcessors = this.GetComponents<SteerForPathResultProcessorComponent>();
            Array.Sort(_resultProcessors, (a, b) => a.processingOrder.CompareTo(b.processingOrder));

            _proximityAngleCos = Mathf.Cos(this.proximityEvaluationMinAngle * Mathf.Deg2Rad);
            _slowingDistanceSquared = this.slowingDistance * this.slowingDistance;
            _requestNextWaypointDistanceSquared = this.requestNextWaypointDistance * this.requestNextWaypointDistance;
            _stopped = true;
        }

        /// <summary>
        /// Asks the object to move to the specified position
        /// </summary>
        /// <param name="position">The position to move to.</param>
        /// <param name="append">if set to <c>true</c> the destination is added as a way point.</param>
        public void MoveTo(Vector3 position, bool append)
        {
            if (_blockMoveOrders)
            {
                return;
            }

            _onFinalApproach = false;

            //If this is a way point and we are already moving, just queue it up
            if (append && !_stopped)
            {
                _wayPoints.Enqueue(position);
                return;
            }

            var from = _isPortaling ? _currentDestination.position : this.transformCached.position;

            //Either we don't have a request or this is the first point in a way point route
            StopInternal();

            RequestPath(from, position, RequestType.Normal);
        }

        /// <summary>
        /// Asks the object to move along the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void MoveAlong(ManualPath path)
        {
            Ensure.ArgumentNotNull(path, "path");

            StopInternal();

            _manualPath = path;
            SetManualPath(path);
        }

        /// <summary>
        /// Waits the specified seconds before continuing the move.
        /// </summary>
        /// <param name="seconds">The seconds to wait or null to wait until explicitly <see cref="Resume" />d.</param>
        public void Wait(float? seconds)
        {
            _wait = true;

            if (seconds.HasValue)
            {
                NavLoadBalancer.defaultBalancer.Add(new OneTimeAction((ignored) => this.Resume()), seconds.Value, true);
            }
        }

        /// <summary>
        /// Resumes movements after a <see cref="Wait" />.
        /// </summary>
        public void Resume()
        {
            _wait = false;
        }

        /// <summary>
        /// Enables the movement orders following a call to <see cref="DisableMovementOrders" />.
        /// </summary>
        public void EnableMovementOrders()
        {
            _blockMoveOrders = false;
        }

        /// <summary>
        /// Disables movement orders, i.e. calls to <see cref="MoveTo" /> will be ignored until <see cref="EnableMovementOrders" /> is called.
        /// </summary>
        public void DisableMovementOrders()
        {
            _blockMoveOrders = true;
        }

        /// <summary>
        /// Stop following the path.
        /// </summary>
        public override void Stop()
        {
            _wait = false;
            _stop = true;
        }

        /// <summary>
        /// Replans the path.
        /// </summary>
        public void ReplanPath()
        {
            if (_stopped || _pendingPathRequest != null)
            {
                return;
            }

            _currentPath = null;
            _pathboundWayPoints.Clear();

            if (_manualPath != null && _manualPath.onReplan != null)
            {
                _manualPath.onReplan(this.gameObject, _endOfPath, _manualPath);
                SetManualPath(_manualPath);
            }
            else
            {
                RequestPath(this.transformCached.position, _endOfPath, RequestType.Normal);
            }
        }

        //Note this may be called from another thread if the PathService is running asynchronously
        void INeedPath.ConsumePathResult(PathResult result)
        {
            lock (_syncLock)
            {
                //If we have stooped or get back the result of a request other than the one we currently expect, just toss it as it will be outdated.
                if (result.originalRequest != _pendingPathRequest)
                {
                    return;
                }

                _pendingResult = result;
                _pendingPathRequest = null;
            }
        }

        /// <summary>
        /// Gets the update interval to use for the next update.
        /// </summary>
        /// <param name="expectedVelocity">The expected velocity the unit will have between now and the next update if no adjustment is made.</param>
        /// <param name="unadjustedInterval">The unadjusted interval, i.e. the time which will pass before the next update if no adjustment is made.</param>
        /// <returns>
        /// The adjusted update interval. If no adjustment is needed, simply return <paramref name="unadjustedInterval" />.
        /// </returns>
        float IAdjustUpdateInterval.GetUpdateInterval(Vector3 expectedVelocity, float unadjustedInterval)
        {
            //In order to get a proper smooth slowdown when approaching the end destination, we need to update each fixed update.
            if (_onFinalApproach)
            {
                return 0.0f;
            }

            //If the expected distance that would be traveled until the next update is greater than what remains to reach the current destination, adjust the interval accordingly
            var remainingDistanceSquared = _currentDestinationDistance * _currentDestinationDistance;
            return GetAppropriateUpdateInterval(expectedVelocity, remainingDistanceSquared, unadjustedInterval);
        }

        /// <summary>
        /// Gets the movement vector.
        /// </summary>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <returns>
        /// The movement vector
        /// </returns>
        protected override Vector3 GetMovementVector(Vector3 currentVelocity)
        {
            if (_stopped || _wait)
            {
                return Vector3.zero;
            }

            if (_stop)
            {
                StopInternal();
                return Vector3.zero;
            }

            //If we have no path but a result is ready, get the path from the result
            if (_currentPath == null && _pendingResult != null)
            {
                ConsumeResult();
            }

            if (_currentDestination == null)
            {
                //If a request exists but we haven't yet gotten a route, do local pathing until we get one
                if ((_currentPath == null) || (_currentPath.count == 0))
                {
                    //need a local var here to ensure thread safety
                    var pending = _pendingPathRequest;
                    if (pending != null)
                    {
                        return SteerLocally(pending.to);
                    }

                    return Vector3.zero;
                }

                //Get the next destination from the path
                ResolveNextPoint();
            }

            //Get the direction
            var curDir = ResolveMoveDirection();

            HandleWaypointsAndArrival(curDir);

            if (_currentDestinationDistance < this.nextNodeDistance)
            {
                //We are inside the bounds of a route node, so transition to the next node or move towards arrival
                if (_currentPath != null && _currentPath.count > 0)
                {
                    if (this.announceAllNodes)
                    {
                        AnnounceEvent(UnitNavigationEventMessage.Event.NodeReached, _currentDestination.position, null);
                    }

                    ResolveNextPoint();

                    return GetMovementVector(currentVelocity);
                }
                else if (_currentDestinationDistance < this.arrivalDistance)
                {
                    if (_pendingResult != null)
                    {
                        //A pending route exists (i.e. next way point) so move on to that one right away
                        if (_pendingResult.originalRequest.type != RequestType.PathboundWaypoint)
                        {
                            AnnounceEvent(UnitNavigationEventMessage.Event.WaypointReached, _currentDestination.position, null);
                        }

                        _currentPath = null;
                        return GetMovementVector(currentVelocity);
                    }

                    if (_pendingPathRequest != null)
                    {
                        //We have reach the end of the current path but have a pending request, so we have to basically stand still and wait for it. Obviously this will only happen if requests take a long time to complete.
                        return Vector3.zero;
                    }

                    //We are within the arrival distance on the end node so stop
                    StopInternal();
                    AnnounceEvent(UnitNavigationEventMessage.Event.DestinationReached, this.transformCached.position, null);
                    return Vector3.zero;
                }
            }
            else
            {
                HandlePathReplan();
            }

            return curDir;
        }

        /// <summary>
        /// Gets the speed adjustment factor, used to increase or decrease the speed of the unit under certain circumstances.
        /// </summary>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <returns>
        /// The speed adjustment factor
        /// </returns>
        public override float GetSpeedAdjustmentFactor(Vector3 currentVelocity)
        {
            if (_onFinalApproach)
            {
                var dirEnd = (_endOfResolvedPath - this.transformCached.position).AdjustAxis(0.0f, this.excludedAxis);

                //We are inside the slowing distance and we are actually moving towards the end (e.g. not parallel to with a thin wall between us or similar)
                if (slowingAlgorithm == SlowingAlgorithm.Linear)
                {
                    return dirEnd.magnitude / this.slowingDistance;
                }
                else
                {
                    return Mathf.Log10(((9.0f / this.slowingDistance) * dirEnd.magnitude) + 1.0f);
                }
            }

            return 1.0f;
        }

        /// <summary>
        /// Requests the path.
        /// </summary>
        /// <param name="request">The request.</param>
        public void RequestPath(IPathRequest request)
        {
            lock (_syncLock)
            {
                request.requester = this;

                _pendingPathRequest = request;

                _stop = false;
                _stopped = false;
            }

            _lastPathRequestTime = Time.time;
            GameServices.pathService.QueueRequest(_pendingPathRequest, this.pathingPriority);
        }

        private void RequestPath(Vector3 from, Vector3 to, RequestType type)
        {
            lock (_syncLock)
            {
                _pendingPathRequest = new BasicPathRequest()
                {
                    from = from,
                    to = to,
                    type = type,
                    maxEscapeCellDistanceIfOriginBlocked = this.maxEscapeCellDistanceIfOriginBlocked,
                    usePathSmoothing = this.usePathSmoothing,
                    allowCornerCutting = this.allowCornerCutting,
                    preventOffGridNavigation = this.preventOffGridNavigation,
                    preventDiagonalMoves = this.preventDiagonalMoves,
                    navigateToNearestIfBlocked = this.navigateToNearestIfBlocked,
                    requester = this
                };

                _stop = false;
                _stopped = false;
            }

            _lastPathRequestTime = Time.time;
            GameServices.pathService.QueueRequest(_pendingPathRequest, this.pathingPriority);
        }

        private void SetManualPath(ManualPath path)
        {
            if (path.path.count == 0)
            {
                StopInternal();
                return;
            }

            _stop = false;
            _stopped = false;
            _currentDestination = null;
            _currentPath = path.path;
            _currentGrid = GridManager.instance.GetGrid(_currentPath.Peek().position);
            _endOfResolvedPath = _currentPath.Last().position;
            _endOfPath = _endOfResolvedPath;
            _lastPathRequestTime = Time.time;
        }

        private void AnnounceEvent(UnitNavigationEventMessage.Event e, Vector3 destination, Vector3[] pendingWaypoints)
        {
            _navMessage.isHandled = false;
            _navMessage.eventCode = e;
            _navMessage.destination = destination;
            _navMessage.pendingWaypoints = pendingWaypoints;
            GameServices.messageBus.Post(_navMessage);
        }

        private void ResolveNextPoint()
        {
            _currentDestination = _currentPath.Pop();

            var portal = _currentDestination as IPortalNode;
            if (portal != null)
            {
                _isPortaling = true;
                this.Wait(null);

                //Since a portal will never be the last node on a path, we can safely pop the next in line as the actual destination of the portal
                //doing it like this also caters for the scenario where the destination is the last node.
                _currentDestination = _currentPath.Pop();

                _currentGrid = portal.Execute(
                    this.transform,
                    _currentDestination,
                    () =>
                    {
                        _isPortaling = false;
                        this.Resume();
                    });
            }
        }

        private void ConsumeResult()
        {
            //Consume way points if appropriate. This must be done prior to the processing of the result, since if the request was a way point request, the first item in line is the one the result concerns.
            if (_pendingResult.originalRequest.type == RequestType.Waypoint)
            {
                _wayPoints.Dequeue();
            }
            else if (_pendingResult.originalRequest.type == RequestType.PathboundWaypoint)
            {
                _pathboundWayPoints.Dequeue();
            }

            //Reset current destination no matter what
            _currentDestination = null;

            //Since result processing may actually repath and consequently a new result may arrive we need to operate on locals and null the pending result
            var result = _pendingResult;
            _pendingResult = null;

            //Process the result
            if (!ProcessAndValidateResult(result))
            {
                return;
            }

            //Consume the result
            _currentPath = result.path;
            _currentGrid = result.originalRequest.fromGrid;
            _endOfResolvedPath = _currentPath.Last().position;
            _endOfPath = _endOfResolvedPath;

            //Update pending way points
            UpdatePathboundWaypoints(result.pendingWaypoints);

            //The first point on the path is always the origin of the request, so we want to skip that.
            _currentPath.Pop();
        }

        private bool ProcessAndValidateResult(PathResult result)
        {
            for (int i = 0; i < _resultProcessors.Length; i++)
            {
                if (_resultProcessors[i].HandleResult(result, this))
                {
                    return (result.status == PathingStatus.Complete);
                }
            }

            UnitNavigationEventMessage.Event msgEvent = UnitNavigationEventMessage.Event.None;

            switch (result.status)
            {
                case PathingStatus.Complete:
                {
                    /* All is good, no more to do */
                    return true;
                }

                case PathingStatus.NoRouteExists:
                {
                    msgEvent = UnitNavigationEventMessage.Event.StoppedNoRouteExists;
                    break;
                }

                case PathingStatus.DestinationBlocked:
                {
                    msgEvent = UnitNavigationEventMessage.Event.StoppedDestinationBlocked;
                    break;
                }

                case PathingStatus.Decayed:
                {
                    //We cannot reissue the request here, since we may be on a different thread, but then again why would we issue the request again if it had a decay threshold its no longer valid.
                    msgEvent = UnitNavigationEventMessage.Event.StoppedRequestDecayed;
                    break;
                }

                case PathingStatus.StartOutsideGrid:
                case PathingStatus.EndOutsideGrid:
                case PathingStatus.Failed:
                {
                    //The first two are handled before going to the pather, by the DirectPather, and since it would be a bug if either happen and not something a handler should care for
                    //we do not send a message
                    Debug.LogError("Unexpected status returned from pather: " + result.status.ToString());
                    break;
                }
            }

            var destination = result.originalRequest.to;
            var pendingWaypoints = _wayPoints.ToArray();

            StopInternal();

            if (msgEvent != UnitNavigationEventMessage.Event.None)
            {
                AnnounceEvent(msgEvent, destination, pendingWaypoints);
            }

            return false;
        }

        private Vector3 ResolveMoveDirection()
        {
            var dirRaw = (_currentDestination.position - this.transformCached.position).AdjustAxis(0.0f, this.excludedAxis);

            _currentDestinationDistance = dirRaw.magnitude;

            return (dirRaw / _currentDestinationDistance);
        }

        private void HandleWaypointsAndArrival(Vector3 curDir)
        {
            //if we are close to the end node, start testing if we need to slow down or queue the next waypoint, unless we are moving on to another waypoint.
            if (_pendingResult == null && _pendingPathRequest == null)
            {
                var dirEnd = (_endOfResolvedPath - this.transformCached.position).AdjustAxis(0.0f, this.excludedAxis);
                var distanceToEndSquared = dirEnd.sqrMagnitude;

                if ((_pathboundWayPoints.Count > 0) && (distanceToEndSquared < _requestNextWaypointDistanceSquared) && Vector3.Dot(dirEnd.normalized, curDir) >= _proximityAngleCos)
                {
                    RequestPath(_endOfResolvedPath, _pathboundWayPoints.Peek(), RequestType.PathboundWaypoint);
                }
                else if ((_wayPoints.Count > 0) && (distanceToEndSquared < _requestNextWaypointDistanceSquared) && Vector3.Dot(dirEnd.normalized, curDir) >= _proximityAngleCos)
                {
                    RequestPath(_endOfResolvedPath, _wayPoints.Peek(), RequestType.Waypoint);
                }
                else if (!_onFinalApproach && (distanceToEndSquared < _slowingDistanceSquared && Vector3.Dot(dirEnd.normalized, curDir) >= _proximityAngleCos))
                {
                    _onFinalApproach = true;
                }
            }
        }

        private void HandlePathReplan()
        {
            //If we are moving entirely off grid, there is no point in replanning, as there is nothing to replan on.
            if (_currentGrid == null || this.replanMode == ReplanMode.NoReplan)
            {
                return;
            }

            var now = Time.time;
            if (now - _lastPathRequestTime < this.replanInterval)
            {
                return;
            }

            bool replan = true;
            if (this.replanMode == ReplanMode.Dynamic)
            {
                replan = _currentGrid.HasSectionsChangedSince(this.transformCached.position, _lastPathRequestTime);
            }

            if (replan)
            {
                ReplanPath();
            }
        }

        private Vector3 SteerLocally(Vector3 nextDestination)
        {
            return Vector3.zero;
        }

        private void UpdatePathboundWaypoints(Vector3[] newPoints)
        {
            if (newPoints == null || newPoints.Length == 0)
            {
                return;
            }

            _endOfPath = newPoints[newPoints.Length - 1];

            _pathboundWayPoints.Clear();
            for (int i = 0; i < newPoints.Length; i++)
            {
                _pathboundWayPoints.Enqueue(newPoints[i]);
            }
        }

        private void StopInternal()
        {
            lock (_syncLock)
            {
                _stopped = true;
                _wayPoints.Clear();
                _pathboundWayPoints.Clear();
                _currentPath = null;
                _pendingPathRequest = null;
                _currentDestination = null;
                _pendingResult = null;
                _manualPath = null;
            }
        }
    }
}
