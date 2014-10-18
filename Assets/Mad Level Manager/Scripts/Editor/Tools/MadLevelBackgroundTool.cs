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

public class MadLevelBackgroundTool : EditorWindow {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    MadPanel panel;
    string objectName = "background";
    MadDraggable draggable;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnGUI() {
        MadGUI.Message("Here you can create backgrounds for your level screens. Draggable is optional, "
            + "but necessary if you want to setup follow animations.", MessageType.None);
        panel = (MadPanel) EditorGUILayout.ObjectField("Panel", panel, typeof(MadPanel), true);
        objectName = EditorGUILayout.TextField("Object Name", objectName);
        draggable = (MadDraggable) EditorGUILayout.ObjectField("Draggable", draggable, typeof(MadDraggable), true);
        
        bool valid = panel != null && !string.IsNullOrEmpty(objectName);
        GUI.enabled = valid;
        if (GUILayout.Button("Create")) {
            Create();
        }
        GUI.enabled = true;
    }
    
    void Create() {
        var background = MadTransform.CreateChild<MadLevelBackground>(panel.transform, objectName);
        Selection.activeGameObject = background.gameObject;
        
        if (draggable != null) {
            background.draggable = draggable;
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void ShowWindow() {
        var tool = EditorWindow.GetWindow<MadLevelBackgroundTool>(false, "Create Background", true);
        tool.panel = MadPanel.UniqueOrNull();
        
        if (tool.panel != null) {
            tool.draggable = tool.panel.GetComponentInChildren<MadDraggable>();
        }
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif