/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using System;
    using System.Collections.Generic;
    using Apex.LoadBalancing;
    using Apex.Messages;
    using Apex.Services;
    using Apex.Units;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Main steering controller that combines the input from attached <see cref="SteeringComponent" />s to move the unit.
    /// </summary>
    [RequireComponent(typeof(UnitComponent))]
    [AddComponentMenu("Apex/Navigation/Steering/Steerable Unit")]
    public class SteerableUnitComponent : ExtendedMonoBehaviour, IProvideFacingOrientation, ILoadBalanced, IMovingObject
    {
        //Currently navigation is only supported on the xz plane so its pointless to offer this option
        /// <summary>
        /// The excluded axis
        /// </summary>
        [HideInInspector]
        public Axis excludeAxis = Axis.Y;

        /// <summary>
        /// How fast the unit transitions from one velocity to another, i.e. acceleration/deceleration/direction change
        /// </summary>
        [Range(0.1f, 1.0f)]
        public float velocitySmootingRate = 0.3f;

        /// <summary>
        /// The priority this component's turn requests have in the <see cref="IControlFacingOrientation"/> component, if one is attached.
        /// </summary>
        public int turnRequestPriority = 1;

        /// <summary>
        /// The speed by which the unit will turn to face a new direction when its movement direction changes
        /// </summary>
        public float turnSpeed = 3.0f;

        /// <summary>
        /// The minimum speed of the unit before it will start to turn in the direction of movement
        /// </summary>
        public float minimumSpeedToTurn = 1.0f;

        /// <summary>
        /// The amount of seconds after which the unit will stop if it is stuck.
        /// </summary>
        public float stopIfStuckForSeconds = 3.0f;

        private Vector3 _targetVelocity;
        private Vector3 _currentVelocity;
        private float _desiredSpeed;
        private float _minimumSpeedToTurnSquared;
        private UnitComponent _unit;

        private Vector3 _stuckCheckPrevPos;
        private float _stuckCheckLastMove;

        private bool _stopped;
        private FacingOrientation _facingOrientation;
        private Transform _transform;
        private IDefineSpeed _speedSource;
        private IMoveUnits _mover;
        private List<SteeringComponent> _steeringComponents;
        private List<IAdjustUpdateInterval> _intervalAdjusters;

        /// <summary>
        /// Gets the speed of the unit in m/s.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public float speed
        {
            get { return _currentVelocity.magnitude; }
        }

        /// <summary>
        /// Gets the velocity of the unit.
        /// </summary>
        /// <value>
        /// The velocity.
        /// </value>
        public Vector3 velocity
        {
            get { return _currentVelocity; }
        }

        /// <summary>
        /// Gets the unit component holding various data related to the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public IUnit unit
        {
            get { return _unit; }
        }

        bool ILoadBalanced.repeat
        {
            get { return true; }
        }

        private void Awake()
        {
            this.WarnIfMultipleInstances();

            _transform = this.transform;
            _facingOrientation = new FacingOrientation
            {
                priority = this.turnRequestPriority,
                turnSpeed = this.turnSpeed
            };

            //Get the speed source
            _speedSource = this.As<IDefineSpeed>();
            if (_speedSource == null)
            {
                Debug.LogError("A speed source component is required to steer.");
                this.enabled = false;
            }

            //Resolve the mover
            _mover = this.As<IMoveUnits>();
            if (_mover == null)
            {
                var fact = this.As<IMoveUnitsFactory>();
                var charController = this.GetComponent<CharacterController>();
                if (fact != null)
                {
                    _mover = fact.Create();
                }
                else if (charController != null)
                {
                    _mover = new CharacterControllerMover(charController);
                }
                else if (this.rigidbody != null)
                {
                    _mover = new RigidBodyMover(_transform, this.rigidbody);
                }
                else
                {
                    _mover = new DefaultMover(_transform);
                }
            }

            _unit = this.GetComponent<UnitComponent>();
            _steeringComponents = new List<SteeringComponent>();
            _minimumSpeedToTurnSquared = this.minimumSpeedToTurn * this.minimumSpeedToTurn;
            _stopped = true;
        }

        /// <summary>
        /// Called on Start and OnEnable, but only one of the two, i.e. at startup it is only called once.
        /// </summary>
        protected override void OnStartAndEnable()
        {
            //Sign up as a provider of facing orientation
            var turner = this.As<IControlFacingOrientation>();
            if (turner != null)
            {
                turner.RegisterOrientationProvider(this);
            }

            //Hook up with the load balancer
            NavLoadBalancer.steering.Add(this);
        }

        private void OnDisable()
        {
            //Sign off as a provider of facing orientation
            var turner = this.As<IControlFacingOrientation>();
            if (turner != null)
            {
                turner.UnregisterOrientationProvider(this);
            }

            Stop();

            NavLoadBalancer.steering.Remove(this);
        }

        private void FixedUpdate()
        {
            Steer(Time.deltaTime);
        }

        /// <summary>
        /// Registers a steering component.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public void RegisterSteeringBehavior(SteeringComponent behavior)
        {
            _steeringComponents.Add(behavior);

            var adjuster = behavior as IAdjustUpdateInterval;
            if (adjuster != null)
            {
                if (_intervalAdjusters == null)
                {
                    _intervalAdjusters = new List<IAdjustUpdateInterval>();
                }

                _intervalAdjusters.Add(adjuster);
            }
        }

        /// <summary>
        /// Unregisters a steering behavior.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public void UnregisterSteeringBehavior(SteeringComponent behavior)
        {
            _steeringComponents.Remove(behavior);

            var adjuster = behavior as IAdjustUpdateInterval;
            if (adjuster != null && _intervalAdjusters != null)
            {
                _intervalAdjusters.Remove(adjuster);
            }
        }

        FacingOrientation IProvideFacingOrientation.GetOrientation()
        {
            if (_currentVelocity.sqrMagnitude >= _minimumSpeedToTurnSquared)
            {
                _facingOrientation.orientation = _currentVelocity;
                _facingOrientation.priority = this.turnRequestPriority;
                _facingOrientation.turnSpeed = this.turnSpeed;

                return _facingOrientation;
            }

            return null;
        }

        float? ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
        {
            if (!_stopped && IsStuck(deltaTime))
            {
                Stop();
                GameServices.messageBus.Post(new UnitNavigationEventMessage(this.gameObject, UnitNavigationEventMessage.Event.Stuck));
                return null;
            }

            //Get all move vectors from the registered steering behaviours
            Vector3 moveVector = Vector3.zero;
            float throttleAdjust = 1.0f;
            for (int i = 0; i < _steeringComponents.Count; i++)
            {
                moveVector += _steeringComponents[i].GetWeightedMovementVector(_currentVelocity);
                throttleAdjust *= _steeringComponents[i].GetSpeedAdjustmentFactor(_currentVelocity);
            }

            moveVector = moveVector.AdjustAxis(0.0f, this.excludeAxis);

            if (moveVector.sqrMagnitude == 0)
            {
                Stop(false);
                return null;
            }

            //Determine the move normal and move speed and set the target velocity
            var moveNorm = moveVector.normalized;
            _desiredSpeed = _speedSource.GetPreferredSpeed(moveNorm) * throttleAdjust;
            _desiredSpeed = Mathf.Min(_desiredSpeed, _speedSource.maximumSpeed);
            _desiredSpeed = Mathf.Max(_desiredSpeed, _speedSource.minimumSpeed);

            //Adjust to height map
            if (HeightMapManager.instance.areHeightMapsEnabled)
            {
                //We use the frontmost point of the unit for the predicted next pos. If using the unit center the front may hit a collider of different height, resulting in the unit getting stuck.
                var moveVectorNext = (moveNorm * _desiredSpeed * nextInterval) + (moveNorm * _unit.radius);
                var expectedPosNext = _transform.position + moveVectorNext;

                var heightMap = HeightMapManager.instance.GetHeightMap(expectedPosNext);
                var curPosHeight = heightMap.SampleHeight(_transform.position);
                var heightDiff = heightMap.SampleHeight(expectedPosNext) - curPosHeight;

                if (_unit.yAxisoffset > 0.0f)
                {
                    heightDiff += (curPosHeight - (_transform.position.y - _unit.yAxisoffset));
                }

                moveVectorNext.y = heightDiff;

                _targetVelocity = _desiredSpeed * moveVectorNext.normalized;
            }
            else
            {
                _targetVelocity = _desiredSpeed * moveNorm;
            }

            if (_stopped)
            {
                _stuckCheckLastMove = Time.time;
                _stopped = false;
            }

            //Check if adjustments are needed to the update interval
            float? adjustedInterval = null;
            for (int i = 0; i < _intervalAdjusters.Count; i++)
            {
                var proposedInterval = _intervalAdjusters[i].GetUpdateInterval(_targetVelocity, nextInterval);
                adjustedInterval = adjustedInterval.HasValue ? Mathf.Min(adjustedInterval.Value, proposedInterval) : proposedInterval;
            }

            return adjustedInterval;
        }

        /// <summary>
        /// Stops the unit from moving.
        /// </summary>
        public void Stop()
        {
            Stop(true);
        }

        private void Stop(bool stopComponents)
        {
            if (_stopped)
            {
                return;
            }

            if (stopComponents)
            {
                for (int i = 0; i < _steeringComponents.Count; i++)
                {
                    _steeringComponents[i].Stop();
                }
            }

            if (_speedSource != null)
            {
                _speedSource.SignalStop();
            }

            _stopped = true;
            _targetVelocity = Vector3.zero;
            _currentVelocity = Vector3.zero;
            _desiredSpeed = 0.0f;

            _mover.Stop();
        }

        private bool IsStuck(float deltaTime)
        {
            if (this.stopIfStuckForSeconds <= 0.0f)
            {
                return false;
            }

            if (!_stuckCheckPrevPos.Approximately(_transform.position, 0.01f))
            {
                _stuckCheckPrevPos = _transform.position;
                _stuckCheckLastMove = Time.time;
                return false;
            }

            return ((Time.time - _stuckCheckLastMove) > this.stopIfStuckForSeconds);
        }

        private void Steer(float deltaTime)
        {
            if (!_stopped)
            {
                _currentVelocity = Vector3.Lerp(_currentVelocity, _targetVelocity, this.velocitySmootingRate);

                _mover.Move(_currentVelocity, deltaTime);
            }
        }

        private class DefaultMover : IMoveUnits
        {
            private Transform _transform;

            public DefaultMover(Transform transform)
            {
                _transform = transform;
            }

            public void Move(Vector3 velocity, float deltaTime)
            {
                _transform.position = _transform.position + (velocity * deltaTime);
            }

            public void Stop()
            {
                /* NOOP */
            }
        }

        private class RigidBodyMover : IMoveUnits
        {
            private Transform _transform;
            private Rigidbody _rigidBody;

            public RigidBodyMover(Transform transform, Rigidbody rigidBody)
            {
                _rigidBody = rigidBody;
                _transform = transform;
            }

            public void Move(Vector3 velocity, float deltaTime)
            {
                _rigidBody.MovePosition(_transform.position + (velocity * deltaTime));
            }

            public void Stop()
            {
                if (!_rigidBody.isKinematic)
                {
                    _rigidBody.velocity = Vector3.zero;
                }
            }
        }

        private class CharacterControllerMover : IMoveUnits
        {
            private CharacterController _controller;

            public CharacterControllerMover(CharacterController controller)
            {
                _controller = controller;
            }

            public void Move(Vector3 velocity, float deltaTime)
            {
                _controller.Move(velocity * deltaTime);
            }

            public void Stop()
            {
                /* NOOP */
            }
        }
    }
}
