/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

[CustomEditor(typeof(MadLevelInputControl))]
public class MadLevelInputControlInspector : Editor {


    #region Fields

    SerializedProperty inputMode;

    SerializedProperty keycodeLeft;
    SerializedProperty keycodeRight;
    SerializedProperty keycodeUp;
    SerializedProperty keycodeDown;
    SerializedProperty keycodeEnter;

    SerializedProperty axisHorizontal;
    SerializedProperty axisVertical;
    SerializedProperty axisEnter;

    SerializedProperty activateOnStart;
    SerializedProperty onlyOnMobiles;

    SerializedProperty repeat;
    SerializedProperty repeatInterval;

    MadLevelInputControl script;

    TraverseRules traverseRule;

    #endregion

    private void OnEnable() {
        script = target as MadLevelInputControl;

        inputMode = serializedObject.FindProperty("inputMode");

        keycodeLeft = serializedObject.FindProperty("keycodeLeft");
        keycodeRight = serializedObject.FindProperty("keycodeRight");
        keycodeUp = serializedObject.FindProperty("keycodeUp");
        keycodeDown = serializedObject.FindProperty("keycodeDown");
        keycodeEnter = serializedObject.FindProperty("keycodeEnter");

        axisHorizontal = serializedObject.FindProperty("axisHorizontal");
        axisVertical = serializedObject.FindProperty("axisVertical");
        axisEnter = serializedObject.FindProperty("axisEnter");

        activateOnStart = serializedObject.FindProperty("activateOnStart");
        onlyOnMobiles = serializedObject.FindProperty("onlyOnMobiles");

        repeat = serializedObject.FindProperty("repeat");
        repeatInterval = serializedObject.FindProperty("repeatInterval");

        // look for traverse rule
        if (script.traverseRule.GetType() == typeof(MadLevelInputControl.SimpleTraverseRule)) {
            traverseRule = TraverseRules.Simple;
        } else if (script.traverseRule.GetType() == typeof(MadLevelInputControl.DirectionTraverseRule)) {
            traverseRule = TraverseRules.Direction;
        } else {
            traverseRule = TraverseRules.Custom;
        }
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();

        MadGUI.PropertyFieldEnumPopup(inputMode, "Input Mode");

        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        switch (script.inputMode) {
            case MadLevelInputControl.InputMode.InputAxes:
                MadGUI.PropertyField(axisHorizontal, "Horizontal Axis");
                MadGUI.PropertyField(axisVertical, "Vertical Axis");
                MadGUI.PropertyField(axisEnter, "Enter Axis");
                break;

            case MadLevelInputControl.InputMode.KeyCodes:
                MadGUI.PropertyField(keycodeLeft, "Key Code Left");
                MadGUI.PropertyField(keycodeRight, "Key Code Right");
                MadGUI.PropertyField(keycodeUp, "Key Code Up");
                MadGUI.PropertyField(keycodeDown, "Key Code Down");
                MadGUI.PropertyField(keycodeEnter, "Key Code Enter");
                break;

            default:
                Debug.LogError("Unknown input mode: " + script.inputMode);
                break;

        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        MadGUI.PropertyFieldEnumPopup(activateOnStart, "Activate On Start");

        EditorGUILayout.Space();

        MadGUI.PropertyField(onlyOnMobiles, "Only On Mobiles");

        MadGUI.PropertyField(repeat, "Repeat");
        EditorGUI.indentLevel++;
        MadGUI.PropertyField(repeatInterval, "Interval");
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        traverseRule = (TraverseRules) EditorGUILayout.EnumPopup("Traverse Rule", traverseRule);

        if (traverseRule == TraverseRules.Custom) {
            MadGUI.Info(
                "Custom traverse rule means that you have to assign your own traverse rule programatically when the scene is loaded."
                + "You don't have to change the rule to 'Custom', just setting it in the component will do. Please refer to the documentation for more information.");
        }

        if (EditorGUI.EndChangeCheck()) {
            switch (traverseRule) {
                case TraverseRules.Simple:
                    script.traverseRule = new MadLevelInputControl.SimpleTraverseRule();
                    break;
                case TraverseRules.Direction:
                    script.traverseRule = new MadLevelInputControl.DirectionTraverseRule();
                    break;
                case TraverseRules.Custom:
                    // do nothing
                    break;
                default:
                    Debug.LogError("Unknown traverse rule: " + traverseRule);
                    break;
            }

            EditorUtility.SetDirty(script);
        }

        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (MadGUI.Button("Help", Color.white, GUILayout.Width(80))) {
            Application.OpenURL(MadLevelHelp.InputControl);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    enum TraverseRules {
        Simple,
        Direction,
        Custom,
    }

}