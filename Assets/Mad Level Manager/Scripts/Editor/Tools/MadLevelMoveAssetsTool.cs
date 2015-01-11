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

public class MadLevelMoveAssetsTool {

    #region Unity Methods

    public static void Execute() {
        if (!string.IsNullOrEmpty(AssetDatabase.ValidateMoveAsset("Assets/Mad Level Manager/Scripts/Base", "Assets/Mad Level Manager/Base"))) {
            EditorUtility.DisplayDialog("Already Converted?", "It looks like your project is already prepared, or you have moved Mad Level Manager asset. Maybe try to delete it and import again?", "Oh...");
            return;
        }

        if (EditorUtility.DisplayDialog("Convert?", "This action will reorganize Mad Level Manager directory to make it "
            + "accessible from UnityScript and Boo languages. You cannot undo this action, but you can "
            + "delete Mad Level Manager directory and reimport it again if you want go back to the "
            + "previous state. Continue?", "Convert", "Cancel")) {

            AssetDatabase.CreateFolder("Assets", "Standard Assets");
            AssetDatabase.CreateFolder("Assets/Standard Assets", "Mad Level Manager");
            AssetDatabase.CreateFolder("Assets/Standard Assets/Mad Level Manager", "Scripts");

            AssetDatabase.MoveAsset("Assets/Mad Level Manager/Scripts/Base", "Assets/Standard Assets/Mad Level Manager/Scripts/Base");
            AssetDatabase.MoveAsset("Assets/Mad Level Manager/Scripts/Mad2D", "Assets/Standard Assets/Mad Level Manager/Scripts/Mad2D");
            AssetDatabase.MoveAsset("Assets/Mad Level Manager/Scripts/MadCommons", "Assets/Standard Assets/Mad Level Manager/Scripts/MadCommons");

            AssetDatabase.CreateFolder("Assets/Mad Level Manager/Scripts", "Mad2D");
            AssetDatabase.MoveAsset("Assets/Standard Assets/Mad Level Manager/Scripts/Mad2D/Editor", "Assets/Mad Level Manager/Scripts/Mad2D/Editor");

            AssetDatabase.CreateFolder("Assets/Mad Level Manager/Scripts", "MadCommons");
            AssetDatabase.MoveAsset("Assets/Standard Assets/Mad Level Manager/Scripts/MadCommons/Editor", "Assets/Mad Level Manager/Scripts/MadCommons/Editor");

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Done!", "It's done! Please remember to remove \"Mad Level Manager\" and \"Standard Assets/Mad Level Manager\" directories before upgrading Mad Level Manager!", "Got it!");
        }
    }

    #endregion

}

#if !UNITY_3_5
} // namespace
#endif