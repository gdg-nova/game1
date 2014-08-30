/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using Apex.DataStructures;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Interface for path requests
    /// </summary>
    public interface IPathRequest
    {
        /// <summary>
        /// Gets or sets where to move from.
        /// </summary>
        Vector3 from { get; set; }

        /// <summary>
        /// Gets or sets where to move to.
        /// </summary>
        Vector3 to { get; set; }

        /// <summary>
        /// Gets or sets the grid on which the navigation starts. Do not set this explicitly, the engine handles that.
        /// </summary>
        IGrid fromGrid { get; set; }

        /// <summary>
        /// Gets or sets the destination grid. Do not set this explicitly, the engine handles that.
        /// </summary>
        IGrid toGrid { get; set; }

        /// <summary>
        /// Gets or sets the requester of this path, i.e. the entity that needs a path.
        /// </summary>
        INeedPath requester { get; set; }

        /// <summary>
        /// Gets the maximum escape cell distance if origin blocked.
        /// This means that when starting a path and the origin (from position) is blocked, this determines how far away the pather will look for a free cell to escape to, before resuming the planned path.
        /// </summary>
        /// <value>
        /// The maximum escape cell distance if origin blocked.
        /// </value>
        int maxEscapeCellDistanceIfOriginBlocked { get; }

        /// <summary>
        /// Gets a value indicating whether to use path smoothing.
        /// Path smoothing creates more natural routes at a small cost to performance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if to path smoothing; otherwise, <c>false</c>.
        /// </value>
        bool usePathSmoothing { get; }

        /// <summary>
        /// Gets a value indicating whether to allow the path to cut corners. Corner cutting has slightly better performance, but produces less natural routes.
        /// </summary>
        bool allowCornerCutting { get; }

        /// <summary>
        /// Gets a value indicating whether diagonal moves are prohibited.
        /// </summary>
        /// <value>
        /// <c>true</c> if diagonal moves are prohibited; otherwise, <c>false</c>.
        /// </value>
        bool preventDiagonalMoves { get; }

        /// <summary>
        /// Gets a value indicating whether navigation off-grid is prohibited.
        /// </summary>
        bool preventOffGridNavigation { get; }

        /// <summary>
        /// Gets a value indicating whether the unit will navigate to the nearest possible position if the actual destination is blocked or otherwise inaccessible.
        /// </summary>
        bool navigateToNearestIfBlocked { get; }

        /// <summary>
        /// Gets or sets the type of this request.
        /// </summary>
        RequestType type { get; set; }

        /// <summary>
        /// Gets or sets the pending waypoints that are not covered by this request. Do not set this explicitly, the engine handles that.
        /// </summary>
        /// <value>
        /// The pending waypoints.
        /// </value>
        Vector3[] pendingWaypoints { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        bool isValid { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has decayed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has decayed; otherwise, <c>false</c>.
        /// </value>
        bool hasDecayed { get; }

        /// <summary>
        /// Completes this request
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="path">The path.</param>
        void Complete(PathingStatus status, StackWithLookAhead<IPositioned> path);

        /// <summary>
        /// Completes this request
        /// </summary>
        /// <param name="result">The result.</param>
        void Complete(PathResult result);
    }
}
