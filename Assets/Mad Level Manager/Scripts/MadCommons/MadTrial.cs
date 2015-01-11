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

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadTrial {

    public const string TRIAL_DURATION = "_TRIAL_DURATION_"; // in seconds
    //public const string TRIAL_DURATION = "100";

    public static bool isTrialVersion {
        get {
            return !TRIAL_DURATION.StartsWith("_");
        }
    }

    public static void InfoLabel(string text) {
        var label = new GUIContent(text);
        var size = GUI.skin.label.CalcSize(label);

        GUI.color = Color.black;
        GUI.Label(new Rect(Screen.width - size.x - 5 + 1, Screen.height - size.y - 5 + 1, size.x, size.y), label);

        GUI.color = Color.white;
        GUI.Label(new Rect(Screen.width - size.x - 5, Screen.height - size.y - 5, size.x, size.y), label);
    }
}

#if !UNITY_3_5
} // namespace
#endif