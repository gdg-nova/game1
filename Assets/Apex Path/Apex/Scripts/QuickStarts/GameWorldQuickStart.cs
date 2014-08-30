/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.QuickStarts
{
    using Apex.Debugging;
    using Apex.LoadBalancing;
    using Apex.PathFinding;
    using Apex.Services;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Sets up the necessary components on the game world to support navigation.
    /// </summary>
    [AddComponentMenu("Apex/QuickStarts/Game World")]
    public class GameWorldQuickStart : QuickStartBase
    {
        /// <summary>
        /// Sets up component on which the quick start is attached.
        /// </summary>
        protected override void Setup()
        {
            GameObject gameWorld = null;

            var servicesInitializer = FindComponent<GameServicesInitializerComponent>();
            if (servicesInitializer != null)
            {
                gameWorld = servicesInitializer.gameObject;
                Debug.Log("Existing game world found, updating that.");
            }
            else
            {
                gameWorld = this.gameObject;
                gameWorld.AddComponent<GameServicesInitializerComponent>();
            }

            if (AddIfMissing<GridComponent>(gameWorld, false))
            {
                AddIfMissing<GridVisualizer>(gameWorld, false);
            }

            AddIfMissing<LayerMappingComponent>(gameWorld, true);
            AddIfMissing<PathServiceComponent>(gameWorld, true);
            AddIfMissing<LoadBalancerComponent>(gameWorld, true);

            Extend(gameWorld);
        }

        /// <summary>
        /// Extends this quick start with additional components.
        /// </summary>
        /// <param name="gameWorld">The game world.</param>
        protected virtual void Extend(GameObject gameWorld)
        {
        }
    }
}
