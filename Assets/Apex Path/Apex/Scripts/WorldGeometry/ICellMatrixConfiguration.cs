/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using UnityEngine;

    /// <summary>
    /// Interface for... yes well have a guess
    /// </summary>
    public interface ICellMatrixConfiguration
    {
        /// <summary>
        /// The origin, i.e. center of the grid
        /// </summary>
        Vector3 origin { get; }

        /// <summary>
        /// size along the x-axis.
        /// </summary>
        int sizeX { get; }

        /// <summary>
        /// size along the z-axis.
        /// </summary>
        int sizeZ { get; }

        /// <summary>
        /// The cell size.
        /// </summary>
        float cellSize { get; }

        /// <summary>
        /// The obstacle sensitivity range, meaning any obstacle within this range of the cell center will cause the cell to be blocked.
        /// </summary>
        float obstacleSensitivityRange { get; }

        /// <summary>
        /// Whether or not to generate a height map to enable units to follow a terrain of differing heights.
        /// </summary>
        bool generateHeightmap { get; }

        /// <summary>
        /// The upper boundary (y - value) of the matrix.
        /// </summary>
        float upperBoundary { get; }

        /// <summary>
        /// The lower boundary (y - value) of the matrix.
        /// </summary>
        float lowerBoundary { get; }

        /// <summary>
        /// Gets the granularity of the height map, i.e. the distance between height samples.
        /// </summary>
        /// <value>
        /// The granularity of the height map.
        /// </value>
        float granularity { get; }
        
        /// <summary>
        /// The maximum angle at which a cell is deemed walkable
        /// </summary>
        float maxWalkableSlopeAngle { get; }

        /// <summary>
        /// The maximum height that the unit can scale, i.e. walk onto even if its is a vertical move. Stairs for instance.
        /// </summary>
        float maxScaleHeight { get; }
    }
}
