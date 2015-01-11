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

[CustomEditor(typeof(MadFreeDraggable))]
public class MadFreeDraggableInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty dragBounds;
    
    SerializedProperty scaleMode;
    SerializedProperty scalingMin;
    SerializedProperty scalingMax;
    
    SerializedProperty moveEasing;
    SerializedProperty moveEasingType;
    SerializedProperty moveEasingDuration;
    
    SerializedProperty scaleEasing;
    SerializedProperty scaleEasingType;
    SerializedProperty scaleEasingDuration;

    MadFreeDraggable script;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        script = target as MadFreeDraggable;

        dragBounds = serializedObject.FindProperty("dragBounds");
        
        scaleMode = serializedObject.FindProperty("scaleMode");
        scalingMin = serializedObject.FindProperty("scalingMin");
        scalingMax = serializedObject.FindProperty("scalingMax");
        
        moveEasing = serializedObject.FindProperty("moveEasing");
        moveEasingType = serializedObject.FindProperty("moveEasingType");
        moveEasingDuration = serializedObject.FindProperty("moveEasingDuration");
        
        scaleEasing = serializedObject.FindProperty("scaleEasing");
        scaleEasingType = serializedObject.FindProperty("scaleEasingType");
        scaleEasingDuration = serializedObject.FindProperty("scaleEasingDuration");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();
    
        MadGUI.PropertyField(dragBounds, "Drag Area");

        EditorGUILayout.Space();

        MadGUI.ConditionallyEnabled(HasBackground(), () => {
            if (MadGUI.Button("Resize Drag Area To Background", Color.yellow)) {
                ResizeDragAreaToBackground();
            }
        });

        EditorGUILayout.Space();
        
        MadGUI.PropertyFieldEnumPopup(scaleMode, "Allow Scaling");
        MadGUI.ConditionallyEnabled(script.scaleMode == MadFreeDraggable.ScaleMode.Free, () => {
            MadGUI.Indent(() => {
                MadGUI.PropertyField(scalingMin, "Scaling Min");
                MadGUI.PropertyField(scalingMax, "Scaling Max");
            });
        });

        EditorGUILayout.Space();
        
        MadGUI.PropertyField(moveEasing, "Move Easing");
        MadGUI.ConditionallyEnabled(moveEasing.boolValue, () => {
            MadGUI.Indent(() => {
                MadGUI.PropertyField(moveEasingType, "Type");
                MadGUI.PropertyField(moveEasingDuration, "Duration");
            });
        });

        EditorGUILayout.Space();
        
        MadGUI.PropertyField(scaleEasing, "Scale Easing");
        MadGUI.ConditionallyEnabled(scaleEasing.boolValue, () => {
            MadGUI.Indent(() => {
                MadGUI.PropertyField(scaleEasingType, "Type");
                MadGUI.PropertyField(scaleEasingDuration, "Duration");
            });
        });
        
        serializedObject.ApplyModifiedProperties();
    }

    private void ResizeDragAreaToBackground() {
        var background = MadTransform.FindChildWithName<MadSprite>(script.transform, "background");

        MadUndo.RecordObject2(script, "Resize Drag Area");

        Rect spriteBounds = background.GetTransformedBounds();
        script.dragBounds = new Bounds(spriteBounds.center, new Vector2(spriteBounds.xMax - spriteBounds.xMin, spriteBounds.yMax - spriteBounds.yMin));

        EditorUtility.SetDirty(script);
    }

    private bool HasBackground() {
        var background = MadTransform.FindChildWithName<MadSprite>(script.transform, "background");
        return background != null;
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