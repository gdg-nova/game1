/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System;
    using Apex.PathFinding;
    using UnityEngine;

    /// <summary>
    /// Simple portal action that does nothing, i.e. the unit will just continue on it's route. Useful for stitching together grids.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Portals/Action None")]
    public class PortalActionNoneComponent : MonoBehaviour, IPortalAction
    {
        /// <summary>
        /// Executes the specified unit.
        /// </summary>
        /// <param name="unit">The unit that has entered the portal.</param>
        /// <param name="from">The portal cell that was entered.</param>
        /// <param name="to">The destination at the other side of the portal.</param>
        /// <param name="callWhenComplete">The callback to call when the move is complete.</param>
        public void Execute(Transform unit, PortalCell from, IPositioned to, Action callWhenComplete)
        {
            callWhenComplete();
        }

        /// <summary>
        /// Gets the action cost.
        /// </summary>
        /// <param name="from">The node from which the action will start.</param>
        /// <param name="to">The node at which the action will end.</param>
        /// <returns></returns>
        public int GetActionCost(IPositioned from, IPositioned to)
        {
            return 0;
        }
    }
}
