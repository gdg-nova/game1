/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MadLevelManager {

/// <summary>
/// Manages player profiles and game progress (save & load).
/// </summary>
public class MadLevelProfile {

    // ===========================================================
    // Constants
    // ===========================================================
    
    const string ProfileLevelName = "__profile__";
    
    private const string KeyProfileCurrent = "profile_current";
    private const string KeyProfileList = "profile_list";
    private const string KeyRecentLevelSelected = "recent_level_selected";
    private const string KeyRecentLevelScreen = "recent_level_screen";
    private const string KeyLevels = "_levels"; // profile_levels
    
    public const string DefaultProfile = "_default";
    
    private const string PropertyCompleted = "@completed@";
    private const string PropertyLocked = "@locked@";
    
    // ===========================================================
    // Fields
    // ===========================================================

    static Level _profileLevel;
    static Level profileLevel {
        get {
            if (_profileLevel == null) {
                LoadProfile();
            }
            
            return _profileLevel;
        }
    }
    
    static Dictionary<string, Level> _levels;
    static Dictionary<string, Level> levels {
        get {
            if (_levels == null) {
                LoadProfile();
            }
            
            return _levels;
        }
    }
    
    public static void LoadProfileFromString(string str) {
        try { 
            var levels = new Dictionary<string, Level>();
        
            using (StringReader reader = new StringReader(str)) {
                while (true) {
                    var line = reader.ReadLine();
                    // on JellyBean 4.3 devices there are spaces on the last line read. Don't know why, but trimming
                    // it, because it will break the process
                    if (line != null) {
                        line = line.Trim();
                    }
                    
                    if (string.IsNullOrEmpty(line)) {
                        break;
                    }
                    
                    var level = Level.Read(line);
                    if (level != null) {
                        if (level.name == ProfileLevelName) {
                            _profileLevel = level;
                        } else {
                            levels.Add(level.name, level);
                        }
                    }
                }
            }
            
            _levels = levels;
            
            if (_profileLevel == null) {
                // profile level not found in the save, creating new
                _profileLevel = new Level(ProfileLevelName);
            }
            
            WriteProfile();
        } catch (Exception) {
            Debug.LogError("Something went wrong when reading profile string. Please copy this message and send it to "
                + MadLevelHelp.Mail + "\nPROFILE_STRING:\n" + str + "\nEND_OF_PROFILE_STRING");
            throw;
        }
    }
    
    /// <summary>
    /// Forces reload of profile data. Useful for testing purposes.
    /// </summary>
    public static void Reload() {
        LoadProfile();
    }
    
    static void LoadProfile() {
        // move legacy default profile data if found
//        string legacy120DefaultKey = "default_" + KeyLevels;
//        if (PlayerPrefs.HasKey(legacy120DefaultKey)) {
//            var oldDefaultProfile = PlayerPrefs.GetString(legacy120DefaultKey);
//            PlayerPrefs.SetString(DefaultProfile + "_" + KeyLevels, oldDefaultProfile);
//            PlayerPrefs.DeleteKey(legacy120DefaultKey);
//        }
        
        var levelsStr = PlayerPrefs.GetString(profile + "_" + KeyLevels, "");
        LoadProfileFromString(levelsStr);

        // apply profile to fix set locked info
        var activeConfiguration = MadLevel.activeConfiguration;
        if (activeConfiguration != null) {
            activeConfiguration.ApplyProfile();
        }
    }
    
    public static string SaveProfileToString() {
        var builder = new StringBuilder();
        foreach (var level in levels.Values) {
            builder.AppendLine(level.Write());
        }
        
        builder.AppendLine(profileLevel.Write());
        
        return builder.ToString();
    }
    
    static void WriteProfile() {
        string levelsString = SaveProfileToString();
        PlayerPrefs.SetString(profile + "_" + KeyLevels, levelsString);
    }

    // completely erases profile data to not keep in on device no longer
    static void EraseProfileData() {
        PlayerPrefs.DeleteKey(profile + "_" + KeyLevels);
    }
    
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    public static string profile {
        get {
            return PlayerPrefs.GetString(KeyProfileCurrent, DefaultProfile);
        }
        
        set {
            if (value != profile) {
                RegisterProfile(value);
            
                PlayerPrefs.SetString(KeyProfileCurrent, value);    
                PlayerPrefs.Save();
                
                // forget level settings
                _profileLevel = null;
                _levels = null;
            }
        }
    }
    
    /// <summary>
    /// Registers profile if not yet registered.
    /// Registration of profile is optional. If you try to use profile for the first time by
    /// assigning profile name to 'profile' field then new profile will be created.
    /// </summary>
    /// <param name='profileName'>
    /// Profile name.
    /// </param>
    public static void RegisterProfile(string profileName) {
        string[] profiles = profileList;
        if (Array.Find(profiles, (obj) => obj == profileName) == null) {
            Array.Resize(ref profiles, profiles.Length + 1);
            profiles[profiles.Length - 1] = profileName;
            profileList = profiles;
        }
    }
    
    /// <summary>
    /// Unregisters the given profile name. All profile data will be permanently removed from the device.
    /// If current profile is profile that is given to be unregistered, current profile will be changed to defualt.
    /// </summary>
    /// <param name="profileName">Profile name to unregister.</param>
    public static void UnregisterProfile(string profileName) {
        if (profileName == DefaultProfile) {
            Debug.LogWarning("Cannot unregister default profile");
            return;
        }
        
        MadDebug.Assert(profileList.Contains(profileName), "No profile called '" + profileName + "' found.");
    
        if (profile != profileName) {
            var currentProfile = profile;
            profile = profileName;
            EraseProfileData();
            profile = currentProfile;
        } else {
            // is on current profile - reset and fallback to default
            EraseProfileData();
            profile = DefaultProfile;
        }
        
        List<string> newProfiles = new List<string>();
        string[] oldProfiles = profileList;
        
        foreach (string oldProfile in oldProfiles) {
            if (oldProfile != profileName) {
                newProfiles.Add(oldProfile);
            }
        }
        
        profileList = newProfiles.ToArray();
    }
    
    public static string[] profileList {
        get {
            var str = PlayerPrefs.GetString(KeyProfileList);
            if (string.IsNullOrEmpty(str)) {
                str = DefaultProfile;
            }
            
            string[] profiles = str.Split(' ');
            for (int i = 0; i < profiles.Length; ++i) {
                profiles[i] = profiles[i].Replace("%20", " ");
            }
            
            return profiles;
        }
        
        set {
            string[] profiles = value;
            for (int i = 0; i < profiles.Length; ++i) {
                profiles[i] = profiles[i].Replace(" ", "%20");
            }

            var str = string.Join(" ", profiles);            
            PlayerPrefs.SetString(KeyProfileList, str);
            PlayerPrefs.Save();
        }
    }
    
    /// <summary>
    /// Gets or sets the name of recently choosen level. This is NOT a scene name but
    /// the name of icon game object.
    /// </summary>
    /// <value>
    /// The recent level selected name (NOT a scene name).
    /// </value>
    [Obsolete("Use MadLevel.currentLevelName instead.")]
    public static string recentLevelSelected {
        get {
            return PlayerPrefs.GetString(profile + "_" + KeyRecentLevelSelected);
        }
        
        set {
            PlayerPrefs.SetString(profile + "_" + KeyRecentLevelSelected, value);
            PlayerPrefs.Save();
        }
    }
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
#region Properties

    public static List<string> GetLevelNames() {
        var levelNames = new List<string>();
        foreach (var level in levels) {
            levelNames.Add(level.Key);
        }

        return levelNames;
    }

    public static List<string> GetLevelPropertyNames(string levelName) {
        if (levels.ContainsKey(levelName)) {
            var level = levels[levelName];
            return level.GetPropertyNames();
        }

        return new List<string>();
    }

    public static List<string> GetProfilePropertyNames() {
        return profileLevel.GetPropertyNames();
    }

    /// <summary>
    /// Gets level-scoped property value of any type as string. Use this method if you only need a string
    /// representation of a value and you don't want to concern about its type.
    /// </summary>
    /// <returns>
    /// String representation of property value. Empty string if value is unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static string GetLevelAny(string levelName, string property) {
        return GetLevelAny(levelName, property, "");
    }

    /// <summary>
    /// Gets level-scoped property value of any type as string. Use this method if you only need a string
    /// representation of a value and you don't want to concern about its type.
    /// </summary>
    /// <returns>
    /// String representation of property value. Empty string if value is unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value.
    /// </param>
    public static string GetLevelAny(string levelName, string property, string def) {
        if (levels.ContainsKey(levelName)) {
            var level = levels[levelName];
            if (level.HasProperty(property)) {
                return level.GetPropertyAny(property);
            } else {
                return def;
            }
        } else {
            return def;
        }
    }
    
    /// <summary>
    /// Gets global-scoped property value of any type as string. Use this method if you only need a string
    /// representation of a value and you don't want to concern about its type.
    /// </summary>
    /// <returns>
    /// String representation of property value. Empty string if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static string GetProfileAny(string property) {
        if (IsProfilePropertySet(property)) {
            return profileLevel.GetPropertyAny(property);
        } else {
            return "";
        }
    }

    /// <summary>
    /// Gets level-scoped property value as boolean type. You can get value of properties only set using
    /// <see cref="SetLevelBoolean"/> method.
    /// </summary>
    /// <returns>
    /// Boolean representation of property value or false if value is unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static bool GetLevelBoolean(string levelName, string property) {
        return GetLevelBoolean(levelName, property, false);
    }
    
    /// <summary>
    /// Gets level-scoped property value as boolean type. You can get value of properties only set using
    /// <see cref="SetLevelBoolean"/> method.
    /// </summary>
    /// <returns>
    /// Boolean representation of property value or given default value if unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value to return if value is unset.
    /// </param>
    public static bool GetLevelBoolean(string levelName, string property, bool def) {
        if (levels.ContainsKey(levelName)) {
            var level = levels[levelName];
            if (level.HasProperty(property)) {
                return level.GetPropertyBoolean(property);
            } else {
                return def;
            }
        } else {
            return def;
        }
    }
    
    /// <summary>
    /// Sets level-scoped property as boolean type. You can access it later using <see cref="GetLevelBoolean"/> or
    /// <see cref="GetLevelAny"/> methods.
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='val'>
    /// Value to set.
    /// </param>
    public static void SetLevelBoolean(string levelName, string property, bool val) {
        Level level;
        
        if (!levels.ContainsKey(levelName)) {
            level = new Level(levelName);
            levels.Add(levelName, level);
        } else {
            level = levels[levelName];
        }
        
        if (level.SetPropertyBoolean(property, val)) {
            WriteProfile();
        }
    }
    
    /// <summary>
    /// Sets profile-scoped property as boolean type. You can access it later using <see cref="GetProfileBoolean"/> or
    /// <see cref="GetProfileAny"/> methods.
    /// </summary>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='val'>
    /// Value to set.
    /// </param>
    public static void SetProfileBoolean(string property, bool val) {
        if (profileLevel.SetPropertyBoolean(property, val)) {
            WriteProfile();
        }
    }
    
    /// <summary>
    /// Gets global-scoped property value as boolean type. You can get value of properties only set using
    /// <see cref="SetProfileBoolean"/> method.
    /// </summary>
    /// <returns>
    /// Boolean representation of property value or false if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static bool GetProfileBoolean(string property) {
        return GetProfileBoolean(property, false);
    }
    
    /// <summary>
    /// Gets global-scoped property value as boolean type. You can get value of properties only set using
    /// <see cref="SetProfileBoolean"/> method.
    /// </summary>
    /// <returns>
    /// Boolean representation of property value or given default if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value to return if value is unset.
    /// </param>
    public static bool GetProfileBoolean(string property, bool def) {
        if (IsProfilePropertySet(property)) {
            return profileLevel.GetPropertyBoolean(property);
        } else {
            return def;
        }
    }
    
    /// <summary>
    /// Gets level-scoped property value as integer type. You can get value of properties only set using
    /// <see cref="SetLevelInteger"/> method.
    /// </summary>
    /// <returns>
    /// Integer representation of property value or 0 if value is unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static int GetLevelInteger(string levelName, string property) {
        return GetLevelInteger(levelName, property, 0);
    }
    
    /// <summary>
    /// Gets level-scoped property value as integer type. You can get value of properties only set using
    /// <see cref="SetLevelInteger"/> method.
    /// </summary>
    /// <returns>
    /// Integer representation of property value or given default value if unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value to return if value is unset.
    /// </param>
    public static int GetLevelInteger(string levelName, string property, int def) {
        if (levels.ContainsKey(levelName)) {
            var level = levels[levelName];
            if (level.HasProperty(property)) {
                return level.GetPropertyInteger(property);
            } else {
                return def;
            }
        } else {
            return def;
        }
    }
    
    /// <summary>
    /// Sets level-scoped property as integer type. You can access it later using <see cref="GetLevelInteger"/> or
    /// <see cref="GetLevelAny"/> methods.
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='val'>
    /// Value to set.
    /// </param>
    public static void SetLevelInteger(string levelName, string property, int val) {
        Level level;
        
        if (!levels.ContainsKey(levelName)) {
            level = new Level(levelName);
            levels.Add(levelName, level);
        } else {
            level = levels[levelName];
        }
        
        if (level.SetPropertyInteger(property, val)) {
            WriteProfile();
        }
    }
    
    /// <summary>
    /// Sets profile-scoped property as integer type. You can access it later using <see cref="GetProfileInteger"/> or
    /// <see cref="GetProfileAny"/> methods.
    /// </summary>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='val'>
    /// Value to set.
    /// </param>
    public static void SetProfileInteger(string property, int val) {
        if (profileLevel.SetPropertyInteger(property, val)) {
            WriteProfile();
        }
    }
    
    /// <summary>
    /// Gets global-scoped property value as integer type. You can get value of properties only set using
    /// <see cref="SetProfileInteger"/> method.
    /// </summary>
    /// <returns>
    /// Integer representation of property value or 0 if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static int GetProfileInteger(string property) {
        return GetProfileInteger(property, 0);
    }
    
    /// <summary>
    /// Gets global-scoped property value as integer type. You can get value of properties only set using
    /// <see cref="SetProfileInteger"/> method.
    /// </summary>
    /// <returns>
    /// Integer representation of property value or given default if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value to return if value is unset.
    /// </param>
   public static int GetProfileInteger(string property, int def) {
        if (IsProfilePropertySet(property)) {
            return profileLevel.GetPropertyInteger(property);
        } else {
            return def;
        }
    }
    
    /// <summary>
    /// Gets level-scoped property value as float type. You can get value of properties only set using
    /// <see cref="SetLevelFloat"/> method.
    /// </summary>
    /// <returns>
    /// Float representation of property value or 0 if value is unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static float GetLevelFloat(string levelName, string property) {
        return GetLevelFloat(levelName, property, 0);
    }
    
    /// <summary>
    /// Gets level-scoped property value as float type. You can get value of properties only set using
    /// <see cref="SetLevelFloat"/> method.
    /// </summary>
    /// <returns>
    /// Float representation of property value or given default value if unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value to return if value is unset.
    /// </param>
    public static float GetLevelFloat(string levelName, string property, float def) {
        if (levels.ContainsKey(levelName)) {
            var level = levels[levelName];
            if (level.HasProperty(property)) {
                return level.GetPropertyFloat(property);
            } else {
                return def;
            }
        } else {
            return def;
        }
    }
    
    /// <summary>
    /// Sets level-scoped property as float type. You can access it later using <see cref="GetLevelFloat"/> or
    /// <see cref="GetLevelAny"/> methods.
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='val'>
    /// Value to set.
    /// </param>
    public static void SetLevelFloat(string levelName, string property, float val) {
        Level level;
        
        if (!levels.ContainsKey(levelName)) {
            level = new Level(levelName);
            levels.Add(levelName, level);
        } else {
            level = levels[levelName];
        }
        
        if (level.SetPropertyFloat(property, val)) {
            WriteProfile();
        }
    }
    
    /// <summary>
    /// Sets profile-scoped property as float type. You can access it later using <see cref="GetProfileFloat"/> or
    /// <see cref="GetProfileAny"/> methods.
    /// </summary>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='val'>
    /// Value to set.
    /// </param>
    public static void SetProfileFloat(string property, float val) {
        if (profileLevel.SetPropertyFloat(property, val)) {
            WriteProfile();
        }
    }
    
    /// <summary>
    /// Gets global-scoped property value as float type. You can get value of properties only set using
    /// <see cref="SetProfileFloat"/> method.
    /// </summary>
    /// <returns>
    /// Float representation of property value or 0 if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static float GetProfileFloat(string property) {
        return GetProfileFloat(property, 0);
    }
    
    /// <summary>
    /// Gets global-scoped property value as float type. You can get value of properties only set using
    /// <see cref="SetProfileFloat"/> method.
    /// </summary>
    /// <returns>
    /// Float representation of property value or given default if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value to return if value is unset.
    /// </param>
   public static float GetProfileFloat(string property, float def) {
        if (IsProfilePropertySet(property)) {
            return profileLevel.GetPropertyFloat(property);
        } else {
            return def;
        }
    }
    
    /// <summary>
    /// Gets level-scoped property value as string type. You can get value of properties only set using
    /// <see cref="SetLevelString"/> method.
    /// </summary>
    /// <returns>
    /// String representation of property value or empty string if value is unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static string GetLevelString(string levelName, string property) {
        return GetLevelString(levelName, property, "");
    }
    
    /// <summary>
    /// Gets level-scoped property value as string type. You can get value of properties only set using
    /// <see cref="SetLevelString"/> method.
    /// </summary>
    /// <returns>
    /// String representation of property value or given default value if unset.
    /// </returns>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value to return if value is unset.
    /// </param>
    public static string GetLevelString(string levelName, string property, string def) {
        if (levels.ContainsKey(levelName)) {
            var level = levels[levelName];
            if (level.HasProperty(property)) {
                return level.GetPropertyString(property);
            } else {
                return def;
            }
        } else {
            return def;
        }
    }
    
    /// <summary>
    /// Sets level-scoped property as string type. You can access it later using <see cref="GetLevelString"/> or
    /// <see cref="GetLevelAny"/> methods.
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='val'>
    /// Value to set.
    /// </param>
    public static void SetLevelString(string levelName, string property, string val) {
        Level level;
        
        if (!levels.ContainsKey(levelName)) {
            level = new Level(levelName);
            levels.Add(levelName, level);
        } else {
            level = levels[levelName];
        }
        
        if (level.SetPropertyString(property, val)) {
            WriteProfile();
        }
    }
    
    /// <summary>
    /// Sets profile-scoped property as string type. You can access it later using <see cref="GetProfileString"/> or
    /// <see cref="GetProfileAny"/> methods.
    /// </summary>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='val'>
    /// Value to set.
    /// </param>
    public static void SetProfileString(string property, string val) {
        if (profileLevel.SetPropertyString(property, val)) {
            WriteProfile();
        }
    }
    
    /// <summary>
    /// Gets global-scoped property value as string type. You can get value of properties only set using
    /// <see cref="SetProfileString"/> method.
    /// </summary>
    /// <returns>
    /// String representation of property value or empty string if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    public static string GetProfileString(string property) {
        return GetProfileString(property, "");
    }
    
    /// <summary>
    /// Gets global-scoped property value as string type. You can get value of properties only set using
    /// <see cref="SetProfileString"/> method.
    /// </summary>
    /// <returns>
    /// String representation of property value or given default if value is unset.
    /// </returns>
    /// <param name='property'>
    /// Property name.
    /// </param>
    /// <param name='def'>
    /// Default value to return if value is unset.
    /// </param>
   public static string GetProfileString(string property, string def) {
        if (IsProfilePropertySet(property)) {
            return profileLevel.GetPropertyString(property);
        } else {
            return def;
        }
    }
    
    [Obsolete("Use MadLevelProfile.GetLevelBoolean instead.")]
    public static bool IsPropertyEnabled(string levelName, string property) {
        return GetLevelBoolean(levelName, property);
    }
    
    [Obsolete("Use MadLevelProfile.SetLevelBoolean instead.")]
    public static void SetPropertyEnabled(string levelName, string property, bool enabled) {
        SetLevelBoolean(levelName, property, enabled);
    }
    
    public static bool IsProfilePropertySet(string property) {
        return profileLevel.HasProperty(property);
    }
    
    public static bool IsLevelPropertySet(string levelName, string property) {
        if (levels.ContainsKey(levelName)) {
            var level = levels[levelName];
            return level.HasProperty(property);
        } else {
            return false;
        }
    }
    
    [Obsolete("Use MadLevelProfile.IsLevelPropertySet instead.")]
    public static bool IsPropertySet(string levelName, string property) {
        return IsLevelPropertySet(levelName, property);
    }
    
    public static bool IsCompleted(string levelName) {
        return GetLevelBoolean(levelName, PropertyCompleted);
    }
    
    public static bool IsCompleted(string levelName, bool def) {
        return GetLevelBoolean(levelName, PropertyCompleted, def);
    }
    
    public static void SetCompleted(string levelName, bool completed) {
        SetLevelBoolean(levelName, PropertyCompleted, completed);
    }
    
    public static bool IsCompletedSet(string levelName) {
        return IsLevelPropertySet(levelName, PropertyCompleted);
    }
    
    public static bool IsLocked(string levelName) {
        return GetLevelBoolean(levelName, PropertyLocked);
    }
    
    public static bool IsLocked(string levelName, bool def) {
        return GetLevelBoolean(levelName, PropertyLocked, def);
    }
    
    public static void SetLocked(string levelName, bool locked) {
        SetLevelBoolean(levelName, PropertyLocked, locked);
    }
    
    public static bool IsLockedSet(string levelName) {
        return IsLevelPropertySet(levelName, PropertyLocked);
    }
#endregion

#region Helpers
    public static bool IsLevelSet(string levelName) {
        return levels.ContainsKey(levelName);
    }
    
    public static void RenameLevel(string oldName, string newName) {
        if (IsLevelSet(oldName) && !IsLevelSet(newName)) {
            var level = levels[oldName];
            levels[newName] = level;
        } else {
            Debug.LogError("Cannot rename level");
        }
    }
#endregion

    public static void Save() {
        PlayerPrefs.Save();
    }
    
    // resets all stored data in current profile
    public static void Reset() {
        ResetLevelScope();
        ResetProfileScope();
        WriteProfile();
        Save();
    }
    
    static void ResetLevelScope() {
        levels.Clear();
    }
    
    static void ResetProfileScope() {
        _profileLevel = new Level(ProfileLevelName);
    }
    

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    private class Level {
        const int Version = 3;
        const int LowestSupportedVersion = 1;
        const string SpaceSubstitue = "%20";
        
        public string name;
        private Dictionary<string, PropertyValue> properties = new Dictionary<string, PropertyValue>();
        
        public Level(string name) {
            this.name = name;
        }

        public List<string> GetPropertyNames() {
            return properties.Keys.ToList();
        }

        public bool SetPropertyBoolean(string key, bool val) {
            var prop = PropertyValue.FromBoolean(val);
            if (properties.ContainsKey(key) && properties[key].Equals(prop)) {
                return false;
            }
            
            properties[key] = prop;
            return true;
        }
        
        public bool SetPropertyInteger(string key, int val) {
            var prop = PropertyValue.FromInteger(val);
            if (properties.ContainsKey(key) && properties[key].Equals(prop)) {
                return false;
            }
            
            properties[key] = prop;
            return true;
        }
        
        public bool SetPropertyFloat(string key, float val) {
            var prop = PropertyValue.FromFloat(val);
            if (properties.ContainsKey(key) && properties[key].Equals(prop)) {
                return false;
            }
            
            properties[key] = prop;
            return true;
        }
        
        public bool SetPropertyString(string key, string val) {
            var prop = PropertyValue.FromString(val);
            if (properties.ContainsKey(key) && properties[key].Equals(prop)) {
                return false;
            }
            
            properties[key] = prop;
            return true;
        }
        
        public bool HasProperty(string key) {
            return properties.ContainsKey(key);
        }
        
        public bool GetPropertyBoolean(string key) {
            return properties[key].BooleanValue();
        }
        
        public int GetPropertyInteger(string key) {
            return properties[key].IntegerValue();
        }
        
        public float GetPropertyFloat(string key) {
            return properties[key].FloatValue();
        }
        
        public string GetPropertyString(string key) {
            return properties[key].StringValue();
        }
        
        public string GetPropertyAny(string key) {
            return properties[key].AnyValue();
        }
        
        public static Level Read(string line) {
            var parts = line.Split(' ');
            int loadedVersion = int.Parse(parts[0]);
            
            if (Version != loadedVersion) {
                if (loadedVersion >= LowestSupportedVersion) {
                    Debug.Log(string.Format(
                        "Current save version is {0}, but {1} found. Will be upgraded.", Version, loadedVersion));
                } else {
                    Debug.LogError(string.Format("Expected version {0} but {1} found", Version, loadedVersion));
                    return null;
                }
            }
            
            string levelName = In(parts[1]);
            var level = new Level(levelName);
            
            // load properties
            for (int i = 2; i < parts.Length; i += 2) {
                string key = In(parts[i]);
                PropertyValue value;
                
                if (loadedVersion == 1) {
                    bool val = bool.Parse(In(parts[i + 1]));
                    value = PropertyValue.FromBoolean(val);
                } else {
                    value = PropertyValue.Read(parts[i + 1]);
                }
                
                // in versions 1 and 2 level select screen was saving 'locked' property
                // with the wrong key
                if (loadedVersion <= 2 && key == "locked") {
                    key = PropertyLocked;
                    if (!level.properties.ContainsKey(key)) {
                        level.properties.Add(key, value);
                    }
                } else {
                    level.properties.Add(key, value);
                }
            }
            
            return level;
        }
        
        public string Write() {
            var builder = new StringBuilder();
            builder.Append(string.Format("{0} {1}", Version, Out(name)));
            
            foreach (var key in properties.Keys) {
                builder.Append(" ");
                builder.Append(Out(key));
                builder.Append(" ");
                builder.Append(properties[key].Write());
            }
            
            return builder.ToString();
        }
        
        private static string Out(string str) {
            str = str.Replace("%", "%25");
            str = str.Replace(" ", "%20");
            return str;
        }
        
        private static string In(string str) {
            str = str.Replace("%20", " ");
            str = str.Replace("%25", "%");
            return str;
        }
    }
    
    private class PropertyValue {
        public readonly Type type;
        public readonly string strValue;
        
        public static PropertyValue FromBoolean(bool val) {
            return new PropertyValue(Type.Boolean, val.ToString());
        }
        
        public static PropertyValue FromInteger(int val) {
            return new PropertyValue(Type.Integer, val.ToString());
        }
        
        public static PropertyValue FromFloat(float val) {
            return new PropertyValue(Type.Float, val.ToString());
        }
        
        public static PropertyValue FromString(string val) {
            return new PropertyValue(Type.String, val);
        }
        
        
        PropertyValue(Type t, string s) {
            type = t;
            strValue = s;
        }
        
        public string Write() {
            string outStr = strValue;
            
            if (type == Type.String) {
                var bytes = Encoding.UTF8.GetBytes(outStr.ToCharArray(), 0, outStr.Length);
                outStr = Convert.ToBase64String(bytes);
            }
        
            return string.Format("{0}:{1}", TypeToString(), outStr);
        }
        
        public static PropertyValue Read(string data) {
            Type type = StringToType(data);
            string strVal = data.Substring(2);
            
            if (type == Type.String) {
                var strData = Convert.FromBase64String(strVal);
                strVal = Encoding.UTF8.GetString(strData, 0, strData.Length);
            }
            
            return new PropertyValue(type, strVal);
        }
        
        string TypeToString() {
            switch (type) {
                case Type.Boolean:
                    return "b";
                case Type.Integer:
                    return "i";
                case Type.Float:
                    return "f";
                case Type.String:
                    return "s";
                default:
                    MadDebug.Assert(false, "Unknown type: " + type);
                    break;
            }
            
            return "?";
        }
        
        static Type StringToType(string str) {
            string prefix = str.Substring(0, 2);
            if (prefix == "b:") {
                return Type.Boolean;
            } else if (prefix == "i:") {
                return Type.Integer;
            } else if (prefix == "f:") {
                return Type.Float;
            } else if (prefix == "s:") {
                return Type.String;
            } else {
                MadDebug.Assert(false, "Unknown type prefix: " + prefix);
                return default(Type);
            }
        }
        
        public bool BooleanValue() {
            MadDebug.Assert(type == Type.Boolean, "Property type is " + type);
            return bool.Parse(strValue);
        }
        
        public int IntegerValue() {
            MadDebug.Assert(type == Type.Integer, "Property type is " + type);
            return int.Parse(strValue);
        }
        
        public float FloatValue() {
            MadDebug.Assert(type == Type.Float, "Property type is " + type);
            return float.Parse(strValue);
        }
        
        public string StringValue() {
            MadDebug.Assert(type == Type.String, "Property type is " + type);
            return strValue;
        }
        
        public string AnyValue() {
            return strValue;
        }
        
        public override bool Equals(object obj) {
            if (!(obj is PropertyValue)) {
                return false;
            }
            
            PropertyValue other = obj as PropertyValue;
            if (type != other.type) {
                return false;
            }
            
            if (strValue != other.strValue) {
                return false;
            }
            
            return true;
        }

        public override int GetHashCode() {
            int hash = 37;
            hash += hash * 17 + type.GetHashCode();
            hash += hash * 17 + (strValue != null ? strValue.GetHashCode() : 0);
            return hash;
        }
        
        public enum Type {
            Boolean,
            Integer,
            Float,
            String,
        }
    }

}

} // namespace
