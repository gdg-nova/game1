/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using UnityEngine;

    /// <summary>
    /// Maps Unity layers to the internally used <see cref="Layers"/>
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Apex/Navigation/Basics/Layer Mapping")]
    public class LayerMappingComponent : SingleInstanceComponent<LayerMappingComponent>
    {
        /// <summary>
        /// The static obstacle layer mask
        /// </summary>
        public LayerMask staticObstacleLayer;

        /// <summary>
        /// The terrain layer mask
        /// </summary>
        public LayerMask terrainLayer;

        /// <summary>
        /// The unit layer mask
        /// </summary>
        public LayerMask unitLayer;

        private void OnEnable()
        {
            Layers.blocks = this.staticObstacleLayer;
            Layers.terrain = this.terrainLayer;
            Layers.units = this.unitLayer;
        }

        private void OnValidate()
        {
            Layers.blocks = this.staticObstacleLayer;
            Layers.terrain = this.terrainLayer;
            Layers.units = this.unitLayer;
        }
    }
}
