/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding.MoveCost
{
    using System;

    /// <summary>
    /// Diagonal distance heuristic. Assumes distance between cells along axis are whole numbers, i.e. a uniform grid.
    /// Cell move cost constant D and D2 (_cellMoveCost) are used, where D2 = sqrt(2)*D. H = D * (dx + dz) + (D2 - 2 * D) * min(dx, dz)
    /// </summary>
    public class DiagonalDistance : MoveCostDiagonalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagonalDistance"/> class.
        /// </summary>
        /// <param name="cellMoveCost">The cost of moving from one cell to an adjacent cell.</param>
        public DiagonalDistance(int cellMoveCost)
            : base(cellMoveCost)
        {
        }

        /// <summary>
        /// Gets the heuristic.
        /// </summary>
        /// <param name="current">The current node.</param>
        /// <param name="goal">The goal node.</param>
        /// <returns>
        /// The heuristic
        /// </returns>
        public override int GetHeuristic(IPathNode current, IPathNode goal)
        {
            var dx = (int)Math.Abs(current.position.x - goal.position.x);
            var dz = (int)Math.Abs(current.position.z - goal.position.z);
            var dy = (int)Math.Abs(current.position.y - goal.position.y);

            return (this.baseMoveCost * (dx + dz + dy)) + ((this.diagonalMoveCost - (2 * this.baseMoveCost)) * Math.Min(dx, dz));
        }
    }
}
