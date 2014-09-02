/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using Apex.Common;
    using Apex.LoadBalancing;
    using Apex.Services;
    using Apex.Units;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Base class for steering components, that is components that steer the unit in some direction at some speed according to some logic.
    /// </summary>
    [RequireComponent(typeof(UnitComponent))]
    [RequireComponent(typeof(SteerableUnitComponent))]
    public abstract class SteeringComponent : ExtendedMonoBehaviour, IPositioned, IHaveAttributes
    {
        /// <summary>
        /// The weight this component's input will have in relation to other steering components.
        /// </summary>
        public float weight = 1.0f;

        private UnitComponent _unit;

        /// <summary>
        /// Gets the attributes of the unit.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public AttributeMask attributes
        {
            get { return _unit.attributes; }
        }

        /// <summary>
        /// Gets the radius of the unit.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public float radius
        {
            get { return _unit.radius; }
        }

        /// <summary>
        /// Gets the position of the unit.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector3 position
        {
            get { return this.transformCached.position; }
        }

        /// <summary>
        /// Gets the cached transform.
        /// </summary>
        /// <value>
        /// The cached transform.
        /// </value>
        protected Transform transformCached
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the unit component holding various data related to the unit.
        /// </summary>
        /// <value>
        /// The unit component.
        /// </value>
        protected IUnit unit
        {
            get { return _unit; }
        }

        /// <summary>
        /// Gets the excluded axis.
        /// </summary>
        /// <value>
        /// The excluded axis.
        /// </value>
        protected Axis excludedAxis
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the weighted movement vector.
        /// </summary>
        /// <param name="currentVelocity">The current weight adjusted velocity.</param>
        /// <returns>The weighted movement vector</returns>
        public Vector3 GetWeightedMovementVector(Vector3 currentVelocity)
        {
            return GetMovementVector(currentVelocity) * weight;
        }

        /// <summary>
        /// Gets the speed adjustment factor, used to increase or decrease the speed of the unit under certain circumstances.
        /// </summary>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <returns>The speed adjustment factor</returns>
        public virtual float GetSpeedAdjustmentFactor(Vector3 currentVelocity)
        {
            return 1.0f;
        }

        /// <summary>
        /// Stop the unit.
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Gets the movement vector.
        /// </summary>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <returns>The movement vector</returns>
        protected abstract Vector3 GetMovementVector(Vector3 currentVelocity);

        /// <summary>
        /// Called on Awake
        /// </summary>
        protected virtual void Awake()
        {
            this.transformCached = this.transform;

            _unit = this.GetComponent<UnitComponent>();
        }

        /// <summary>
        /// Called on Start and OnEnable, but only one of the two, i.e. at startup it is only called once.
        /// </summary>
        protected override void OnStartAndEnable()
        {
            var parent = GetComponent<SteerableUnitComponent>();
            parent.RegisterSteeringBehavior(this);
            this.excludedAxis = parent.excludeAxis;
        }

        /// <summary>
        /// Called when disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            var parent = GetComponent<SteerableUnitComponent>();
            parent.UnregisterSteeringBehavior(this);
        }

        /// <summary>
        /// Gets the appropriate update interval.
        /// </summary>
        /// <param name="expectedVelocity">The expected velocity.</param>
        /// <param name="remainingDistanceSquared">The remaining distance squared.</param>
        /// <param name="unadjustedInterval">The unadjusted interval.</param>
        /// <returns>The appropriate update interval to use for the next update</returns>
        protected float GetAppropriateUpdateInterval(Vector3 expectedVelocity, float remainingDistanceSquared, float unadjustedInterval)
        {
            var expectedDistancePerSecondSquared = expectedVelocity.sqrMagnitude;
            if (expectedDistancePerSecondSquared * unadjustedInterval > remainingDistanceSquared)
            {
                return remainingDistanceSquared / expectedDistancePerSecondSquared;
            }

            return unadjustedInterval;
        }
    }
}
