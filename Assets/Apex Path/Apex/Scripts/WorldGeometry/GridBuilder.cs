/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System;
    using System.Collections;
    using Apex.DataStructures;
    using Apex.LoadBalancing;
    using Apex.Utilities;
    using UnityEngine;

    /// <summary>
    /// Builds grids
    /// </summary>
    public class GridBuilder : ICellMatrixConfiguration
    {
        /// <summary>
        /// The origin, i.e. center of the grid
        /// </summary>
        public Vector3 origin { get; set; }

        /// <summary>
        /// size along the x-axis.
        /// </summary>
        public int sizeX { get; set; }

        /// <summary>
        /// size along the z-axis.
        /// </summary>
        public int sizeZ { get; set; }

        /// <summary>
        /// The cell size.
        /// </summary>
        public float cellSize { get; set; }

        /// <summary>
        /// The obstacle sensitivity range, meaning any obstacle within this range of the cell center will cause the cell to be blocked.
        /// </summary>
        public float obstacleSensitivityRange { get; set; }

        /// <summary>
        /// Whether or not to generate a height map to enable units to follow a terrain of differing heights.
        /// </summary>
        public bool generateHeightmap { get; set; }

        /// <summary>
        /// The upper boundary (y - value) of the matrix.
        /// </summary>
        public float upperBoundary { get; set; }

        /// <summary>
        /// The lower boundary (y - value) of the matrix.
        /// </summary>
        public float lowerBoundary { get; set; }

        /// <summary>
        /// Gets the granularity of the height map, i.e. the distance between height samples.
        /// </summary>
        /// <value>
        /// The granularity of the height map.
        /// </value>
        public float granularity { get; set; }

        /// <summary>
        /// The sub sections along the x-axis.
        /// </summary>
        public int subSectionsX { get; set; }

        /// <summary>
        /// The sub sections along the z-axis.
        /// </summary>
        public int subSectionsZ { get; set; }

        /// <summary>
        /// The sub sections cell overlap
        /// </summary>
        public int subSectionsCellOverlap { get; set; }

        /// <summary>
        /// The maximum angle at which a cell is deemed walkable
        /// </summary>
        public float maxWalkableSlopeAngle { get; set; }

        /// <summary>
        /// The maximum height that the unit can scale, i.e. walk onto even if its is a vertical move. Stairs for instance.
        /// </summary>
        public float maxScaleHeight { get; set; }

        /// <summary>
        /// Creates a grid configuration based on the property values of this instance.
        /// </summary>
        /// <returns>The grid configuration</returns>
        public IGrid Create()
        {
            var cellMatrix = CellMatrix.Create(this);
            var subSections = CreateSubSections(cellMatrix.start);

            return Grid.Create(cellMatrix, subSections);
        }

        /// <summary>
        /// Creates a grid configuration based on the prebaked data
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The grid.</returns>
        public IGrid Create(CellMatrixData data)
        {
            Ensure.ArgumentNotNull(data, "data");

            var cellMatrix = CellMatrix.Create(this, data);
            var subSections = CreateSubSections(cellMatrix.start);

            return Grid.Create(cellMatrix, subSections);
        }

        internal IGrid CreateForEditor(CellMatrixData data)
        {
            var cellMatrix = CellMatrix.CreateForEditor(this, data);
            var subSections = CreateSubSections(cellMatrix.start);

            return Grid.Create(cellMatrix, subSections);
        }

        internal void Create(int maxMillisecondsUsedPerFrame, Action<IGrid> callback)
        {
            LoadBalancer.defaultBalancer.Add(
                new LongRunningAction(() => CreateIncrementally(callback), maxMillisecondsUsedPerFrame),
                0.01f,
                true);
        }

        internal void Create(CellMatrixData data, int maxMillisecondsUsedPerFrame, Action<IGrid> callback)
        {
            Ensure.ArgumentNotNull(data, "data");

            LoadBalancer.defaultBalancer.Add(
                new LongRunningAction(() => CreateIncrementally(data, callback), maxMillisecondsUsedPerFrame),
                0.01f,
                true);
        }

        private IEnumerator CreateIncrementally(CellMatrixData data, Action<IGrid> callback)
        {
            var cellMatrixInitializer = CellMatrix.CreateIncrementally(this, data);
            while (cellMatrixInitializer.isInitializing)
            {
                yield return null;
            }

            var cellMatrix = cellMatrixInitializer.matrix;

            var subSections = CreateSubSections(cellMatrix.start);
            yield return null;

            var grid = Grid.Create(cellMatrix, subSections);
            callback(grid);
        }

        private IEnumerator CreateIncrementally(Action<IGrid> callback)
        {
            var cellMatrixInitializer = CellMatrix.CreateIncrementally(this);
            while (cellMatrixInitializer.isInitializing)
            {
                yield return null;
            }

            var cellMatrix = cellMatrixInitializer.matrix;

            var subSections = CreateSubSections(cellMatrix.start);
            yield return null;

            var grid = Grid.Create(cellMatrix, subSections);
            callback(grid);
        }

        private GridSection[] CreateSubSections(Vector3 start)
        {
            var subSectionsX = Math.Max(this.subSectionsX, 1);
            var subSectionsZ = Math.Max(this.subSectionsZ, 1);

            var overLap = this.subSectionsCellOverlap * this.cellSize;
            var subSectionWidth = ((this.sizeX + ((subSectionsX - 1) * this.subSectionsCellOverlap)) * this.cellSize) / (subSectionsX * 1.0f);
            var subSectionDepth = ((this.sizeZ + ((subSectionsZ - 1) * this.subSectionsCellOverlap)) * this.cellSize) / (subSectionsZ * 1.0f);

            var subSectionCount = subSectionsX * subSectionsZ;
            var gridSections = new GridSection[subSectionCount];

            int idx = 0;
            for (int i = 0; i < subSectionsX; i++)
            {
                for (int j = 0; j < subSectionsZ; j++)
                {
                    var rect = new RectangleXZ(
                        start.x + (subSectionWidth * i) - (overLap * i),
                        start.z + (subSectionDepth * j) - (overLap * j),
                        subSectionWidth,
                        subSectionDepth);

                    gridSections[idx++] = new GridSection(rect);
                }
            }

            return gridSections;
        }
    }
}
