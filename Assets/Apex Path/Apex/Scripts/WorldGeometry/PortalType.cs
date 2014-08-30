/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    /// <summary>
    /// Define the portal types available
    /// </summary>
    public enum PortalType
    {
        /// <summary>
        /// Portals of this type will be evaluated during path finding even if they are not on the natural route towards the destination, as the portal is thought to possibly provide a short-cut even if it means moving away from the destination initially.
        /// </summary>
        Shortcut,

        /// <summary>
        /// Connector portals work as normal cells, but are virtual with the purpose of connection grids.
        /// </summary>
        Connector
    }
}
