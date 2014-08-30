namespace Apex.Editor
{
    using Apex.WorldGeometry;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GridComponent), false), CanEditMultipleObjects]
    public class GridComponentEditor : Editor
    {
        private SerializedProperty _friendlyName;
        private SerializedProperty _linkOriginToTransform;
        private SerializedProperty _origin;
        private SerializedProperty _sizeX;
        private SerializedProperty _sizeZ;
        private SerializedProperty _cellSize;
        private SerializedProperty _obstacleSensitivityRange;
        private SerializedProperty _subSectionsX;
        private SerializedProperty _subSectionsZ;
        private SerializedProperty _subSectionsCellOverlap;
        private SerializedProperty _generateHeightMap;
        private SerializedProperty _lowerBoundary;
        private SerializedProperty _upperBoundary;
        private SerializedProperty _heightGranularity;
        private SerializedProperty _maxWalkableSlopeAngle;
        private SerializedProperty _maxScaleHeight;
        private SerializedProperty _storeBakedDataAsAsset;
        private SerializedProperty _automaticInitialization;

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("These settings cannot be edited in play mode.", MessageType.Info);
                return;
            }

            this.serializedObject.Update();

            int baked = 0;
            var editedObjects = this.serializedObject.targetObjects;
            for (int i = 0; i < editedObjects.Length; i++)
            {
                var g = editedObjects[i] as GridComponent;
                if (g.bakedData != null)
                {
                    baked++;
                }
            }

            if (baked > 0 && baked < editedObjects.Length)
            {
                EditorGUILayout.LabelField("A mix of baked and unbaked grids cannot be edited at the same time.");
                return;
            }

            //If data is baked, only offer an option to edit or rebake
            if (baked == editedObjects.Length)
            {
                EditorGUILayout.LabelField("The grid has been baked. To change it press the Edit button below.");

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Edit"))
                {
                    foreach (var o in editedObjects)
                    {
                        var g = o as GridComponent;
                        EditorUtilities.RemoveAsset(g.bakedData);
                        g.bakedData = null;
                        EditorUtility.SetDirty(g);
                    }
                }

                if (GUILayout.Button("Re-bake Grid"))
                {
                    foreach (var o in editedObjects)
                    {
                        var g = o as GridComponent;
                        BakeGrid(g);
                    }
                }

                GUILayout.EndHorizontal();
                return;
            }

            var indentLevel = EditorGUI.indentLevel;

            EditorGUILayout.PropertyField(_friendlyName, new GUIContent("Friendly Name", "An optional friendly name for the grid that will be used in messages and such."));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Layout");
            EditorGUI.indentLevel = ++indentLevel;
            EditorGUILayout.PropertyField(_linkOriginToTransform, new GUIContent("Link Origin to Transform", "Link the center of the grid to the position of the game object."));

            if (!_linkOriginToTransform.hasMultipleDifferentValues && !_linkOriginToTransform.boolValue)
            {
                EditorGUILayout.PropertyField(_origin, new GUIContent("Origin", "The center of the grid."), true);
            }

            EditorGUILayout.PropertyField(_sizeX, new GUIContent("Size X", "Number of cells along the x-axis."));
            EditorGUILayout.PropertyField(_sizeZ, new GUIContent("Size Z", "Number of cells along the z-axis."));
            EditorGUILayout.PropertyField(_cellSize, new GUIContent("Cell Size", "The size of each grid cell, expressed as the length of one side."));
            EditorGUILayout.PropertyField(_lowerBoundary, new GUIContent("Lower Boundary", "How far below the grid's plane does the grid have influence."));
            EditorGUILayout.PropertyField(_upperBoundary, new GUIContent("Upper Boundary", "How far above the grid's plane does the grid have influence."));
            EditorGUILayout.PropertyField(_obstacleSensitivityRange, new GUIContent("Obstacle Sensitivity Range", "How close to the center of a cell must an obstacle be to block the cell."));

            if (_obstacleSensitivityRange.floatValue > (_cellSize.floatValue / 2.0f))
            {
                _obstacleSensitivityRange.floatValue = _cellSize.floatValue / 2.0f;
                Debug.LogWarning("The Obstacle Sensitivity Range of a grid cannot exceed half its Cell Size, this has been corrected.");
            }

            EditorGUI.indentLevel = --indentLevel;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Subsections");
            EditorGUI.indentLevel = ++indentLevel;

            EditorGUILayout.PropertyField(_subSectionsX, new GUIContent("Subsections X", "Number of subsections along the x-axis."));
            EditorGUILayout.PropertyField(_subSectionsZ, new GUIContent("Subsections Z", "Number of subsections along the z-axis."));
            EditorGUILayout.PropertyField(_subSectionsCellOverlap, new GUIContent("Subsections Cell Overlap", "The number of cells by which subsections overlap neighbouring subsections."));

            EditorGUI.indentLevel = --indentLevel;
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_generateHeightMap, new GUIContent("Generate Height Map", "Controls whether the grid generates a height map to allow height sensitive navigation."));
            EditorGUI.indentLevel = ++indentLevel;

            if (!_generateHeightMap.hasMultipleDifferentValues && _generateHeightMap.boolValue)
            {
                EditorGUILayout.PropertyField(_heightGranularity, new GUIContent("Height Granularity", "The precision of the height map, expressed as the distance between height samples."));
                EditorGUILayout.PropertyField(_maxWalkableSlopeAngle, new GUIContent("Max Walkable Slope Angle", "The maximum angle at which a cell is deemed walkable."));
                EditorGUILayout.PropertyField(_maxScaleHeight, new GUIContent("Max Scale Height", "The maximum height that the unit can scale, i.e. walk onto even if its is a vertical move."));
            }

            EditorGUI.indentLevel = --indentLevel;
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_automaticInitialization, new GUIContent("Automatic Initialization", "Controls whether the grid is automatically initialized when enabled. If unchecked the grid must be manually initialized."));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_storeBakedDataAsAsset, new GUIContent("Store Grid data as asset", "Store baked data in a separate asset file instead of storing to the scene, this enables prefab'ing."));

            this.serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Bake Grid"))
            {
                foreach (var o in editedObjects)
                {
                    var g = o as GridComponent;
                    BakeGrid(g);
                }
            }
        }

        private static void BakeGrid(GridComponent g)
        {
            var builder = g.GetBuilder();

            var matrix = CellMatrix.Create(builder);

            var data = g.bakedData;
            if (data == null)
            {
                data = CellMatrixData.Create(matrix);

                g.bakedData = data;
            }
            else
            {
                data.Refresh(matrix);
            }

            if (g.storeBakedDataAsAsset)
            {
                EditorUtilities.CreateOrUpdateAsset(data, g.friendlyName.Trim());
            }
            else
            {
                EditorUtility.SetDirty(data);
            }

            g.ResetGrid();
            EditorUtility.SetDirty(g);

            Debug.Log(string.Format("The grid {0} was successfully baked.", g.friendlyName));
        }

        private void OnEnable()
        {
            _friendlyName = this.serializedObject.FindProperty("_friendlyName");

            _linkOriginToTransform = this.serializedObject.FindProperty("_linkOriginToTransform");
            _origin = this.serializedObject.FindProperty("_origin");
            _sizeX = this.serializedObject.FindProperty("sizeX");
            _sizeZ = this.serializedObject.FindProperty("sizeZ");
            _cellSize = this.serializedObject.FindProperty("cellSize");
            _obstacleSensitivityRange = this.serializedObject.FindProperty("obstacleSensitivityRange");

            _subSectionsX = this.serializedObject.FindProperty("subSectionsX");
            _subSectionsZ = this.serializedObject.FindProperty("subSectionsZ");
            _subSectionsCellOverlap = this.serializedObject.FindProperty("subSectionsCellOverlap");

            _generateHeightMap = this.serializedObject.FindProperty("generateHeightmap");
            _lowerBoundary = this.serializedObject.FindProperty("lowerBoundary");
            _upperBoundary = this.serializedObject.FindProperty("upperBoundary");
            _heightGranularity = this.serializedObject.FindProperty("heightGranularity");
            _maxWalkableSlopeAngle = this.serializedObject.FindProperty("maxWalkableSlopeAngle");
            _maxScaleHeight = this.serializedObject.FindProperty("maxScaleHeight");

            _storeBakedDataAsAsset = this.serializedObject.FindProperty("_storeBakedDataAsAsset");
            _automaticInitialization = this.serializedObject.FindProperty("_automaticInitialization");
        }
    }
}
