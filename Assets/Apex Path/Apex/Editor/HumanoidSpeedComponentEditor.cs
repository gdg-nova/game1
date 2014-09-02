namespace Apex.Editor
{
    using Apex.Steering.Components;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(HumanoidSpeedComponent), false), CanEditMultipleObjects]
    public class HumanoidSpeedComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Test Changes"))
                {
                    var hs = this.target as HumanoidSpeedComponent;
                    hs.Refresh();
                }
            }
        }
    }
}
