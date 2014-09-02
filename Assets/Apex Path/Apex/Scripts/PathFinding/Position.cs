/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Represents a position in world space
    /// </summary>
    public struct Position : IPositioned
    {
        private Vector3 _pos;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct.
        /// </summary>
        /// <param name="pos">The position.</param>
        public Position(Vector3 pos)
        {
            _pos = pos;
        }

        /// <summary>
        /// Gets the position vector.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector3 position
        {
            get { return _pos; }
        }
    }
}
