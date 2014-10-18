/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

#pragma warning disable 0618

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[ExecuteInEditMode]   
public class MadLevelIcon : MadSprite {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // if true then levelSceneName and levelArguments are loaded from configuration
    public bool hasLevelConfiguration;

    // level index in group
    public int levelIndex;
    
    public string levelSceneName;
    public string levelArguments;
    
    public MadLevelProperty completedProperty;
    public MadLevelProperty lockedProperty;
    public MadText levelNumber;

    public bool canFocusIfLocked = true;
    
    // list of level icons to unlock on completion of this one
    public List<MadLevelIcon> unlockOnComplete = new List<MadLevelIcon>();
    private List<MadLevelIcon> unlockOnCompleteBackRef = new List<MadLevelIcon>(); // back reference

    public List<GameObject> showWhenLevelLocked = new List<GameObject>();
    public List<GameObject> showWhenLevelUnlocked = new List<GameObject>();
    public List<GameObject> showWhenLevelNotCompleted = new List<GameObject>();
    public List<GameObject> showWhenLevelCompleted = new List<GameObject>();

    [HideInInspector]
    public int version = 0;

    private bool justEnabled;
    
    // ===========================================================
    // Properties
    // ===========================================================

    public bool isTemplate {
        get {
            if (!_isTemplateCached) {
                _isTemplate = MadTransform.FindParent<MadLevelAbstractLayout>(transform) == null;
                _isTemplateCached = true;
            }

            return _isTemplate;
        }
    }
    private bool _isTemplate;
    private bool _isTemplateCached;

    public MadLevelConfiguration configuration {
        get {
            if (_configuration == null) {
                var layout = MadTransform.FindParent<MadLevelAbstractLayout>(transform);
                if (layout != null) {
                    _configuration = layout.configuration;
                }
            }

            return _configuration;
        }

        set {
            _configuration = value;
        }
    }
    private MadLevelConfiguration _configuration;

    // level group
    public int levelGroup {
        get {
            if (_levelGroup == -1) {
                var layout = MadTransform.FindParent<MadLevelAbstractLayout>(transform);
                _levelGroup = layout.configurationGroup;
            }

            return _levelGroup;
        }

        set {
            _levelGroup = value;
        }
    }
    private int _levelGroup = -1;

    public bool generated {
        get {
            return hasLevelConfiguration;
        }
    }
    
    public bool completed {
        get {
            return MadLevelProfile.IsCompleted(level.name);
        }

        set {
            if (completedProperty != null) {
                completedProperty.propertyEnabled = value; // deprecated
            }

            if (value) {
                UnlockOnComplete();
            }

            MadLevelProfile.SetCompleted(level.name, value);

            ChangeState(showWhenLevelCompleted, value);
            ChangeState(showWhenLevelNotCompleted, !value);
        }
    }
    
    public bool locked {
        get {
            if (lockedProperty != null) {
                return lockedProperty.propertyEnabled; // deprecated
            }

            if (!justEnabled) {
                return MadLevelProfile.IsLocked(level.name);
            } else {
                // icon has been just enabled and may be unlocked in a moment
                // I will need to check manually if this icon should be treated as locked
                if (!MadLevelProfile.IsLocked(level.name)) {
                    return false;
                }

                for (int i = 0; i < unlockOnCompleteBackRef.Count; i++) {
                    var otherIcon = unlockOnCompleteBackRef[i];
                    if (otherIcon.completed) {
                        return false;
                    }
                }

                return true;
            }
        }
        
        set {
            if (!MadGameObject.IsActive(gameObject)) {
                return;
            }

            if (lockedProperty != null) {
                lockedProperty.propertyEnabled = value; // deprecated
            }

            MadLevelProfile.SetLocked(level.name, value);

            ChangeState(showWhenLevelLocked, value);
            ChangeState(showWhenLevelUnlocked, !value);

            if (!value && !isTemplate && !canFocusIfLocked) {
                var sprite = GetComponent<MadSprite>();
                sprite.eventFlags = sprite.eventFlags | MadSprite.EventFlags.Focus;
            }
        }
    }

    public List<MadLevelProperty> properties {
        get {
            return MadTransform.FindChildren<MadLevelProperty>(transform);
        }
    }
    
    /// <summary>
    /// Level refrerence. You can get here all information about the referenced level.
    /// </summary>
    /// <value>The level or null if level or configuration is not set yet.</value>
    public MadLevelConfiguration.Level level {
        get {
        	if (configuration != null) {
                    return configuration.GetLevel(MadLevel.Type.Level, levelGroup, levelIndex);
            } else {
            	return null;
            }
        }
    }    
                    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    protected override void OnEnable() {
        base.OnEnable();
        Upgrade();
        justEnabled = true;

        for (int i = 0; i < unlockOnComplete.Count; i++) {
            var otherIcon = unlockOnComplete[i];
            if (!otherIcon.unlockOnCompleteBackRef.Contains(this)) {
                otherIcon.unlockOnCompleteBackRef.Add(this);
            }
        }
    }
    
    void Upgrade() {
        if (version == 0) {
            // in free layout of 1.3.x icon names were mistaken for level names
            // check if there's profile entry for level name of icon name
            // but without level name itself
            if (MadLevelProfile.IsLevelSet(name) && !MadLevelProfile.IsLevelSet(level.name)) {
                MadLevelProfile.RenameLevel(name, level.name);
            }
        }
        
        version = 1;
    }

    protected override void Update() {
        base.Update();

        if (justEnabled) {
            if (Application.isPlaying) {
                if (isTemplate) {
                    MadGameObject.SetActive(gameObject, false);
                }

                // completed property object is optional
                // if it's not present, check the completed property manually
                if (completedProperty == null) {
                    if (level != null) {
                        completed = MadLevelProfile.IsCompleted(level.name);
                    }
                }

                onMouseUp += (sprite) => Activate();
                onTap += (sprite) => Activate();

                if (!isTemplate) {
                    if (!canFocusIfLocked && locked) {
                        var sprite = GetComponent<MadSprite>();
                        sprite.eventFlags = sprite.eventFlags & ~MadSprite.EventFlags.Focus;
                    }
                }
            }

            // init child objects visibility
            if (level != null) {
                ChangeState(showWhenLevelLocked, locked);
                ChangeState(showWhenLevelUnlocked, !locked);
                ChangeState(showWhenLevelCompleted, completed);
                ChangeState(showWhenLevelNotCompleted, !completed);
            }

            justEnabled = false;
        }
    }

    /// <summary>
    /// Activates this icon.
    /// </summary>
    public void Activate() {
        var layout = MadTransform.FindParent<MadLevelAbstractLayout>(transform);
        layout.Activate(this);
    }

    // ===========================================================
    // Methods
    // ===========================================================

    public void ApplyConnections() {
        foreach (var property in properties) {
            property.ApplyConnections();
        }
    }
    
    public MadLevelProperty.SpecialType TypeFor(MadLevelProperty property) {
        if (property == completedProperty) {
            return MadLevelProperty.SpecialType.Completed;
        }
        
        if (property == lockedProperty) {
            return MadLevelProperty.SpecialType.Locked;
        }

        if (property.gameObject == levelNumber.gameObject) { // comparing game object because of type differences
            return MadLevelProperty.SpecialType.LevelNumber;
        }
        
        return MadLevelProperty.SpecialType.Regular;
    }

    public void UpdateProperty(string propertyName, bool state) {
        var properties = GetComponentsInChildren<MadLevelProperty>();
        bool found = false;
        
        foreach (var property in properties) {
            if (property.name == propertyName) {
                property.propertyEnabled = state;
                found = true;
            }
        }
        
        if (!found) {
            Debug.LogError("Cannot find property '" + propertyName + "'", gameObject);
        }
    }
    
    public void LoadLevel() {
        if (hasLevelConfiguration) {
            var level = configuration.GetLevel(MadLevel.Type.Level, levelGroup, levelIndex);
            MadLevel.LoadLevelByName(level.name);
        } else {
            if (!string.IsNullOrEmpty(levelSceneName)) {
                MadLevelProfile.recentLevelSelected = level.name;
            
                MadLevel.currentLevelName = level.name;
                MadLevel.arguments = "";
                Application.LoadLevel(levelSceneName);
            } else {
                Debug.LogError("Level scene name not set. I don't know what to load!");
                return;
            }
        }
        
    }
    
    void OnPropertyChange(MadLevelProperty property) {
        if (property.specialType == MadLevelProperty.SpecialType.Completed) {
            UnlockOnComplete();
        }
    }
    
    void UnlockOnComplete() {
        if (unlockOnComplete != null) {
            foreach (var icon in unlockOnComplete) {
                icon.locked = false;
            }
        }
    }

    private void Enable(List<GameObject> objects) {
        ChangeState(objects, true);
    }

    private void Disable(List<GameObject> objects) {
        ChangeState(objects, true);
    }

    private void ChangeState(List<GameObject> objects, bool state) {
        for (int i = 0; i < objects.Count; ++i) {
            var obj = objects[i];
            if (obj == null) {
                continue;
            }

            obj.active = state;

            var sprite = obj.GetComponent<MadSprite>();
            if (sprite != null) {
                sprite.visible = state;
            }
        }
    }
        
    // ===========================================================
    // Message methods
    // ===========================================================
    
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