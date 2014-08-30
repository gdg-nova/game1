namespace Apex.Editor
{
    using System.Linq;
    using Apex.WorldGeometry;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(LayerMappingComponent), false)]
    public class LayerMappingComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("These settings cannot be edited in play mode.", MessageType.Info);
                return;
            }

            DrawDefaultInspector();
        }
    }
}
