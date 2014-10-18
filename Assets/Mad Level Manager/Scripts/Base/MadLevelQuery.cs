/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadLevelManager {

public class MadLevelQuery {

    #region Public Fields

    private Selector selector = Selector.All;

    private string[] selectorGroupName;
    private string[] selectorLevelName;

    private bool hasLevelType;
    private MadLevel.Type levelType;

    private string[] propertyName;

    #endregion

    #region Public Properties
    #endregion

    #region Public Methods

    public MadLevelQuery ForAll() {
        selector = Selector.All;
        return this;
    }

    public MadLevelQuery ForGroup(params string[] groupName) {
        selectorGroupName = groupName;
        selector = Selector.Groups;
        return this;
    }

    public MadLevelQuery ForLevel(params string[] levelName) {
        selectorLevelName = levelName;
        selector = Selector.Levels;
        return this;
    }

    public MadLevelQuery OfLevelType(MadLevel.Type levelType) {
        hasLevelType = true;
        this.levelType = levelType;
        return this;
    }

    public MadLevelQuery SelectProperty(params string[] propertyName) {
        this.propertyName = propertyName;
        return this;
    }

    public int CountLevels() {
        return GetLevelNames().Count;
    }

    public void SetLocked(bool val) {
        var levelNames = GetLevelNames();
        for (int i = 0; i < levelNames.Count; ++i) {
            var levelName = levelNames[i];
            MadLevelProfile.SetLocked(levelName, val);
        }
    }

    public void SetCompleted(bool val) {
        var levelNames = GetLevelNames();
        for (int i = 0; i < levelNames.Count; ++i) {
            var levelName = levelNames[i];
            MadLevelProfile.SetCompleted(levelName, val);
        }
    }

    public int CountProperties() {
        if (propertyName == null || propertyName.Length == 0) {
            Debug.LogError("Missing SelectProperty() directive");
            return 0;
        }

        return GetLevelNames().Count * propertyName.Length;
    }

    public int CountEnabled() {
        return Count(bool.TrueString);
    }

    public int CountDisabled() {
        return Count(bool.FalseString);
    }

    public void SetEnabled() {
        SetBoolean(true);
    }

    public void SetDisabled() {
        SetBoolean(false);
    }

    public void SetBoolean(bool val) {
        ProcessProperties((levelName, propertyName, v) => {
            MadLevelProfile.SetLevelBoolean(levelName, propertyName, val);
        });
    }

    public int CountLocked() {
        var levelNames = GetLevelNames();
        int result = 0;

        var layout = MadLevelLayout.TryGet();

        for (int i = 0; i < levelNames.Count; ++i) {
            var levelName = levelNames[i];

            // first look for a layout, because icons locked state are more trustworthy
            if (layout != null) {
                var icon = layout.GetIcon(levelName);
                if (icon != null) {
                    if (icon.locked) {
                        result++;
                    }

                    continue;
                }
            }

            // no layout or no icon
            if (MadLevelProfile.IsLockedSet(levelName)) {
                if (MadLevelProfile.IsLocked(levelName)) {
                    result++;
                }
            } else {
                var lockedByDefault = MadLevel.activeConfiguration.FindLevelByName(levelName).lockedByDefault;
                if (lockedByDefault) {
                    result++;
                }
            }
        }

        return result;
    }

    public int CountUnlocked() {
        return CountLevels() - CountLocked();
    }

    public int CountCompleted() {
        var levelNames = GetLevelNames();

        int result = 0;
        for (int i = 0; i < levelNames.Count; ++i) {
            if (MadLevelProfile.IsCompleted(levelNames[i])) {
                result++;
            }
        }

        return result;
    }

    public int CountNotCompleted() {
        return CountLevels() - CountCompleted();
    }

    public int SumIntegers() {
        int sum = 0;
        ProcessProperties((ln, pn, value) => {
            int result;
            if (int.TryParse(value, out result)) {
                sum += result;
            } else {
                Debug.LogError("Cannot parse property value '" + value + "' to integer.");
            }
        });

        return sum;
    }

    public float SumFloats() {
        float sum = 0;

        ProcessProperties((ln, pn, value) => {
            float result;
            if (float.TryParse(value, out result)) {
                sum += result;
            } else {
                Debug.LogError("Cannot parse property value '" + value + "' to float.");
            }
        });

        return sum;
    }

    #endregion

    #region Private Methods

    private int Count(string val) {
        int result = 0;

        bool done = ProcessProperties((ln, pn, value) => {
            if (value == val) {
                result++;
            }
        });

        if (!done) {
            return 0;
        } else {
            return result;
        }
    }

    private bool ProcessProperties(PropertyProcessor processor) {
        var levelNames = GetLevelNames();
        if (propertyName == null || propertyName.Length == 0) {
            Debug.LogError("Missing SelectProperty() directive");
            return false;
        }

        for (int i = 0; i < levelNames.Count; ++i) {
            var ln = levelNames[i];
            for (int j = 0; j < propertyName.Length; ++j) {
                var pn = propertyName[j];
                if (!MadLevelProfile.IsLevelPropertySet(ln, pn)) {
                    processor(ln, pn, null);
                } else {
                    processor(ln, pn, MadLevelProfile.GetLevelAny(ln, pn));
                }
            }
        }

        return true;
    }

    private List<string> GetLevelNames() {
        var conf = MadLevel.activeConfiguration;
        switch (selector) {
            case Selector.All:
                if (!hasLevelType) {
                    return (from l in conf.levels select l.name).ToList();
                } else {
                    return (from l in conf.levels where l.type == levelType select l.name).ToList();
                }
            case Selector.Groups:
                if (!hasLevelType) {
                    return (from l in conf.levels 
                            where Array.IndexOf(selectorGroupName, l.GetGroup().name) != -1 
                            select l.name).ToList();
                } else {
                    return (from l in conf.levels
                            where l.type == levelType && Array.IndexOf(selectorGroupName, l.GetGroup().name) != -1
                            select l.name).ToList();
                }
            case Selector.Levels:
                if (!hasLevelType) {
                    return (from l in conf.levels 
                            where Array.IndexOf(selectorLevelName, l.name) != -1 
                            select l.name).ToList();
                } else {
                    return (from l in conf.levels
                            where l.type == levelType && Array.IndexOf(selectorLevelName, l.name) != -1
                            select l.name).ToList();
                }
            default:
                Debug.LogError("Unknown selector: " + selector);
                return new List<string>();
        }
    }

    #endregion

    #region Inner and Anonymous Classes

    private enum Selector {
        All,
        Groups,
        Levels,
    }

    private delegate void PropertyProcessor(string level, string name, string propertyValue);

    #endregion
}

} // namespace