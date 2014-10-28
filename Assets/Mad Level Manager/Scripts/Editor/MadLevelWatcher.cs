/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEngine;
using MadLevelManager;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[InitializeOnLoad]
public class MadLevelConfigurationWatcher {

    #region Methods

    static MadLevelConfigurationWatcher() {
        EditorApplication.playmodeStateChanged += () => {
            if (GameObject.Find("/_mlm_ignore") != null) {
                return;
            }


            MadLevelConfiguration configuration;

            var layout = GameObject.FindObjectOfType(typeof(MadLevelAbstractLayout)) as MadLevelAbstractLayout;
            if (layout != null && layout.configuration != null) {
                configuration = layout.configuration;
            } else {
                configuration = MadLevel.activeConfiguration;
            }

            if (configuration != null
                && EditorApplication.isPlayingOrWillChangePlaymode
                && !EditorApplication.isPlaying) {

                if (!configuration.IsValid()) {
                    if (!EditorUtility.DisplayDialog(
                        "Invalid Configuration",
                        "Your level configuration has errors. Do you want to continue anyway?",
                        "Yes", "No")) {
                        EditorApplication.isPlaying = false;
                        Selection.activeObject = configuration;
                        return;
                    }
                }

                if (configuration != MadLevel.activeConfiguration
                    || !MadLevelConfigurationEditor.CheckBuildSynchronized(configuration)
                    || !configuration.active) {
                    if (EditorUtility.DisplayDialog(
                        "Not Synchronized",
                        "Your level configuration is not active or synchronized "
                        + "(runtime errors may occur). Do it now?",
                        "Yes", "No")) {
                        var active = MadLevelConfiguration.GetActive();
                        if (active != null) {
                            active.active = false; // workaround
                        }
                        configuration.active = true;
                        MadLevelConfigurationEditor.SynchronizeBuild(configuration);
                    }
                }
            }
        };
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif