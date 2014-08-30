namespace Apex.Editor
{
    using Apex.PathFinding;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(PathServiceComponent), false)]
    public class PathServiceComponentEditor : Editor
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
