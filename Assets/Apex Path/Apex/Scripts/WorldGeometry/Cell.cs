/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System.Collections.Generic;
    using Apex.Common;
    using Apex.DataStructures;
    using Apex.PathFinding;
    using Apex.Utilities;
    using UnityEngine;

    /// <summary>
    /// Represents a single grid cell.
    /// </summary>
    public class Cell : IPathNode
    {
        private CellMatrix _parent;
        private bool _permanentlyBlocked;
        private int _blockMask;
        private NeighbourPosition _neighbours;
        private List<IPathNode> _virtualNeighbours;
        private List<IDynamicObstacle> _dynamicObstacles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="parent">The cell matrix that owns this cell.</param>
        /// <param name="position">The position.</param>
        /// <param name="matrixPosX">The matrix position x.</param>
        /// <param name="matrixPosZ">The matrix position z.</param>
        /// <param name="blocked">if set to <c>true</c> the cell will appear permanently blocked.</param>
        public Cell(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked)
        {
            Ensure.ArgumentNotNull(parent, "parent");

            this.position = position;
            this.matrixPosX = matrixPosX;
            this.matrixPosZ = matrixPosZ;

            _parent = parent;
            _permanentlyBlocked = blocked;

            _neighbours = NeighbourPosition.None;

            if (matrixPosX < (parent.columns - 1))
            {
                _neighbours |= NeighbourPosition.Right;

                if (matrixPosZ < (parent.rows - 1))
                {
                    _neighbours |= (NeighbourPosition.Top | NeighbourPosition.TopRight);
                }

                if (matrixPosZ > 0)
                {
                    _neighbours |= (NeighbourPosition.Bottom | NeighbourPosition.BottomRight);
                }
            }

            if (matrixPosX > 0)
            {
                _neighbours |= NeighbourPosition.Left;

                if (matrixPosZ < (parent.rows - 1))
                {
                    _neighbours |= (NeighbourPosition.Top | NeighbourPosition.TopLeft);
                }

                if (matrixPosZ > 0)
                {
                    _neighbours |= (NeighbourPosition.Bottom | NeighbourPosition.BottomLeft);
                }
            }
        }

        /// <summary>
        /// Gets the parent cell matrix.
        /// </summary>
        /// <value>
        /// The parent matrix.
        /// </value>
        public CellMatrix parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the position of the cell.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector3 position
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the cell's x position in the grid matrix
        /// </summary>
        /// <value>
        /// The matrix position x.
        /// </value>
        public int matrixPosX
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the cell's z position in the grid matrix
        /// </summary>
        /// <value>
        /// The matrix position z.
        /// </value>
        public int matrixPosZ
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the arbitrary cost of walking this cell.
        /// </summary>
        /// <value>
        /// The cost.
        /// </value>
        public int cost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the mask of neighbours for the cell.
        /// </summary>
        /// <value>
        /// The neighbours mask.
        /// </value>
        public NeighbourPosition neighbours
        {
            get { return _neighbours; }
        }

        /// <summary>
        /// Gets or sets the mask of height blocked neighbours, i.e. neighbours that are not walkable from this cell to to the slope angle between them being too big.
        /// </summary>
        /// <value>
        /// The height blocked neighbours mask.
        /// </value>
        public NeighbourPosition heightBlockedNeighbours
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this cell is permanently blocked.
        /// Note that this is automatically set depending on level geometry when the Grid initializes, but it can also be changed manually.
        /// </summary>
        public bool isPermanentlyBlocked
        {
            get { return _permanentlyBlocked; }
            set { _permanentlyBlocked = value; }
        }

        int IPathNode.g
        {
            get;
            set;
        }

        int IPathNode.h
        {
            get;
            set;
        }

        int IPathNode.f
        {
            get;
            set;
        }

        IPathNode IPathNode.predecessor
        {
            get;
            set;
        }

        bool IPathNode.isClosed
        {
            get;
            set;
        }

        bool IPathNode.hasVirtualNeighbour
        {
            get { return (_virtualNeighbours != null) && (_virtualNeighbours.Count > 0); }
        }

        /// <summary>
        /// Determines whether the cell is walkable.
        /// </summary>
        /// <param name="mask">The attribute mask used to determine walk-ability.</param>
        /// <returns>
        ///   <c>true</c> if the cell is walkable, otherwise <c>false</c>
        /// </returns>
        public bool isWalkable(AttributeMask mask)
        {
            if (_permanentlyBlocked)
            {
                return false;
            }

            if (mask == AttributeMask.None)
            {
                return (_blockMask == AttributeMask.None);
            }

            return (_blockMask & mask) != mask;
        }

        /// <summary>
        /// Determines whether the cell is walkable from all directions.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns><c>true</c> if the cell is walkable, otherwise <c>false</c></returns>
        public bool isWalkableFromAllDirections(AttributeMask mask)
        {
            if (!isWalkable(mask))
            {
                return false;
            }

            return (this.heightBlockedNeighbours == NeighbourPosition.None);
        }

        /// <summary>
        /// Determines whether the cell is walkable from the specified neighbour position.
        /// </summary>
        /// <param name="pos">The neighbour position.</param>
        /// <param name="mask">The attribute mask used to determine walk-ability.</param>
        /// <returns><c>true</c> if the cell is walkable, otherwise <c>false</c></returns>
        public bool isWalkableFrom(NeighbourPosition pos, AttributeMask mask)
        {
            if (!isWalkable(mask))
            {
                return false;
            }

            return (this.heightBlockedNeighbours & pos) == 0;
        }

        /// <summary>
        /// Determines whether the cell is walkable from the specified neighbour.
        /// </summary>
        /// <param name="neighbour">The neighbour.</param>
        /// <param name="mask">The attribute mask used to determine walk-ability.</param>
        /// <returns><c>true</c> if the cell is walkable, otherwise <c>false</c></returns>
        public bool isWalkableFrom(IGridCell neighbour, AttributeMask mask)
        {
            var pos = neighbour.GetRelativePositionTo(this);

            return isWalkableFrom(pos, mask);
        }

        /// <summary>
        /// Gets this cell's relative position to the other cell.
        /// </summary>
        /// <param name="other">The other cell.</param>
        /// <returns>
        /// The relative position
        /// </returns>
        public NeighbourPosition GetRelativePositionTo(IGridCell other)
        {
            var dx = Mathf.Clamp(this.matrixPosX - other.matrixPosX, -1, 1);
            var dz = Mathf.Clamp(this.matrixPosZ - other.matrixPosZ, -1, 1);

            //____________
            //|_6_|_7_|_8_| 1
            //|_3_|_4_|_5_| 0
            //|_0_|_1_|_2_|-1
            // -1   0   1
            return (NeighbourPosition)(1 << (dx + (3 * dz) + 4));
        }

        /// <summary>
        /// Gets the direction to a neighbouring cell in matrix deltas.
        /// </summary>
        /// <param name="other">The other cell.</param>
        /// <returns>
        /// A vector representing the matrix deltas to apply to reach the other cell in the matrix.
        /// </returns>
        public VectorXZ GetDirectionTo(IGridCell other)
        {
            var dx = Mathf.Clamp(other.matrixPosX - this.matrixPosX, -1, 1) + 1;
            var dz = Mathf.Clamp(other.matrixPosZ - this.matrixPosZ, -1, 1) + 1;

            return MatrixDirection.Directions[(dz * 3) + dx];
        }

        /// <summary>
        /// Gets the neighbour at the specified matrix offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// The neighbour cell or null
        /// </returns>
        public Cell GetNeighbour(VectorXZ offset)
        {
            return GetNeighbour(offset.x, offset.z);
        }

        /// <summary>
        /// Gets the neighbour (or other cell for that matter) at the specified matrix index.
        /// </summary>
        /// <param name="dx">The x offset.</param>
        /// <param name="dz">The z offset.</param>
        /// <returns>
        /// he neighbour cell or null
        /// </returns>
        public Cell GetNeighbour(int dx, int dz)
        {
            var x = this.matrixPosX + dx;
            var z = this.matrixPosZ + dz;

            return _parent[x, z];
        }

        bool IPathNode.TryGetWalkableNeighbour(int dx, int dz, AttributeMask requesterAttributes, DynamicArray<IPathNode> neighbours)
        {
            var x = this.matrixPosX + dx;
            var z = this.matrixPosZ + dz;

            var neighbour = _parent[x, z];

            if (neighbour == null)
            {
                return false;
            }

            if (neighbour.isWalkableFrom(this, requesterAttributes))
            {
                neighbours.Add(neighbour);
                return true;
            }

            return false;
        }

        void IPathNode.GetWalkableNeighbours(DynamicArray<IPathNode> neighbours, AttributeMask requesterAttributes, bool cornerCuttingAllowed, bool preventDiagonalMoves)
        {
            var @this = this as IPathNode;

            //Straight move neighbours
            bool uw = @this.TryGetWalkableNeighbour(0, 1, requesterAttributes, neighbours);

            bool dw = @this.TryGetWalkableNeighbour(0, -1, requesterAttributes, neighbours);

            bool rw = @this.TryGetWalkableNeighbour(1, 0, requesterAttributes, neighbours);

            bool lw = @this.TryGetWalkableNeighbour(-1, 0, requesterAttributes, neighbours);

            if (preventDiagonalMoves)
            {
                return;
            }

            //Diagonal neighbours. First determine if they are unwalkable as a consequence of their straight neighbours
            bool urw, drw, dlw, ulw;
            if (cornerCuttingAllowed)
            {
                urw = uw || rw;
                drw = dw || rw;
                dlw = dw || lw;
                ulw = uw || lw;
            }
            else
            {
                urw = uw && rw;
                drw = dw && rw;
                dlw = dw && lw;
                ulw = uw && lw;
            }

            if (urw)
            {
                @this.TryGetWalkableNeighbour(1, 1, requesterAttributes, neighbours);
            }

            if (drw)
            {
                @this.TryGetWalkableNeighbour(1, -1, requesterAttributes, neighbours);
            }

            if (dlw)
            {
                @this.TryGetWalkableNeighbour(-1, -1, requesterAttributes, neighbours);
            }

            if (ulw)
            {
                @this.TryGetWalkableNeighbour(-1, 1, requesterAttributes, neighbours);
            }
        }

        void IPathNode.GetVirtualNeighbours(DynamicArray<IPathNode> neighbours, AttributeMask requesterAttributes)
        {
            if (_virtualNeighbours == null)
            {
                return;
            }

            for (int i = 0; i < _virtualNeighbours.Count; i++)
            {
                var vn = _virtualNeighbours[i];
                if (vn.isWalkable(requesterAttributes))
                {
                    neighbours.Add(vn);
                }
            }
        }

        void IPathNode.RegisterVirtualNeighbour(IPathNode neighbour)
        {
            if (_virtualNeighbours == null)
            {
                _virtualNeighbours = new List<IPathNode>(1);
            }

            _virtualNeighbours.Add(neighbour);
        }

        void IPathNode.UnregisterVirtualNeighbour(IPathNode neighbour)
        {
            if (_virtualNeighbours != null)
            {
                _virtualNeighbours.Remove(neighbour);
            }
        }

        internal void UpdateState(float newHeight, bool isBlocked)
        {
            var pos = this.position;
            this.position = new Vector3(pos.x, newHeight, pos.z);
            _permanentlyBlocked = isBlocked;
        }

        internal bool AddDynamicObstacle(IDynamicObstacle dynamicObstacle)
        {
            if (_permanentlyBlocked)
            {
                return false;
            }

            if (_dynamicObstacles == null)
            {
                _dynamicObstacles = new List<IDynamicObstacle>();
            }

            _dynamicObstacles.AddUnique(dynamicObstacle);

            return UpdateExclusionMask();
        }

        internal bool RemoveDynamicObstacle(IDynamicObstacle dynamicObstacle)
        {
            if (!_permanentlyBlocked && _dynamicObstacles != null)
            {
                _dynamicObstacles.Remove(dynamicObstacle);

                return UpdateExclusionMask();
            }

            return false;
        }

        private bool UpdateExclusionMask()
        {
            var prev = _blockMask;

            _blockMask = 0;
            for (int i = 0; i < _dynamicObstacles.Count; i++)
            {
                _blockMask |= ~_dynamicObstacles[i].exceptionsMask;
            }

            return (prev != _blockMask);
        }
    }
}
