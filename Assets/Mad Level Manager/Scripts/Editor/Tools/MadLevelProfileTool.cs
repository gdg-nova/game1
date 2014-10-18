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

public class MadLevelProfileTool : EditorWindow {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    int selectedProfileIndex;
    
    bool onCurrentProfile;
    string selectedProfileName;
    
    string newProfileName;

    Vector2 scrollPosition;

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnGUI() {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        MadGUI.BeginBox("Profiles");
        MadGUI.Indent(() => {
        
            int currentProfileIndex = 0;
            string[] profiles = MadLevelProfile.profileList;
            for (int i = 0; i < profiles.Length; ++i) {
                string profile = profiles[i];
                if (profile == MadLevelProfile.profile) {
                    profiles[i] = profile + " (Current)";
                    currentProfileIndex = i;
                }
            }
            
            selectedProfileIndex =
                EditorGUILayout.Popup("Profiles", selectedProfileIndex, profiles);
            
            onCurrentProfile = currentProfileIndex == selectedProfileIndex;
            selectedProfileName = MadLevelProfile.profileList[selectedProfileIndex];
            
            if (onCurrentProfile) {
                GUI.enabled = false;
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Switch")) {
                SwitchProfile(selectedProfileName);
            }
            
            GUI.enabled = true;
            
            
            if (onCurrentProfile || selectedProfileName == MadLevelProfile.DefaultProfile) {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Delete")) {
                DeleteProfile(selectedProfileName);
            }
            
            GUI.enabled = true;
            
            if (GUILayout.Button("Reset")) {
                ResetProfile(selectedProfileName);
            }
            
            EditorGUILayout.EndHorizontal();
        });
        
        MadGUI.BeginBox("New Profile");
            EditorGUILayout.BeginHorizontal();
            newProfileName = EditorGUILayout.TextField(newProfileName);
            
            if (string.IsNullOrEmpty(newProfileName)) {
                GUI.enabled = false;
            }
            
            if (GUILayout.Button("Create")) {
                CreateProfile();
            }
            
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            MadGUI.EndBox();
            
            MadGUI.BeginBox("Save/Load");
            GUILayout.Label("These actions will be taken on selected profile above.");
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Profile")) {
                SaveProfile();
            }
            
            if (GUILayout.Button("Load Profile")) {
                LoadProfile();
            }
            EditorGUILayout.EndHorizontal();
            MadGUI.EndBox();
        MadGUI.EndBox();

        MadGUI.BeginBox("Toolbox");
        EditorGUILayout.BeginHorizontal();
        if (MadGUI.Button("Unlock All Levels")) {
            UnlockAllLevels();
        }

        if (MadGUI.Button("Complete All Levels")) {
            CompleteAllLevels();
        }
        EditorGUILayout.EndHorizontal();

        MadGUI.EndBox();

        EditorGUILayout.EndScrollView();
    }

    void UnlockAllLevels() {
        if (!CheckPlaying()) {
            return;
        }

        var levelNames = MadLevel.GetAllLevelNames();
        foreach (var levelName in levelNames) {
            MadLevelProfile.SetLocked(levelName, false);
        }

        MadLevel.ReloadCurrent();
    }

    void CompleteAllLevels() {
        if (!CheckPlaying()) {
            return;
        }

        var levelNames = MadLevel.GetAllLevelNames();
        foreach (var levelName in levelNames) {
            MadLevelProfile.SetCompleted(levelName, true);
        }

        MadLevel.ReloadCurrent();
    }
    
    void SwitchProfile(string profile) {
        if (!CheckPlaying()) {
            return;
        }
    
        MadLevelProfile.profile = profile;
        MadLevel.ReloadCurrent();
    }
    
    void CreateProfile() {
        if (!CheckPlaying()) {
            return;
        }
    
        foreach (var profileName in MadLevelProfile.profileList) {
            if (newProfileName == profileName) {
                EditorUtility.DisplayDialog(
                    "Profile Exists!", "Profile called '" + newProfileName + "' already exists!", "Ouch!");
                return;
            }
        }
        
        MadLevelProfile.RegisterProfile(newProfileName);
        
        EditorUtility.DisplayDialog(
            "Profile Created!", "Profile '" + newProfileName + "' created successufully!", "OK");
    }
    
    void DeleteProfile(string profile) {
        if (!CheckPlaying()) {
            return;
        }
    
        if (EditorUtility.DisplayDialog(
            "Delete Profile " + profile + "?",
            "Are you sure you want to delete profile '" + profile + "'? This cannot be undone.",
            "Yes", "No")) {
                MadLevelProfile.UnregisterProfile(profile);
                selectedProfileIndex = 0;
        }
    }
    
    void ResetProfile(string profile) {
        if (!CheckPlaying()) {
            return;
        }
        
        if (EditorUtility.DisplayDialog(
            "Reset Profile " + profile + "?",
            "Are you sure you want to reset profile '" + profile + "'? This cannot be undone.",
            "Yes", "No")) {
                string prevProfile = MadLevelProfile.profile;
                MadLevelProfile.profile = selectedProfileName;
                MadLevelProfile.Reset();
                MadLevelProfile.profile = prevProfile;
                
                MadLevel.ReloadCurrent();
        }
    }
    
    void SaveProfile() {
        if (!CheckPlaying()) {
            return;
        }
        
        string prevProfile = MadLevelProfile.profile;
        MadLevelProfile.profile = selectedProfileName;
    
        var path = EditorUtility.SaveFilePanel(
            "Save Profile", "", MadLevelProfile.profile + ".profile", "profile");
            
        if (path.Length != 0) {
            string levels = MadLevelProfile.SaveProfileToString();
            System.IO.File.WriteAllText(path, levels);
            EditorUtility.DisplayDialog("Profile Saved!", "Profile saved to " + path, "OK");
        }
        
        MadLevelProfile.profile = prevProfile;
    }
    
    void LoadProfile() {
        if (!CheckPlaying()) {
            return;
        }
        
        string prevProfile = MadLevelProfile.profile;
        MadLevelProfile.profile = selectedProfileName;
    
        var path = EditorUtility.OpenFilePanel("Load Profile", "", "profile");
        if (path.Length != 0) {
            var levels = System.IO.File.ReadAllText(path);
            MadLevelProfile.LoadProfileFromString(levels);
            MadLevelProfile.Save();
            
            // reload level
            Application.LoadLevel(Application.loadedLevel);
        }
        
        MadLevelProfile.profile = prevProfile;
    }
    
    bool CheckPlaying() {
        if (!Application.isPlaying) {
            EditorUtility.DisplayDialog("Not in Play Mode", "This tool can be used only when playing.", "OK");
            return false;
        }
        
        return true;
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