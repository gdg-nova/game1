/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.DataStructures
{
    /// <summary>
    /// Represents a matrix boundary consisting of min/max values for matrix indexes.
    /// </summary>
    public struct MatrixBounds
    {
        /// <summary>
        /// The minimum column index
        /// </summary>
        public int minColumn;

        /// <summary>
        /// The maximum column index
        /// </summary>
        public int maxColumn;

        /// <summary>
        /// The minimum row index
        /// </summary>
        public int minRow;

        /// <summary>
        /// The maximum row index
        /// </summary>
        public int maxRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixBounds"/> struct.
        /// </summary>
        /// <param name="minColumn">The minimum column index.</param>
        /// <param name="minRow">The minimum row index.</param>
        /// <param name="maxColumn">The maximum column index.</param>
        /// <param name="maxRow">The maximum row index.</param>
        public MatrixBounds(int minColumn, int minRow, int maxColumn, int maxRow)
        {
            this.minColumn = minColumn;
            this.minRow = minRow;
            this.maxColumn = maxColumn;
            this.maxRow = maxRow;
        }
    }
}
