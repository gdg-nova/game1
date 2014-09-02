/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using System;
    using System.Collections.Generic;
    using Apex.Common;
    using Apex.DataStructures;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// The default path smoothing implementation. This uses tangents between points to optimize routes.
    /// </summary>
    public class PathSmoother : ISmoothPaths
    {
        /// <summary>
        /// Smooths a path.
        /// </summary>
        /// <param name="goal">The goal node of the calculated path.</param>
        /// <param name="maxPathLength">Maximum length of the path.</param>
        /// <param name="request">The path request.</param>
        /// <returns>
        /// The path in smoothed form
        /// </returns>
        public StackWithLookAhead<IPositioned> Smooth(IPathNode goal, int maxPathLength, IPathRequest request)
        {
            var requesterAttributes = request.requester.attributes;
            var requesterRadius = request.requester.radius;

            //Next prune superfluous path nodes
            var reversePath = new List<IPositioned>(maxPathLength);

            var current = goal;
            var next = current.predecessor;

            int bends = -1;
            var prevDir = Vector3.zero;

            while (next != null)
            {
                var dir = next.position - current.position;

                if ((dir != prevDir) || (next is IPortalNode))
                {
                    reversePath.Add(current);
                    prevDir = dir;
                    bends++;
                }

                current = next;
                next = current.predecessor;
            }

            //Correct the end nodes and inject a mid point if too much was pruned (can happen on straight paths with no direction change, which can lead to obstacle collision if the unit is offset)
            if (reversePath.Count == 0)
            {
                reversePath.Add(new Position(request.to));
            }
            else
            {
                reversePath[0] = new Position(request.to);
            }

            if (reversePath.Count == 1 && bends <= 0)
            {
                reversePath.Add(goal.predecessor);
            }

            reversePath.Add(new Position(request.from));

            //Next see if we can reduce the path further by excluding unnecessary bends
            if (!request.preventDiagonalMoves)
            {
                var matrix = goal.parent;

                for (int i = 0; i < reversePath.Count - 2; i++)
                {
                    var c1 = reversePath[i];
                    var c2 = reversePath[i + 1];
                    var c3 = reversePath[i + 2];

                    var skip = AdjustIfPortal(c1, c2, c3);

                    if (skip > -1)
                    {
                        //One of the candidate nodes is a portal so skip to the node following the portal and resolve the grid at the other end of the portal.
                        //Since a portal node will never be the last node we can safely do this here. Since we are moving in the reverse direction here the portal will be on the other side.
                        i += skip;
                        matrix = ((IPortalNode)reversePath[i]).parent;
                        continue;
                    }

                    if (CanReducePath(c1, c3, requesterAttributes, requesterRadius, matrix))
                    {
                        reversePath[i + 1] = null;
                        i++;
                    }
                }
            }

            //Construct the final path replacing the start and goal nodes with the actual start and goal positions
            var path = new StackWithLookAhead<IPositioned>();

            for (int i = 0; i < reversePath.Count; i++)
            {
                var node = reversePath[i];
                if (node != null)
                {
                    path.Push(node);
                }
            }

            return path;
        }

        private static int AdjustIfPortal(params IPositioned[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] is IPortalNode)
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool CanReducePath(IPositioned point1, IPositioned point3, AttributeMask requesterAttributes, float requesterRadius, CellMatrix matrix)
        {
            Vector3 p1;
            Vector3 p3;

            //Assign the points so we start with the point with the lowest x-value to simplify things
            if (point1.position.x > point3.position.x)
            {
                p1 = point3.position;
                p3 = point1.position;
            }
            else
            {
                p1 = point1.position;
                p3 = point3.position;
            }

            var tan = Tangents.Create(p1, p3, requesterRadius);

            var incZ = tan.slopeDir;
            var cellSize = matrix.cellSize;
            var halfCell = cellSize / 2.0f;

            //Adjust the start and end cells to possibly include their immediate neighbour if the unit's radius crossed said boundary.
            var radiusAdjust = new Vector3(requesterRadius, 0.0f, requesterRadius * incZ);

            var startCell = matrix.GetCell(p1 - radiusAdjust);
            if (startCell == null)
            {
                startCell = matrix.GetCell(p1);
            }

            var endCell = matrix.GetCell(p3 + radiusAdjust);
            if (endCell == null)
            {
                endCell = matrix.GetCell(p3);
            }

            //We want x to end up on cell boundaries, the first of which is this far from the first points position
            var xAdj = p1.x + (startCell.position.x - p1.x) + halfCell;

            //We want to adjust z so that we correctly count impacted cells, this adjusts z so it starts at the bottom boundary of the first cell (for purposes of calculation)
            var zAdj = p1.z - (halfCell + ((p1.z - startCell.position.z) * incZ));

            //The movement across the x-axis
            float deltaX = 0.0f;

            var cellMatrix = matrix.rawMatrix;
            int indexX = 0;
            for (int x = startCell.matrixPosX; x <= endCell.matrixPosX; x++)
            {
                //So instead of just checking all cells in the bounding rect defined by the two cells p1 and p3,
                //we limit it to the cells immediately surrounding the proposed line (tangents), including enough cells that we ensure the unit will be able to pass through,
                //at the extreme routes between the two cells (i.e top corner to top corner and bottom corner to bottom corner
                int startZ;
                int endZ;

                //If the tangents are horizontal or vertical z range is obvious
                if (tan.isAxisAligned)
                {
                    startZ = startCell.matrixPosZ;
                    endZ = endCell.matrixPosZ + incZ;
                }
                else
                {
                    if (indexX == 0)
                    {
                        startZ = startCell.matrixPosZ;
                    }
                    else
                    {
                        var startCellsPassed = Mathf.FloorToInt((tan.LowTangent(deltaX) - zAdj) / cellSize) * incZ;

                        startZ = LimitStart(
                            startCell.matrixPosZ + startCellsPassed,
                            startCell.matrixPosZ,
                            incZ);
                    }

                    //The movement this step will perform across the x-axis
                    deltaX = xAdj + (indexX * cellSize);

                    var endCellsIntercepted = Mathf.FloorToInt((tan.HighTangent(deltaX) - zAdj) / cellSize) * incZ;

                    endZ = LimitEnd(
                        startCell.matrixPosZ + endCellsIntercepted,
                        endCell.matrixPosZ,
                        incZ) + incZ;
                }

                indexX++;

                for (int z = startZ; z != endZ; z += incZ)
                {
                    var intermediary = cellMatrix[x, z];
                    if (!intermediary.isWalkableFrom(startCell, requesterAttributes))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static int LimitStart(int value, int limit, int sign)
        {
            if (sign < 0)
            {
                return Math.Min(value, limit);
            }

            return Math.Max(value, limit);
        }

        private static int LimitEnd(int value, int limit, int sign)
        {
            if (sign < 0)
            {
                return Math.Max(value, limit);
            }

            return Math.Min(value, limit);
        }

        private struct Tangents
        {
            public int slopeDir;

            private float _alpha;
            private float _betaTanHigh;
            private float _betaTanLow;

            public bool isAxisAligned
            {
                get { return _alpha == 0.0f; }
            }

            public static Tangents Create(Vector3 p1, Vector3 p3, float circleRadius)
            {
                var t = new Tangents();
                t.Init(p1, p3, circleRadius);

                return t;
            }

            public float HighTangent(float deltaX)
            {
                return (_alpha * deltaX) + _betaTanHigh;
            }

            public float LowTangent(float deltaX)
            {
                return (_alpha * deltaX) + _betaTanLow;
            }

            private void Init(Vector3 p1, Vector3 p3, float circleRadius)
            {
                var dx = p3.x - p1.x;
                var dz = p3.z - p1.z;

                slopeDir = Math.Sign(dz);
                if (slopeDir == 0)
                {
                    slopeDir = 1;
                }

                if (dx == 0.0f)
                {
                    return;
                }

                _alpha = Mathf.Abs(dz / (dx * 1.0f));

                var tanx = Mathf.Sqrt((circleRadius * circleRadius) / (1.0f + (1.0f / (_alpha * _alpha))));
                var tanz = (1.0f / _alpha) * tanx;

                _betaTanHigh = (p1.z + (tanz)) - (_alpha * (p1.x - tanx));
                _betaTanLow = (p1.z - (tanz)) - (_alpha * (p1.x + tanx));
            }
        }
    }
}
