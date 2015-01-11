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

[CustomEditor(typeof(MadLevelFreeLayout))]
public class MadLevelFreeLayoutInspector : MadLevelAbstractLayoutInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // won't display offset because it's used to placing icons and it's mostly useless for users
//    SerializedProperty offset;
    
    SerializedProperty backgroundTexture;

    MadLevelFreeLayout script;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected override void OnEnable() {
        base.OnEnable();

        script = target as MadLevelFreeLayout;

        backgroundTexture = serializedObject.FindProperty("backgroundTexture");
    }

    public override void OnInspectorGUI() {
        if (MadTrialEditor.isTrialVersion && MadTrialEditor.expired) {
            MadTrialEditor.OnEditorGUIExpired("Mad Level Manager");
            return;
        }

        serializedObject.UpdateIfDirtyOrScript();
        
        GUILayout.Label("Fundaments", "HeaderLabel");
        MadGUI.Indent(() => {
            MadGUI.PropertyField(configuration, "Configuration", MadGUI.ObjectIsSet);
            if (script.configuration != null) {
                var group = script.configuration.FindGroupById(script.configurationGroup);
                int index = GroupToIndex(script.configuration, group);
                var names = GroupNames(script.configuration);

                EditorGUI.BeginChangeCheck();
                index = EditorGUILayout.Popup("Group", index, names);
                if (EditorGUI.EndChangeCheck()) {
                    MadUndo.RecordObject2(script, "Changed Group");
                    script.configurationGroup = IndexToGroup(script.configuration, index).id;
                    script.dirty = true;
                    EditorUtility.SetDirty(script);
                }
                GUI.enabled = true;
            }

            EditorGUILayout.Space();

            MadGUI.PropertyField(iconTemplate, "Icon Template", MadGUI.ObjectIsSet);
            if (script.iconTemplate != null) {
                var prefabType = PrefabUtility.GetPrefabType(script.iconTemplate);
                if (prefabType == PrefabType.None) {
                    MadGUI.Warning("It's recommended to use prefab as a template. All visible icon instances will be linked to this prefab.");
                }
            }

            using (MadGUI.Indent()) {
                MadGUI.PropertyFieldEnumPopup(enumerationType, "Enumeration");
            }

            EditorGUILayout.Space();

            MadGUI.PropertyField(backgroundTexture, "Background Texture");

            EditorGUILayout.Space();

            MadGUI.Info("Use the button below if you've updated your icon template and you want to replace all icons in your layout with it.");

            if (MadGUI.Button("Replace All Icons", Color.yellow)) {
                ReplaceAllIcons();
            }

            MadGUI.Info("More customization options are available in the Draggable object.");

            if (MadGUI.Button("Select Draggable", Color.magenta)) {
                SelectDraggable();
            }
        });

        EditorGUILayout.Space();
        
        GUILayout.Label("Mechanics", "HeaderLabel");
        
        MadGUI.Indent(() => {
            LookAtLastLevel();
            EditorGUILayout.Space();
            HandleMobileBack();
            EditorGUILayout.Space();
            TwoStepActivation();
            EditorGUILayout.Space();
            LoadLevel();
        });
        
        serializedObject.ApplyModifiedProperties();
    }

    private void SelectDraggable() {
        Selection.activeGameObject = script.draggable.gameObject;
    }

    private void ReplaceAllIcons() {
        if (EditorUtility.DisplayDialog("Replace all icons?",
            "Are you sure that you want to replace all icons with selected prefab?",
            "Replace", "Cancel")) {

                script.ReplaceIcons(script.iconTemplate.gameObject);
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