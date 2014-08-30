/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.DataStructures
{
    using UnityEngine;

    /// <summary>
    /// Represents a polygon in the XZ plane. This is not necessarily axis aligned.
    /// </summary>
    public class PolygonXZ
    {
        /// <summary>
        /// Represents an empty polygon, i.e. zero edges.
        /// </summary>
        public static readonly PolygonXZ empty = new PolygonXZ(new Vector3[0]);

        private Vector3[] _points;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonXZ"/> class.
        /// </summary>
        /// <param name="points">The points making up the polygon.</param>
        public PolygonXZ(params Vector3[] points)
        {
            _points = points;
        }

        /// <summary>
        /// Determines whether the specified point is contained within this polygon.
        /// </summary>
        /// <param name="test">The point to test.</param>
        /// <returns><c>true</c> if the point is contained, otherwise <c>false</c></returns>
        public bool Contains(Vector3 test)
        {
            int i;
            int j;
            bool result = false;
            for (i = 0, j = _points.Length - 1; i < _points.Length; j = i++)
            {
                if ((_points[i].z > test.z) != (_points[j].z > test.z) &&
                    (test.x < ((_points[j].x - _points[i].x) * (test.z - _points[i].z) / (_points[j].z - _points[i].z)) + _points[i].x))
                {
                    result = !result;
                }
            }

            return result;
        }
    }
}
