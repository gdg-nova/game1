/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Debugging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Apex.Common;
    using Apex.DataStructures;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Visualization component that draws gizmos to represent the grid and show obstructed areas.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Apex/Debugging/Grid Visualizer")]
    public class GridVisualizer : Visualizer
    {
        /// <summary>
        /// Controls how the grid is drawn
        /// </summary>
        public GridMode drawMode;

        /// <summary>
        /// Whether to draw the grids sub sections
        /// </summary>
        public bool drawSubSections;

        /// <summary>
        /// Controls whether the visualizer draws all grids in the scene or only those on the same GameObject as the visualizer.
        /// </summary>
        public bool drawAllGrids = false;

        /// <summary>
        /// The editor refresh delay, i.e. increase this value if you experience too much CPU activity during scene modification
        /// </summary>
        public int editorRefreshDelay = 100;

        /// <summary>
        /// The distance threshold controlling when the grid is drawn. If the camera is showing more than this looking at the diagonal, the grid will no longer be drawn.
        /// This is for performance reasons plus when zoomed out too far the grid visualization is useless.
        /// </summary>
        public float drawDistanceThreshold = 175f;

        /// <summary>
        /// The grid lines color
        /// </summary>
        public Color gridLinesColor = new Color(135f / 255f, 135f / 255f, 135f / 255f);

        /// <summary>
        /// The obstacle color
        /// </summary>
        public Color obstacleColor = new Color(226f / 255f, 41f / 255f, 32f / 255f, 150f / 255f);

        /// <summary>
        /// The sub sections color
        /// </summary>
        public Color subSectionsColor = new Color(0f, 150f / 255f, 211f / 255f);

        /// <summary>
        /// The color of the bounds wire frame
        /// </summary>
        public Color boundsColor = Color.grey;

        private DateTime? _nextRefresh;

        /// <summary>
        /// How the grid visualization is displayed
        /// </summary>
        public enum GridMode
        {
            /// <summary>
            /// Draws the grid to represent how it is actually laid out
            /// </summary>
            Layout,

            /// <summary>
            /// Draws the grid by showing accessibility between grid cells
            /// </summary>
            Accessibility
        }

        /// <summary>
        /// Draws the actual visualization.
        /// </summary>
        protected override void DrawVisualization()
        {
            var forceRefresh = false;
            if (_nextRefresh.HasValue && _nextRefresh < DateTime.UtcNow)
            {
                _nextRefresh = null;
                forceRefresh = true;
            }

            var grids = this.drawAllGrids ? FindObjectsOfType<GridComponent>() : GetComponents<GridComponent>();

            if (grids != null)
            {
                foreach (var grid in grids)
                {
                    if (grid.enabled)
                    {
                        bool outlineOnly;
                        var drawArea = GetDrawArea(grid.origin.y, out outlineOnly);
                        drawArea = grid.EnsureForEditor(forceRefresh, drawArea);
                        if (!DrawGrid(grid.grid, drawArea, outlineOnly))
                        {
                            grid.EnsureForEditor(true, drawArea);
                            DrawGrid(grid.grid, drawArea, outlineOnly);
                        }
                    }
                }
            }
        }

        private Bounds GetDrawArea(float gridElevation, out bool outlineOnly)
        {
            outlineOnly = false;

            //Determine the area visible through the camera, starting with the bottom left and top right corners.
            var cam = Camera.current;
            var bl = cam.ScreenToGroundPoint(Vector3.zero, gridElevation);
            var tr = cam.ScreenToGroundPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, 0f), gridElevation);
            if (bl == Vector3.zero || tr == Vector3.zero)
            {
                outlineOnly = true;
            }

            //We do not want to draw the grids if zoom level is too high since it will simply crash the editor to draw that many lines.
            var diagDistSquared = (tr - bl).sqrMagnitude;
            if (diagDistSquared > this.drawDistanceThreshold * this.drawDistanceThreshold)
            {
                outlineOnly = true;
            }

            //Get the remaining corners resolved and calculate the proper bounds to draw
            var tl = cam.ScreenToGroundPoint(new Vector3(0f, cam.pixelHeight, 0f), gridElevation);
            var br = cam.ScreenToGroundPoint(new Vector3(cam.pixelWidth, 0f, 0f), gridElevation);

            var wpMin = new Vector3(Mathf.Min(bl.x, tl.x, br.x, tr.x), 0f, Mathf.Min(bl.z, tl.z, br.z, tr.z));
            var wpMax = new Vector3(Mathf.Max(bl.x, tl.x, br.x, tr.x), 0f, Mathf.Max(bl.z, tl.z, br.z, tr.z));
            var drawArea = new Bounds();
            drawArea.SetMinMax(wpMin, wpMax);

            return drawArea;
        }

        private bool DrawGrid(IGrid grid, Bounds drawArea, bool outlineOnly)
        {
            if (grid.sizeX == 0 || grid.sizeZ == 0 || grid.cellSize == 0f)
            {
                return true;
            }

            Gizmos.color = this.boundsColor;
            Gizmos.DrawWireCube(grid.bounds.center, grid.bounds.size);

            if (outlineOnly)
            {
                return true;
            }

            var matrix = grid.cellMatrix;
            var start = grid.GetCell(drawArea.min);
            var end = grid.GetCell(drawArea.max);
            if (start == null || end == null)
            {
                return false;
            }

            var step = grid.cellSize;
            var halfCell = (step / 2.0f);
            var y = grid.origin.y + 0.05f;

            Gizmos.color = this.gridLinesColor;

            if (this.drawMode == GridMode.Layout)
            {
                var xMin = start.position.x - halfCell;
                var xMax = end.position.x + halfCell;
                var zMin = start.position.z - halfCell;
                var zMax = end.position.z + halfCell;

                for (float x = xMin; x <= xMax; x += step)
                {
                    Gizmos.DrawLine(new Vector3(x, y, zMin), new Vector3(x, y, zMax));
                }

                for (float z = zMin; z <= zMax; z += step)
                {
                    Gizmos.DrawLine(new Vector3(xMin, y, z), new Vector3(xMax, y, z));
                }
            }
            else
            {
                VectorXZ[] directions = new[] { new VectorXZ(-1, 0), new VectorXZ(-1, 1), new VectorXZ(0, 1), new VectorXZ(1, 1) };
                var heightAdj = new Vector3(0.0f, 0.05f, 0.0f);

                for (int x = start.matrixPosX; x <= end.matrixPosX; x++)
                {
                    for (int z = start.matrixPosZ; z <= end.matrixPosZ; z++)
                    {
                        var c = matrix[x, z];
                        if (c == null)
                        {
                            return false;
                        }

                        if (!c.IsWalkableToAny())
                        {
                            continue;
                        }

                        var curPos = new VectorXZ(x, z);
                        for (int i = 0; i < 4; i++)
                        {
                            var checkPos = curPos + directions[i];
                            var other = matrix[checkPos.x, checkPos.z];

                            if (other != null && other.isWalkableFrom(c, AttributeMask.All))
                            {
                                Gizmos.DrawLine(c.position + heightAdj, other.position + heightAdj);
                            }
                        }
                    }
                }
            }

            for (int x = start.matrixPosX; x <= end.matrixPosX; x++)
            {
                for (int z = start.matrixPosZ; z <= end.matrixPosZ; z++)
                {
                    var c = matrix[x, z];
                    if (c == null)
                    {
                        return false;
                    }

                    var walkableToSomeone = c.isWalkable(AttributeMask.All);
                    var walkableToEveryone = c.isWalkable(AttributeMask.None);

                    if (!walkableToSomeone)
                    {
                        Gizmos.color = this.obstacleColor;
                        Gizmos.DrawCube(c.position, new Vector3(step, 0.05f, step));
                    }
                    else if (!walkableToEveryone)
                    {
                        var half = this.obstacleColor;
                        half.a = half.a * 0.5f;
                        Gizmos.color = half;
                        Gizmos.DrawCube(c.position, new Vector3(step, 0.05f, step));
                    }
                }
            }

            if (this.drawSubSections)
            {
                Gizmos.color = this.subSectionsColor;
                foreach (var section in grid.gridSections)
                {
                    var subCenter = section.bounds.center;
                    subCenter.y = y;
                    Gizmos.DrawWireCube(subCenter, section.bounds.size);
                }
            }

            return true;
        }

        private void Update()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                _nextRefresh = DateTime.UtcNow.AddMilliseconds(this.editorRefreshDelay);
            }
        }
    }
}
