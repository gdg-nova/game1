/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Height map with zero height
    /// </summary>
    public class ZeroHeightMap : IHeightMap
    {
        private Bounds _bounds = new Bounds(Vector3.zero, Vector3.zero);

        /// <summary>
        /// Gets the bounds of the height map.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public Bounds bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is grid bound.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is grid bound; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool isGridBound
        {
            get { return false; }
        }

        /// <summary>
        /// Samples the height at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The height at the position
        /// </returns>
        public float SampleHeight(Vector3 position)
        {
            return position.y;
        }
    }
}
