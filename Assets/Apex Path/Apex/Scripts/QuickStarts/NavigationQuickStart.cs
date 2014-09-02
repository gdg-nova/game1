/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.QuickStarts
{
    using Apex.Debugging;
    using Apex.LoadBalancing;
    using Apex.PathFinding;
    using Apex.Services;
    using Apex.Steering;
    using Apex.Steering.Components;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// A component that adds all components necessary for an object to become a navigating unit.
    /// </summary>
    [AddComponentMenu("Apex/QuickStarts/Navigating Unit")]
    public class NavigationQuickStart : QuickStartBase
    {
        /// <summary>
        /// Sets up component on which the quick start is attached.
        /// </summary>
        protected sealed override void Setup()
        {
            var go = this.gameObject;

            //Add the required components
            AddIfMissing<Rigidbody>(go, false);
            AddIfMissing<SteerableUnitComponent>(go, false);
            AddIfMissing<HumanoidSpeedComponent>(go, false);
            AddIfMissing<SteerForPathComponent>(go, false);
            AddIfMissing<TurnableUnitComponent>(go, false);
            AddIfMissing<PathVisualizer>(go, false);
           
            //Adjust components
            if (this.rigidbody != null)
            {
                this.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            GameObject gameWorld = null;

            var servicesInitializer = FindComponent<GameServicesInitializerComponent>();
            if (servicesInitializer != null)
            {
                gameWorld = servicesInitializer.gameObject;
            }
            else
            {
                gameWorld = new GameObject("Game World");
                gameWorld.AddComponent<GameServicesInitializerComponent>();
                Debug.Log("No game world found, creating one.");
            }

            if (AddIfMissing<GridComponent>(gameWorld, true))
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
