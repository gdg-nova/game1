/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System;
    using System.Linq;
    using Apex.Common;
    using Apex.DataStructures;
    using Apex.PathFinding;
    using Apex.PathFinding.MoveCost;
    using UnityEngine;

    /// <summary>
    /// Represents a virtual cell that represents one end of a portal.
    /// </summary>
    public class PortalCell : IPortalNode
    {
        private GridPortal _parentPortal;
        private IPortalAction _action;
        private PortalCell _partner;
        private IPathNode[] _neighbourNodes;

        internal PortalCell(GridPortal parentPortal, IPortalAction action)
        {
            _parentPortal = parentPortal;
            _action = action;
        }

        /// <summary>
        /// Gets the portal to which this cell belongs.
        /// </summary>
        /// <value>
        /// The portal.
        /// </value>
        public GridPortal portal
        {
            get { return _parentPortal; }
        }

        /// <summary>
        /// Gets the partner portal cell.
        /// </summary>
        /// <value>
        /// The partner.
        /// </value>
        public IPortalNode partner
        {
            get { return _partner; }
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
        /// Gets the position of the component.
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
        /// Gets the parent cell matrix.
        /// </summary>
        /// <value>
        /// The parent matrix.
        /// </value>
        public CellMatrix parent
        {
            get;
            private set;
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
            get { return false; }
        }

        int IGridCell.matrixPosX
        {
            get { return -1; }
        }

        int IGridCell.matrixPosZ
        {
            get { return -1; }
        }

        NeighbourPosition IGridCell.neighbours
        {
            get { return NeighbourPosition.None; }
        }

        NeighbourPosition IGridCell.heightBlockedNeighbours
        {
            get { return NeighbourPosition.None; }
        }

        /// <summary>
        /// Gets the heuristic from this portal to the goal.
        /// </summary>
        /// <param name="goal">The goal.</param>
        /// <param name="moveCostProvider">The move cost provider.</param>
        /// <returns>The heuristic</returns>
        public int GetHeuristic(IPathNode goal, IMoveCost moveCostProvider)
        {
            return moveCostProvider.GetHeuristic(_partner, goal);
        }

        /// <summary>
        /// Gets the heuristic for a node in relation to this portal.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="goal">The goal.</param>
        /// <param name="moveCostProvider">The move cost provider.</param>
        /// <returns>The heuristic</returns>
        public int GetHeuristic(IPathNode node, IPathNode goal, IMoveCost moveCostProvider)
        {
            return moveCostProvider.GetHeuristic(node, this) + moveCostProvider.GetHeuristic(_partner, goal);
        }

        /// <summary>
        /// Gets the action cost.
        /// </summary>
        /// <param name="from">The node from which the action will start.</param>
        /// <param name="to">The node at which the action will end.</param>
        /// <returns>The cost</returns>
        public int GetCost(IPositioned from, IPositioned to)
        {
            return _action.GetActionCost(from, to);
        }

        /// <summary>
        /// Executes the portal move.
        /// </summary>
        /// <param name="unit">The unit that is entering the portal.</param>
        /// <param name="to">The destination at the other side of the portal.</param>
        /// <param name="callWhenComplete">The callback to call when the move is complete.</param>
        /// <returns>The grid of the destination.</returns>
        public IGrid Execute(Transform unit, IPositioned to, Action callWhenComplete)
        {
            _action.Execute(unit, this, to, callWhenComplete);

            return _parentPortal.GetGridFor(this);
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
            return _parentPortal.IsUsableBy(mask);
        }

        bool IGridCell.isWalkableFromAllDirections(AttributeMask mask)
        {
            return isWalkable(mask);
        }

        bool IGridCell.isWalkableFrom(NeighbourPosition pos, AttributeMask mask)
        {
            return _parentPortal.IsUsableBy(mask);
        }

        bool IGridCell.isWalkableFrom(IGridCell neighbour, AttributeMask mask)
        {
            return _parentPortal.IsUsableBy(mask);
        }

        NeighbourPosition IGridCell.GetRelativePositionTo(IGridCell other)
        {
            return NeighbourPosition.None;
        }

        VectorXZ IGridCell.GetDirectionTo(IGridCell other)
        {
            return MatrixDirection.None;
        }

        Cell IGridCell.GetNeighbour(VectorXZ offset)
        {
            return null;
        }

        Cell IGridCell.GetNeighbour(int dx, int dz)
        {
            return null;
        }

        void IPathNode.GetWalkableNeighbours(DynamicArray<IPathNode> neighbours, AttributeMask requesterAttributes, bool cornerCuttingAllowed, bool preventDiagonalMoves)
        {
            var destinationNodes = _partner._neighbourNodes;
            var nodeCount = destinationNodes.Length;
            for (int i = 0; i < nodeCount; i++)
            {
                if (destinationNodes[i].isWalkable(requesterAttributes))
                {
                    neighbours.Add(destinationNodes[i]);
                }
            }
        }

        bool IPathNode.TryGetWalkableNeighbour(int dx, int dz, AttributeMask requesterAttributes, DynamicArray<IPathNode> neighbours)
        {
            return false;
        }

        void IPathNode.GetVirtualNeighbours(DynamicArray<IPathNode> neighbours, AttributeMask requesterAttributes)
        {
            /* Currently not supported */
        }

        void IPathNode.RegisterVirtualNeighbour(IPathNode neighbour)
        {
            /* Currently not supported */
        }

        void IPathNode.UnregisterVirtualNeighbour(IPathNode neighbour)
        {
            /* Currently not supported */
        }

        internal IGrid Initialize(PortalCell partner, Bounds portalBounds)
        {
            var grid = GridManager.instance.GetGrid(portalBounds.center);
            if (grid == null)
            {
                return null;
            }

            _partner = partner;
            this.parent = grid.cellMatrix;
            this.position = portalBounds.center;
            _neighbourNodes = grid.GetCoveredCells(portalBounds).ToArray();

            return grid;
        }

        internal void Activate()
        {
            if (_parentPortal.type == PortalType.Shortcut)
            {
                this.parent.shortcutPortals.Add(this);
            }

            for (int i = 0; i < _neighbourNodes.Length; i++)
            {
                _neighbourNodes[i].RegisterVirtualNeighbour(this);
            }
        }

        internal void Deactivate()
        {
            if (_parentPortal.type == PortalType.Shortcut)
            {
                this.parent.shortcutPortals.Remove(this);
            }

            for (int i = 0; i < _neighbourNodes.Length; i++)
            {
                _neighbourNodes[i].UnregisterVirtualNeighbour(this);
            }
        }
    }
}
