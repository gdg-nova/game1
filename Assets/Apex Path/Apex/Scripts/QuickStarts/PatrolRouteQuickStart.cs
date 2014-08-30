/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.QuickStarts
{
    using Apex.Steering.Props;
    using UnityEngine;

    /// <summary>
    /// A component that adds a <see cref="PatrolRoute"/> with two <see cref="PatrolPoint"/>s. Additional PatrolPoints can then be added as needed.
    /// </summary>
    [AddComponentMenu("Apex/QuickStarts/Patrol Route")]
    public class PatrolRouteQuickStart : QuickStartBase
    {
        /// <summary>
        /// Sets up component on which the quick start is attached.
        /// </summary>
        protected override void Setup()
        {
            var go = this.gameObject;

            var routeIndex = go.GetComponentsInChildren<PatrolRoute>(true).Length + 1;

            var parent = go.transform;
            go = new GameObject("Patrol Route " + routeIndex);
            go.transform.parent = parent;

            go.AddComponent<PatrolRoute>();

            for (int i = 1; i < 3; i++)
            {
                var point = new GameObject("point " + i);
                
                var p = point.AddComponent<PatrolPoint>();
                p.orderIndex = i;

                point.transform.parent = go.transform;
            }
        }
    }
}
