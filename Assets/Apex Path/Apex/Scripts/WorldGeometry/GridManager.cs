/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System;
    using System.Collections.Generic;
    using Apex.Common;
    using Apex.DataStructures;
    using Apex.PathFinding;
    using UnityEngine;

    /// <summary>
    /// Manages all <see cref="Grid"/>s in the game world.
    /// </summary>
    public sealed class GridManager : IGridManager
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static readonly IGridManager instance = new GridManager();

        private List<IGrid> _grids;
        private DynamicArray<GridPortal> _portals;
        private Dictionary<string, GridPortal> _portalsLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridManager"/> class.
        /// </summary>
        public GridManager()
        {
            _grids = new List<IGrid>();
            _portals = new DynamicArray<GridPortal>(0);
            _portalsLookup = new Dictionary<string, GridPortal>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets the grids managed by this manager.
        /// </summary>
        /// <value>
        /// The grids.
        /// </value>
        public IEnumerable<IGrid> grids
        {
            get { return _grids; }
        }

        /// <summary>
        /// Gets the grid portals managed by this manager.
        /// </summary>
        /// <value>
        /// The portals.
        /// </value>
        public IIndexable<GridPortal> portals
        {
            get { return _portals; }
        }

        /// <summary>
        /// Registers the grid with this manager.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public void RegisterGrid(IGrid grid)
        {
            _grids.AddUnique(grid);
        }

        /// <summary>
        /// Unregisters the grid with this manager.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public void UnregisterGrid(IGrid grid)
        {
            _grids.Remove(grid);
        }

        /// <summary>
        /// Gets the grid at the specified position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>
        /// The matching grid or null if no match is found.
        /// </returns>
        public IGrid GetGrid(Vector3 pos)
        {
            for (int i = 0; i < _grids.Count; i++)
            {
                var g = _grids[i];

                if (g.bounds.Contains(pos))
                {
                    return g;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the grid properties of the request.
        /// If 
        /// <paramref name="from" /> is not on a grid it will assign the closest grid to 
        /// <paramref name="from" /> that is crossed on the route from 
        /// <paramref name="from" /> to 
        /// <paramref name="to" />.
        /// </summary>
        /// <param name="from">The from position.</param>
        /// <param name="to">The to position.</param>
        /// <param name="request">The request to inject.</param>
        public void InjectGrids(Vector3 from, Vector3 to, IPathRequest request)
        {
            request.fromGrid = GetGrid(from);
            request.toGrid = GetGrid(to);

            if (request.fromGrid != null)
            {
                return;
            }

            //Adjust to avoid divide by zero. This small adjustment should not impact the result in any changing way.
            if (from.x == to.x)
            {
                from.x += 0.1f;
            }

            //Find the closest grid to 'from' that is crossed en route to 'to', if any
            IGrid g = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < _grids.Count; i++)
            {
                var intersection = CheckLineIntersection(_grids[i], from, to);
                if (intersection.HasValue)
                {
                    var d = (from - intersection.Value).sqrMagnitude;
                    if (d < bestDistance)
                    {
                        bestDistance = d;
                        g = _grids[i];
                    }
                }
            }

            request.fromGrid = g;
        }

        /// <summary>
        /// Updates the specified region in the scene with regards to accessibility, i.e. if static obstacles are destroyed or terrain changes.
        /// </summary>
        /// <param name="extent">The extent to update.</param>
        /// <param name="maxMillisecondsUsedPerFrame">The maximum number of ms to update is allowed to use per frame until it is done.</param>
        public void Update(Bounds extent, int maxMillisecondsUsedPerFrame)
        {
            for (int i = 0; i < _grids.Count; i++)
            {
                var g = _grids[i];

                if (g.bounds.Intersects(extent))
                {
                    g.Update(extent, maxMillisecondsUsedPerFrame);
                }
            }
        }

        private Vector3? CheckLineIntersection(IGrid g, Vector3 from, Vector3 to)
        {
            Vector3? intersect;

            var diagonalOneP1 = new Vector3(g.left.edge, 0.0f, g.bottom.edge);
            var diagonalOneP2 = new Vector3(g.right.edge, 0.0f, g.top.edge);

            if (Geometry.DoLinesIntersect(from, to, diagonalOneP1, diagonalOneP2, out intersect))
            {
                return intersect;
            }

            var diagonalTwoP1 = new Vector3(g.left.edge, 0.0f, g.top.edge);
            var diagonalTwoP2 = new Vector3(g.right.edge, 0.0f, g.bottom.edge);

            if (Geometry.DoLinesIntersect(from, to, diagonalTwoP1, diagonalTwoP2, out intersect))
            {
                return intersect;
            }

            return null;
        }

        /// <summary>
        /// Determines if a portal exists between two grids.
        /// </summary>
        /// <param name="first">The first grid</param>
        /// <param name="second">The second grid.</param>
        /// <param name="requesterAttributes">The attribute mask of the requester, i.e. the entity asking to use the portal.</param>
        /// <returns>
        ///   <c>true</c> if at least one portal exists; otherwise <c>false</c>
        /// </returns>
        public bool PortalExists(IGrid first, IGrid second, AttributeMask requesterAttributes)
        {
            int portalCount = _portals.count;
            for (int i = 0; i < portalCount; i++)
            {
                var p = _portals[i];

                if (p.enabled && p.IsUsableBy(requesterAttributes) && ((p.gridOne == first && p.gridTwo == second) || (p.gridOne == second && p.gridTwo == first)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the portal.
        /// </summary>
        /// <param name="name">The name of the portal.</param>
        /// <returns>
        /// The portal or null if not found
        /// </returns>
        public GridPortal GetPortal(string name)
        {
            GridPortal p;
            _portalsLookup.TryGetValue(name, out p);

            return p;
        }

        /// <summary>
        /// Registers a portal.
        /// </summary>
        /// <param name="name">The unique name of the portal.</param>
        /// <param name="portal">The portal.</param>
        /// <returns>The actual name the portal received</returns>
        public string RegisterPortal(string name, GridPortal portal)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = string.Concat("Grid Portal ", (_portalsLookup.Count + 1));
            }

            int idx = 1;
            while (_portalsLookup.ContainsKey(name))
            {
                name = string.Concat("Grid Portal ", idx++);
            }

            _portalsLookup.Add(name, portal);
            _portals.Add(portal);

            return name;
        }

        /// <summary>
        /// Unregisters a portal.
        /// </summary>
        /// <param name="name">The portal name.</param>
        public void UnregisterPortal(string name)
        {
            _portalsLookup.Remove(name);

            _portals.Clear();
            foreach (var p in _portalsLookup.Values)
            {
                _portals.Add(p);
            }
        }
    }
}
