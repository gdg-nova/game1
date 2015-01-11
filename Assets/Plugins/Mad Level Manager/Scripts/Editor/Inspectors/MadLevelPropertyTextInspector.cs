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

[CustomEditor(typeof(MadLevelPropertyText))]
public class MadLevelPropertyTextInspector : Editor {

    #region Methods

    public override void OnInspectorGUI() {

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Property Name: " + target.name);
        EditorGUILayout.Space();

        MadGUI.Info("Rename this object to match the property name that text value you want to display. "
            + "When property is not set then the default text will be displayed.");

        EditorGUILayout.Space();
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif