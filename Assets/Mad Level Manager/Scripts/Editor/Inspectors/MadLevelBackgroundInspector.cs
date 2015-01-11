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

[CustomEditor(typeof(MadLevelBackground))]
public class MadLevelBackgroundInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    MadLevelBackground script;
    
    SerializedProperty draggable;
    SerializedProperty startDepth;
    SerializedProperty ignoreXMovement;
    SerializedProperty ignoreYMovement;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        script = target as MadLevelBackground;
        
        draggable = serializedObject.FindProperty("draggable");
        startDepth = serializedObject.FindProperty("startDepth");
        ignoreXMovement = serializedObject.FindProperty("ignoreXMovement");
        ignoreYMovement = serializedObject.FindProperty("ignoreYMovement");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();
        
        MadGUI.PropertyField(draggable, "Draggable", MadGUI.ObjectIsSet);
        MadGUI.PropertyField(startDepth, "Start Depth", "Depth value of first layer added. "
            + "Every next layer will receive +1 to it's depth value.");
        MadGUI.PropertyField(ignoreXMovement, "Ignore X Movement");
        MadGUI.PropertyField(ignoreYMovement, "Ignore Y Movement");
        
        serializedObject.ApplyModifiedProperties();
        
        MadGUI.Info("Add new layers, then select them to edit properties for each layer.");
        
        var arrayList = new MadGUI.ArrayList<MadLevelBackgroundLayer>(script.layers, (layer) => {
                if (layer == null) {
                    return null;
                }
            
                var so = new SerializedObject(layer);
                so.UpdateIfDirtyOrScript();
                
                var texture = so.FindProperty("texture");
                
                EditorGUILayout.BeginHorizontal();
                
                MadGUI.PropertyField(texture, "");
                
                MadGUI.ConditionallyEnabled(CanMoveUp(layer), () => {
                    if (GUILayout.Button("Up")) {
                        MoveUp(layer);
                    }
                });
                
                MadGUI.ConditionallyEnabled(CanMoveDown(layer), () => {
                    if (GUILayout.Button("Down")) {
                        MoveDown(layer);
                    }
                });
                
                GUI.color = Color.yellow;
                if (GUILayout.Button("Select")) {
                    Selection.activeGameObject = layer.gameObject;
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                
                if (so.ApplyModifiedProperties()) {
                    layer.SetDirty();
                }

                return layer;
        });
        
        arrayList.addLabel = "Add Layer";
        arrayList.emptyLabel = "There are currently no layers.";
        
        arrayList.createFunctionGeneric = () => {
            var layer = MadTransform.CreateChild<MadLevelBackgroundLayer>(script.transform, "layer (empty)");
            layer.GetComponent<MadSprite>().hideFlags = HideFlags.HideInInspector;
            return layer;
        };
        arrayList.onRemove = (layer) => layer.Cleanup();
        
        arrayList.beforeAdd = () => {
            MadUndo.RecordObject(script, "Add Background Layer");
            MadUndo.LegacyRegisterSceneUndo("Add Background Layer");
        };
        
        arrayList.beforeRemove = (arg) => {
            MadUndo.RecordObject(script, "Remove Background Layer");
            MadUndo.LegacyRegisterSceneUndo("Remove Background Layer");
        };
        
        if (arrayList.Draw()) {
            EditorUtility.SetDirty(script);
        }
        
    
        EditorGUI.BeginChangeCheck();
        
        if (EditorGUI.EndChangeCheck()) {
            // changed
        }
    }
    
    bool CanMoveUp(MadLevelBackgroundLayer layer) {
        int index = script.layers.IndexOf(layer);
        return index > 0;
    }
    
    void MoveUp(MadLevelBackgroundLayer layer) {
        const string UndoName = "Move Layer Up";
    
        MadUndo.RecordObject(script, UndoName);
        MadUndo.LegacyRegisterSceneUndo(UndoName);
        
        int index = script.layers.IndexOf(layer);
        if (index > 0) {
            var temp = script.layers[index - 1];
            script.layers[index - 1] = layer;
            script.layers[index] = temp;
            
            MadUndo.RecordObject(temp.gameObject, UndoName);
            MadUndo.RecordObject(layer.gameObject, UndoName);
            
            temp.SetDirty();
            layer.SetDirty();
        }
        
        script.UpdateDepth();
    }
    
    bool CanMoveDown(MadLevelBackgroundLayer layer) {
        int index = script.layers.IndexOf(layer);
        return index < script.layers.Count - 1;
    }
    
    void MoveDown(MadLevelBackgroundLayer layer) {
        const string UndoName = "Move Layer Down";
    
        MadUndo.RecordObject(script, UndoName);
        MadUndo.LegacyRegisterSceneUndo(UndoName);
    
        int index = script.layers.IndexOf(layer);
        if (index < script.layers.Count - 1) {
            var temp = script.layers[index + 1];
            script.layers[index + 1] = layer;
            script.layers[index] = temp;
            
            MadUndo.RecordObject(temp.gameObject, UndoName);
            MadUndo.RecordObject(layer.gameObject, UndoName);
            
            temp.SetDirty();
            layer.SetDirty();
        }
        
        script.UpdateDepth();
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