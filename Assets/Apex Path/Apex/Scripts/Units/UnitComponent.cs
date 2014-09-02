/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Units
{
    using Apex.Common;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Basic unit properties component.
    /// </summary>
    [AddComponentMenu("Apex/Units/Unit")]
    public partial class UnitComponent : AttributedComponent, IUnit
    {
        private Transform _transform;

        /// <summary>
        /// The radius of the unit.
        /// </summary>
        public float radius = 0.5f;

        /// <summary>
        /// The field of view in degrees
        /// </summary>
        [Range(0f, 360f)]
        public float fieldOfView = 200f;

        /// <summary>
        /// If the unit is not properly grounded at y = 0, set this offset such that when the unit is in a grounded position, its transform.y - yAxisoffset == 0.
        /// This is only relevant if your unit has no rigidbody with gravity.
        /// </summary>
        public float yAxisoffset = 0.0f;

        /// <summary>
        /// Gets the position of the component.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector3 position
        {
            get { return _transform.position; }
        }

        /// <summary>
        /// Gets the forward vector of the unit, i.e. the direction its nose is pointing (provided it has a nose).
        /// </summary>
        /// <value>
        /// The forward vector.
        /// </value>
        public Vector3 forward
        {
            get { return _transform.forward; }
        }

        float IUnit.radius
        {
            get { return this.radius; }
        }

        float IUnit.fieldOfView
        {
            get { return this.fieldOfView; }
        }

        private void Awake()
        {
            this.WarnIfMultipleInstances();

            if (this.rigidbody != null && this.rigidbody.useGravity)
            {
                yAxisoffset = 0.0f;
            }

            _transform = this.transform;
        }
    }
}
