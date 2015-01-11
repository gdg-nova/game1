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
 
[CustomEditor(typeof(MadLevelGridLayout))]
public class MadLevelGridLayoutInspector : MadLevelAbstractLayoutInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty rightSlideSprite;
    SerializedProperty leftSlideSprite;
    
    SerializedProperty iconScale;
    SerializedProperty iconOffset;
    
    SerializedProperty leftSlideScale;
    SerializedProperty leftSlideOffset;
    
    SerializedProperty rightSlideScale;
    SerializedProperty rightSlideOffset;
    
    SerializedProperty gridWidth;
    SerializedProperty gridHeight;

    SerializedProperty horizontalAlign;
    SerializedProperty verticalAlign;
    
    SerializedProperty pixelsWidth;
    SerializedProperty pixelsHeight;
    
    SerializedProperty pagesOffsetFromResolution;
    SerializedProperty pagesOffsetManual;
    
    SerializedProperty hideManagerdObjects;

    MadLevelGridLayout script;        

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    bool generate {
        get {
            return script.setupMethod == MadLevelGridLayout.SetupMethod.Generate;
        }
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected override void OnEnable() {
        base.OnEnable();
        script = target as MadLevelGridLayout;
    
        rightSlideSprite = serializedObject.FindProperty("rightSlideSprite");
        leftSlideSprite = serializedObject.FindProperty("leftSlideSprite");
        
        iconScale = serializedObject.FindProperty("iconScale");
        iconOffset = serializedObject.FindProperty("iconOffset");
        
        leftSlideScale = serializedObject.FindProperty("leftSlideScale");
        leftSlideOffset = serializedObject.FindProperty("leftSlideOffset");
        
        rightSlideScale = serializedObject.FindProperty("rightSlideScale");
        rightSlideOffset = serializedObject.FindProperty("rightSlideOffset");
        
        gridWidth = serializedObject.FindProperty("gridWidth");
        gridHeight = serializedObject.FindProperty("gridHeight");

        horizontalAlign = serializedObject.FindProperty("horizontalAlign");
        verticalAlign = serializedObject.FindProperty("verticalAlign");
        
        pixelsWidth = serializedObject.FindProperty("pixelsWidth");
        pixelsHeight = serializedObject.FindProperty("pixelsHeight");
        
        pagesOffsetFromResolution = serializedObject.FindProperty("pagesOffsetFromResolution");
        pagesOffsetManual = serializedObject.FindProperty("pagesOffsetManual");
        
        configuration = serializedObject.FindProperty("configuration");
        hideManagerdObjects = serializedObject.FindProperty("hideManagedObjects");
    }

    MadLevelGridLayout.SetupMethod newSetupMethod;

    public override void OnInspectorGUI() {
        if (MadTrialEditor.isTrialVersion && MadTrialEditor.expired) {
            MadTrialEditor.OnEditorGUIExpired("Mad Level Manager");
            return;
        }

        newSetupMethod = (MadLevelGridLayout.SetupMethod) EditorGUILayout.EnumPopup("Setup Method", script.setupMethod);
        if (newSetupMethod != script.setupMethod) {
            if (newSetupMethod == MadLevelGridLayout.SetupMethod.Generate && EditorUtility.DisplayDialog(
                "Are you sure?",
                "Are you sure that you want to switch to Generate setup method? If you've made any changes to grid "
                + "object, these changes will be lost!", "Set to Generate", "Cancel")) {
                    script.setupMethod = newSetupMethod;
                    script.deepClean = true;
                    EditorUtility.SetDirty(script);
            } else if (EditorUtility.DisplayDialog(
                "Are you sure?",
                "Are you sure that you want to switch to Manual setup method? Be aware that after doing this:\n"
                + "- You won't be able to change your level group (currently " + script.configuration.FindGroupById(script.configurationGroup).name + ")\n"
                + "- You won't be able to regenerate grid without losing custom changes",
                "Set to Manual", "Cancel")) {
                script.setupMethod = newSetupMethod;
                EditorUtility.SetDirty(script);
            }
        }
    
        serializedObject.UpdateIfDirtyOrScript();
        
        if (script.setupMethod == MadLevelGridLayout.SetupMethod.Generate) {
            RebuildButton();
        }
        
        GUILayout.Label("Fundaments", "HeaderLabel");
        
        MadGUI.Indent(() => {
        
            MadGUI.PropertyField(configuration, "Configuration", MadGUI.ObjectIsSet);
            
            if (script.configuration != null) {
                var group = script.configuration.FindGroupById(script.configurationGroup);
                int index = GroupToIndex(script.configuration, group);
                var names = GroupNames(script.configuration);
                
                GUI.enabled = script.setupMethod == MadLevelGridLayout.SetupMethod.Generate;
                EditorGUI.BeginChangeCheck();
                index = EditorGUILayout.Popup("Group", index, names);
                if (EditorGUI.EndChangeCheck()) {
                    script.configurationGroup = IndexToGroup(script.configuration, index).id;
                    Rebuild();
                }
                GUI.enabled = true;
            }
            
            EditorGUILayout.Space();
            
            GUI.enabled = generate;
            
            MadGUI.PropertyField(iconTemplate, "Icon Template", MadGUI.ObjectIsSet);
            if (script.iconTemplate != null) {
                var prefabType = PrefabUtility.GetPrefabType(script.iconTemplate);
                if (prefabType == PrefabType.None) {
                    MadGUI.Warning("It's recommended to use prefab as a template. All visible icon instances will be linked to this prefab.");
                }
            }

            using (MadGUI.Indent()) {
                MadGUI.PropertyFieldEnumPopup(enumerationType, "Enumeration");

                MadGUI.PropertyFieldVector2(iconScale, "Scale");
                MadGUI.PropertyFieldVector2(iconOffset, "Offset");
            }

            EditorGUILayout.Space();
            
            GUI.enabled = true;
            
            MadGUI.PropertyField(leftSlideSprite, "Prev Page Sprite");
            MadGUI.Indent(() => {
                MadGUI.PropertyFieldVector2(leftSlideScale, "Scale");
                MadGUI.PropertyFieldVector2(leftSlideOffset, "Offset");
            });
                
            MadGUI.PropertyField(rightSlideSprite, "Next Page Sprite");
            MadGUI.Indent(() => {
                MadGUI.PropertyFieldVector2(rightSlideScale, "Scale");
                MadGUI.PropertyFieldVector2(rightSlideOffset, "Offset");
            });
        
        });

        EditorGUILayout.Space();
        
        GUILayout.Label("Dimensions", "HeaderLabel");
        
        MadGUI.Indent(() => {
            MadGUI.PropertyField(pixelsWidth, "Pixels Width");
            MadGUI.PropertyField(pixelsHeight, "Pixels Height");

            EditorGUILayout.Space();
            GUI.enabled = generate;
            MadGUI.PropertyField(gridHeight, "Grid Rows");
            MadGUI.PropertyField(gridWidth, "Grid Columns");

            EditorGUILayout.Space();

            MadGUI.PropertyFieldEnumPopup(horizontalAlign, "Horizontal Align");
            MadGUI.PropertyFieldEnumPopup(verticalAlign, "Vertical Align");

            GUI.enabled = true;
            EditorGUILayout.Space();

            MadGUI.PropertyField(pagesOffsetFromResolution, "Page Offset Auto");
            MadGUI.ConditionallyEnabled(!pagesOffsetFromResolution.boolValue, () => {
                MadGUI.Indent(() => {
                    MadGUI.PropertyField(pagesOffsetManual, "Pixels Offset");
                });
            });
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
        
        GUILayout.Label("Debug", "HeaderLabel");
        
        MadGUI.Indent(() => {
            MadGUI.PropertyField(hideManagerdObjects, "Hide Managed",
                "Hides managed by Mad Level Manager objects from the Hierarchy. If you want to have a look at what the hierarchy "
                + "looks like exacly, you can unckeck this option. Be aware that all direct changes to generated "
                + "objects will be lost!");
        });

        serializedObject.ApplyModifiedProperties();
    }
    
    void RebuildButton() {
        GUILayout.Label("Rebuild", "HeaderLabel");
        MadGUI.Indent(() => {
            MadGUI.Info("In a case when you've changed something, and result is not available on the screen, "
                + "please hit this button.");
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.color = Color.green;
            if (GUILayout.Button("Rebuild Now")) {
                Rebuild();
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        });
    }
    
    void Rebuild() {
        script.dirty = true;
        script.deepClean = true;
        EditorUtility.SetDirty(script);
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