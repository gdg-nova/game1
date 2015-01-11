/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[CustomEditor(typeof(MadLevelProperty))]
public class MadLevelPropertyInspector : MadLevelManager.MadEditorBase {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    SerializedProperty showWhenEnabled;
    SerializedProperty showWhenDisabled;
    SerializedProperty textFromProperty;
    SerializedProperty textPropertyName;

    MadLevelProperty property;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        property = target as MadLevelProperty;

        showWhenEnabled = serializedObject.FindProperty("showWhenEnabled");
        showWhenDisabled = serializedObject.FindProperty("showWhenDisabled");
        textFromProperty = serializedObject.FindProperty("textFromProperty");
        textPropertyName = serializedObject.FindProperty("textPropertyName");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();

        EditorGUILayout.Space();

        if (MadGUI.Button("Select Parent Icon", Color.yellow)) {
            Selection.activeObject = property.icon.gameObject;
        }


        EditorGUILayout.Space();
        GUILayout.Label("Property", "HeaderLabel");
        EditorGUILayout.Space();

        MadGUI.Indent(() => {

            EditorGUILayout.Space();
            GUILayout.Label("Property Name: " + property.name);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("State");

            GUILayout.FlexibleSpace();

            if (property.linked) {
                if (MadGUI.Button("LINKED", Color.cyan, GUILayout.Width(60))) {
                    if (EditorUtility.DisplayDialog(
                        "State Linked",
                        "This property state is linked by '" + property.linkage.name
                        + "' property and cannot be changed directly. Do you want to select the linkage owner?",
                        "Yes", "No")) {
                        Selection.activeObject = property.linkage.gameObject;
                    }
                }

            } else if (property.propertyEnabled) {
                if (MadGUI.Button("On", Color.green, GUILayout.Width(50))) {
                    property.propertyEnabled = false;
                    EditorUtility.SetDirty(property);
                }
            } else {
                if (MadGUI.Button("Off", Color.red, GUILayout.Width(50))) {
                    property.propertyEnabled = true;
                    EditorUtility.SetDirty(property);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (property.GetComponent<MadText>() != null) {
                EditorGUILayout.Space();

                MadGUI.PropertyField(textFromProperty, "Text From Property");
                MadGUI.ConditionallyEnabled(textFromProperty.boolValue, () => {
                    MadGUI.PropertyField(textPropertyName, "Text Property Name");
                });
            }
        });


        GUILayout.Label("Connections", "HeaderLabel");
        EditorGUILayout.Space();

        MadGUI.Indent(() => {
            bool connectionChanged;

            GUILayout.Label("Things to show when this property is enabled:");
            connectionChanged = new MadGUI.ArrayList<GameObject>(showWhenEnabled, (p) => {
                MadGUI.PropertyField(p, "");
            }).Draw();

            if (connectionChanged) {
                property.icon.ApplyConnections();
            }

            EditorGUILayout.Space();

            GUILayout.Label("Things to show when this property is disabled:");
            connectionChanged = new MadGUI.ArrayList<GameObject>(showWhenDisabled, (p) => {
                MadGUI.PropertyField(p, "");
            }).Draw();

            if (connectionChanged) {
                property.icon.ApplyConnections();
            }
        });

        serializedObject.ApplyModifiedProperties();
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif