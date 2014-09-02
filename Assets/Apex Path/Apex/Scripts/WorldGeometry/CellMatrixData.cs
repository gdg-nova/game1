/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System.Collections.Generic;
    using Apex.DataStructures;
    using UnityEngine;

    /// <summary>
    /// Data class for encapsulating and storing grid data.
    /// </summary>
    public class CellMatrixData : ScriptableObject
    {
        [HideInInspector]
        [SerializeField]
        private float[] _heights;

        [HideInInspector]
        [SerializeField]
        private int _heightEntries;

        [HideInInspector]
        [SerializeField]
        private int[] _blockedIndexes;

        [HideInInspector]
        [SerializeField]
        private NeighbourPosition[] _heightBlockStatus;

        /// <summary>
        /// Creates a data instance from the specified configuration.
        /// </summary>
        /// <param name="matrix">The matrix to store.</param>
        /// <returns>The data instance</returns>
        public static CellMatrixData Create(CellMatrix matrix)
        {
            var so = ScriptableObject.CreateInstance<CellMatrixData>();
            so.Initialize(matrix);

            return so;
        }

        /// <summary>
        /// Updates the data with the new state of the matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void Refresh(CellMatrix matrix)
        {
            Initialize(matrix);
        }

        internal DataAccessor GetAccessor()
        {
            return new DataAccessor(this);
        }

        private void Initialize(CellMatrix cellMatrix)
        {
            var sizeX = cellMatrix.columns;
            var sizeZ = cellMatrix.rows;

            var matrix = cellMatrix.rawMatrix;

            _heightBlockStatus = new NeighbourPosition[sizeX * sizeZ];

            var blockedIndexsList = new List<int>();
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    var arrIdx = (z * sizeX) + x;

                    var cell = matrix[x, z];

                    if (!cell.IsWalkableToAny())
                    {
                        blockedIndexsList.Add(arrIdx);
                    }

                    _heightBlockStatus[arrIdx] = cell.heightBlockedNeighbours;
                }
            }

            _blockedIndexes = blockedIndexsList.ToArray();

            var heightSize = cellMatrix.heightMapSize;
            var heightX = heightSize.x;
            var heightZ = heightSize.z;

            _heightEntries = cellMatrix.heightMapEntries;
            _heights = new float[heightX * heightZ];

            for (int x = 0; x < heightX; x++)
            {
                for (int z = 0; z < heightZ; z++)
                {
                    var arrIdx = (z * heightX) + x;

                    _heights[arrIdx] = cellMatrix.SampleHeight(x, z);
                }
            }
        }

        internal class DataAccessor
        {
            private CellMatrixData _data;
            private HashSet<int> _blockedLookup;

            internal DataAccessor(CellMatrixData data)
            {
                _data = data;
                _blockedLookup = new HashSet<int>(_data._blockedIndexes);
            }

            internal int heightEntries
            {
                get { return _data._heightEntries; }
            }

            internal float GetHeight(int idx)
            {
                return _data._heights[idx];
            }

            internal NeighbourPosition GetHeightBlockStatus(int idx)
            {
                return _data._heightBlockStatus[idx];
            }

            internal bool IsPermaBlocked(int idx)
            {
                return _blockedLookup.Contains(idx);
            }
        }
    }
}
