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

public class MadLevelUpdater : MadUpdater {

    #region Constants

    private const string Prefix = "MadLevelManager_";
    private const string Root = "Assets/Mad Level Manager";

    #endregion

    #region Methods

    public static void OnPostprocessAllAssets(string[] i, string[] d, string[] m, string[] mf) {
        if (i.Length == 0) {
            return;
        }

        AssetMoved(Prefix + "grid.png_2.1.0",
            Root + "/Editor/Resources/MadLevelManager/Textures/grid.png",
            Root + "/Scripts/Mad2D/Editor/Resources/Mad2D/Textures/grid.png");
            
        // script move and rename
        AssetMoved(Prefix + "MadLevelKeyboardControl_Move_2.1.2",
            Root + "/Examples/Scripts/MadLevelKeyboardControl.cs",
            Root + "/Scripts/Base/MadLevelInputControl.cs");
            
        AssetMoved(Prefix + "MadLevelKeyboardControl_Rename_2.1.2",
            Root + "/Examples/Scripts/MadLevelInputControl.cs",
            Root + "/Scripts/Base/MadLevelInputControl.cs");
    }

    public static void ResetUpdateData() {
        Reset(Prefix + "grid.png");
        EditorUtility.DisplayDialog("Update Data removed!", "Don't worry if you did it by accident :-)", "OK");
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif