/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using System.Collections.Generic;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Handles off grid navigation and other pre path finding tasks.
    /// </summary>
    public class DirectPather : IDirectPather
    {
        private List<PerimeterCell> _crossedPerimeters = new List<PerimeterCell>();

        /// <summary>
        /// Resolves the direct path or delegates the request on to path finding.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <returns>A path request to use in path finding or null if the path was resolved.</returns>
        public IPathRequest ResolveDirectPath(IPathRequest req)
        {
            var to = req.to;
            var from = req.from;
            var toGrid = req.toGrid;
            var fromGrid = req.fromGrid;
            var unitMask = req.requester.attributes;

            //If no grids were resolved for this request it means the request involves two points outside the grid(s) that do not cross any grid(s), so we can move directly between them
            if (fromGrid == null && toGrid == null)
            {
                req.Complete(new DirectPathResult(from, to, req));
                return null;
            }

            //Is the start node on a grid
            Cell fromCell = null;
            if (fromGrid != null)
            {
                fromCell = fromGrid.GetCell(from);
            }

            //If the from cell is blocked, we have to get out to a free cell first
            if (fromCell != null && !fromCell.isWalkable(unitMask))
            {
                //Check for a free cell to escape to before resuming the planned path. If no free cell is found we are stuck.
                //The unit may still get stuck even if a free cell is found, in case there is a physically impassable blockade
                var escapeCell = fromGrid.GetNearestWalkableCell(from, from, true, req.maxEscapeCellDistanceIfOriginBlocked, unitMask);
                if (escapeCell == null)
                {
                    req.Complete(DirectPathResult.Fail(PathingStatus.NoRouteExists, req));
                    return null;
                }

                //Move to the free cell and then on with the planned path
                req.Complete(DirectPathResult.CreateWithPath(from, escapeCell.position, to, req));
                return null;
            }

            //Is the destination node on a grid
            Cell toCell = null;
            if (toGrid != null)
            {
                toCell = toGrid.GetCell(to);
            }

            //If the destination is blocked then...
            if (toCell != null && !toCell.isWalkable(unitMask))
            {
                req.Complete(DirectPathResult.Fail(PathingStatus.DestinationBlocked, req));
                return null;
            }

            //If both cells have been resolved, we need a final check to ensure that they are either on the same grid or that a portal exists
            bool crossGrid = (fromGrid != toGrid);
            if (fromCell != null && toCell != null)
            {
                if (!crossGrid || req.preventOffGridNavigation || GridManager.instance.PortalExists(fromGrid, toGrid, unitMask))
                {
                    return req;
                }
            }
            else if (req.preventOffGridNavigation)
            {
                req.Complete(DirectPathResult.Fail(PathingStatus.NoRouteExists, req));
                return null;
            }

            //We have now determined that no path can be found using standard pathfinding, so now we find a path by way of off-grid navigation.
            //Find the perimeters that are in play. There can at most be two. The reference is 'from' unless 'from' is inside the grid, then its 'to'
            //The positions are considered to be both outside, when the from position is outside and either the destination is outside or the from position has an associated (crossed) grid that is different from the destination grid.
            bool bothOutside = (fromCell == null) && (toCell == null || ((fromGrid != null) && crossGrid));
            var reference = (fromCell == null) ? from : to;
            IGrid grid = null;
            if (fromGrid == null)
            {
                grid = toGrid;
            }
            else
            {
                grid = fromGrid;
                req.toGrid = fromGrid;
            }

            _crossedPerimeters.Clear();
            Vector3 crossingPoint;

            if (reference.x < grid.left.edge)
            {
                if (CheckEdgeX(from, to, grid.left, grid.bottom, grid.top, out crossingPoint))
                {
                    var pc = new PerimeterCell(grid.left);
                    pc.cell = grid.GetNearestWalkablePerimeterCell(crossingPoint, Vector3.left, unitMask, true);
                    _crossedPerimeters.Add(pc);
                }
            }
            else if (reference.x > grid.right.edge)
            {
                if (CheckEdgeX(from, to, grid.right, grid.bottom, grid.top, out crossingPoint))
                {
                    var pc = new PerimeterCell(grid.right);
                    pc.cell = grid.GetNearestWalkablePerimeterCell(crossingPoint, Vector3.right, unitMask, true);
                    _crossedPerimeters.Add(pc);
                }
            }

            if (reference.z < grid.bottom.edge)
            {
                if (CheckEdgeZ(from, to, grid.bottom, grid.left, grid.right, out crossingPoint))
                {
                    var pc = new PerimeterCell(grid.bottom);
                    pc.cell = grid.GetNearestWalkablePerimeterCell(crossingPoint, Vector3.back, unitMask, true);
                    _crossedPerimeters.Add(pc);
                }
            }
            else if (reference.z > grid.top.edge)
            {
                if (CheckEdgeZ(from, to, grid.top, grid.left, grid.right, out crossingPoint))
                {
                    var pc = new PerimeterCell(grid.top);
                    pc.cell = grid.GetNearestWalkablePerimeterCell(crossingPoint, Vector3.forward, unitMask, true);
                    _crossedPerimeters.Add(pc);
                }
            }

            //If no actual boundary crossings exist, it means both are outside the grid and a direct route between them, that will not cross the grid exists.
            if (_crossedPerimeters.Count == 0)
            {
                req.Complete(new DirectPathResult(from, to, req));
                return null;
            }

            //if from and to are both outside the grid, but the path between them does cross a perimeter.
            //No direct path exist so just go to the corner nearest to 'to' and try again, since there is a path.
            float shortestDistance = float.MaxValue;
            if (bothOutside)
            {
                Vector3 closestCorner = Vector3.zero;

                for (int i = 0; i < _crossedPerimeters.Count; i++)
                {
                    var p = _crossedPerimeters[i].perimeter;
                    var d1 = (to - p.outsideCornerOne).sqrMagnitude;
                    var d2 = (to - p.outsideCornerTwo).sqrMagnitude;

                    if (d1 < shortestDistance)
                    {
                        shortestDistance = d1;
                        closestCorner = p.outsideCornerOne;
                    }

                    if (d2 < shortestDistance)
                    {
                        shortestDistance = d2;
                        closestCorner = p.outsideCornerTwo;
                    }
                }

                req.Complete(DirectPathResult.CreateWithPath(from, closestCorner, to, req));
                return null;
            }

            //Find the closest edge cell
            IGridCell edgeCell = null;
            Perimeter targetPerimeter = null;
            for (int i = 0; i < _crossedPerimeters.Count; i++)
            {
                var p = _crossedPerimeters[i];
                if (p.cell == null)
                {
                    continue;
                }

                var d = (from - p.cell.position).sqrMagnitude + (to - p.cell.position).sqrMagnitude;
                if (d < shortestDistance)
                {
                    shortestDistance = d;
                    edgeCell = p.cell;
                    targetPerimeter = p.perimeter;
                }
            }

            Vector3 firstOutsidePoint;
            //At least one actual boundary crossing was found, but no walkable cells were found on that boundary.
            //Now we could check perpendiculars all (at most 2) perimeters to be sure to get the closest route, but...
            if (edgeCell == null)
            {
                targetPerimeter = _crossedPerimeters[0].perimeter;

                //Check the two perpendicular perimeters
                var cellOne = grid.GetNearestWalkablePerimeterCell(targetPerimeter.insideCornerOne, targetPerimeter.perpendicularOne.edgeVector, unitMask, false);
                var cellTwo = grid.GetNearestWalkablePerimeterCell(targetPerimeter.insideCornerTwo, targetPerimeter.perpendicularTwo.edgeVector, unitMask, false);

                //If both have a point of entry, choose the one resulting in the shortest path
                if (cellOne != null && cellTwo != null)
                {
                    var dOne = (from - targetPerimeter.outsideCornerOne).sqrMagnitude + (to - targetPerimeter.outsideCornerOne).sqrMagnitude + (cellOne.position - targetPerimeter.outsideCornerOne).sqrMagnitude;
                    var dTwo = (from - targetPerimeter.outsideCornerTwo).sqrMagnitude + (to - targetPerimeter.outsideCornerTwo).sqrMagnitude + (cellTwo.position - targetPerimeter.outsideCornerTwo).sqrMagnitude;

                    if (dOne < dTwo)
                    {
                        cellTwo = null;
                    }
                    else
                    {
                        cellOne = null;
                    }
                }

                if (cellOne != null)
                {
                    //If moving from outside the grid to the inside, just move to the corner
                    if (fromCell == null)
                    {
                        req.Complete(DirectPathResult.CreateWithPath(from, targetPerimeter.outsideCornerOne, to, req));
                        return null;
                    }

                    //Otherwise request a path to the edgeCell with a way point just outside + the destination
                    firstOutsidePoint = cellOne.position + (targetPerimeter.perpendicularOne.edgeVector * grid.cellSize);
                    return UpdateRequest(req, cellOne, firstOutsidePoint, to);
                }
                else if (cellTwo != null)
                {
                    //If moving from outside the grid to the inside, just move to the corner
                    if (fromCell == null)
                    {
                        req.Complete(DirectPathResult.CreateWithPath(from, targetPerimeter.outsideCornerTwo, to, req));
                        return null;
                    }

                    //Otherwise request a path to the edgeCell with a way point just outside + the destination
                    firstOutsidePoint = cellTwo.position + (targetPerimeter.perpendicularTwo.edgeVector * grid.cellSize);
                    return UpdateRequest(req, cellTwo, firstOutsidePoint, to);
                }
                else
                {
                    //Check the opposing side
                    var opposing = targetPerimeter.perpendicularOne.perpendicularOne;

                    edgeCell = grid.GetNearestWalkablePerimeterCell(opposing.GetPoint(to), opposing.edgeVector, unitMask, true);
                    if (edgeCell == null)
                    {
                        req.Complete(DirectPathResult.Fail(PathingStatus.NoRouteExists, req));
                        return null;
                    }

                    //If moving from outside the grid to the inside, still just move to corner from which the shortest route will go
                    if (fromCell == null)
                    {
                        var c1 = (from - targetPerimeter.outsideCornerOne).sqrMagnitude + (edgeCell.position - targetPerimeter.outsideCornerOne).sqrMagnitude;
                        var c2 = (from - targetPerimeter.outsideCornerTwo).sqrMagnitude + (edgeCell.position - targetPerimeter.outsideCornerTwo).sqrMagnitude;
                        var corner = (c1 < c2) ? targetPerimeter.outsideCornerOne : targetPerimeter.outsideCornerTwo;

                        req.Complete(DirectPathResult.CreateWithPath(from, corner, to, req));
                        return null;
                    }

                    //Otherwise request a path to the edgeCell with a way point just outside + the destination
                    firstOutsidePoint = edgeCell.position + (opposing.edgeVector * grid.cellSize);
                    return UpdateRequest(req, edgeCell, firstOutsidePoint, to);
                }
            }

            //We found a valid perimeter crossing point, so create the path
            firstOutsidePoint = edgeCell.position + (targetPerimeter.edgeVector * grid.cellSize);

            //outside in
            if (fromCell == null)
            {
                if (from == firstOutsidePoint)
                {
                    req.Complete(DirectPathResult.CreateWithPath(from, edgeCell.position, to, req));
                }
                else
                {
                    req.Complete(DirectPathResult.CreateWithPath(new Vector3[] { from, firstOutsidePoint, edgeCell.position }, to, req));
                }

                return null;
            }

            //If we are currently on the grid, first see if we are at the suggested edge cell, if so we can walk straight to the destination outside
            if (edgeCell == fromCell)
            {
                if (firstOutsidePoint == to)
                {
                    req.Complete(new DirectPathResult(from, to, req));
                    return null;
                }

                //While it would be possible to make a complete path here, we want to use a way point since 'to' could be inside another grid, so we need to do another request there
                req.Complete(DirectPathResult.CreateWithPath(new Vector3[] { from, firstOutsidePoint }, to, req));
                return null;
            }

            //inside out
            return UpdateRequest(req, edgeCell, firstOutsidePoint, to);
        }

        private static IPathRequest UpdateRequest(IPathRequest req, IGridCell to, params Vector3[] waypoints)
        {
            req.to = to.position;
            req.pendingWaypoints = waypoints;

            return req;
        }

        private static bool CheckEdgeZ(Vector3 from, Vector3 to, Perimeter edgeZ, Perimeter edgeMinX, Perimeter edgeMaxX, out Vector3 crossingPoint)
        {
            if ((from.z < edgeZ.edge && to.z < edgeZ.edge) || (from.z > edgeZ.edge && to.z > edgeZ.edge) ||
                (from.x < edgeMinX.edge && to.x < edgeMinX.edge) || (from.x > edgeMaxX.edge && to.x > edgeMaxX.edge))
            {
                crossingPoint = Vector3.zero;
                return false;
            }

            var heading = to - from;

            var dzmin = edgeZ.edge - from.z;

            crossingPoint = new Vector3
            {
                x = from.x + ((heading.x / heading.z) * dzmin),
                z = edgeZ.edgeMid
            };

            return true;
        }

        private static bool CheckEdgeX(Vector3 from, Vector3 to, Perimeter edgeX, Perimeter edgeMinZ, Perimeter edgeMaxZ, out Vector3 crossingPoint)
        {
            if ((from.x < edgeX.edge && to.x < edgeX.edge) || (from.x > edgeX.edge && to.x > edgeX.edge) ||
                (from.z < edgeMinZ.edge && to.z < edgeMinZ.edge) || (from.z > edgeMaxZ.edge && to.z > edgeMaxZ.edge))
            {
                crossingPoint = Vector3.zero;
                return false;
            }

            var heading = to - from;

            var dxmin = edgeX.edge - from.x;

            crossingPoint = new Vector3
            {
                x = edgeX.edgeMid,
                z = from.z + ((heading.z / heading.x) * dxmin)
            };

            return true;
        }

        private struct PerimeterCell
        {
            public Perimeter perimeter;
            public IGridCell cell;

            public PerimeterCell(Perimeter p)
            {
                perimeter = p;
                cell = null;
            }
        }
    }
}
