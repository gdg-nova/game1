/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    /// <summary>
    /// The status of the path request
    /// </summary>
    public enum PathingStatus
    {
        /// <summary>
        /// The request is currently running
        /// </summary>
        Running,

        /// <summary>
        /// The request failed
        /// </summary>
        Failed,

        /// <summary>
        /// The request decayed
        /// </summary>
        Decayed,

        /// <summary>
        /// The start node is outside grid
        /// </summary>
        StartOutsideGrid,

        /// <summary>
        /// The end node is outside grid
        /// </summary>
        EndOutsideGrid,

        /// <summary>
        /// The no route exists
        /// </summary>
        NoRouteExists,

        /// <summary>
        /// The destination is blocked
        /// </summary>
        DestinationBlocked,

        /// <summary>
        /// The request completed successfully
        /// </summary>
        Complete
    }
}
