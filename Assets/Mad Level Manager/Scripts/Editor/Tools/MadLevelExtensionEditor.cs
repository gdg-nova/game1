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

public class MadLevelExtensionEditor : EditorWindow {

    #region Fields

    private MadLevelConfiguration configuration;

    private int selectedExtensionIndex;

    #endregion

    #region Properties

    private MadLevelExtension currentExtension {
        get {
            if (configuration.extensions.Count > selectedExtensionIndex) {
                return configuration.extensions[selectedExtensionIndex];
            } else {
                return null;
            }
        }

        set {
            int index = configuration.extensions.IndexOf(value);
            selectedExtensionIndex = index;
        }
    }

    #endregion

    #region Unity Methods

    void OnGUI() {
        TopPanel();

        EditorGUILayout.Space();
        MadGUI.Separator();
        EditorGUILayout.Space();

        if (currentExtension != null) {
            BottomPanel();
        }

        ButtonsPanel();
    }

    #endregion

    #region Private Methods

    private void TopPanel() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("For Configuration: ");
        if (MadGUI.Button(configuration.name, Color.cyan)) {
            Selection.activeObject = configuration;
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        int extensionCount = configuration.extensions.Count;

        if (extensionCount > 0) {
            MadGUI.LookLikeControls(120, 150);
            selectedExtensionIndex = MadGUI.DynamicPopup(selectedExtensionIndex, "Current Extension:", extensionCount, (index) => {
                var extension = configuration.extensions[index];
                return extension.name;
            });
            MadGUI.LookLikeControls(0);

            GUILayout.FlexibleSpace();
            if (MadGUI.Button("Remove", Color.red)) {
                RemoveExtension(selectedExtensionIndex);
            }
        } else {
            GUILayout.Label("There's no extensions yet.");
        }

        GUILayout.Space(10);

        if (MadGUI.Button("Create New Extension", Color.green)) {
            var builder = new MadInputDialog.Builder("Create New Extension", "Enter a new extension name.", (result) => {
                if (!string.IsNullOrEmpty(result)) {
                    var extension = CreateNewExtension(result);
                    if (extension != null) {
                        currentExtension = extension;
                    }

                }
            });
            builder.BuildAndShow();
        }
        EditorGUILayout.EndHorizontal();
    }

    private MadLevelExtension CreateNewExtension(string name) {
        if (ExtensionExists(name)) {
            EditorUtility.DisplayDialog("Extension exists!", "Extension with name '" + name + "' already exists!", "OK");
            return null;
        }

        MadUndo.RecordObject2(configuration, "Create New Extension");

        var extension = new MadLevelExtension(name);
        configuration.extensions.Add(extension);
        EditorUtility.SetDirty(configuration);

        Repaint();

        return extension;
    }

    private void RemoveExtension(int index) {
        if (EditorUtility.DisplayDialog(
            "Remove Extension?",
            "Are you sure you want to remove extension \"" + configuration.extensions[index].name + "\"?",
            "Yes", "No")) {

                MadUndo.RecordObject2(configuration, "Remove Extension");

                configuration.extensions.RemoveAt(index);
                EditorUtility.SetDirty(configuration);

                Repaint();
        }
    }

    private bool ExtensionExists(string name) {
        foreach (var e in configuration.extensions) {
            if (e.name == name) {
                return true;
            }
        }

        return false;
    }

    private void BottomPanel() {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(Screen.width / 2));
        SceneList("Load Before Level", currentExtension.scenesBefore);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(Screen.width / 2));
        SceneList("Load After Level", currentExtension.scenesAfter);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void SceneList(string label, List<MadLevelScene> scenes) {
        GUILayout.Label(label, "HeaderLabel");

        int counter = 1;
        new MadGUI.ArrayList<MadLevelScene>(scenes, (scene) => {
            MadGUI.Validate(() => scene.sceneObject != null, () => {
                MadGUI.LookLikeControls(70);
                scene.sceneObject = MadGUI.SceneField(scene.sceneObject, "Scene " + (counter++));
                MadGUI.LookLikeControls(0);
            });
            return scene;
        }).Draw();
    }

    private void ButtonsPanel() {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Help")) {
            Application.OpenURL(MadLevelHelp.ExtensionEditor);
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Close")) {
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Static Methods

    public static void Show(MadLevelConfiguration configuration) {
        var editor = EditorWindow.GetWindow<MadLevelExtensionEditor>(true, "Extension Editor", true);
        editor.configuration = configuration;
    }

    #endregion

}

#if !UNITY_3_5
} // namespace
#endif