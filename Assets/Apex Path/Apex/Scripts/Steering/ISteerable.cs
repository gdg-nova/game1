/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using System.Collections.Generic;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Interface for objects that steer along a path
    /// </summary>
    public interface ISteerable : IMovable
    {
        /// <summary>
        /// Gets the current path.
        /// </summary>
        /// <value>
        /// The current path.
        /// </value>
        IEnumerable<IPositioned> currentPath { get; }

        /// <summary>
        /// Gets the current way points.
        /// </summary>
        /// <value>
        /// The current way points.
        /// </value>
        IEnumerable<Vector3> currentWaypoints { get; }
    }
}
