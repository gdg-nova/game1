/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding.MoveCost
{
    using System;

    /// <summary>
    ///  Euclidean heuristic move cost provider
    /// </summary>
    public class EuclideanDistance : MoveCostDiagonalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanDistance"/> class.
        /// </summary>
        /// <param name="cellMoveCost">The cost to move from one cell to an adjacent cell parallel to ONE axis, i.e. not diagonally</param>
        public EuclideanDistance(int cellMoveCost)
            : base(cellMoveCost)
        {
        }

        /// <summary>
        /// Gets the move cost.
        /// </summary>
        /// <param name="current">The current node.</param>
        /// <param name="other">The other node.</param>
        /// <returns>
        /// The move cost
        /// </returns>
        public override int GetMoveCost(IPathNode current, IPathNode other)
        {
            var dx = (int)(current.position.x - other.position.x);
            var dz = (int)(current.position.z - other.position.z);
            var dy = (int)(current.position.y - other.position.y);

            return this.baseMoveCost * (int)Math.Sqrt((dx * dx) + (dz * dz) + (dy * dy));
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
            var dx = (int)(current.position.x - goal.position.x);
            var dz = (int)(current.position.z - goal.position.z);
            var dy = (int)(current.position.y - goal.position.y);

            return this.baseMoveCost * (int)Math.Sqrt((dx * dx) + (dz * dz) + (dy * dy));
        }
    }
}
