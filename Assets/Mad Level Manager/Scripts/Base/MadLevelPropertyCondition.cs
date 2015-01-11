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

public class MadLevelPropertyCondition : MonoBehaviour {

    #region Public Fields

    public PropertyType propertyType = PropertyType.Completed;

    public string customPropertyName;

    public Comparer comparer = Comparer.IsEqual;

    public string rightSideValue = "";

    public Action action = Action.Show;

    //public Action defaultAction = Action.Hide;

    #endregion

    #region Private Fields

    private MadSprite sprite;

    private MadLevelIcon icon;

    #endregion

    #region Slots

    void Start() {
        sprite = GetComponent<MadSprite>();
        if (sprite == null) {
            Debug.LogError("Condition needs a MadSprite to be attached to this game object.", this);
            return;
        }

        icon = MadTransform.FindParent<MadLevelIcon>(transform);
        if (icon == null) {
            Debug.LogError("Condition need to be set under MadLevelIcon.", this);
            return;
        }

        Apply();
    }

    #endregion

    #region Private Methods

    private void Apply() {
        string propertyValue = GetPropertyValue();
        if (propertyValue == null) {
            ApplyAction(Opposite(action));
        } else if (Compare(propertyValue)) {
            ApplyAction(action);
        } else {
            ApplyAction(Opposite(action));
        }
    }

    private Action Opposite(Action action) {
        switch (action) {
            case Action.Hide:
                return Action.Show;
            case Action.Show:
                return Action.Hide;
            default:
                Debug.LogError("Unknown action: " + action, this);
                return Action.Hide;
        }
    }

    private string GetPropertyValue() {
        string levelName = icon.level.name;

        switch (propertyType) {
            case PropertyType.Completed:
                return MadLevelProfile.IsCompleted(levelName).ToString();
            case PropertyType.Locked:
                return MadLevelProfile.IsLocked(levelName).ToString();
            case PropertyType.LevelNumber:
                return icon.levelNumber.text;
            case PropertyType.Custom:
                return MadLevelProfile.GetLevelAny(levelName, customPropertyName, null);
            default:
                Debug.LogError("Unknown property type: " + propertyType);
                return null;
        }
    }

    private bool Compare(string leftSideValue) {
        switch (comparer) {
            case Comparer.IsEqual:
                return leftSideValue == rightSideValue;
            case Comparer.IsNotEqual:
                return leftSideValue != rightSideValue;
            case Comparer.IsGreater:
                return CompareDoubles(leftSideValue, rightSideValue) == 1;
            case Comparer.IsGreaterOrEqual: {
                    int r = CompareDoubles(leftSideValue, rightSideValue);
                    return r == 1 || r == 0;
                }
            case Comparer.IsLower:
                return CompareDoubles(leftSideValue, rightSideValue) == -1;
            case Comparer.IsLowerOrEqual: {
                    int r = CompareDoubles(leftSideValue, rightSideValue);
                    return r == -1 || r == 0;
                }

            default:
                Debug.LogError("Unknown comparer: " + comparer);
                return false;
        }
    }

    private int CompareDoubles(string a, string b) {
        double da, db;
        if (!double.TryParse(a, out da)) {
            return -2;
        }

        if (!double.TryParse(b, out db)) {
            return -2;
        }

        if (da < db) {
            return -1;
        } else if (da > db) {
            return 1;
        }

        return 0;
    }

    private void ApplyAction(Action action) {
        switch (action) {
            case Action.Show:
                sprite.visible = true;
                break;

            case Action.Hide:
                sprite.visible = false;
                break;
        }
    }

    #endregion

    #region Inner and Anonymous Classes

    public enum PropertyType {
        Completed,
        Locked,
        LevelNumber,
        Custom,
    }

    public enum Comparer {
        IsEqual,
        IsNotEqual,
        IsGreater,
        IsGreaterOrEqual,
        IsLower,
        IsLowerOrEqual,
    }

    public enum Action {
        Show,
        Hide,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif