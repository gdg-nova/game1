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

public class MadAtlasUtil : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    public static string GetItemOriginPath(MadAtlas.Item item) {
        var path = AssetDatabase.GUIDToAssetPath(item.textureGUID);
        return path;
    }

    public static Texture2D GetItemOrigin(MadAtlas.Item item) {
        var path = AssetDatabase.GUIDToAssetPath(item.textureGUID);
        if (string.IsNullOrEmpty(path)) {
            return null;
        }
        
        return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
    }

    public static void AtlasField(SerializedProperty textureField, MadAtlas atlas, string label) {
        string guid = textureField.stringValue;
        string spriteName = "";
        if (!string.IsNullOrEmpty(guid) && atlas != null) {
            var item = atlas.GetItem(guid);
            if (item != null) {
                spriteName = item.name;
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(label, spriteName);
        MadGUI.ConditionallyEnabled(atlas != null, () => {
            if (GUILayout.Button("Browse", GUILayout.Width(55))) {
                MadAtlasBrowser.Show(atlas, textureField);
            }
        });
        EditorGUILayout.EndHorizontal();
    }

    public static void AtlasField(string guid, MadAtlas atlas, string label, MadAtlasBrowser.Changed callback) {
        string spriteName = "";
        if (!string.IsNullOrEmpty(guid)) {
            var guids = atlas.ListItemGUIDs();
            var index = guids.FindIndex((s) => s == guid);

            if (index != -1) {
                spriteName = atlas.items[index].name;
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(label, spriteName);
        if (GUILayout.Button("Browse", GUILayout.Width(55))) {
            MadAtlasBrowser.Show(atlas, guid, callback);
        }
        EditorGUILayout.EndHorizontal();
    }

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