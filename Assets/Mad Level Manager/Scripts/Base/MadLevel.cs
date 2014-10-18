/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

/// <summary>
/// Level access & loading routines.
/// </summary>
public class MadLevel  {

    // ===========================================================
    // Constants
    // ===========================================================
    
    // ===========================================================
    // Fields
    // ===========================================================
    
    private static MadLevelConfiguration _configuration;

    private static string _arguments = null;

    private static string _currentLevelName = null;

    // set to true when extension is set for the first time
    private static bool extensionDefined = false;


    public static string defaultGroupName {
        get {
            return activeConfiguration.defaultGroup.name;
        }
    }

    /// <summary>
    /// Gets the active configuration.
    /// </summary>
    /// <value>
    /// The active configuration.
    /// </value>
    public static MadLevelConfiguration activeConfiguration {
        get {
            if (_configuration == null || !_configuration.active) {
                _configuration = MadLevelConfiguration.GetActive();
            }
            
            return _configuration;
        }
    }
    
    /// <summary>
    /// Gets a value indicating whether application has active configuration.
    /// </summary>
    /// <value>
    /// <c>true</c> if has active configuration; otherwise, <c>false</c>.
    /// </value>
    public static bool hasActiveConfiguration {
        get {
            if (_configuration == null) {
                    var configurations = Resources.LoadAll(
                        "LevelConfig", typeof(MadLevelConfiguration));
                    return configurations.Length > 0;
            } else {
                return true;
            }
        }
    }
    
    /// <summary>
    /// Gets or sets this level arguments.
    /// </summary>
    /// <value>
    /// The arguments.
    /// </value>
    public static string arguments {
        get {
            if (_arguments == null) {
                FindCurrentSceneLevel();
            }
        
            return _arguments;
        }
        
        set {
            _arguments = value;
        }
    }
    
    /// <summary>
    /// Gets the name of the current level.
    /// </summary>
    /// <value>
    /// The name of the current level.
    /// </value>
    public static string currentLevelName {
        get {
            if (_currentLevelName == null) {
                FindCurrentSceneLevel();
            }
        
            return _currentLevelName;
        }
        
        set {
            _currentLevelName = value;
        }
    }

    public static string currentGroupName {
        get {
            var levelName = currentLevelName;
            var level = activeConfiguration.FindLevelByName(levelName);
            return level.group.name;
        }
    }

    public static bool hasExtension {
        get {
            return currentExtension != null;
        }
    }

    public static MadLevelExtension currentExtension {
        get {
            if (!extensionDefined) {
                // if extension is undefined, try to get it using level name
                var levelName = currentLevelName;
                var level = activeConfiguration.FindLevelByName(levelName);
                if (level != null) {
                    currentExtension = level.extension;
                }
            }

            return _currentExtension;
        }
        set {
            _currentExtension = value;
            extensionDefined = true;
        }
    }

    private static MadLevelExtension _currentExtension;

    public static int currentExtensionProgress {
        get;
        set;
    }
    
    static void FindCurrentSceneLevel() {
        // find first level with matching scene name
        bool hasMany;
        var level = activeConfiguration.FindFirstForScene(Application.loadedLevelName, out hasMany);
        if (level != null) {
            currentLevelName = level.name;
            arguments = level.arguments;

            if (hasMany) {
                Debug.Log("Mad Level Manager: This was first scene opened. Assuming that this was '"
                + _currentLevelName + "' level, but there are many scenes with this name. " + MadLevelHelp.FAQ);
            }
            
        } else {
            Debug.LogError("Mad Level Manager: Cannot find scene " + Application.loadedLevelName
                + " in the configuration. Is the level configuration broken or wrong configuration is active?");
            currentLevelName = "";
            arguments = "";
        }
    }
    
    [System.Obsolete("Use lastPlayedLevelName instead.")]
    public static string lastLevelName {
        get {
            return lastPlayedLevelName;
        }
        
        set {
            lastPlayedLevelName = value;
        }
    }
    
    /// <summary>
    /// Gets or sets the name of the last visited level (of any type).
    /// </summary>
    /// <value>
    /// The name of the last visited level.
    /// </value>
    public static string lastPlayedLevelName {
        get; set;
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    /// <summary>
    /// Reloads current level. If this level has an extension, it will load first scene of its extension.
    /// </summary>
    public static void ReloadCurrent() {
        MadLevel.LoadLevelByName(currentLevelName);
    }
    
    /// <summary>
    /// Reloads the current level asynchronously in the background. This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation ReloadCurrentAsync() {
        return Application.LoadLevelAsync(Application.loadedLevel);
    }
    
    /// <summary>
    /// Loads level by its name defined in level configuration.
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    public static void LoadLevelByName(string levelName) {
        CheckHasConfiguration();
        var level = activeConfiguration.FindLevelByName(levelName);
        if (level != null) {
            LoadLevel(level);
        } else {
            Debug.LogError(string.Format("Level \"{0}\" not found. Please verify your configuration.", levelName));
        }
    }
    
    /// <summary>
    /// Loads level by its name defined in level configuration asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadLevelByNameAsync(string levelName) {
        CheckHasConfiguration();
        var level = activeConfiguration.FindLevelByName(levelName);
        if (level != null) {
            return LoadLevelAsync(level);
        } else {
            Debug.LogError(string.Format("Level \"{0}\" not found. Please verify your configuration.", levelName));
            return null;
        }
    }
    
    /// <summary>
    /// Determines whether there is next level present in level configuration
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if next level is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasNext() {
        CheckHasConfiguration();
        return activeConfiguration.FindNextLevel(currentLevelName) != null;
    }

    /// <summary>
    /// Determines whether there is next level present in level configuration group
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if next level is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasNextInGroup() {
        CheckHasConfiguration();
        return activeConfiguration.FindNextLevel(currentLevelName, true) != null;
    }
    
    /// <summary>
    /// Determines whether there is next level present of the specified levelType in level configuration
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if next level is present; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='levelType'>
    /// Type of next level to look after.
    /// </param>
    public static bool HasNext(Type levelType) {
        CheckHasConfiguration();
        return activeConfiguration.FindNextLevel(currentLevelName, levelType) != null;
    }

    /// <summary>
    /// Determines whether there is next level present of the specified levelType in level configuration group
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if next level is present; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='levelType'>
    /// Type of next level to look after.
    /// </param>
    public static bool HasNextInGroup(Type levelType) {
        CheckHasConfiguration();
        return activeConfiguration.FindNextLevel(currentLevelName, levelType, true) != null;
    }
    
    /// <summary>
    /// Loads next level defined in level configuration.
    /// </summary>
    public static void LoadNext() {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName);
        if (nextLevel != null) {
            LoadLevel(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level.");
        }
    }

    /// <summary>
    /// Loads next level defined in level configuration group.
    /// </summary>
    public static void LoadNextInGroup() {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName, true);
        if (nextLevel != null) {
            LoadLevel(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level.");
        }
    }
    
    /// <summary>
    /// Loads next level defined in level configuration asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadNextAsync() {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName);
        if (nextLevel != null) {
            return LoadLevelAsync(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level.");
            return null;
        }
    }

    /// <summary>
    /// Loads next level defined in level configuration group asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadNextInGroupAsync() {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName, true);
        if (nextLevel != null) {
            return LoadLevelAsync(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level.");
            return null;
        }
    }
    
    /// <summary>
    /// Loads first level with type <code>levelType</code> found after current level in level configuration.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    public static void LoadNext(Type levelType) {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName, levelType);
        if (nextLevel != null) {
            LoadLevel(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level of requested type.");
        }
    }

    /// <summary>
    /// Loads first level with type <code>levelType</code> found after current level in level configuration group.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    public static void LoadNextInGroup(Type levelType) {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName, levelType, true);
        if (nextLevel != null) {
            LoadLevel(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level of requested type.");
        }
    }

    /// <summary>
    /// Loads first level with type <code>levelType</code> found after current level in level configuration 
    /// asynchronously in the background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadNextAsync(Type levelType) {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName, levelType);
        if (nextLevel != null) {
            return LoadLevelAsync(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level of requested type.");
            return null;
        }
    }

    /// <summary>
    /// Loads first level with type <code>levelType</code> found after current level in level configuration group
    /// asynchronously in the background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadNextInGroupAsync(Type levelType) {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName, levelType, true);
        if (nextLevel != null) {
            return LoadLevelAsync(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level of requested type.");
            return null;
        }
    }
    
    /// <summary>
    /// Determines whether there is previous level present in level configuration
    /// (are there any other levels before this one?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if previous level is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasPrevious() {
        CheckHasConfiguration();
        return activeConfiguration.FindPreviousLevel(currentLevelName) != null;
    }

    /// <summary>
    /// Determines whether there is previous level present in current level configuration group
    /// (are there any other levels before this one?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if previous level is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasPreviousInGroup() {
        CheckHasConfiguration();
        return activeConfiguration.FindPreviousLevel(currentLevelName, true) != null;
    }
    
    /// <summary>
    /// Determines whether there is previous level present of the specified levelType in level configuration
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if previous level is present; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='levelType'>
    /// Type of previous level to look after.
    /// </param>
    public static bool HasPrevious(Type levelType) {
        CheckHasConfiguration();
        return activeConfiguration.FindPreviousLevel(currentLevelName, levelType) != null;
    }

    /// <summary>
    /// Determines whether there is previous level present of the specified levelType in level configuration group
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if previous level is present; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='levelType'>
    /// Type of previous level to look after.
    /// </param>
    public static bool HasPreviousInGroup(Type levelType) {
        CheckHasConfiguration();
        return activeConfiguration.FindPreviousLevel(currentLevelName, levelType, true) != null;
    }

    /// <summary>
    /// Loads previous level defined in level configuration.
    /// </summary>
    public static void LoadPrevious() {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName);
        if (previousLevel != null) {
            LoadLevel(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level.");
        }
    }

    /// <summary>
    /// Loads previous level defined in level configuration group.
    /// </summary>
    public static void LoadPreviousInGroup() {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName, true);
        if (previousLevel != null) {
            LoadLevel(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level.");
        }
    }
    
    /// <summary>
    /// Loads previous level defined in level configuration asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadPreviousAsync() {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName);
        if (previousLevel != null) {
            return LoadLevelAsync(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level.");
            return null;
        }
    }

    /// <summary>
    /// Loads previous level defined in level configuration group asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadPreviousInGroupAsync() {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName, true);
        if (previousLevel != null) {
            return LoadLevelAsync(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level.");
            return null;
        }
    }
    
    /// <summary>
    /// Loads first level with type <code>levelType</code> found before current level in level configuration.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    public static void LoadPrevious(Type levelType) {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName, levelType);
        if (previousLevel != null) {
            LoadLevel(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level of requested type.");
        }
    }

    /// <summary>
    /// Loads first level with type <code>levelType</code> found before current level in level configuration group.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    public static void LoadPreviousInGroup(Type levelType) {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName, levelType, true);
        if (previousLevel != null) {
            LoadLevel(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level of requested type.");
        }
    }
    
    /// <summary>
    /// Loads first level with type <code>levelType</code> found before current level in level configuration
    /// asynchronously in the background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadPreviousAsync(Type levelType) {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName, levelType);
        if (previousLevel != null) {
            return LoadLevelAsync(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level of requested type.");
            return null;
        }
    }

    /// <summary>
    /// Loads first level with type <code>levelType</code> found before current level in level configuration group
    /// asynchronously in the background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadPreviousInGroupAsync(Type levelType) {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName, levelType, true);
        if (previousLevel != null) {
            return LoadLevelAsync(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level of requested type.");
            return null;
        }
    }
    
    /// <summary>
    /// Tells if there is at least one level set in active level configuration.
    /// </summary>
    /// <returns>
    /// <c>true</c> if there is at least one level configured; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFirst() {
        return activeConfiguration.LevelCount() != 0;
    }

    /// <summary>
    /// Tells if there is at least one level set in current group.
    /// </summary>
    /// <param name="groupName">Name of a group.</param>
    /// <returns>
    /// <c>true</c> if there is at least one level configured; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFirstInGroup() {
        return HasFirstInGroup(currentGroupName);
    }

    /// <summary>
    /// Tells if there is at least one level set in active level configuration group.
    /// </summary>
    /// <param name="groupName">Name of a group.</param>
    /// <returns>
    /// <c>true</c> if there is at least one level configured; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFirstInGroup(string groupName) {
        var group = activeConfiguration.FindGroupByName(groupName);
        if (group == null) {
            Debug.LogError("There's no group named " + groupName);
            return false;
        }

        var level = activeConfiguration.GetLevel(group.id, 0);
        return level != null;
    }
    
    /// <summary>
    /// Tells if there is at least one level of type <code>levelType</code> set in active level configuration.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to find.
    /// </param>
    /// <returns>
    /// <c>true</c> if there is at least one level of given type configured; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFirst(Type levelType) {
        return activeConfiguration.LevelCount(levelType) != 0;
    }

    /// <summary>
    /// Tells if there is at least one level of type <code>levelType</code> set in current group.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to find.
    /// </param>
    /// <returns>
    /// <c>true</c> if there is at least one level of given type configured; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFirstInGroup(Type levelType) {
        return HasFirstInGroup(levelType, currentGroupName);
    }

    /// <summary>
    /// Tells if there is at least one level of type <code>levelType</code> set in active level configuration group.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to find.
    /// </param>
    /// <returns>
    /// <c>true</c> if there is at least one level of given type configured; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFirstInGroup(Type levelType, string groupName) {
        var group = activeConfiguration.FindGroupByName(groupName);
        if (group == null) {
            Debug.LogError("There's no group named " + groupName);
            return false;
        }

        var level = activeConfiguration.GetLevel(levelType, group.id, 0);
        return level != null;
    }
    
    /// <summary>
    /// Loads the first level set in level configuration.
    /// </summary>
    public static void LoadFirst() {
        if (activeConfiguration.LevelCount() != 0) {
            var firstLevel = activeConfiguration.GetLevel(0);
            LoadLevel(firstLevel);
        } else {
            Debug.LogError("Cannot load first level: there's no levels defined");
        }
    }

    /// <summary>
    /// Loads the first level set in current group.
    /// </summary>
    public static void LoadFirstInGroup() {
        LoadFirstInGroup(currentGroupName);
    }

    /// <summary>
    /// Loads the first level set in level configuration group.
    /// </summary>
    public static void LoadFirstInGroup(string groupName) {
        var group = activeConfiguration.FindGroupByName(groupName);
        if (group == null) {
            Debug.LogError("There's no group named " + groupName);
            return;
        }

        var level = activeConfiguration.GetLevel(group.id, 0);
        if (level != null) {
            LoadLevel(level);
        } else {
            Debug.LogError("Cannot load first level: there are no levels defined in this group");
        }
    }
    
    /// <summary>
    /// Loads the first level set in level configuration asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadFirstAsync() {
        if (activeConfiguration.LevelCount() != 0) {
            var firstLevel = activeConfiguration.GetLevel(0);
            return LoadLevelAsync(firstLevel);
        } else {
            Debug.LogError("Cannot load first level: there are no levels defined");
            return null;
        }
    }

    /// <summary>
    /// Loads the first level set in current group asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadFirstInGroupAsync() {
        return LoadFirstInGroupAsync(currentGroupName);
    }

    /// <summary>
    /// Loads the first level set in level configuration group asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadFirstInGroupAsync(string groupName) {
        var group = activeConfiguration.FindGroupByName(groupName);
        if (group == null) {
            Debug.LogError("There's no group named " + groupName);
            return null;
        }

        var level = activeConfiguration.GetLevel(group.id, 0);
        if (level != null) {
            return LoadLevelAsync(level);
        } else {
            Debug.LogError("Cannot load first level: there are no levels defined in that group");
            return null;
        }
    }
    
    /// <summary>
    /// Loads the first level of type <code>levelType</code> set in level configuration.
    /// </summary>
    /// <param name='levelType'>
    /// Level type.
    /// </param>
    public static void LoadFirst(Type levelType) {
        if (activeConfiguration.LevelCount(levelType) != 0) {
            var firstLevel = activeConfiguration.GetLevel(levelType, 0);
            LoadLevel(firstLevel);
        } else {
            Debug.LogError("Cannot load first level: there's no level of type " + levelType);
        }
    }

    /// <summary>
    /// Loads the first level of type <code>levelType</code> set in current group.
    /// </summary>
    /// <param name='levelType'>
    /// Level type.
    /// </param>
    public static void LoadFirstInGroup(Type levelType) {
        LoadFirstInGroupAsync(levelType, currentGroupName);
    }

    /// <summary>
    /// Loads the first level of type <code>levelType</code> set in level configuration group.
    /// </summary>
    /// <param name='levelType'>
    /// Level type.
    /// </param>
    public static void LoadFirstInGroup(Type levelType, string groupName) {
        var group = activeConfiguration.FindGroupByName(groupName);
        if (group == null) {
            Debug.LogError("There's no group named " + groupName);
            return;
        }

        var level = activeConfiguration.GetLevel(levelType, group.id, 0);
        if (level != null) {
            LoadLevel(level);
        } else {
            Debug.LogError("Cannot find requested level.");
        }
    }
    
    /// <summary>
    /// Loads the first level of type <code>levelType</code> set in level configuration asynchronously in the
    /// background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadFirstAsync(Type levelType) {
        if (activeConfiguration.LevelCount(levelType) != 0) {
            var firstLevel = activeConfiguration.GetLevel(levelType, 0);
            return LoadLevelAsync(firstLevel);
        } else {
            Debug.LogError("Cannot load first level: there's no level of type " + levelType);
            return null;
        }
    }

    /// <summary>
    /// Loads the first level of type <code>levelType</code> set in current group in the
    /// background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadFirstInGroupAsync(Type levelType) {
        return LoadFirstInGroupAsync(levelType, currentGroupName);
    }

    /// <summary>
    /// Loads the first level of type <code>levelType</code> set in level configuration group asynchronously in the
    /// background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadFirstInGroupAsync(Type levelType, string groupName) {
        var group = activeConfiguration.FindGroupByName(groupName);
        if (group == null) {
            Debug.LogError("There's no group named " + groupName);
            return null;
        }

        var level = activeConfiguration.GetLevel(levelType, group.id, 0);
        if (level != null) {
            return LoadLevelAsync(level);
        } else {
            Debug.LogError("Cannot find requested level");
            return null;
        }
    }

    /// <summary>
    /// When game is currently in a level with an extension set, this method will tell if the level can be
    /// continued using the MadLevel.Continue() method.
    /// </summary>
    /// <returns></returns>
    public static bool CanContinue() {
        if (currentExtension == null) {
            Debug.LogWarning("CanContinue() should be called only within levels with extensions.");
            return false;
        }

        var currentLevel = activeConfiguration.FindLevelByName(currentLevelName);
        return currentExtension.CanContinue(currentLevel, currentExtensionProgress);
    }

    /// <summary>
    /// When game is currently in a level with an extension set, this method will load the next scene defined by that
    /// extension (if exists). Remember to always check if there is a possibility of continuation using MadLevel.CanContinue()
    /// </summary>
    public static void Continue() {
        if (currentExtension == null) {
            Debug.LogWarning("Continue() should be called only within levels with extensions.");
            return;
        }

        var currentLevel = activeConfiguration.FindLevelByName(currentLevelName);
        currentExtension.Continue(currentLevel, currentExtensionProgress);
    }

    /// <summary>
    /// When game is currently in a level with an extension set, this method will load the next scene defined by that
    /// extension (if exists). Remember to always check if there is a possibility of continuation using MadLevel.CanContinue().
    /// This is an async operation and will return AsyncOperation object.
    /// </summary>
    public static AsyncOperation ContinueAsync() {
        if (currentExtension == null) {
            Debug.LogWarning("Continue() should be called only within levels with extensions.");
            return null;
        }

        var currentLevel = activeConfiguration.FindLevelByName(currentLevelName);
        return currentExtension.ContinueAsync(currentLevel, currentExtensionProgress);
    }
    
    /// <summary>
    /// Gets the all defined level names.
    /// </summary>
    /// <returns>The all defined level names.</returns>
    public static string[] GetAllLevelNames(string group = null) {
        CheckHasConfiguration();

        List<string> result = new List<string>();

        for (int i = 0; i < activeConfiguration.levels.Count; ++i) {
            var level = activeConfiguration.levels[i];
            if (group != null && level.group.name != group) {
                continue;
            }

            result.Add(level.name);
        }

        return result.ToArray();
    }
    
    /// <summary>
    /// Gets the all defined level names of given type.
    /// </summary>
    /// <returns>The all defined level names of given type.</returns>
    public static string[] GetAllLevelNames(MadLevel.Type type, string group = null) {
        CheckHasConfiguration();
        var output = new List<string>();
        for (int i = 0; i < activeConfiguration.levels.Count; ++i) {
            var level = activeConfiguration.levels[i];
            if (group != null && level.group.name != group) {
                continue;
            }

            if (level.type == type) {
                output.Add(level.name);
            }
        }
        
        return output.ToArray();
    }

    /// <summary>
    /// Finds the first completed level (in the order).
    /// </summary>
    /// <returns>The first completed level name or <code>null</code> if there's no level
    /// that is marked as completed.</returns>
    public static string FindFirstCompletedLevelName() {
        return FindFirstCompletedLevelName(null);
    }

    /// <summary>
    /// Finds the first completed level (in the order).
    /// </summary>
    /// <returns>The first completed level name or <code>null</code> if there's no level
    /// that is marked as completed.</returns>
    public static string FindFirstCompletedLevelName(string groupName) {
        return FindFirstLevelName(groupName, (level) => MadLevelProfile.IsCompleted(level.name));
    }

    /// <summary>
    /// Finds the last completed level (in the order).
    /// </summary>
    /// <returns>The last completed level name or <code>null</code> if there's no level
    /// that is marked as completed.</returns>
    public static string FindLastCompletedLevelName() {
        return FindLastCompletedLevelName(null);
    }

    /// <summary>
    /// Finds the last completed level (in the order).
    /// </summary>
    /// <returns>The last completed level name or <code>null</code> if there's no level
    /// that is marked as completed.</returns>
    public static string FindLastCompletedLevelName(string groupName) {
        return FindLastLevelName(groupName, (level) => MadLevelProfile.IsCompleted(level.name));
    }

    /// <summary>
    /// Finds the first locked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindFirstLockedLevelName() {
        return FindFirstLockedLevelName(null);
    }

    /// <summary>
    /// Finds the first locked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindFirstLockedLevelName(string groupName) {
        return FindFirstLevelName(groupName, (level) => MadLevelProfile.IsLocked(level.name));
    }

    /// <summary>
    /// Finds the last locked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The last locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindLastLockedLevelName() {
        return FindLastLockedLevelName(null);
    }

    /// <summary>
    /// Finds the last locked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The last locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindLastLockedLevelName(string groupName) {
        return FindLastLevelName(groupName, (level) => MadLevelProfile.IsLocked(level.name));
    }

    /// <summary>
    /// Finds the first unlocked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindFirstUnlockedLevelName() {
        return FindFirstUnlockedLevelName(null);
    }

    /// <summary>
    /// Finds the first unlocked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindFirstUnlockedLevelName(string groupName) {
        LayoutUninitializedCheck();

        return FindFirstLevelName(groupName, (level) => {
            if (MadLevelProfile.IsLockedSet(level.name)) {
                return !MadLevelProfile.IsLocked(level.name);
            } else {
                return !level.lockedByDefault;
            }
        });
    }

    /// <summary>
    /// Finds the last unlocked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindLastUnlockedLevelName() {
        return FindLastUnlockedLevelName(null);
    }

    /// <summary>
    /// Finds the last unlocked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindLastUnlockedLevelName(string groupName) {
        LayoutUninitializedCheck();

        return FindLastLevelName(groupName, (level) => {
            if (MadLevelProfile.IsLockedSet(level.name)) {
                return !MadLevelProfile.IsLocked(level.name);
            } else {
                return !level.lockedByDefault;
            }
        });
    }

    private static void LayoutUninitializedCheck() {
        if (GameObject.Find("/Mad Level Root") != null) {
            // cheap way of checking if we are on level select screen
            var layout = MadLevelLayout.current;
            if (layout != null && !layout.fullyInitialized) {
                Debug.LogWarning(
                    "This operation have unexpected behavior when executed so soon. Please move it to LateUpdate().");
            }
        }
    }

    /// <summary>
    /// Finds the first name of the level, that predicate returns <code>true</code> value.
    /// </summary>
    /// <returns>The first found level or <code>null<code> if not found.</returns>
    /// <param name="predicate">The predicate.</param>
    public static string FindFirstLevelName(string groupName, LevelPredicate predicate) {
        CheckHasConfiguration();

        MadLevelConfiguration.Group group = null;

        if (groupName != null) {
            group = activeConfiguration.FindGroupByName(groupName);
            if (group == null) {
                Debug.LogError("Cannot find group named " + groupName);
                return null;
            }
        }

        var levels = activeConfiguration.GetLevelsInOrder();
        for (int i = 0; i < levels.Length; i++) {
            var level = levels[i];

            if (group != null && level.groupId != group.id) {
                continue;
            }

            if (predicate(level)) {
                return level.name;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds the first name of the level, that predicate returns <code>true</code> value.
    /// </summary>
    /// <returns>The first found level or <code>null<code> if not found.</returns>
    /// <param name="predicate">The predicate.</param>
    public static string FindFirstLevelName(LevelPredicate predicate) {
        return FindFirstLevelName(null, predicate);
    }

    /// <summary>
    /// Finds the first name of the level of given <code>type</code>.
    /// </summary>
    /// <returns>The first found level or <code>null<code> if not found.</returns>
    /// <param name="levelType">The level type.</param>
    public static string FindFirstLevelName(MadLevel.Type levelType, string groupName) {
        return FindFirstLevelName(groupName, (level) => level.type == levelType);
    }

    /// <summary>
    /// Finds the first name of the level of given <code>type</code>.
    /// </summary>
    /// <returns>The first found level or <code>null<code> if not found.</returns>
    /// <param name="levelType">The level type.</param>
    public static string FindFirstLevelName(MadLevel.Type levelType) {
        return FindFirstLevelName(null, (level) => level.type == levelType);
    }

    /// <summary>
    /// Finds the last name of the level, that predicate returns <code>true</code> value.
    /// </summary>
    /// <returns>The last found level or <code>null<code> if not found.</returns>
    /// <param name="predicate">The predicate.</param>
    public static string FindLastLevelName(string groupName, LevelPredicate predicate) {
        CheckHasConfiguration();

        MadLevelConfiguration.Group group = null;
        if (groupName != null) {
            group = activeConfiguration.FindGroupByName(groupName);
            if (group == null) {
                Debug.LogError("Cannot find group named " + groupName);
                return null;
            }
        }

        var levels = activeConfiguration.GetLevelsInOrder();
        for (int i = levels.Length - 1; i >= 0; i--) {
            var level = levels[i];

            if (group != null && level.groupId != group.id) {
                continue;
            }

            if (predicate(level)) {
                return level.name;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds the last name of the level, that predicate returns <code>true</code> value.
    /// </summary>
    /// <returns>The last found level or <code>null<code> if not found.</returns>
    /// <param name="predicate">The predicate.</param>
    public static string FindLastLevelName(LevelPredicate predicate) {
        return FindLastLevelName(null, predicate);
    }

    /// <summary>
    /// Finds the last name of the level of given <code>type</code>.
    /// </summary>
    /// <returns>The last found level or <code>null<code> if not found.</returns>
    /// <param name="levelType">The level type.</param>
    public static string FindLastLevelName(MadLevel.Type levelType, string groupName) {
        return FindLastLevelName(groupName, (level) => level.type == levelType);
    }

    /// <summary>
    /// Finds the last name of the level of given <code>type</code>.
    /// </summary>
    /// <returns>The last found level or <code>null<code> if not found.</returns>
    /// <param name="levelType">The level type.</param>
    public static string FindLastLevelName(MadLevel.Type levelType) {
        return FindLastLevelName(null, (level) => level.type == levelType);
    }
    
    /// <summary>
    /// Gets the name of the previous level.
    /// </summary>
    /// <returns>The previous level name or <code>null<code> if not found.</returns>
    public static string GetPreviousLevelName() {
        CheckHasConfiguration();
        
        var level = activeConfiguration.FindPreviousLevel(currentLevelName);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of the previous level of given type.
    /// </summary>
    /// <returns>The previous level name or <code>null</code> if not found.</returns>
    /// <param name="type">Level type.</param>
    public static string GetPreviousLevelName(MadLevel.Type type) {
        CheckHasConfiguration();
        
        var level = activeConfiguration.FindPreviousLevel(currentLevelName, type);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of a level that exists before given level.
    /// </summary>
    /// <returns>The name of previous level or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    public static string GetPreviousLevelNameTo(string levelName) {
        CheckHasConfiguration();
        CheckLevelExists(levelName);
        
        var level = activeConfiguration.FindPreviousLevel(levelName);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of a level (with given type) that exists before given level.
    /// </summary>
    /// <returns>The name of previous level or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    /// <param name="type">Type of previous level.</param>
    public static string GetPreviousLevelNameTo(string levelName, MadLevel.Type type) {
        CheckHasConfiguration();
        CheckLevelExists(levelName);
        
        var level = activeConfiguration.FindPreviousLevel(levelName, type);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of the next level.
    /// </summary>
    /// <returns>The next level name or <code>null</code> if not found.</returns>
    public static string GetNextLevelName() {
        CheckHasConfiguration();
        
        var level = activeConfiguration.FindNextLevel(currentLevelName);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of the next level of given type.
    /// </summary>
    /// <returns>The next level name or <code>null</code> if not found.</returns>
    /// <param name="type">Level type.</param>
    public static string GetNextLevelName(MadLevel.Type type) {
        CheckHasConfiguration();
        
        var level = activeConfiguration.FindNextLevel(currentLevelName, type);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of a level that exists after given level.
    /// </summary>
    /// <returns>The name of next level or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    public static string GetNextLevelNameTo(string levelName) {
        CheckHasConfiguration();
        CheckLevelExists(levelName);
        
        var level = activeConfiguration.FindNextLevel(levelName);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of a level (with given type) that exists after given level.
    /// </summary>
    /// <returns>The name of next level or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    /// <param name="type">Type of previous level.</param>
    public static string GetNextLevelNameTo(string levelName, MadLevel.Type type) {
        CheckHasConfiguration();
        CheckLevelExists(levelName);
        
        var level = activeConfiguration.FindNextLevel(levelName, type);
        return level != null ? level.name : null;
    }
    
    static void CheckLevelExists(string levelName) {
        var level = activeConfiguration.FindLevelByName(levelName);
        MadDebug.Assert(level != null, "There's no level with name '" + levelName + "'");
    }
    
    static void CheckHasConfiguration() {
        MadDebug.Assert(hasActiveConfiguration,
            "This method may only be used when level configuration is set. Please refer to "
            + MadLevelHelp.ConfigurationWiki);
    }

    static void LoadLevel(MadLevelConfiguration.Level level) {
        currentExtension = null; // loading level that way resets the extension

        if (level.hasExtension) {
            var extension = level.extension;
            extension.Load(level);
        } else {
            level.Load();
        }
    }

    static AsyncOperation LoadLevelAsync(MadLevelConfiguration.Level level) {
        currentExtension = null; // loading level that way resets the extension

        if (level.hasExtension) {
            var extension = level.extension;
            return extension.LoadAsync(level);
        } else {
            return level.LoadAsync();
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum Type {
        Other,
        Level,
        Extra,
    }
    
    public delegate bool LevelPredicate(MadLevelConfiguration.Level level);

}

} // namespace