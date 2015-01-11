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

[CustomEditor(typeof(MadAnimColor))]
public class MadAnimColorInspector : MadAnimInspector {

    #region Fields

    SerializedProperty colorFrom;
    SerializedProperty colorFromValue;
    SerializedProperty colorTo;
    SerializedProperty colorToValue;

    #endregion

    #region Methods

    protected override void OnEnable() {
        base.OnEnable();

        colorFrom = serializedObject.FindProperty("colorFrom");
        colorFromValue = serializedObject.FindProperty("colorFromValue");
        colorTo = serializedObject.FindProperty("colorTo");
        colorToValue = serializedObject.FindProperty("colorToValue");
    }

    protected override void DrawInspector() {
        MadGUI.PropertyFieldEnumPopup(colorFrom, "From");

        if (colorFrom.enumValueIndex != (int) MadAnimColor.ValueType.Current) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(colorFromValue, "Value");
            }
        }

        MadGUI.PropertyFieldEnumPopup(colorTo, "To");

        if (colorTo.enumValueIndex != (int) MadAnimColor.ValueType.Current) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(colorToValue, "Value");
            }
        }
    }

    #endregion
    
}

#if !UNITY_3_5
} // namespace
#endif