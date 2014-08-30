namespace Apex.Editor
{
    using Apex.Debugging;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GridVisualizer), false)]
    public class GridVisualizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var v = this.target as GridVisualizer;

            if (v.drawMode == GridVisualizer.GridMode.Accessibility)
            {
                EditorGUILayout.HelpBox("Please note that in order to show height map data, the grid(s) must be baked.", MessageType.Info);
            }
        }
    }
}
