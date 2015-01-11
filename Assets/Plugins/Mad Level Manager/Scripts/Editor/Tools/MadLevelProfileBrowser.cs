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

public class MadLevelProfileBrowser : EditorWindow {

    #region Fields

    private Vector2 scrollPosition;

    readonly Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

    #endregion

    #region Public Properties
    #endregion

    #region Slots

    void OnGUI() {
        if (!Application.isPlaying) {
            EditorGUILayout.LabelField("Profile Browser can be used only in the Play Mode.");
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        string profileOrig = MadLevelProfile.profile;

        try {
            var profiles = MadLevelProfile.profileList;
            foreach (var profile in profiles) {
                string path = "/Profile " + profile;
                if (Foldout("Profile \"" + profile + "\"", path)) {
                    GUIProfile(profile, path);
                }
            }
        } finally {
            MadLevelProfile.profile = profileOrig;
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void GUIProfile(string profile, string path) {
        using (MadGUI.Indent(2)) {
            MadLevelProfile.profile = profile;

            if (Foldout("Levels", path + "/Levels")) {
                GUILevels(path + "/Levels");
            }

            var profilePropertyNames = MadLevelProfile.GetProfilePropertyNames();
            foreach (var profilePropertyName in profilePropertyNames) {
                GUIProperty(profilePropertyName, MadLevelProfile.GetProfileAny(profilePropertyName));
            }
        }
    }

    private void GUILevels(string path) {
        using (MadGUI.Indent(2)) {
            var levelNames = MadLevelProfile.GetLevelNames();
            levelNames.Sort(new MadNaturalSortComparer());
            foreach (var levelName in levelNames) {
                path += "/" + levelName;
                if (Foldout(levelName, path)) {
                    GUILevel(levelName);
                }
            }
        }
    }

    private static void GUILevel(string levelName) {
        using (MadGUI.Indent(2)) {
            var propertyNames = MadLevelProfile.GetLevelPropertyNames(levelName);
            propertyNames.Sort(new MadNaturalSortComparer());

            foreach (var propertyName in propertyNames) {
                var propertyValue = MadLevelProfile.GetLevelAny(levelName, propertyName);
                GUIProperty(propertyName, propertyValue);
            }
        }
    }

    private static void GUIProperty(string propertyName, string propertyValue) {
        EditorGUILayout.LabelField(propertyName + " = " + propertyValue);
    }

    private bool Foldout(string name, string path) {
        bool state = false;

        if (foldouts.ContainsKey(path)) {
            state = foldouts[path];
        }

        bool newState = EditorGUILayout.Foldout(state, name);
        if (newState != state) {
            foldouts[path] = newState;
        }

        return newState;
    }

    #endregion

    #region Public Static Methods

    public new static void Show() {
        GetWindow<MadLevelProfileBrowser>(true, "Profile Browser", true);
    }

    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

#if !UNITY_3_5
} // namespace
#endif