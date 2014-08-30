/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System.Collections;
    using System.Collections.Generic;
    using Apex.DataStructures;
    using Apex.Services;
    using UnityEngine;

    /// <summary>
    /// Matrix of <see cref="Cell"/>s.
    /// </summary>
    public class CellMatrix : Matrix<Cell>, IHeightMap
    {
        private const float SquareRootTwo = 1.414214f;

        private Vector3 _origin;
        private Vector3 _start;
        private Bounds _bounds;
        private float _cellSize;
        private float _obstacleSensitivityRange;
        private bool _generateHeightMap;
        private float _upperBoundary;
        private float _lowerBoundary;
        private float _granularity;
        private float _maxWalkableSlopeAngle;
        private float _maxScaleHeight;
        private int _heightMapSizeX;
        private int _heightMapSizeZ;
        private Dictionary<int, float> _heightIndex;
        private IList<PortalCell> _shortcutPortals;

        private CellMatrix(ICellMatrixConfiguration cfg)
            : base(cfg.sizeX, cfg.sizeZ)
        {
            Initialize(cfg);
        }

        /// <summary>
        /// Gets the bounds of the matrix.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public Bounds bounds
        {
            get { return _bounds; }
        }

        bool IHeightMap.isGridBound
        {
            get { return true; }
        }

        internal Vector3 origin
        {
            get { return _origin; }
        }

        internal Vector3 start
        {
            get { return _start; }
        }

        internal float cellSize
        {
            get { return _cellSize; }
        }

        internal bool hasHeightMap
        {
            get { return (_heightIndex.Count > 0); }
        }

        internal VectorXZ heightMapSize
        {
            get { return new VectorXZ(_heightMapSizeX, _heightMapSizeZ); }
        }

        internal int heightMapEntries
        {
            get { return _heightIndex.Count; }
        }

        internal IList<PortalCell> shortcutPortals
        {
            get { return _shortcutPortals; }
        }

        /// <summary>
        /// Creates a cell matrix based on the given configuration.
        /// </summary>
        /// <param name="cfg">The configuration.</param>
        /// <returns>The matrix</returns>
        public static CellMatrix Create(ICellMatrixConfiguration cfg)
        {
            var matrix = new CellMatrix(cfg);

            var iter = matrix.Populate();
            while (iter.MoveNext())
            {
                /* NOOP, we just iterate over all */
            }

            return matrix;
        }

        /// <summary>
        /// Creates a cell matrix based on the given configuration and stored data.
        /// </summary>
        /// <param name="cfg">The configuration.</param>
        /// <param name="data">The data.</param>
        /// <returns>The matrix</returns>
        public static CellMatrix Create(ICellMatrixConfiguration cfg, CellMatrixData data)
        {
            var matrix = new CellMatrix(cfg);

            var iter = matrix.Populate(data);
            while (iter.MoveNext())
            {
                /* NOOP, we just iterate over all */
            }

            return matrix;
        }

        internal static CellMatrix CreateForEditor(ICellMatrixConfiguration cfg, CellMatrixData data)
        {
            var matrix = new CellMatrix(cfg);

            if (data == null)
            {
                matrix._heightIndex = new Dictionary<int, float>();
            }
            else
            {
                matrix.InitHeightMapForEditor(data);
            }

            return matrix;
        }

        internal static MatrixIncrementalInitializer CreateIncrementally(ICellMatrixConfiguration cfg)
        {
            return new MatrixIncrementalInitializer(cfg);
        }

        internal static MatrixIncrementalInitializer CreateIncrementally(ICellMatrixConfiguration cfg, CellMatrixData data)
        {
            return new MatrixIncrementalInitializer(cfg, data);
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
            if (!_bounds.Contains(position))
            {
                return position.y;
            }

            var fx = (position.x - _start.x) / _granularity;
            var fz = (position.z - _start.z) / _granularity;

            var x = Mathf.RoundToInt(fx);
            var z = Mathf.RoundToInt(fz);

            float height;
            var idx = (z * _heightMapSizeX) + x;
            if (_heightIndex.TryGetValue(idx, out height))
            {
                return height;
            }

            return this.origin.y;
        }

        internal float SampleHeight(int x, int z)
        {
            float height;
            var idx = (z * _heightMapSizeX) + x;
            if (_heightIndex.TryGetValue(idx, out height))
            {
                return height;
            }

            return this.origin.y;
        }

        internal Cell GetCell(Vector3 position)
        {
            var fx = (position.x - _start.x) / _cellSize;
            var fz = (position.z - _start.z) / _cellSize;

            var x = Mathf.FloorToInt(fx);
            var z = Mathf.FloorToInt(fz);

            return this[x, z];
        }

        internal MatrixBounds GetMatrixBounds(Vector3 position, float radiusX, float radiusZ, float minOverlapToInclude, bool adjustToBounds)
        {
            if (minOverlapToInclude > 0.0f)
            {
                //We only want to include a cell when it is overlapped by a certain amount. The 0.01 is to cater for borderline cases, i.e. when radius exactly matches the overlap
                radiusX = radiusX - minOverlapToInclude + 0.01f;
                radiusZ = radiusZ - minOverlapToInclude + 0.01f;
            }
            else
            {
                //We do not want to include a cell in the borderline case, i.e. when radius hits right on a cell boundary. If not adjusted cells along the positive axis will include the next cell in such cases.
                //This assumes that the second decimal is otherwise irrelevant and will never cause the adverse effect of excluding a cell that should have been included. This is faster than having to check for borderline cases.
                radiusX -= 0.01f;
                radiusZ -= 0.01f;
            }

            if (adjustToBounds)
            {
                return new MatrixBounds
                {
                    minColumn = AdjustColumnToBounds(Mathf.FloorToInt((position.x - _start.x - radiusX) / _cellSize)),
                    minRow = AdjustRowToBounds(Mathf.FloorToInt((position.z - _start.z - radiusZ) / _cellSize)),
                    maxColumn = AdjustColumnToBounds(Mathf.FloorToInt((position.x - _start.x + radiusX) / _cellSize)),
                    maxRow = AdjustRowToBounds(Mathf.FloorToInt((position.z - _start.z + radiusZ) / _cellSize))
                };
            }

            return new MatrixBounds
            {
                minColumn = Mathf.FloorToInt((position.x - _start.x - radiusX) / _cellSize),
                minRow = Mathf.FloorToInt((position.z - _start.z - radiusZ) / _cellSize),
                maxColumn = Mathf.FloorToInt((position.x - _start.x + radiusX) / _cellSize),
                maxRow = Mathf.FloorToInt((position.z - _start.z + radiusZ) / _cellSize)
            };
        }

        internal IEnumerator Update(Bounds extent)
        {
            if (_generateHeightMap)
            {
                //First update the height map
                var min = extent.min;
                var max = extent.max;

                var startX = Mathf.Clamp(Mathf.FloorToInt((min.x - _start.x) / _granularity), 0, _heightMapSizeX);
                var startZ = Mathf.Clamp(Mathf.FloorToInt((min.z - _start.z) / _granularity), 0, _heightMapSizeZ);
                var endX = Mathf.Clamp(Mathf.CeilToInt((max.x - _start.x) / _granularity), 0, _heightMapSizeX);
                var endZ = Mathf.Clamp(Mathf.CeilToInt((max.z - _start.z) / _granularity), 0, _heightMapSizeZ);

                var plotRange = _lowerBoundary + _upperBoundary;
                var down = Vector3.down;
                var mapOriginY = this.origin.y;

                RaycastHit hit;
                for (int x = startX; x <= endX; x++)
                {
                    for (int z = startZ; z <= endZ; z++)
                    {
                        var pos = new Vector3(_start.x + (x * _granularity), _start.y + _upperBoundary, _start.z + (z * _granularity));

                        UnityServices.physics.Raycast(pos, down, out hit, plotRange, Layers.terrain);
                        var height = hit.point.y;
                        var idx = (z * _heightMapSizeX) + x;

                        if (height == mapOriginY)
                        {
                            _heightIndex.Remove(idx);
                        }
                        else
                        {
                            _heightIndex[idx] = height;
                        }

                        yield return null;
                    }
                }
            }

            //Next update the affected grid cells
            var bounds = GetMatrixBounds(extent.center, extent.extents.x, extent.extents.z, 0.0f, true);

            var blockThreshold = Mathf.Min(Mathf.Max(_obstacleSensitivityRange, 0.1f), _cellSize / 2.0f);
            for (int x = bounds.minColumn; x <= bounds.maxColumn; x++)
            {
                for (int z = bounds.minRow; z <= bounds.maxRow; z++)
                {
                    var cell = this.rawMatrix[x, z];

                    var cellPos = cell.position;
                    cellPos.y = this.SampleHeight(cellPos);

                    var blocked = IsBlocked(cellPos, blockThreshold);

                    cell.UpdateState(cellPos.y, blocked);

                    yield return null;
                }
            }

            if (this.hasHeightMap)
            {
                var iter = AssignHeightSettings(bounds);
                while (iter.MoveNext())
                {
                    yield return null;
                }
            }
        }

        internal void UpdateForEditor(Bounds extent, CellMatrixData data)
        {
            var bounds = GetMatrixBounds(extent.center, extent.extents.x + (_cellSize * 2), extent.extents.z + (_cellSize * 2), 0.0f, true);

            if (data == null)
            {
                var blockThreshold = Mathf.Min(Mathf.Max(_obstacleSensitivityRange, 0.1f), _cellSize / 2.0f);
                for (int x = bounds.minColumn; x <= bounds.maxColumn; x++)
                {
                    for (int z = bounds.minRow; z <= bounds.maxRow; z++)
                    {
                        var cell = this.rawMatrix[x, z];

                        if (cell == null)
                        {
                            var position = new Vector3(_start.x + (x * _cellSize) + (_cellSize / 2.0f), _origin.y, _start.z + (z * _cellSize) + (_cellSize / 2.0f));

                            var blocked = IsBlocked(position, blockThreshold);

                            this.rawMatrix[x, z] = new Cell(this, position, x, z, blocked);
                        }
                        else
                        {
                            var cellPos = cell.position;
                            var blocked = IsBlocked(cellPos, blockThreshold);

                            cell.UpdateState(cellPos.y, blocked);
                        }
                    }
                }
            }
            else
            {
                var accessor = data.GetAccessor();

                //Init the cells
                for (int x = bounds.minColumn; x <= bounds.maxColumn; x++)
                {
                    for (int z = bounds.minRow; z <= bounds.maxRow; z++)
                    {
                        var cell = this.rawMatrix[x, z];

                        if (cell == null)
                        {
                            var idx = (z * this.columns) + x;

                            var position = new Vector3(_start.x + (x * _cellSize) + (_cellSize / 2.0f), _origin.y, _start.z + (z * _cellSize) + (_cellSize / 2.0f));
                            position.y = this.SampleHeight(position);

                            var blocked = accessor.IsPermaBlocked(idx);

                            cell = new Cell(this, position, x, z, blocked);
                            cell.heightBlockedNeighbours = accessor.GetHeightBlockStatus(idx);

                            this.rawMatrix[x, z] = cell;
                        }
                    }
                }
            }
        }

        private void InitHeightMapForEditor(CellMatrixData data)
        {
            var accessor = data.GetAccessor();

            //Get the height map
            _heightIndex = new Dictionary<int, float>(accessor.heightEntries);

            var mapOriginY = this.origin.y;

            for (int x = 0; x < _heightMapSizeX; x++)
            {
                for (int z = 0; z < _heightMapSizeZ; z++)
                {
                    var idx = (z * _heightMapSizeX) + x;
                    var height = accessor.GetHeight(idx);

                    if (height != mapOriginY)
                    {
                        _heightIndex[idx] = height;
                    }
                }
            }
        }

        private void Initialize(ICellMatrixConfiguration cfg)
        {
            _cellSize = cfg.cellSize;
            _origin = cfg.origin;
            _start = cfg.origin;
            _start.x -= this.columns * 0.5f * _cellSize;
            _start.z -= this.rows * 0.5f * _cellSize;

            _obstacleSensitivityRange = cfg.obstacleSensitivityRange;
            _generateHeightMap = cfg.generateHeightmap;
            _upperBoundary = cfg.upperBoundary;
            _lowerBoundary = cfg.lowerBoundary;

            if (_generateHeightMap)
            {
                _granularity = cfg.granularity;
                _maxWalkableSlopeAngle = cfg.maxWalkableSlopeAngle;
                _maxScaleHeight = cfg.maxScaleHeight;

                _heightMapSizeX = Mathf.RoundToInt(this.columns * this.cellSize / _granularity) + 1;
                _heightMapSizeZ = Mathf.RoundToInt(this.rows * this.cellSize / _granularity) + 1;
            }

            var yoffset = (_upperBoundary - _lowerBoundary) * 0.5f;
            var boundsCenter = _origin;
            boundsCenter.y += yoffset;
            _bounds = new Bounds(boundsCenter, new Vector3(this.columns * _cellSize, _upperBoundary + _lowerBoundary, this.rows * _cellSize));

            _shortcutPortals = new List<PortalCell>();
        }

        private IEnumerator Populate()
        {
            //Create the height map
            _heightIndex = new Dictionary<int, float>();

            if (_generateHeightMap)
            {
                var plotRange = _lowerBoundary + _upperBoundary;
                var mapOriginY = this.origin.y;

                RaycastHit hit;
                for (int x = 0; x < _heightMapSizeX; x++)
                {
                    for (int z = 0; z < _heightMapSizeZ; z++)
                    {
                        var pos = new Vector3(_start.x + (x * _granularity), _start.y + _upperBoundary, _start.z + (z * _granularity));

                        UnityServices.physics.Raycast(pos, Vector3.down, out hit, plotRange, Layers.terrain);

                        var idx = (z * _heightMapSizeX) + x;
                        var height = hit.point.y;

                        if (height != mapOriginY)
                        {
                            _heightIndex[idx] = height;
                        }

                        yield return null;
                    }
                }
            }

            //Populate the cell matrix
            var blockThreshold = Mathf.Min(Mathf.Max(_obstacleSensitivityRange, 0.1f), _cellSize / 2.0f);
            for (int x = 0; x < this.columns; x++)
            {
                for (int z = 0; z < this.rows; z++)
                {
                    var position = new Vector3(_start.x + (x * _cellSize) + (_cellSize / 2.0f), _origin.y, _start.z + (z * _cellSize) + (_cellSize / 2.0f));
                    if (this.hasHeightMap)
                    {
                        position.y = this.SampleHeight(position);
                    }

                    var blocked = IsBlocked(position, blockThreshold);

                    this.rawMatrix[x, z] = new Cell(this, position, x, z, blocked);

                    yield return null;
                }
            }

            //Set the height block status of each cell
            if (this.hasHeightMap)
            {
                var entireMatrix = new MatrixBounds(0, 0, this.columns - 1, this.rows - 1);

                //Set height map settings for all cells
                var iter = AssignHeightSettings(entireMatrix);
                while (iter.MoveNext())
                {
                    yield return null;
                }
            }
        }

        private IEnumerator Populate(CellMatrixData data)
        {
            var accessor = data.GetAccessor();

            //Get the height map
            _heightIndex = new Dictionary<int, float>(accessor.heightEntries);

            var mapOriginY = this.origin.y;

            for (int x = 0; x < _heightMapSizeX; x++)
            {
                for (int z = 0; z < _heightMapSizeZ; z++)
                {
                    var idx = (z * _heightMapSizeX) + x;
                    var height = accessor.GetHeight(idx);

                    if (height != mapOriginY)
                    {
                        _heightIndex[idx] = height;
                    }

                    yield return null;
                }
            }

            //Init the cells
            for (int x = 0; x < this.columns; x++)
            {
                for (int z = 0; z < this.rows; z++)
                {
                    var idx = (z * this.columns) + x;

                    var position = new Vector3(_start.x + (x * _cellSize) + (_cellSize / 2.0f), _origin.y, _start.z + (z * _cellSize) + (_cellSize / 2.0f));
                    position.y = this.SampleHeight(position);

                    var blocked = accessor.IsPermaBlocked(idx);

                    var cell = new Cell(this, position, x, z, blocked);
                    cell.heightBlockedNeighbours = accessor.GetHeightBlockStatus(idx);

                    this.rawMatrix[x, z] = cell;

                    yield return null;
                }
            }
        }

        private IEnumerator AssignHeightSettings(MatrixBounds bounds)
        {
            var maxHeightDiff = Mathf.Tan(_maxWalkableSlopeAngle * Mathf.Deg2Rad) * _granularity;
            var maxHeightDiffDiag = maxHeightDiff * SquareRootTwo;

            var maxColumn = bounds.maxColumn;
            var maxRow = bounds.maxRow;
            bool isPartial = (maxColumn - bounds.minColumn < maxColumn) || (maxRow - bounds.minRow < maxRow);

            for (int x = bounds.minColumn; x <= maxColumn; x++)
            {
                for (int z = bounds.minRow; z <= maxRow; z++)
                {
                    var c = this.rawMatrix[x, z];
                    c.heightBlockedNeighbours = NeighbourPosition.None;

                    var ns = GetConcentricNeighbours(x, z, 1);
                    foreach (var n in ns)
                    {
                        var pos = n.GetRelativePositionTo(c);

                        //If doing a partial update we need to remove the blocked flag from the neighbour first since he may not be processed on his own.
                        if (isPartial)
                        {
                            n.heightBlockedNeighbours &= ~c.GetRelativePositionTo(n);
                        }

                        switch (pos)
                        {
                            //Straight neighbours
                            case NeighbourPosition.Bottom:
                            case NeighbourPosition.Top:
                            case NeighbourPosition.Left:
                            case NeighbourPosition.Right:
                            {
                                UpdateCellHeightData(c, n, pos, maxHeightDiff, true);
                                break;
                            }

                            //diagonals
                            default:
                            {
                                UpdateCellHeightData(c, n, pos, maxHeightDiffDiag, false);
                                break;
                            }
                        }

                        yield return null;
                    }
                }
            }

            if (!isPartial)
            {
                yield break;
            }

            //Update corner diagonals, this is only relevant for partial updates
            //Since the cells being updated only update their own relation to neighbours, there will potentially be 4 connections not updated,
            //those are the diagonals between the cells surround each corner of the bounds, e.g. bottom left corner the connection between the cell to the left and the cell below that corner.
            //since updates also update the involved neighbour, we only have to update 4 additional cells, and only on a specific diagonal.
            var bll = this[bounds.minColumn - 1, bounds.minRow];
            var blb = this[bounds.minColumn, bounds.minRow - 1];

            var tll = this[bounds.minColumn - 1, bounds.maxRow];
            var tlt = this[bounds.minColumn, bounds.maxRow + 1];

            var brr = this[bounds.maxColumn + 1, bounds.minRow];
            var brb = this[bounds.maxColumn, bounds.minRow - 1];

            var trr = this[bounds.maxColumn + 1, bounds.maxRow];
            var trt = this[bounds.maxColumn, bounds.maxRow + 1];

            if (bll != null && blb != null)
            {
                bll.heightBlockedNeighbours &= ~NeighbourPosition.BottomRight;
                blb.heightBlockedNeighbours &= ~NeighbourPosition.TopLeft;
                UpdateCellHeightData(bll, blb, NeighbourPosition.BottomRight, maxHeightDiffDiag, false);
                yield return null;
            }

            if (tll != null && tlt != null)
            {
                tll.heightBlockedNeighbours &= ~NeighbourPosition.TopRight;
                tlt.heightBlockedNeighbours &= ~NeighbourPosition.BottomLeft;
                UpdateCellHeightData(tll, tlt, NeighbourPosition.TopRight, maxHeightDiffDiag, false);
                yield return null;
            }

            if (brr != null && brb != null)
            {
                brr.heightBlockedNeighbours &= ~NeighbourPosition.BottomLeft;
                brb.heightBlockedNeighbours &= ~NeighbourPosition.TopRight;
                UpdateCellHeightData(brr, brb, NeighbourPosition.BottomLeft, maxHeightDiffDiag, false);
                yield return null;
            }

            if (trr != null && trt != null)
            {
                trr.heightBlockedNeighbours &= ~NeighbourPosition.TopLeft;
                trt.heightBlockedNeighbours &= ~NeighbourPosition.BottomRight;
                UpdateCellHeightData(trr, trt, NeighbourPosition.TopLeft, maxHeightDiffDiag, false);
                yield return null;
            }
        }

        private void UpdateCellHeightData(Cell reference, Cell neighbour, NeighbourPosition neighbourPos, float maxHeightDiff, bool testForPermanentBlock)
        {
            var dir = reference.GetDirectionTo(neighbour);
            var offsets = GetPerpendicularOffsets(dir, _granularity);
            var steps = _cellSize / _granularity;
            var diffAccumulator = 0.0f;

            for (int o = 0; o < 3; o++)
            {
                var samplePos = reference.position + offsets[o];
                var fromHeight = this.SampleHeight(samplePos);
                var distFromCenter = 0.0f;

                for (int i = 0; i < steps; i++)
                {
                    samplePos.x += (dir.x * _granularity);
                    samplePos.z += (dir.z * _granularity);
                    distFromCenter += _granularity;

                    var toHeight = this.SampleHeight(samplePos);

                    var heightDiff = Mathf.Abs(fromHeight - toHeight);
                    if (heightDiff > maxHeightDiff)
                    {
                        diffAccumulator += heightDiff;

                        if (diffAccumulator > _maxScaleHeight)
                        {
                            if (testForPermanentBlock && distFromCenter < _obstacleSensitivityRange)
                            {
                                reference.isPermanentlyBlocked = true;
                                return;
                            }

                            reference.heightBlockedNeighbours |= neighbourPos;
                            neighbour.heightBlockedNeighbours |= reference.GetRelativePositionTo(neighbour);
                            return;
                        }
                    }
                    else
                    {
                        diffAccumulator = 0.0f;
                    }

                    fromHeight = toHeight;
                }
            }
        }

        private Vector3[] GetPerpendicularOffsets(VectorXZ dir, float sampleGranularity)
        {
            Vector3 ppd;

            if (dir.x != 0 && dir.z != 0)
            {
                var offSet = _obstacleSensitivityRange / SquareRootTwo;
                ppd = new Vector3(offSet * -dir.x, 0.0f, offSet * dir.z);
            }
            else
            {
                ppd = new Vector3(_obstacleSensitivityRange * dir.z, 0.0f, _obstacleSensitivityRange * dir.x);
            }

            return new Vector3[]
            {
                Vector3.zero,
                ppd,
                ppd * -1
            };
        }

        private bool IsBlocked(Vector3 position, float blockThreshold)
        {
            var above = position + (Vector3.up * _upperBoundary);
            var below = position + (Vector3.down * _lowerBoundary);

            return UnityServices.physics.CheckCapsule(above, below, blockThreshold, Layers.blocks) || !UnityServices.physics.Raycast(above, Vector3.down, _upperBoundary + _lowerBoundary, Layers.terrain);
        }

        internal class MatrixIncrementalInitializer
        {
            private CellMatrix _matrix;
            private IEnumerator _iter;

            internal MatrixIncrementalInitializer(ICellMatrixConfiguration cfg, CellMatrixData data)
            {
                _matrix = new CellMatrix(cfg);
                _iter = _matrix.Populate(data);
            }

            internal MatrixIncrementalInitializer(ICellMatrixConfiguration cfg)
            {
                _matrix = new CellMatrix(cfg);
                _iter = _matrix.Populate();
            }

            internal bool isInitializing
            {
                get
                {
                    var moreWork = _iter.MoveNext();
                    if (!moreWork)
                    {
                        this.matrix = _matrix;
                    }

                    return moreWork;
                }
            }

            internal CellMatrix matrix
            {
                get;
                private set;
            }
        }
    }
}
