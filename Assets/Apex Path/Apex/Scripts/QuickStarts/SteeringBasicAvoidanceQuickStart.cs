namespace Apex.QuickStarts
{
    using Apex.Steering;
    using Apex.Steering.Components;
    using UnityEngine;

    /// <summary>
    /// Quick start that adds basic avoidance steering to a unit.
    /// </summary>
    [AddComponentMenu("Apex/QuickStarts/Steering: Basic avoidance")]
    public class SteeringBasicAvoidanceQuickStart : QuickStartBase
    {
        /// <summary>
        /// Sets up component on which the quick start is attached.
        /// </summary>
        protected override void Setup()
        {
            var go = this.gameObject;

            //Add the required components
            AddIfMissing<BasicScanner>(go, false);
            AddIfMissing<SteerForBasicAvoidanceComponent>(go, false);
        }
    }
}
