/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.QuickStarts
{
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// A component that adds a <see cref="GridPortalComponent"/>.
    /// </summary>
    [AddComponentMenu("Apex/QuickStarts/Portal")]
    public class PortalQuickStart : QuickStartBase
    {
        /// <summary>
        /// Sets up component on which the quick start is attached.
        /// </summary>
        protected override void Setup()
        {
            var go = this.gameObject;

            var portalIndex = go.GetComponentsInChildren<GridPortalComponent>(true).Length + 1;

            var parent = go.transform;
            go = new GameObject("Portal " + portalIndex);
            go.transform.parent = parent;
            go.transform.localPosition = Vector3.zero;

            go.AddComponent<GridPortalComponent>();
        }
    }
}
