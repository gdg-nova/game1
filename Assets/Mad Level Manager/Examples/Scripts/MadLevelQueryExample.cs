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

public class MadLevelQueryExample : MonoBehaviour {

    #region Fields

    private MadText text;

    #endregion

    #region Slots

    void Start() {
        text = GetComponent<MadText>();

        int levelNumber = new MadLevelQuery()
            .ForGroup(MadLevel.currentGroupName)
            .OfLevelType(MadLevel.Type.Level)
            .CountLevels();

        int unlocked = new MadLevelQuery()
            .ForGroup(MadLevel.currentGroupName)
            .OfLevelType(MadLevel.Type.Level)
            .CountUnlocked();

        int starsTotal = new MadLevelQuery()
            .ForGroup(MadLevel.currentGroupName)
            .OfLevelType(MadLevel.Type.Level)
            .SelectProperty("star_1", "star_2", "star_3")
            .CountProperties();

        int starsGained = new MadLevelQuery()
            .ForGroup(MadLevel.currentGroupName)
            .OfLevelType(MadLevel.Type.Level)
            .SelectProperty("star_1", "star_2", "star_3")
            .CountEnabled();

        text.text = "Levels: " + levelNumber + ", Unlocked: " + unlocked + "\nStars: " + starsTotal + ", Acquired: " + starsGained;
    }

    #endregion

    #region Public Static Methods
    #endregion

    #region Inner and Anonymous Classes
    #endregion
}