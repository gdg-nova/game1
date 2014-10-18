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

[CustomEditor(typeof(MadAnimRotate))]
public class MadAnimRotateInspector : MadAnimInspector {

    #region Fields

    SerializedProperty rotateFrom;
    SerializedProperty rotateFromValue;
    SerializedProperty rotateTo;
    SerializedProperty rotateToValue;

    #endregion

    #region Methods

    protected override void OnEnable() {
        base.OnEnable();

        rotateFrom = serializedObject.FindProperty("rotateFrom");
        rotateFromValue = serializedObject.FindProperty("rotateFromValue");
        rotateTo = serializedObject.FindProperty("rotateTo");
        rotateToValue = serializedObject.FindProperty("rotateToValue");
    }

    protected override void DrawInspector() {
        MadGUI.PropertyFieldEnumPopup(rotateFrom, "From");

        if (rotateFrom.enumValueIndex >= 2) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(rotateFromValue, "Value");
            }
        }

        MadGUI.PropertyFieldEnumPopup(rotateTo, "To");

        if (rotateTo.enumValueIndex >= 2) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(rotateToValue, "Value");
            }
        }
    }

    #endregion
    
}

#if !UNITY_3_5
} // namespace
#endif