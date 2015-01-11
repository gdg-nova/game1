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

[CustomEditor(typeof(MadAnimMove))]
public class MadAnimMoveInspector : MadAnimInspector {

    #region Fields

    SerializedProperty moveFrom;
    SerializedProperty moveFromPosition;
    SerializedProperty moveTo;
    SerializedProperty moveToPosition;

    #endregion

    #region Methods

    protected override void OnEnable() {
        base.OnEnable();

        moveFrom = serializedObject.FindProperty("moveFrom");
        moveFromPosition = serializedObject.FindProperty("moveFromPosition");
        moveTo = serializedObject.FindProperty("moveTo");
        moveToPosition = serializedObject.FindProperty("moveToPosition");
    }

    protected override void DrawInspector() {
        MadGUI.PropertyFieldEnumPopup(moveFrom, "From");

        if (moveFrom.enumValueIndex >= 2) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(moveFromPosition, "Position");
            }
        }

        MadGUI.PropertyFieldEnumPopup(moveTo, "To");

        if (moveTo.enumValueIndex >= 2) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(moveToPosition, "Position");
            }
        }
    }

    #endregion
    
}

#if !UNITY_3_5
} // namespace
#endif