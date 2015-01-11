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

// helper for making updates
public class MadUpdater : AssetPostprocessor {

    #region Check Methods

    protected static void AssetMoved(string id, string from, string to) {
#if !MAD_UPDATE_DEBUG
        if (IsDone(id)) {
            return;
        }
#endif

        MarkDone(id);

        string message = AssetDatabase.MoveAsset(from, to);
        if (string.IsNullOrEmpty(message)) {
            Log("Moved " + from + " to " + to);
            AssetDatabase.Refresh();
        } else {
#if MAD_UPDATE_DEBUG
            Debug.Log(message);
#endif
        }
    }

    private static void Log(string message) {
        Debug.Log("Upgrade info (This message is NOT harmful): " + message);
    }

    private static bool IsDone(string id) {
        return EditorPrefs.HasKey(id);
    }

    private static void MarkDone(string id) {
        EditorPrefs.SetBool(id, true);
    }

    protected static void Reset(string id) {
        EditorPrefs.DeleteKey(id);
    }

    #endregion

    #region Unity Methods

    //static void OnPostprocessAllAssets(string[] i, string[] d, string[] m, string[] mf) {
    //}

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif