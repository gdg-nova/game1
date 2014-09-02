/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Interface for height maps
    /// </summary>
    public interface IHeightMap
    {
        /// <summary>
        /// Gets the bounds of the height map.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        Bounds bounds { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is grid bound.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is grid bound; otherwise, <c>false</c>.
        /// </value>
        bool isGridBound { get; }

        /// <summary>
        /// Samples the height at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The height at the position</returns>
        float SampleHeight(Vector3 position);
    }
}
