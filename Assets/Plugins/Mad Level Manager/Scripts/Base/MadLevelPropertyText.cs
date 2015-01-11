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

using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelPropertyText : MonoBehaviour {

    #region Public Fields
    #endregion

    #region Public Properties

    private MadLevelIcon _icon;
    public MadLevelIcon icon {
        get {
            if (_icon == null) {
                _icon = MadTransform.FindParent<MadLevelIcon>(transform);
            }

            return _icon;
        }
    }

    #endregion

    #region Slots

    void Start() {
        var text = GetComponent<MadText>();
        text.text = MadLevelProfile.GetLevelAny(icon.level.name, name, text.text);
    }

    #endregion

    #region Public Static Methods
    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

#if !UNITY_3_5
} // namespace
#endif