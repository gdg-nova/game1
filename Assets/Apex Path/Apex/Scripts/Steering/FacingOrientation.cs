/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using UnityEngine;

    /// <summary>
    /// Encapsulates the details of a facing orientation.
    /// </summary>
    public class FacingOrientation
    {
        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public Vector3 orientation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the priority this orientation has in relation to other possible orientations from other providers.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int priority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the speed by which the unit will turn to face the orientation.
        /// </summary>
        /// <value>
        /// The turn speed.
        /// </value>
        public float turnSpeed
        {
            get;
            set;
        }
    }
}
