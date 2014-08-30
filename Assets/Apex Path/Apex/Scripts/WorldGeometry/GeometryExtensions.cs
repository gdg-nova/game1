/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Apex.Common;
    using UnityEngine;

    /// <summary>
    /// Various geometry related extensions.
    /// </summary>
    public static partial class GeometryExtensions
    {
        /// <summary>
        /// Gets the nearest walkable cell to a position.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position to initiate from.</param>
        /// <param name="inRelationTo">The position of approach.</param>
        /// <param name="requireWalkableFromPosition">Whether the cell to find must be accessible from the direction of <paramref name="position"/></param>
        /// <param name="maxCellDistance">The maximum cell distance to check.</param>
        /// <param name="requesterAttributes">The requester's attributes.</param>
        /// <returns>The first walkable cell in the neighbourhood of <paramref name="position"/> that is the closest to <paramref name="inRelationTo"/>. If no such cell is found, null is returned.</returns>
        public static Cell GetNearestWalkableCell(this IGrid grid, Vector3 position, Vector3 inRelationTo, bool requireWalkableFromPosition, int maxCellDistance, AttributeMask requesterAttributes)
        {
            var cell = grid.GetCell(position);
            if (cell == null)
            {
                return null;
            }

            if (cell.isWalkable(requesterAttributes))
            {
                return cell;
            }

            int dist = 1;
            var candidates = new List<Cell>();
            while (candidates.Count == 0 && dist <= maxCellDistance)
            {
                foreach (var c in grid.GetConcentricNeighbours(cell, dist++))
                {
                    if (requireWalkableFromPosition)
                    {
                        if (c.isWalkableFrom(cell, requesterAttributes))
                        {
                            candidates.Add(c);
                        }
                    }
                    else if (c.isWalkable(requesterAttributes))
                    {
                        candidates.Add(c);
                    }
                }
            }

            Cell winner = null;
            float lowestDist = float.MaxValue;
            for (int i = 0; i < candidates.Count; i++)
            {
                var distSqr = (candidates[i].position - inRelationTo).sqrMagnitude;
                if (distSqr < lowestDist)
                {
                    winner = candidates[i];
                    lowestDist = distSqr;
                }
            }

            return winner;
        }

        /// <summary>
        /// Gets the walkable neighbours.
        /// </summary>
        /// <param name="grid">The grid</param>
        /// <param name="c">The cell whose walkable neighbours to return.</param>
        /// <param name="requesterAttributes">The attributes of the requesting unit</param>
        /// <param name="excludeCornerCutting">if set to <c>true</c> otherwise walkable neighbours on the diagonal that would cause a move from it to the current cell to cut a corner are excluded (deemed not walkable).</param>
        /// <returns>The walkable neighbours to the referenced cell.</returns>
        public static IEnumerable<Cell> GetWalkableNeighbours(this IGrid grid, IGridCell c, AttributeMask requesterAttributes, bool excludeCornerCutting)
        {
            Cell n;

            //Straight move neighbours
            bool uw = grid.TryGetWalkableNeighbour(c, 0, 1, requesterAttributes, out n);
            if (uw)
            {
                yield return n;
            }

            bool dw = grid.TryGetWalkableNeighbour(c, 0, -1, requesterAttributes, out n);
            if (dw)
            {
                yield return n;
            }

            bool rw = grid.TryGetWalkableNeighbour(c, 1, 0, requesterAttributes, out n);
            if (rw)
            {
                yield return n;
            }

            bool lw = grid.TryGetWalkableNeighbour(c, -1, 0, requesterAttributes, out n);
            if (lw)
            {
                yield return n;
            }

            //Diagonal neighbours. First determine if they are unwalkable as a consequence of their straight neighbours
            bool urw, drw, dlw, ulw;
            if (excludeCornerCutting)
            {
                urw = uw && rw;
                drw = dw && rw;
                dlw = dw && lw;
                ulw = uw && lw;
            }
            else
            {
                urw = uw || rw;
                drw = dw || rw;
                dlw = dw || lw;
                ulw = uw || lw;
            }

            urw = urw && grid.TryGetWalkableNeighbour(c, 1, 1, requesterAttributes, out n);
            if (urw)
            {
                yield return n;
            }

            drw = drw && grid.TryGetWalkableNeighbour(c, 1, -1, requesterAttributes, out n);
            if (drw)
            {
                yield return n;
            }

            dlw = dlw && grid.TryGetWalkableNeighbour(c, -1, -1, requesterAttributes, out n);
            if (dlw)
            {
                yield return n;
            }

            ulw = ulw && grid.TryGetWalkableNeighbour(c, -1, 1, requesterAttributes, out n);
            if (ulw)
            {
                yield return n;
            }
        }

        /// <summary>
        /// Determines whether a cell is walkable to all (regardless of their attributes).
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns><c>true</c> if walkable to all units, otherwise <c>false</c></returns>
        public static bool IsWalkableToAll(this IGridCell cell)
        {
            return cell.isWalkable(AttributeMask.None);
        }

        /// <summary>
        /// Determines whether is walkable to any unit, i.e. if at least one of the defined special attributes will make the cell resolve as walkable.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns><c>true</c> if walkable to any units, otherwise <c>false</c></returns>
        public static bool IsWalkableToAny(this IGridCell cell)
        {
            return cell.isWalkable(AttributeMask.All);
        }
    }
}
