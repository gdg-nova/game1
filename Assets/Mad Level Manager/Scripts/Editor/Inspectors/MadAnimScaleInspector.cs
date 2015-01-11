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

[CustomEditor(typeof(MadAnimScale))]
public class MadAnimScaleInspector : MadAnimInspector {

    #region Fields

    SerializedProperty scaleFrom;
    SerializedProperty scaleFromValue;
    SerializedProperty scaleTo;
    SerializedProperty scaleToValue;

    #endregion

    #region Methods

    protected override void OnEnable() {
        base.OnEnable();

        scaleFrom = serializedObject.FindProperty("scaleFrom");
        scaleFromValue = serializedObject.FindProperty("scaleFromValue");
        scaleTo = serializedObject.FindProperty("scaleTo");
        scaleToValue = serializedObject.FindProperty("scaleToValue");
    }

    protected override void DrawInspector() {
        MadGUI.PropertyFieldEnumPopup(scaleFrom, "From");

        if (scaleFrom.enumValueIndex >= 2) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(scaleFromValue, "Value");
            }
        }

        MadGUI.PropertyFieldEnumPopup(scaleTo, "To");

        if (scaleTo.enumValueIndex >= 2) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(scaleToValue, "Value");
            }
        }
    }

    #endregion
    
}

#if !UNITY_3_5
} // namespace
#endif