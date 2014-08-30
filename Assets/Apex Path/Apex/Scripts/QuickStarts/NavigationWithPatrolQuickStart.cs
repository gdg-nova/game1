/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.QuickStarts
{
    using Apex.Steering.Behaviours;
    using Apex.Steering.Props;
    using Apex.Units;
    using UnityEngine;

    /// <summary>
    /// Extended version of <see cref="NavigationQuickStart"/> that adds a <see cref="PatrolBehaviour"/> to the unit.
    /// </summary>
    [AddComponentMenu("Apex/QuickStarts/Navigating Unit on Patrol")]
    public class NavigationWithPatrolQuickStart : NavigationQuickStart
    {
        /// <summary>
        /// Extends this quick start with additional components.
        /// </summary>
        /// <param name="gameWorld">The game world.</param>
        protected override void Extend(GameObject gameWorld)
        {
            PatrolBehaviour patroller;
            if (!AddIfMissing<PatrolBehaviour>(this.gameObject, false, out patroller))
            {
                return;
            }

            //Create the route
            var routeIndex = gameWorld.GetComponentsInChildren<PatrolRoute>(true).Length + 1;
            var routeGO = new GameObject("Patrol Route " + routeIndex);
            var route = routeGO.AddComponent<PatrolRoute>();

            for (int i = 1; i < 3; i++)
            {
                var point = new GameObject("point " + i);
                
                var p = point.AddComponent<PatrolPoint>();
                p.orderIndex = i;

                point.transform.parent = routeGO.transform;
            }

            routeGO.transform.parent = gameWorld.transform;

            patroller.route = route;
        }
    }
}
