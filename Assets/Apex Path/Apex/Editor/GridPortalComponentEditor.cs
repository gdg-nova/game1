namespace Apex.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Apex.WorldGeometry;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GridPortalComponent))]
    public class GridPortalComponentEditor : Editor
    {
        private static List<KeyValuePair<string, Type>> _actionsList;
        private static string[] _actionsNames;

        private Vector3 _lastPosition;

        private void OnEnable()
        {
            var p = this.target as GridPortalComponent;
            _lastPosition = p.transform.position;

            if (p.portalOne.size == Vector3.zero)
            {
                p.portalOne = new Bounds(new Vector3(_lastPosition.x - 3f, _lastPosition.y, _lastPosition.z - 3f), new Vector3(1f, 0.2f, 1f));
            }

            if (p.portalTwo.size == Vector3.zero)
            {
                p.portalTwo = new Bounds(new Vector3(_lastPosition.x + 3f, _lastPosition.y, _lastPosition.z - 3f), new Vector3(1f, 0.2f, 1f));
            }
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("These settings cannot be edited in play mode.", MessageType.Info);
                return;
            }

            var owner = this.target as GridPortalComponent;

            if (owner.transform.position != _lastPosition)
            {
                var diff = (owner.transform.position - _lastPosition);
                _lastPosition = owner.transform.position;
                owner.portalOne.center += diff;
                owner.portalTwo.center += diff;
            }

            DrawDefaultInspector();

            ShowActionSelector(owner);
        }

        private void ShowActionSelector(GridPortalComponent portal)
        {
            object pa = portal.As<IPortalAction>();
            if (pa == null)
            {
                pa = portal.As<IPortalActionFactory>();
            }

            if (pa == null)
            {
                if (_actionsList == null)
                {
                    _actionsList = new List<KeyValuePair<string, Type>>();

                    var asm = Assembly.GetAssembly(typeof(GridPortalComponent));
                    foreach (var actionType in asm.GetTypes().Where(t => (typeof(IPortalActionFactory).IsAssignableFrom(t) || typeof(IPortalAction).IsAssignableFrom(t)) && t.IsClass && !t.IsAbstract))
                    {
                        var actionName = actionType.Name;

                        var acm = Attribute.GetCustomAttribute(actionType, typeof(AddComponentMenu)) as AddComponentMenu;
                        if (acm != null)
                        {
                            var startIdx = acm.componentMenu.LastIndexOf('/') + 1;
                            actionName = acm.componentMenu.Substring(startIdx);
                        }

                        var pair = new KeyValuePair<string, Type>(actionName, actionType);
                        _actionsList.Add(pair);
                    }

                    _actionsList.Sort((a, b) => a.Key.CompareTo(b.Key));
                    _actionsNames = _actionsList.Select(p => p.Key).ToArray();
                }

                EditorGUILayout.Separator();
                var style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.yellow;
                EditorGUILayout.LabelField("Select a Portal Action", style);
                var selectedActionIdx = EditorGUILayout.Popup(-1, _actionsNames);
                if (selectedActionIdx >= 0)
                {
                    portal.gameObject.AddComponent(_actionsList[selectedActionIdx].Value);
                }
            }
        }

        private void OnSceneGUI()
        {
            var owner = this.target as GridPortalComponent;

            var portalOneCenter = Handles.PositionHandle(owner.portalOne.center, Quaternion.identity);
            var portalTwoCenter = Handles.PositionHandle(owner.portalTwo.center, Quaternion.identity);

            var handleSize = HandleUtility.GetHandleSize(owner.portalOne.center) * .75f;
            var portalOneScale = Handles.ScaleHandle(owner.portalOne.size, owner.portalOne.center, Quaternion.identity, handleSize);

            handleSize = HandleUtility.GetHandleSize(owner.portalTwo.center) * .75f;
            var portalTwoScale = Handles.ScaleHandle(owner.portalTwo.size, owner.portalTwo.center, Quaternion.identity, handleSize);

            if (GUI.changed)
            {
                owner.portalOne.center = portalOneCenter;
                owner.portalTwo.center = portalTwoCenter;
                owner.portalOne.size = portalOneScale.AdjustAxis(0.2f, Axis.Y);
                owner.portalTwo.size = portalTwoScale.AdjustAxis(0.2f, Axis.Y);
                EditorUtility.SetDirty(owner);
            }
        }
    }
}
