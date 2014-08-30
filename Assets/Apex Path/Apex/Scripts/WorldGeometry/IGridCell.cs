/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using Apex.Common;
    using Apex.DataStructures;

    /// <summary>
    /// Interface for grid cell basic properties.
    /// </summary>
    public interface IGridCell : IPositioned
    {
        /// <summary>
        /// Gets the parent cell matrix.
        /// </summary>
        /// <value>
        /// The parent matrix.
        /// </value>
        CellMatrix parent { get; }

        /// <summary>
        /// Gets the cell's x position in the grid matrix
        /// </summary>
        /// <value>
        /// The matrix position x.
        /// </value>
        int matrixPosX { get; }

        /// <summary>
        /// Gets the cell's z position in the grid matrix
        /// </summary>
        /// <value>
        /// The matrix position z.
        /// </value>
        int matrixPosZ { get; }

        /// <summary>
        /// Gets the arbitrary cost of walking this cell.
        /// </summary>
        /// <value>
        /// The cost.
        /// </value>
        int cost { get; }

        /// <summary>
        /// Gets the mask of neighbours for the cell.
        /// </summary>
        /// <value>
        /// The neighbours mask.
        /// </value>
        NeighbourPosition neighbours { get; }

        /// <summary>
        /// Gets or sets the mask of height blocked neighbours, i.e. neighbours that are not walkable from this cell to to the slope angle between them being too big.
        /// </summary>
        /// <value>
        /// The height blocked neighbours mask.
        /// </value>
        NeighbourPosition heightBlockedNeighbours { get; }

        /// <summary>
        /// Determines whether the cell is walkable.
        /// </summary>
        /// <param name="mask">The attribute mask used to determine walk-ability.</param>
        /// <returns><c>true</c> if the cell is walkable, otherwise <c>false</c></returns>
        bool isWalkable(AttributeMask mask);

        /// <summary>
        /// Determines whether the cell is walkable from all directions.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns><c>true</c> if the cell is walkable, otherwise <c>false</c></returns>
        bool isWalkableFromAllDirections(AttributeMask mask);

        /// <summary>
        /// Determines whether the cell is walkable from the specified neighbour position.
        /// </summary>
        /// <param name="pos">The neighbour position.</param>
        /// <param name="mask">The attribute mask used to determine walk-ability.</param>
        /// <returns><c>true</c> if the cell is walkable, otherwise <c>false</c></returns>
        bool isWalkableFrom(NeighbourPosition pos, AttributeMask mask);

        /// <summary>
        /// Determines whether the cell is walkable from the specified neighbour.
        /// </summary>
        /// <param name="neighbour">The neighbour.</param>
        /// <param name="mask">The attribute mask used to determine walk-ability.</param>
        /// <returns><c>true</c> if the cell is walkable, otherwise <c>false</c></returns>
        bool isWalkableFrom(IGridCell neighbour, AttributeMask mask);

        /// <summary>
        /// Gets this cell's relative position to the other cell.
        /// </summary>
        /// <param name="other">The other cell.</param>
        /// <returns>The relative position</returns>
        NeighbourPosition GetRelativePositionTo(IGridCell other);

        /// <summary>
        /// Gets the direction to a neighbouring cell in matrix deltas.
        /// </summary>
        /// <param name="other">The other cell.</param>
        /// <returns>A vector representing the matrix deltas to apply to reach the other cell in the matrix.</returns>
        VectorXZ GetDirectionTo(IGridCell other);

        /// <summary>
        /// Gets the neighbour at the specified matrix offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The neighbour cell or null</returns>
        Cell GetNeighbour(VectorXZ offset);

        /// <summary>
        /// Gets the neighbour (or other cell for that matter) at the specified matrix index.
        /// </summary>
        /// <param name="dx">The x offset.</param>
        /// <param name="dz">The z offset.</param>
        /// <returns>he neighbour cell or null</returns>
        Cell GetNeighbour(int dx, int dz);
    }
}
