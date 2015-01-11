/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MadLevelManager;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelConfigurationEditor {

    public static bool CheckBuildSynchronized(MadLevelConfiguration config) {
        var scenes = EditorBuildSettings.scenes;

        if (config.levels.Count == 0) {
            // do not synchronize anything if it's nothing there
            return true;
        }

        if (scenes.Length == 0 && config.levels.Count > 0 || scenes.Length > 0 && config.levels.Count == 0) {
            //            Debug.Log("Failed size test");
            return false;
        }

        if (scenes.Length == 0 && config.levels.Count == 0) {
            return true;
        }

        var firstLevel = config.GetLevel(0);

        // check if first scene is my first scene
        if (scenes[0].path != firstLevel.scenePath) {
            //            Debug.Log("Different start scene");
            return false;
        }

        // find all configuration scenes that are not in build
        List<MadLevelScene> allScenes = new List<MadLevelScene>();

        foreach (var level in config.levels) {
            allScenes.Add(level);
        }

        foreach (var extension in config.extensions) {
            allScenes.AddRange(extension.scenesBefore);
            allScenes.AddRange(extension.scenesAfter);
        }

        foreach (var level in allScenes) {
            if (!level.IsValid()) {
                continue;
            }

            var obj = Array.Find(scenes, (scene) => scene.path == level.scenePath);
            if (obj == null) {  // scene not found in build
                //                Debug.Log("Scene not found in build: " + item.level.scene);
                return false;
            }
        }

        return true;
    }

    public static void SynchronizeBuild(MadLevelConfiguration config) {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        foreach (var configuredScene in config.ScenesInOrder()) {
            if (!configuredScene.IsValid()) {
                continue;
            }

            string path = configuredScene.scenePath;
            if (scenes.Find((obj) => obj.path == path) == null) {
                var scene = new EditorBuildSettingsScene(path, true);
                scenes.Add(scene);
            }
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}

#if !UNITY_3_5
} // namespace
#endif