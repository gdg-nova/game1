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

[CustomEditor(typeof(MadDragStopDraggable))]
public class MadDragStopDraggableInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty moveEasingType;
    SerializedProperty moveEasingDuration;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        moveEasingType = serializedObject.FindProperty("moveEasingType");
        moveEasingDuration = serializedObject.FindProperty("moveEasingDuration");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();
    
        MadGUI.PropertyField(moveEasingType, "Type");
        MadGUI.PropertyField(moveEasingDuration, "Duration");
        
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