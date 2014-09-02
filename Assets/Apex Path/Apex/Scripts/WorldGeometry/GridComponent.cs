/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System;
    using Apex.Utilities;
    using UnityEngine;

    /// <summary>
    /// Component for configuration of a <see cref="Grid"/>
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Basics/Grid")]
    public class GridComponent : MonoBehaviour
    {
        /// <summary>
        /// Size along the x-axis.
        /// </summary>
        [MinCheck(1)]
        public int sizeX = 10;

        /// <summary>
        /// Size along the z-axis.
        /// </summary>
        [MinCheck(1)]
        public int sizeZ = 10;

        /// <summary>
        /// The sub sections across the x-axis
        /// </summary>
        [MinCheck(1)]
        public int subSectionsX = 2;

        /// <summary>
        /// The sub sections across the z-axis
        /// </summary>
        [MinCheck(1)]
        public int subSectionsZ = 2;

        /// <summary>
        /// The sub sections cell overlap
        /// </summary>
        [MinCheck(0)]
        public int subSectionsCellOverlap = 2;

        /// <summary>
        /// The cell size
        /// </summary>
        [MinCheck(0.1f)]
        public float cellSize = 2;

        /// <summary>
        /// Whether or not to generate a height map to enable units to follow a terrain of differing heights.
        /// </summary>
        public bool generateHeightmap = true;

        /// <summary>
        /// The distance below the grid's origin that defines the lower boundary of the grid
        /// </summary>
        [MinCheck(0f)]
        public float lowerBoundary = 1.0f;

        /// <summary>
        /// The distance above the grid's origin that defines the lower boundary of the grid
        /// </summary>
        [MinCheck(0f)]
        public float upperBoundary = 10.0f;

        /// <summary>
        /// The height granularity of the grid, i.e. distance between height samples.
        /// </summary>
        [MinCheck(0.05f)]
        public float heightGranularity = 0.1f;

        /// <summary>
        /// The obstacle sensitivity range, meaning any obstacle within this range of the cell center will cause the cell to be blocked.
        /// </summary>
        [MinCheck(0f)]
        public float obstacleSensitivityRange = 0.5f;

        /// <summary>
        /// The maximum angle at which a cell is deemed walkable
        /// </summary>
        [Range(1f, 90f)]
        public float maxWalkableSlopeAngle = 30.0f;

        /// <summary>
        /// The maximum height that the unit can scale, i.e. walk onto even if its is a vertical move. Stairs for instance.
        /// </summary>
        [MinCheck(0f)]
        public float maxScaleHeight = 0.5f;

        /// <summary>
        /// The baked grid data
        /// </summary>
        [SerializeField, HideInInspector]
        public CellMatrixData bakedData;

        //In order to prevent causing a breaking change with the introduction of the new editor,
        //the properties that has always been exposed as properties remain that way
        [SerializeField]
        private Vector3 _origin;

        [SerializeField]
        private string _friendlyName;

        [SerializeField]
        private bool _linkOriginToTransform = true;

        [SerializeField]
        private bool _storeBakedDataAsAsset;

        [SerializeField]
        private bool _automaticInitialization = true;

        /// <summary>
        /// Gets or sets the friendly name of the grid. Used in messages and such.
        /// </summary>
        public string friendlyName
        {
            get { return _friendlyName; }
            set { _friendlyName = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to link origin to the transform.
        /// </summary>
        public bool linkOriginToTransform
        {
            get { return _linkOriginToTransform; }
            set { _linkOriginToTransform = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to store baked data as an asset. This is an editor setting and should not be manipulated manually.
        /// </summary>
        public bool storeBakedDataAsAsset
        {
            get { return _storeBakedDataAsAsset; }
            set { _storeBakedDataAsAsset = value; }
        }

        /// <summary>
        /// Controls whether the grid is automatically initialized when enabled. If set to <c>false</c> the grid must be manually initialized by calling <see cref="Initialize(int, Action&lt;GridComponent&gt;)"/>
        /// </summary>
        public bool automaticInitialization
        {
            get { return _automaticInitialization; }
            set { _automaticInitialization = value; }
        }

        /// <summary>
        /// The origin, i.e. center, of the grid
        /// </summary>
        public Vector3 origin
        {
            get
            {
                if (_linkOriginToTransform)
                {
                    return this.transform.position;
                }

                return _origin;
            }

            set
            {
                if (!_linkOriginToTransform)
                {
                    _origin = value;
                }
            }
        }

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>
        /// The grid.
        /// </value>
        public IGrid grid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a builder for initializing the grid. This is a framework method, and should not be called.
        /// </summary>
        /// <returns>The builder configured for this grid.</returns>
        public GridBuilder GetBuilder()
        {
            return new GridBuilder
            {
                origin = this.origin,
                sizeX = this.sizeX,
                sizeZ = this.sizeZ,
                cellSize = this.cellSize,
                obstacleSensitivityRange = this.obstacleSensitivityRange,
                generateHeightmap = this.generateHeightmap,
                lowerBoundary = this.lowerBoundary,
                upperBoundary = this.upperBoundary,
                granularity = this.heightGranularity,
                subSectionsX = this.subSectionsX,
                subSectionsZ = this.subSectionsZ,
                subSectionsCellOverlap = this.subSectionsCellOverlap,
                maxWalkableSlopeAngle = this.maxWalkableSlopeAngle,
                maxScaleHeight = this.maxScaleHeight
            };
        }

        /// <summary>
        /// Initializes the grid. This is only intended for use if <see cref="automaticInitialization"/> is set to <c>false</c>.
        /// The grid will be initialized over a number of frames, as to smooth out the initialization.
        /// </summary>
        /// <param name="maxMillisecondsUsedPerFrame">The maximum milliseconds used per frame while initializing.</param>
        /// <param name="callback">Callback that will be called once initialization is complete. The callback will receive a reference to this component for convenience.</param>
        public void Initialize(int maxMillisecondsUsedPerFrame, Action<GridComponent> callback)
        {
            if (this.grid != null)
            {
                return;
            }

            var builder = GetBuilder();

            Action<IGrid> cb = (g) =>
            {
                this.grid = g;
                RegisterWithManagers();

                this.enabled = true;
                callback(this);
            };

            if (this.bakedData != null)
            {
                builder.Create(this.bakedData, maxMillisecondsUsedPerFrame, cb);
            }
            else
            {
                builder.Create(maxMillisecondsUsedPerFrame, cb);
            }
        }

        /// <summary>
        /// Disables the grid and releases all memory. If <see cref="automaticInitialization"/> is <c>true</c>, the grid will be reinitialized if re-enabled, otherwise <see cref="Initialize(int, Action&lt;GridComponent&gt;)"/> must be called to re-enable and re-initialize the grid.
        /// </summary>
        public void Disable()
        {
            this.enabled = false;
            this.grid = null;
        }

        /// <summary>
        /// Editor function, do not use this.
        /// </summary>
        public void ResetGrid()
        {
            this.grid = null;
        }

        internal void EnsureGrid()
        {
            if (this.grid == null)
            {
                var builder = GetBuilder();

                if (this.bakedData != null)
                {
                    this.grid = builder.Create(this.bakedData);

                    if (!this.storeBakedDataAsAsset)
                    {
                        //No need to hang on to this anymore.
                        this.bakedData = null;
                    }
                }
                else
                {
                    this.grid = builder.Create();
                }
            }
        }

        internal Bounds EnsureForEditor(bool refresh, Bounds area)
        {
            if (this.grid == null)
            {
                var builder = GetBuilder();

                this.grid = builder.CreateForEditor(this.bakedData);
            }

            var gb = this.grid.bounds;
            gb.Expand(-this.cellSize);

            var bl = new Vector3(Mathf.Max(area.min.x, gb.min.x), 0f, Mathf.Max(area.min.z, gb.min.z));
            var tr = new Vector3(Mathf.Min(area.max.x, gb.max.x), 0f, Mathf.Min(area.max.z, gb.max.z));
            area.SetMinMax(bl, tr);

            if (refresh)
            {
                this.grid.cellMatrix.UpdateForEditor(area, this.bakedData);
            }

            return area;
        }

        private void OnEnable()
        {
            if (this.automaticInitialization)
            {
                EnsureGrid();
                RegisterWithManagers();
            }
        }

        private void OnDisable()
        {
            GridManager.instance.UnregisterGrid(this.grid);
            HeightMapManager.instance.UnregisterMap(this.grid.cellMatrix);
        }

        private void OnValidate()
        {
            //This is only called in the editor and is done to ensure a refresh of the grid when values change.
            if (!Application.isPlaying)
            {
                this.grid = null;
            }
        }

        private void RegisterWithManagers()
        {
            GridManager.instance.RegisterGrid(this.grid);

            var matrix = this.grid.cellMatrix;
            if (matrix.hasHeightMap)
            {
                HeightMapManager.instance.RegisterMap(matrix);
            }
        }
    }
}
