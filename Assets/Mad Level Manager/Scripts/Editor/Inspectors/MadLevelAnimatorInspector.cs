/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[CustomEditor(typeof(MadLevelAnimator))]
public class MadLevelAnimatorInspector : MadAnimatorInspector {

    #region Fields

    SerializedProperty onLevelLocked;
    SerializedProperty onLevelUnlocked;

    SerializedProperty delayModifiers;
    SerializedProperty offsetModifiers;

    SerializedProperty startupPositionApplyMethod;
    SerializedProperty startupPosition;

    SerializedProperty startupRotationApplyMethod;
    SerializedProperty startupRotation;

    SerializedProperty startupScaleApplyMethod;
    SerializedProperty startupScale;

    #endregion

    #region Methods

    protected override void OnEnable() {
        base.OnEnable();

        onLevelLocked = serializedObject.FindProperty("onLevelLocked");
        onLevelUnlocked = serializedObject.FindProperty("onLevelUnlocked");

        delayModifiers = serializedObject.FindProperty("delayModifiers");
        offsetModifiers = serializedObject.FindProperty("offsetModifiers");

        startupPositionApplyMethod = serializedObject.FindProperty("startupPositionApplyMethod");
        startupPosition = serializedObject.FindProperty("startupPosition");

        startupRotationApplyMethod = serializedObject.FindProperty("startupRotationApplyMethod");
        startupRotation = serializedObject.FindProperty("startupRotation");

        startupScaleApplyMethod = serializedObject.FindProperty("startupScaleApplyMethod");
        startupScale = serializedObject.FindProperty("startupScale");
    }

    protected override void GUIEvents() {
        base.GUIEvents();

        GUIEvent("On Level Locked", onLevelLocked);
        GUIEvent("On Level Unlocked", onLevelUnlocked);
    }

    protected override void GUIElements() {
        base.GUIElements();

        GUILayout.Label("Modifiers", "HeaderLabel");

        using (MadGUI.Indent()) {
            EditorGUILayout.LabelField("Delay");
            using (MadGUI.Indent()) {
                GUIModifiers(delayModifiers);
            }

            EditorGUILayout.LabelField("Offset");
            using (MadGUI.Indent()) {
                GUIModifiers(offsetModifiers);
            }
        }

        GUILayout.Label("Startup Properties", "HeaderLabel");

        using (MadGUI.Indent()) {
            GUIStartupProperty(startupPositionApplyMethod, startupPosition, "Position");
            GUIStartupProperty(startupRotationApplyMethod, startupRotation, "Rotation");
            GUIStartupProperty(startupScaleApplyMethod, startupScale, "Scale");
        }
    }

    private void GUIModifiers(SerializedProperty listProperty) {
        var list = new MadGUI.ArrayList<MadLevelAnimator.Modifier>(listProperty, (p) => {
            GUIModifier(p);
        });
        list.Draw();
    }

    private void GUIModifier(SerializedProperty modifierProperty) {
        var animationName = modifierProperty.FindPropertyRelative("animationName");

        var modifierFunction = modifierProperty.FindPropertyRelative("modifierFunction");
        var baseOperator = modifierProperty.FindPropertyRelative("baseOperator");
        var firstParameter = modifierProperty.FindPropertyRelative("firstParameter");
        var valueOperator = modifierProperty.FindPropertyRelative("valueOperator");
        var secondParameter = modifierProperty.FindPropertyRelative("secondParameter");


        //var function = modifierProperty.FindPropertyRelative("modifierFunction");
        //var method = modifierProperty.FindPropertyRelative("method");

        GUIAnimationName(animationName);
        MadGUI.PropertyFieldEnumPopup(modifierFunction, "Function");
        if (modifierFunction.enumValueIndex == (int) MadLevelAnimator.Modifier.ModifierFunc.Predefined) {
            MadGUI.PropertyFieldEnumPopup(baseOperator, "Base Operator");

            using (MadGUI.Indent()) {
                EditorGUILayout.LabelField("Expression");

                EditorGUILayout.BeginHorizontal();
                MadGUI.PropertyFieldEnumPopup(firstParameter, "", GUILayout.Width(150));
                //MadGUI.LookLikeControls(0, 0);
                MadGUI.PropertyFieldEnumPopup(valueOperator, "", GUILayout.Width(100));
                EditorGUILayout.PropertyField(secondParameter, new GUIContent(""), GUILayout.Width(80));
                //MadGUI.LookLikeControls(0);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }

        


        //MadGUI.PropertyFieldEnumPopup(function, "Function");
        //MadGUI.PropertyFieldEnumPopup(method, "Method");
    }

    private void GUIStartupProperty(SerializedProperty method, SerializedProperty value, string label) {
        MadGUI.PropertyFieldEnumPopup(method, label);
        if (method.enumValueIndex != (int) MadLevelAnimator.ApplyMethod.DoNotChange) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(value, "");
                EditorGUILayout.Space();
            }
        }
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif