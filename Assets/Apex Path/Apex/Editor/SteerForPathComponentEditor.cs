namespace Apex.Editor
{
    using Apex.Steering.Components;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(SteerForPathComponent), false), CanEditMultipleObjects]
    public class SteerForPathComponentEditor : Editor
    {
        private SerializedProperty _pathingPriority;
        private SerializedProperty _usePathSmoothing;
        private SerializedProperty _allowCornerCutting;
        private SerializedProperty _preventOffGridNavigation;
        private SerializedProperty _preventDiagonalMoves;
        private SerializedProperty _navigateToNearestIfBlocked;
        private SerializedProperty _slowingDistance;
        private SerializedProperty _slowingAlgorithm;
        private SerializedProperty _requestNextWaypointDistance;
        private SerializedProperty _proximityEvaluationMinAngle;
        private SerializedProperty _maxEscapeCellDistanceIfOriginBlocked;
        private SerializedProperty _nextNodeDistance;
        private SerializedProperty _arrivalDistance;
        private SerializedProperty _replanMode;
        private SerializedProperty _replanInterval;
        private SerializedProperty _announceAllNodes;
        private SerializedProperty _weight;

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("These settings cannot be edited in play mode.", MessageType.Info);
                return;
            }

            this.serializedObject.Update();

            EditorGUILayout.PropertyField(_weight, new GUIContent("Weight", "The weight of this steering behavior in relation to other steering behaviors."));

            var indentLevel = EditorGUI.indentLevel;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Path Finder Options");
            EditorGUI.indentLevel = ++indentLevel;

            EditorGUILayout.PropertyField(_pathingPriority, new GUIContent("Pathing Priority", "The priority with which this unit's path requests should be processed."));
            EditorGUILayout.PropertyField(_usePathSmoothing, new GUIContent("Use Path Smoothing", "Whether to use path smoothing to create more natural paths."));
            EditorGUILayout.PropertyField(_allowCornerCutting, new GUIContent("Allow Corner Cutting", "Whether to allow the path to cut corners. Corner cutting has slightly better performance, but produces less natural routes."));
            EditorGUILayout.PropertyField(_preventOffGridNavigation, new GUIContent("Prevent Off Grid Navigation", "Whether navigation off-grid is prohibited."));
            EditorGUILayout.PropertyField(_preventDiagonalMoves, new GUIContent("Prevent Diagonal Moves", "Whether the unit is allowed to move to diagonal neighbours."));
            EditorGUILayout.PropertyField(_navigateToNearestIfBlocked, new GUIContent("Navigate To Nearest If Blocked", "Whether the unit will navigate to the nearest possible position if the actual destination is blocked or otherwise inaccessible."));

            EditorGUI.indentLevel = --indentLevel;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Path Following");
            EditorGUI.indentLevel = ++indentLevel;

            EditorGUILayout.PropertyField(_nextNodeDistance, new GUIContent("Next Node Distance", "The distance from the current destination node on the path at which the unit will switch to the next node."));
            EditorGUILayout.PropertyField(_arrivalDistance, new GUIContent("Arrival Distance", "The distance from the final destination where the unit will stop."));
            EditorGUILayout.PropertyField(_slowingDistance, new GUIContent("Slowing Distance", "The distance within which the unit will start to slow down for arrival."));
            EditorGUILayout.PropertyField(_slowingAlgorithm, new GUIContent("Slowing Algorithm", "The algorithm used to slow the unit for arrival."));
            EditorGUILayout.PropertyField(_requestNextWaypointDistance, new GUIContent("Request Next Waypoint Distance", "The distance from the current way point at which the next way point will be requested."));
            EditorGUILayout.PropertyField(_proximityEvaluationMinAngle, new GUIContent("Proximity Evaluation Min Angle", "The angle at which the end target must be approached before a proximity check is deemed valid. This applies to slowingDistance and requestNextWaypointDistance."));
            EditorGUILayout.PropertyField(_maxEscapeCellDistanceIfOriginBlocked, new GUIContent("Max Escape Cell Distance If Origin Blocked", "The maximum escape cell distance if the unit's starting position is blocked."));
            EditorGUILayout.PropertyField(_announceAllNodes, new GUIContent("Announce All Nodes", "Whether to raise a navigation event for each node reached along a path."));

            EditorGUI.indentLevel = --indentLevel;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Replanning");
            EditorGUI.indentLevel = ++indentLevel;

            EditorGUILayout.PropertyField(_replanMode, new GUIContent("Replan Mode", "The replan mode."));
            EditorGUILayout.PropertyField(_replanInterval, new GUIContent("Replan Interval", "The time between replans, the exact meaning depends on the replan mode."));

            this.serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _pathingPriority = this.serializedObject.FindProperty("pathingPriority");
            _usePathSmoothing = this.serializedObject.FindProperty("usePathSmoothing");
            _allowCornerCutting = this.serializedObject.FindProperty("allowCornerCutting");
            _preventOffGridNavigation = this.serializedObject.FindProperty("preventOffGridNavigation");
            _preventDiagonalMoves = this.serializedObject.FindProperty("preventDiagonalMoves");
            _navigateToNearestIfBlocked = this.serializedObject.FindProperty("navigateToNearestIfBlocked");
            _slowingDistance = this.serializedObject.FindProperty("slowingDistance");
            _slowingAlgorithm = this.serializedObject.FindProperty("slowingAlgorithm");
            _requestNextWaypointDistance = this.serializedObject.FindProperty("requestNextWaypointDistance");
            _proximityEvaluationMinAngle = this.serializedObject.FindProperty("proximityEvaluationMinAngle");
            _maxEscapeCellDistanceIfOriginBlocked = this.serializedObject.FindProperty("maxEscapeCellDistanceIfOriginBlocked");
            _nextNodeDistance = this.serializedObject.FindProperty("nextNodeDistance");
            _arrivalDistance = this.serializedObject.FindProperty("arrivalDistance");
            _replanMode = this.serializedObject.FindProperty("replanMode");
            _replanInterval = this.serializedObject.FindProperty("replanInterval");
            _announceAllNodes = this.serializedObject.FindProperty("announceAllNodes");
            _weight = this.serializedObject.FindProperty("weight");
        }
    }
}