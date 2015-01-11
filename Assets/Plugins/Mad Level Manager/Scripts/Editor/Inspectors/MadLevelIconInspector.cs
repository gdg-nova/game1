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

[CustomEditor(typeof(MadLevelIcon))]
public class MadLevelIconInspector : MadSpriteInspector {

    #region Contants

    private const string DefaultFontGUID = "7fd3c65ab8c2c7a4cb5be2ccd1aee57c";

    #endregion

    #region Fields
    
    SerializedProperty unlockOnComplete;
    SerializedProperty canFocusIfLocked;

    private MadLevelIcon levelIcon;

    #endregion

    #region Unity Methods
    
    protected new void OnEnable() {
        base.OnEnable();
        
        unlockOnComplete = serializedObject.FindProperty("unlockOnComplete");
        canFocusIfLocked = serializedObject.FindProperty("canFocusIfLocked");
        levelIcon = target as MadLevelIcon;
    }

    public override void OnInspectorGUI() {
        

        if (levelIcon.generated && SetupMethodGenerate()) {
            if (MadGUI.WarningFix("This icon instance has been generated. If you want to modify this icon, "
                + "please switch your Setup Method to Manual or change the template.", "Help")) {
                    Application.OpenURL(MadLevelHelp.IconGenerated);
            }

            GUI.enabled = false;
        }

        if (MadGameObject.IsActive(levelIcon.gameObject)) {
            Properties();
        } else {
            MadGUI.Warning("Not all functions are available if this object is disabled! Before editing please enable this game object!");
        }

        MadGUI.BeginBox("Visibility");

        MadGUI.Indent(() => {

            EditorGUILayout.Space();

            CheckPropertyError(levelIcon.showWhenLevelLocked);
            GUILayout.Label("Show when level is locked");
            ArrayFor(levelIcon.showWhenLevelLocked);

            CheckPropertyError(levelIcon.showWhenLevelUnlocked);
            GUILayout.Label("Show when level is unlocked");
            ArrayFor(levelIcon.showWhenLevelUnlocked);

            CheckPropertyError(levelIcon.showWhenLevelCompleted);
            GUILayout.Label("Show when level is completed");
            ArrayFor(levelIcon.showWhenLevelCompleted);

            CheckPropertyError(levelIcon.showWhenLevelNotCompleted);
            GUILayout.Label("Show when level is not completed");
            ArrayFor(levelIcon.showWhenLevelNotCompleted);

            CheckConflictError();

            serializedObject.UpdateIfDirtyOrScript();
            MadGUI.PropertyField(canFocusIfLocked, "Can Focus If Locked");

            serializedObject.ApplyModifiedProperties();

            if (levelIcon.generated) {
                serializedObject.UpdateIfDirtyOrScript();
                if (MadGUI.Foldout("Unlock On Complete", false)) {
                    var arrayList = new MadGUI.ArrayList<MadLevelIcon>(
                        unlockOnComplete, (p) => { MadGUI.PropertyField(p, ""); });
                    arrayList.Draw();
                }
                serializedObject.ApplyModifiedProperties();
            }
        });
        MadGUI.EndBox();

        if (levelIcon.completedProperty != null || levelIcon.lockedProperty != null) {
            SectionSecialProperties();
        }

        MadGUI.BeginBox("Sprite");
        MadGUI.Indent(() => {
            SectionSprite();
        });
        MadGUI.EndBox();
        
    }

    #endregion

    #region Other Methods

    private void SectionSecialProperties() {
        MadGUI.BeginBox("Special Properties");
        MadGUI.Indent(() => {
            MadGUI.Warning("Special properties are deprecated. Please use Children section instead.");

            if (levelIcon.generated) {
                int levelCount = levelIcon.configuration.LevelCount(MadLevel.Type.Level);
                if (levelCount > levelIcon.levelIndex) {
                    var level = levelIcon.level;
                    if (level != null) {
                        MadGUI.Disabled(() => {
                            EditorGUILayout.TextField("Level Name", level.name);
                            EditorGUILayout.TextField("Level Arguments", level.arguments);
                        });
                    } else {
                        MadGUI.Warning("Level for this icon no longer exists.");
                    }
                }
                if (MadGUI.InfoFix("These values are set and managed by level configuration.",
                    "Configuration")) {
                    Selection.objects = new Object[] { levelIcon.configuration };
                }
            }

            //
            // Completed property select popup
            //
            EditorGUI.BeginChangeCheck();

            MadGUI.PropertyFieldObjectsPopup<MadLevelProperty>(
                target,
                "\"Completed\" Property",
                ref levelIcon.completedProperty,
                PropertyList(),
                false
            );

            MadGUI.PropertyFieldObjectsPopup<MadLevelProperty>(
                target,
                "\"Locked\" Property",
                ref levelIcon.lockedProperty,
                PropertyList(),
                false
            );
            if (EditorGUI.EndChangeCheck()) {
                if (levelIcon.completedProperty == null && levelIcon.lockedProperty == null) {
                    EditorUtility.DisplayDialog("Hiding Special Properties",
                        "Special properties are deprecated and they will be now hidden from your inspector view. Please use the Visibility section instead.",
                        "I understand");
                }
            }
            
        });
        MadGUI.EndBox();
    }

    private void CheckConflictError() {
        var set = new HashSet<GameObject>();

        foreach (List<GameObject> list in new List<GameObject>[] {
            levelIcon.showWhenLevelCompleted,
            levelIcon.showWhenLevelLocked,
            levelIcon.showWhenLevelNotCompleted,
            levelIcon.showWhenLevelUnlocked }) {

                foreach (var el in list) {
                    if (el == null) {
                        continue;
                    }

                    if (!set.Contains(el)) {
                        set.Add(el);
                    } else {
                        MadGUI.Error("Child \"" + el.name + "\" is set in multiple places.");
                    }
                }
        }
    }

    private void CheckPropertyError(List<GameObject> list) {
        foreach (var el in list) {
            if (el == null) {
                continue;
            }

            if (el.GetComponent<MadLevelProperty>() != null) {
                MadGUI.Error("\"" + el.name + "\" is a property. It means that its enabled/disabled state is determined by saved game state. Consider creating another non-property child.");
            }
        }
    }

    private void ArrayFor(List<GameObject> list) {
        var arrayList = new MadGUI.ArrayList<GameObject>(list, (s) => {
            MadGUI.PropertyFieldObjectsPopup<GameObject>(target, "", ref s, SpriteList(), false);
            return s;
        });

        arrayList.createFunctionGeneric = () => { return null; };
        arrayList.drawOrderButtons = false;

        arrayList.Draw();
    }

    private void Properties() {
        MadGUI.BeginBox("Children");
        MadGUI.Indent(() => {

            MadGUI.Info("Properties are persistent values saved and loaded from the current game state. "
                + "They are good for things like stars or medals.");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Create:");

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Empty")) {
                if (ShouldCreateProperty()) {
                    CreateEmptyProperty();
                } else {
                    CreateEmpty();
                }
            }
            if (GUILayout.Button("Sprite")) {
                if (ShouldCreateProperty()) {
                    CreateSpriteProperty();
                } else {
                    CreateSprite();
                }
            }
            if (GUILayout.Button("Text")) {
                if (ShouldCreateProperty()) {
                    CreateTextProperty();
                } else {
                    CreateText();
                }
            }
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var properties = PropertyList();
            foreach (MadLevelProperty property in properties) {
                GUILayout.BeginHorizontal();
                MadGUI.LookLikeControls(0, 150);
                property.name = EditorGUILayout.TextField(property.name, GUILayout.Width(120));
                MadGUI.LookLikeControls(0);
                //GUILayout.Label(property.name, GUILayout.Width(170));

                GUILayout.FlexibleSpace();

                GUILayout.Label("Default State: ");
                if (property.linked) {
                    if (MadGUI.Button("LINKED", Color.cyan, GUILayout.Width(60))) {
                        if (EditorUtility.DisplayDialog(
                            "State Linked",
                            "This property state is linked by '" + property.linkage.name
                            + "' property and cannot be changed directly. Do you want to select the linkage owner?",
                            "Yes", "No")) {
                            Selection.activeObject = property.linkage.gameObject;
                        }
                    }
                } else if (property.propertyEnabled) {
                    if (MadGUI.Button("ON", Color.green, GUILayout.Width(60))) {
                        property.propertyEnabled = false;
                        EditorUtility.SetDirty(property);
                    }
                } else {
                    if (MadGUI.Button("OFF", Color.red, GUILayout.Width(60))) {
                        property.propertyEnabled = true;
                        EditorUtility.SetDirty(property);
                    }
                }

                GUILayout.FlexibleSpace();

                GUI.backgroundColor = Color.yellow;

                if (GUILayout.Button("Select", GUILayout.Width(55))) {
                    Selection.activeGameObject = property.gameObject;
                }

                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
        });
        EditorGUILayout.Space();

        MadGUI.Info("Level icon's text value will be set to current level number.");

        if (TextList().Count == 0) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level Number");
            if (MadGUI.Button("Create New", Color.green)) {
                var text = CreateText();
                text.gameObject.name = "level number";
                text.font = MadAssets.TryLoadComponent<MadFont>(DefaultFontGUID);
                text.text = "1";
                text.scale = 90;
                levelIcon.levelNumber = text;
            }
            EditorGUILayout.EndHorizontal();
        } else {
            MadGUI.PropertyFieldObjectsPopup<MadText>(
                target,
                "Level Number",
                ref levelIcon.levelNumber,
                TextList(),
                false
            );
        }

        MadGUI.EndBox();
    }

    private bool SetupMethodGenerate() {
        var gridLayout = MadTransform.FindParent<MadLevelGridLayout>(sprite.transform);
        if (gridLayout != null && gridLayout.setupMethod == MadLevelGridLayout.SetupMethod.Generate) {
            return true;
        } else {
            return false;
        }
    }

    private bool ShouldCreateProperty() {
        return EditorUtility.DisplayDialog("Make it a Property?", "Do you want this object to be a property?\n\n"
            + "Property will be enabled or disabled based on current game state. It's good for things like stars, medals or scores.", "Yes", "No");
    }

    private void CreateEmptyProperty() {
        var property = sprite.CreateChild<MadLevelProperty>("new_empty_property");

        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;

        Selection.activeGameObject = property.gameObject;
    }

    private void CreateSpriteProperty() {
        var property = MadTransform.CreateChild<MadLevelProperty>(sprite.transform, "new_sprite_property");
        var s = property.gameObject.AddComponent<MadSprite>();

        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;

        s.ResizeToTexture();
        s.guiDepth = NextDepth();

        Selection.activeGameObject = s.gameObject;
    }

    private void CreateTextProperty() {
        var property = MadTransform.CreateChild<MadLevelPropertyText>(sprite.transform, "new_text_property");
        var text = property.gameObject.AddComponent<MadText>();

        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;

        text.text = "Default Text";
        text.guiDepth = NextDepth();

        Selection.activeGameObject = text.gameObject;
    }

    private void CreateEmpty() {
        var empty = MadTransform.CreateChild<Transform>(sprite.transform, "new_empty");

        empty.transform.localPosition = Vector3.zero;
        empty.transform.localScale = Vector3.one;

        Selection.activeGameObject = empty.gameObject;
    }

    private void CreateSprite() {
        var s = MadTransform.CreateChild<MadSprite>(sprite.transform, "new_sprite");

        s.transform.localPosition = Vector3.zero;
        s.transform.localScale = Vector3.one;

        s.ResizeToTexture();
        s.guiDepth = NextDepth();

        Selection.activeGameObject = s.gameObject;
    }

    private MadText CreateText() {
        var text = MadTransform.CreateChild<MadText>(sprite.transform, "new_text");
        Selection.activeGameObject = text.gameObject;

        text.transform.localPosition = Vector3.zero;
        text.transform.localScale = Vector3.one;

        text.text = "Default Text";
        text.guiDepth = NextDepth();

        return text;
    }

    private int NextDepth() {
        int maxDepth = sprite.guiDepth;
        foreach (var s in MadTransform.FindChildren<MadSprite>(sprite.transform)) {
            if (s.guiDepth > maxDepth) {
                maxDepth = s.guiDepth;
            }
        }

        return maxDepth + 1;
    }
    
    List<MadLevelProperty> PropertyList() {
        var properties = ((MonoBehaviour) target).GetComponentsInChildren<MadLevelProperty>();
        return new List<MadLevelProperty>(properties);
    }

    List<GameObject> SpriteList() {
        var result = new List<GameObject>(levelIcon.transform.childCount);
        for (int i = 0; i < levelIcon.transform.childCount; ++i) {
            result.Add(levelIcon.transform.GetChild(i).gameObject);
        }

        return result;
    }
    
    List<MadText> TextList() {
        var texts = ((MonoBehaviour) target).GetComponentsInChildren<MadText>();
        return new List<MadText>(texts);
    }
    
    MadLevelProperty GetProperty(string name) {
        var properties = ((MonoBehaviour) target).GetComponentsInChildren<MadLevelProperty>();
        foreach (var property in properties) {
            if (property.name == name) {
                return property;
            }
        }
        
        Debug.LogError("Property " + name + " not found?!");
        return null;
    }

    #endregion

}

#if !UNITY_3_5
} // namespace
#endif