/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding.MoveCost
{
    using System;

    /// <summary>
    /// Cardinal heuristics move cost provider
    /// </summary>
    public class CardinalDistance : MoveCostDiagonalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardinalDistance"/> class.
        /// </summary>
        /// <param name="cellMoveCost">The cost to move from one cell to an adjacent cell.</param>
        public CardinalDistance(int cellMoveCost)
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

            return (int)((Math.Min(dx, dz) * 1.41f) + Math.Max(dx, dz) - Math.Min(dx, dz) + dy);
        }
    }
}
