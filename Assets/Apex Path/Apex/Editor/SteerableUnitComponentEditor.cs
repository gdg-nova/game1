namespace Apex.Editor
{
    using Apex.Steering;
    using UnityEditor;

    /// <summary>
    /// Editor for <see cref="SteerableUnitComponent"/>s.
    /// </summary>
    [CustomEditor(typeof(SteerableUnitComponent)), CanEditMultipleObjects]
    public class SteerableUnitComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var c = this.target as SteerableUnitComponent;
            EditorGUILayout.LabelField("Current Speed", c.speed.ToString());
            DrawDefaultInspector();
        }
    }
}
