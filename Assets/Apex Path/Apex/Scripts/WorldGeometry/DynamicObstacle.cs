/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System.Collections.Generic;
    using Apex.Common;
    using Apex.LoadBalancing;
    using Apex.Services;
    using Apex.Steering;
    using UnityEngine;

    /// <summary>
    /// Represents an obstacle with a dynamic nature, meaning it can be an obstacle to only some, only at certain times, etc.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Obstacles/Dynamic Obstacle")]
    [RequireComponent(typeof(Collider))]
    public class DynamicObstacle : ExtendedMonoBehaviour, IDynamicObstacle, ILoadBalanced
    {
        /// <summary>
        /// Controls how the obstacle updates its state, and thereby its associated grid.
        /// </summary>
        public UpdateMode updateMode;

        /// <summary>
        /// Determines how far in the obstacles direction of movement, that cells will be considered blocked.
        /// </summary>
        public float velocityPredictionFactor = 1.5f;

        /// <summary>
        /// Setting this to true, will stop the dynamic updates if the obstacle remains stationary for <see cref="stationaryThresholdSeconds"/> seconds.
        /// </summary>
        public bool stopUpdatingIfStationary = false;

        /// <summary>
        /// The amount of seconds after which dynamic updates on the obstacle will stop if <see cref="stopUpdatingIfStationary"/> is set to <c>true</c>.
        /// </summary>
        public float stationaryThresholdSeconds = 5.0f;

        /// <summary>
        /// Setting this to a value other than 0, will override the default update interval of the load balancer.
        /// </summary>
        public float customUpdateInterval = 0.0f;

        private IMovingObject _mover;
        private float _hasBeenStationaryForSeconds;
        private Vector3 _lastPosition;
        private bool _isScheduledForUpdates;

        private IGrid _grid;
        private IList<Cell> _coveredCells;
        private IList<GridSection> _affectedSections;

        [SerializeField, AttributeProperty]
        private int _exceptionsMask;

        /// <summary>
        /// How the obstacle updates its state, and thereby its associated grid.
        /// </summary>
        public enum UpdateMode
        {
            /// <summary>
            /// The obstacle is repeatedly updated at the default or <see cref="customUpdateInterval"/>
            /// </summary>
            OnInterval,

            /// <summary>
            /// The obstacle is updated once on start, and then only on request by calling <see cref="ActivateUpdates"/>
            /// </summary>
            OnRequest
        }

        /// <summary>
        /// Gets the position of the component.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector3 position
        {
            get { return this.transform.position; }
        }

        /// <summary>
        /// Gets the attribute mask that defines the attributes for which this obstacle will not be considered an obstacle.
        /// </summary>
        /// <value>
        /// The exceptions mask.
        /// </value>
        public AttributeMask exceptionsMask
        {
            get { return _exceptionsMask; }
            set { _exceptionsMask = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this entity is repeatedly updated each interval.
        /// </summary>
        /// <value>
        /// <c>true</c> if the entity will be updated each interval; <c>false</c> if it will only be updated once and then removed from the load balancer.
        /// </value>
        public bool repeat
        {
            get;
            private set;
        }

        private void Awake()
        {
            _mover = this.As<IMovingObject>();

            if ((this.updateMode == UpdateMode.OnRequest) && (_mover != null || this.rigidbody != null))
            {
                Debug.LogWarning("Please note that this obstacle is marked to update on request, and will not be automatically updating after moving.");
            }
        }

        /// <summary>
        /// Called on Start and OnEnable, but only one of the two, i.e. at startup it is only called once.
        /// </summary>
        protected override void OnStartAndEnable()
        {
            var repeat = (this.updateMode == UpdateMode.OnInterval);
            InitializeState(this.customUpdateInterval, repeat);
        }

        private void OnDisable()
        {
            if (_isScheduledForUpdates)
            {
                NavLoadBalancer.dynamicObstacles.Remove(this);
            }

            _isScheduledForUpdates = false;

            UnblockCells();

            _coveredCells = null;
            _affectedSections = null;
        }

        /// <summary>
        /// Explicitly starts updating the dynamic obstacle, making it reevaluate its state.
        /// </summary>
        /// <param name="interval">The interval by which to update. Pass null to use the default interval defined by the load balancer.</param>
        /// <param name="repeat">if set to <c>true</c> it will repeatedly update every <paramref name="interval" /> otherwise it will update only once.</param>
        public void ActivateUpdates(float? interval, bool repeat)
        {
            if (_isScheduledForUpdates)
            {
                NavLoadBalancer.dynamicObstacles.Remove(this);
                _isScheduledForUpdates = false;
            }

            InitializeState(interval.GetValueOrDefault(this.customUpdateInterval), repeat);
        }

        /// <summary>
        /// Toggles the obstacle on or off. This is preferred to enabling/disabling the component if it is a regularly recurring action.
        /// </summary>
        /// <param name="active">if set to <c>true</c> the obstacle is toggle on, otherwise off.</param>
        public void Toggle(bool active)
        {
            NavLoadBalancer.dynamicObstacles.Add(new ToggleAction(this, active));
        }

        float? ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
        {
            if (this.repeat && this.position.Approximately(_lastPosition, 0.01f))
            {
                if (this.stopUpdatingIfStationary)
                {
                    _hasBeenStationaryForSeconds += deltaTime;
                    if (_hasBeenStationaryForSeconds > this.stationaryThresholdSeconds)
                    {
                        _isScheduledForUpdates = false;
                        this.repeat = false;
                    }
                }

                return null;
            }

            _hasBeenStationaryForSeconds = 0.0f;
            _isScheduledForUpdates = this.repeat;
            _lastPosition = this.position;

            UnblockCells();
            BlockCells(deltaTime);

            return null;
        }

        private void GetCoveredCellsAndSections(float deltaTime)
        {
            _grid = GridManager.instance.GetGrid(this.position);
            if (_grid == null)
            {
                return;
            }

            var velocity = GetVelocity() * this.velocityPredictionFactor;

            var bounds = GrowBoundsByVelocity(this.collider.bounds, velocity);

            _grid.GetCoveredCells(bounds, _coveredCells);
            _grid.GetSections(bounds, _affectedSections);
        }

        private void BlockCells(float deltaTime)
        {
            GetCoveredCellsAndSections(deltaTime);

            bool changed = false;
            for (int i = 0; i < _coveredCells.Count; i++)
            {
                changed |= _coveredCells[i].AddDynamicObstacle(this);
            }

            if (changed)
            {
                TouchAffectedSections();
            }
        }

        private void UnblockCells()
        {
            if (_coveredCells.Count == 0)
            {
                return;
            }

            bool changed = false;
            for (int i = 0; i < _coveredCells.Count; i++)
            {
                changed |= _coveredCells[i].RemoveDynamicObstacle(this);
            }

            if (changed)
            {
                TouchAffectedSections();
            }

            _coveredCells.Clear();
            _affectedSections.Clear();
        }

        private void TouchAffectedSections()
        {
            for (int i = 0; i < _affectedSections.Count; i++)
            {
                _affectedSections[i].Touch();
            }
        }

        private void InitializeState(float updateInterval, bool repeat)
        {
            if (_isScheduledForUpdates)
            {
                return;
            }

            //Do the initial block
            if (_coveredCells == null)
            {
                _coveredCells = new List<Cell>();
                _affectedSections = new List<GridSection>();

                _lastPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            }

            _isScheduledForUpdates = true;
            this.repeat = repeat;

            if (updateInterval > 0.0f)
            {
                NavLoadBalancer.dynamicObstacles.Add(this, updateInterval);
            }
            else
            {
                NavLoadBalancer.dynamicObstacles.Add(this);
            }
        }

        private Vector3 GetVelocity()
        {
            Vector3 velocity = Vector3.zero;

            if (_mover != null)
            {
                velocity = _mover.velocity;
            }

            if (this.rigidbody != null)
            {
                velocity = velocity + this.rigidbody.velocity;
            }

            return velocity;
        }

        private Bounds GrowBoundsByVelocity(Bounds bounds, Vector3 velocity)
        {
            if (velocity.x != 0f || velocity.z != 0f)
            {
                var vMin = bounds.min;
                var vMax = bounds.max;

                if (velocity.x < 0f)
                {
                    vMin.x += velocity.x;
                }
                else if (velocity.x > 0f)
                {
                    vMax.x += velocity.x;
                }

                if (velocity.z < 0f)
                {
                    vMin.z += velocity.z;
                }
                else if (velocity.z > 0f)
                {
                    vMax.z += velocity.z;
                }

                bounds.SetMinMax(vMin, vMax);
            }

            return bounds;
        }

        private class ToggleAction : ILoadBalanced
        {
            private DynamicObstacle _target;
            private bool _block;

            public ToggleAction(DynamicObstacle target, bool block)
            {
                _target = target;
                _block = block;
            }

            public bool repeat
            {
                get { return false; }
            }

            public float? ExecuteUpdate(float deltaTime, float nextInterval)
            {
                if (_block)
                {
                    _target.BlockCells(deltaTime);
                }
                else
                {
                    _target.UnblockCells();
                }

                return null;
            }
        }
    }
}
