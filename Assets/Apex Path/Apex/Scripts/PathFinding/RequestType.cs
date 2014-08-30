/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    /// <summary>
    /// The types that an <see cref="IPathRequest"/> can be.
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// A normal request
        /// </summary>
        Normal,

        /// <summary>
        /// A way point request
        /// </summary>
        Waypoint,

        /// <summary>
        /// A special way point that is used during off grid navigation
        /// </summary>
        PathboundWaypoint
    }
}
