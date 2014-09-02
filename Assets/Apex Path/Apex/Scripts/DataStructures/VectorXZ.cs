/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.DataStructures
{
    /// <summary>
    /// A vector in the XZ plane.
    /// </summary>
    public struct VectorXZ
    {
        /// <summary>
        /// The x coordinate
        /// </summary>
        public int x;

        /// <summary>
        /// The z coordinate
        /// </summary>
        public int z;

        /// <summary>
        /// Initializes a new instance of the <see cref="VectorXZ"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public VectorXZ(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static VectorXZ operator +(VectorXZ a, VectorXZ b)
        {
            return new VectorXZ(a.x + b.x, a.z + b.z);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static VectorXZ operator -(VectorXZ a, VectorXZ b)
        {
            return new VectorXZ(a.x - b.x, a.z - b.z);
        }
    }
}
