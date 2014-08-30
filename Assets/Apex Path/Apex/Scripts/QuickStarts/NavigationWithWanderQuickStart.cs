/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.QuickStarts
{
    using Apex.Steering.Behaviours;
    using UnityEngine;

    /// <summary>
    /// Extended version of <see cref="NavigationQuickStart"/> that adds a <see cref="WanderBehaviour"/> to the unit.
    /// </summary>
    [AddComponentMenu("Apex/QuickStarts/Navigating Unit Wandering")]
    public class NavigationWithWanderQuickStart : NavigationQuickStart
    {
        /// <summary>
        /// Extends this quick start with additional components.
        /// </summary>
        /// <param name="gameWorld">The game world.</param>
        protected override void Extend(GameObject gameWorld)
        {
            var go = this.gameObject;

            AddIfMissing<WanderBehaviour>(go, false);
        }
    }
}
