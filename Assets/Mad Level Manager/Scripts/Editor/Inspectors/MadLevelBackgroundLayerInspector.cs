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

[CustomEditor(typeof(MadLevelBackgroundLayer))]
public class MadLevelBackgroundLayerInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty texture;
    SerializedProperty tint;
    SerializedProperty scaleMode;
    SerializedProperty dontStretch;
	SerializedProperty repeatX;
	SerializedProperty repeatY;

    SerializedProperty fillMarginLeft;
    SerializedProperty fillMarginTop;
    SerializedProperty fillMarginRight;
    SerializedProperty fillMarginBottom;

    SerializedProperty scale;
    SerializedProperty align;
    SerializedProperty position;
    SerializedProperty followSpeed;
    SerializedProperty scrollSpeed;
    
    MadLevelBackgroundLayer layer;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        layer = target as MadLevelBackgroundLayer;
    
        texture = serializedObject.FindProperty("texture");
        tint = serializedObject.FindProperty("tint");
        scaleMode = serializedObject.FindProperty("scaleMode");
		dontStretch = serializedObject.FindProperty("dontStretch");
		repeatX = serializedObject.FindProperty("repeatX");
		repeatY = serializedObject.FindProperty("repeatY");

        fillMarginLeft = serializedObject.FindProperty("fillMarginLeft");
        fillMarginTop = serializedObject.FindProperty("fillMarginTop");
        fillMarginRight = serializedObject.FindProperty("fillMarginRight");
        fillMarginBottom = serializedObject.FindProperty("fillMarginBottom");

        scale = serializedObject.FindProperty("scale");
        align = serializedObject.FindProperty("align");
        position = serializedObject.FindProperty("position");
        followSpeed = serializedObject.FindProperty("followSpeed");
        scrollSpeed = serializedObject.FindProperty("scrollSpeed");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();
        
        GUI.color = Color.yellow;
        if (GUILayout.Button("<< Back To Layer Listing")) {
            Selection.activeGameObject = layer.parent.gameObject;
        }
        GUI.color = Color.white;
        GUILayout.Space(16);
        
        MadGUI.PropertyField(texture, "Texture");
        MadGUI.PropertyField(tint, "Tint");
        
        EditorGUILayout.Space();
        
        MadGUI.PropertyField(scaleMode, "Scale Mode");
        
		MadGUI.PropertyField(repeatX, "Repeat X");
		MadGUI.PropertyField(repeatY, "Repeat Y");

        MadGUI.ConditionallyEnabled(layer.scaleMode == MadLevelBackgroundLayer.ScaleMode.Fill && !layer.repeatX && !layer.repeatY, () => {
            if (MadGUI.Foldout("Margin", false)) {
                MadGUI.Indent(() => {
                    MadGUI.PropertyField(fillMarginLeft, "Left");
                    MadGUI.PropertyField(fillMarginTop, "Top");
                    MadGUI.PropertyField(fillMarginRight, "Right");
                    MadGUI.PropertyField(fillMarginBottom, "Bottom");
                });
            }
        });

		MadGUI.ConditionallyEnabled(!layer.repeatX && !layer.repeatY, () => {
			MadGUI.PropertyField(dontStretch, "Don't Stretch");
		});
        
        if (scaleMode.enumValueIndex == (int) MadLevelBackgroundLayer.ScaleMode.Manual) {
            MadGUI.PropertyField(align, "Align");
            EditorGUILayout.Space();
            MadGUI.PropertyFieldVector2Compact(position, "Position", 70);
            MadGUI.PropertyFieldVector2Compact(scale, "Scale", 70);
        } else {
            MadGUI.PropertyFieldVector2Compact(position, "Position", 70);
        }
        
        EditorGUILayout.Space();
        
        MadGUI.PropertyField(followSpeed, "Follow Speed");
        MadGUI.PropertyFieldVector2Compact(scrollSpeed, "Auto Scroll", 70);
        
        if (serializedObject.ApplyModifiedProperties()) {
            layer.SetDirty();
        }
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