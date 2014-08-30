/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// A height map that encapsulates Unity Terrain
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Basics/Terrain Height Map")]
    public sealed class TerrainHeightMap : MonoBehaviour, IHeightMap
    {
        /// <summary>
        /// The terrain
        /// </summary>
        public Terrain terrain;

        /// <summary>
        /// Gets the bounds of the height map.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public Bounds bounds
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is grid bound.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is grid bound; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool isGridBound
        {
            get { return false; }
        }

        private void Start()
        {
            //We do this in start instead of Awake in order to allow for this component to be added dynamically (AddComponent) otherwise it is not possible to set the required parameters.
            if (this.terrain == null)
            {
                Debug.LogError("You must assign a Terrain asset to the TerrainHeightMap.");
                this.enabled = false;
                return;
            }

            var data = this.terrain.terrainData;

            var bottomLeft = this.terrain.GetPosition();

            var origin = new Vector3(bottomLeft.x + (data.size.x / 2.0f), bottomLeft.y + (data.size.y / 2.0f), bottomLeft.z + (data.size.z / 2.0f));
            this.bounds = new Bounds(origin, data.size);
        }

        private void OnEnable()
        {
            HeightMapManager.instance.RegisterMap(this);
        }

        private void OnDisable()
        {
            HeightMapManager.instance.UnregisterMap(this);
        }

        /// <summary>
        /// Samples the height at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The height at the position
        /// </returns>
        public float SampleHeight(Vector3 position)
        {
            return terrain.SampleHeight(position);
        }
    }
}
