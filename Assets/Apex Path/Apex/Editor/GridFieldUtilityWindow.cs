namespace Apex.Editor
{
    using Apex.WorldGeometry;
    using UnityEditor;
    using UnityEngine;

    public class GridSticherUtilityWindow : EditorWindow
    {
        private static GridSticherUtilityWindow _instance;

        private int _fieldSizeX = 4;
        private int _fieldSizeZ = 4;
        private bool _disableAutoInit = true;

        private string _friendlyName = "Grid";
        private int _sizeX = 10;
        private int _sizeZ = 10;
        private float _cellSize = 2f;
        private float _obstacleSensitivityRange = 0.5f;
        private int _subSectionsX = 2;
        private int _subSectionsZ = 2;
        private int _subSectionsCellOverlap = 2;
        private bool _generateHeightMap = true;
        private float _lowerBoundary = 1f;
        private float _upperBoundary = 10f;
        private float _heightGranularity = 0.1f;
        private float _maxWalkableSlopeAngle = 30f;
        private float _maxScaleHeight = 0.5f;

        [MenuItem("Window/Apex Path - Grid Field")]
        public static void ShowWindow()
        {
            _instance = EditorWindow.GetWindow<GridSticherUtilityWindow>(true, "Apex Path - Grid Field");
        }

        private void OnGUI()
        {
            var indentLevel = EditorGUI.indentLevel + 1;

            EditorGUILayout.HelpBox("This tool allows you to easily generate a grid field, that is a number of grids stitched together in order to cover a large scene", MessageType.Info);
            EditorGUILayout.Separator();

            EditorGUI.indentLevel = indentLevel;
            EditorGUILayout.LabelField("Field");
            EditorGUI.indentLevel = ++indentLevel;
            _fieldSizeX = EditorGUILayout.IntField(new GUIContent("Grids along x-axis", "Number of grids along the x-axis."), _fieldSizeX);
            _fieldSizeZ = EditorGUILayout.IntField(new GUIContent("Grids along z-axis", "Number of grids along the x-axis."), _fieldSizeZ);
            _disableAutoInit = EditorGUILayout.Toggle(new GUIContent("Disable Automatic Initialization.", "Sets the grid's automatic initialization setting to false."), _disableAutoInit);

            EditorGUILayout.Separator();
            _friendlyName = EditorGUILayout.TextField(new GUIContent("Grid Base Name", "An optional friendly name for the grids, each grid will also get a number post fix."), _friendlyName);

            EditorGUI.indentLevel = --indentLevel;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Layout");
            EditorGUI.indentLevel = ++indentLevel;

            _sizeX = EditorGUILayout.IntField(new GUIContent("Size X", "Number of cells along the x-axis."), _sizeX);
            _sizeZ = EditorGUILayout.IntField(new GUIContent("Size Z", "Number of cells along the z-axis."), _sizeZ);

            _cellSize = EditorGUILayout.FloatField(new GUIContent("Cell Size", "The size of each grid cell, expressed as the length of one side."), _cellSize);
            _lowerBoundary = EditorGUILayout.FloatField(new GUIContent("Lower Boundary", "How far below the grid's plane does the grid have influence."), _lowerBoundary);
            _upperBoundary = EditorGUILayout.FloatField(new GUIContent("Upper Boundary", "How far above the grid's plane does the grid have influence."), _upperBoundary);
            _obstacleSensitivityRange = EditorGUILayout.FloatField(new GUIContent("Obstacle Sensitivity Range", "How close to the center of a cell must an obstacle be to block the cell."), _obstacleSensitivityRange);

            if (_obstacleSensitivityRange > (_cellSize / 2.0f))
            {
                _obstacleSensitivityRange = _cellSize / 2.0f;
            }

            EditorGUI.indentLevel = --indentLevel;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Subsections");
            EditorGUI.indentLevel = ++indentLevel;

            EditorGUILayout.IntField(new GUIContent("Subsections X", "Number of subsections along the x-axis."), _subSectionsX);
            _subSectionsZ = EditorGUILayout.IntField(new GUIContent("Subsections Z", "Number of subsections along the z-axis."), _subSectionsZ);
            _subSectionsCellOverlap = EditorGUILayout.IntField(new GUIContent("Subsections Cell Overlap", "The number of cells by which subsections overlap neighbouring subsections."), _subSectionsCellOverlap);

            EditorGUI.indentLevel = --indentLevel;
            EditorGUILayout.Separator();
            _generateHeightMap = EditorGUILayout.Toggle(new GUIContent("Generate Height Map", "Controls whether the grid generates a height map to allow height sensitive navigation."), _generateHeightMap);
            EditorGUI.indentLevel = ++indentLevel;

            if (_generateHeightMap)
            {
                _heightGranularity = EditorGUILayout.FloatField(new GUIContent("Height Granularity", "The precision of the height map, expressed as the distance between height samples."), _heightGranularity);
                _maxWalkableSlopeAngle = EditorGUILayout.FloatField(new GUIContent("Max Walkable Slope Angle", "The maximum angle at which a cell is deemed walkable."), _maxWalkableSlopeAngle);
                _maxScaleHeight = EditorGUILayout.FloatField(new GUIContent("Max Scale Height", "The maximum height that the unit can scale, i.e. walk onto even if its is a vertical move."), _maxScaleHeight);
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Create Grid Field"))
            {
                CreateGridField();

                _instance.Close();
                _instance = null;
            }
        }

        private void CreateGridField()
        {
            var root = new GameObject("Grids");

            float gridWidth = _sizeX * _cellSize;
            float gridDepth = _sizeZ * _cellSize;

            float startX = -((_fieldSizeX - 1) / 2f) * gridWidth;
            float startZ = -((_fieldSizeZ - 1) / 2f) * gridDepth;

            for (int x = 0; x < _fieldSizeX; x++)
            {
                for (int z = 0; z < _fieldSizeZ; z++)
                {
                    var name = string.Format("{0}_{1}_{2}", _friendlyName, x, z);

                    var goGrid = new GameObject(name);
                    goGrid.transform.parent = root.transform;
                    goGrid.transform.localPosition = new Vector3(startX + (x * gridWidth), 0f, startZ + (z * gridDepth));
                                        
                    //Init the grid
                    var g = goGrid.AddComponent<GridComponent>();
                    g.friendlyName = name;
                    g.sizeX = _sizeX;
                    g.sizeZ = _sizeZ;
                    g.cellSize = _cellSize;
                    g.obstacleSensitivityRange = _obstacleSensitivityRange;
                    g.subSectionsX = _subSectionsX;
                    g.subSectionsZ = _subSectionsZ;
                    g.subSectionsCellOverlap = _subSectionsCellOverlap;
                    g.generateHeightmap = _generateHeightMap;
                    g.lowerBoundary = _lowerBoundary;
                    g.upperBoundary = _upperBoundary;
                    g.heightGranularity = _heightGranularity;
                    g.maxWalkableSlopeAngle = _maxWalkableSlopeAngle;
                    g.maxScaleHeight = _maxScaleHeight;
                    g.automaticInitialization = !_disableAutoInit;

                    //Add portals
                    if (x < _fieldSizeX - 1 || z < _fieldSizeZ - 1)
                    {
                        var goPortals = new GameObject("Portals");
                        goPortals.transform.parent = goGrid.transform;
                        goPortals.transform.localPosition = Vector3.zero;

                        if (x < _fieldSizeX - 1)
                        {
                            var centerX = goPortals.transform.position.x + ((gridWidth - _cellSize) / 2.0f);
                            var centerZ = goPortals.transform.position.z;

                            ConstructPortal(
                                goPortals,
                                new Bounds(new Vector3(centerX, 0f, centerZ), new Vector3(1f, 0.1f, gridDepth)),
                                new Bounds(new Vector3(centerX + _cellSize, 0f, centerZ), new Vector3(1f, 0.1f, gridDepth)),
                                "RightPortal");
                        }

                        if (z < _fieldSizeZ - 1)
                        {
                            var centerX = goPortals.transform.position.x;
                            var centerZ = goPortals.transform.position.z + ((gridDepth - _cellSize) / 2.0f);

                            ConstructPortal(
                                goPortals,
                                new Bounds(new Vector3(centerX, 0f, centerZ), new Vector3(gridWidth, 0.1f, 1f)),
                                new Bounds(new Vector3(centerX, 0f, centerZ + _cellSize), new Vector3(gridWidth, 0.1f, 1f)),
                                "TopPortal");
                        }

                        goPortals.AddComponent<PortalActionNoneComponent>();
                    }       
                }
            }
        }

        private void ConstructPortal(GameObject parent, Bounds one, Bounds two, string name)
        {
            var portal = parent.AddComponent<GridPortalComponent>();

            portal.portalName = name;
            portal.type = PortalType.Connector;
            portal.portalOne = one;
            portal.portalTwo = two;
        }
    }
}
