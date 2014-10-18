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

/// <summary>
/// Browser window for atlas
/// </summary>
public class MadAtlasBrowser : EditorWindow {

    #region Private Fields

    // atlas that should be displayed
    MadAtlas atlas;

    // texture field that should be modified
    Changed changedCallback;

    #endregion

    #region Private Fields Helpers

    int currentItemIndex;
    string selectedItemGUID;

    Vector2 scrollPosition;

    Texture2D whiteTexture;
    string searchFilter = "";
    private float zoomLevel = 1;

    // double click
    DateTime lastClickTime;

    // None item
    MadAtlas.Item noneItem;

    #endregion

    #region Private Properties

    public Vector2 textureSize {
        get {
            return new Vector2(64, 64) * zoomLevel;
        }
    }

    public Vector2 labelSize {
        get {
            return new Vector2(textureSize.x, 15);
        }
    }

    public Vector2 tileSize {
        get {
            return new Vector2(textureSize.x, textureSize.y + labelSize.y);
        }
    }

    public Vector2 tileMargin {
        get {
            return new Vector2(2, 2);
        }
    }

    #endregion

    #region Slots

    void OnEnable() {
        noneItem = new MadAtlas.Item();
        noneItem.name = "None";
    }

    void OnGUI() {
        if (whiteTexture == null) {
            whiteTexture = Resources.Load("Textures/white", typeof(Texture2D)) as Texture2D;
        }

        DrawSearchBox();
        DrawZoomSlider();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        Vector2 bounds = DrawTiles();

        // spaces to tell the layout about used size
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(bounds.x);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(bounds.y);

        EditorGUILayout.EndScrollView();

        DrawInfoPanel();
    }

    #endregion

    #region Private Methods

    void DrawSearchBox() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Search:", GUILayout.Width(50));
        searchFilter = EditorGUILayout.TextField(searchFilter);
        EditorGUILayout.EndHorizontal();
    }

    void DrawZoomSlider() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        zoomLevel = EditorGUILayout.Slider(zoomLevel, 1, 3, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
    }

    MadAtlas.Item[] atlasItems;

    Vector2 DrawTiles() {
        atlasItems = (from i in atlas.items orderby i.name select i).ToArray(); // sort

        RewindTiles();

        Vector2 bounds = new Vector2(0, 0);
        Vector2 nextPosition = Vector2.zero;

        // first tile will be 'none'
        nextPosition = DrawNextTile(nextPosition, ref bounds);
        
        for (int i = 0; i < atlas.items.Count; ++i) {
            nextPosition = DrawNextTile(nextPosition, ref bounds);
        }

        return bounds;
    }

    void RewindTiles() {
        currentItemIndex = -1;
    }

    Vector2 DrawNextTile(Vector2 position, ref Vector2 bounds) {
        if (currentItemIndex == -1) {
            currentItemIndex++;
            return DrawTile(noneItem, position, ref bounds);
        } else {
            var item = atlasItems[currentItemIndex++];
            return DrawTile(item, position, ref bounds);
        }
    }

    Vector2 DrawTile(MadAtlas.Item item, Vector2 position, ref Vector2 bounds) {
        // apply search filter
        if (!string.IsNullOrEmpty(searchFilter)) {
            if (!item.name.ToUpper().Contains(searchFilter.ToUpper())) {
                return position;
            }
        }

        // check for click event
        var allRect = new Rect(position.x, position.y, tileSize.x, tileSize.y);

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            if (allRect.Contains(Event.current.mousePosition)) {

                // handle double click
                if (selectedItemGUID == item.textureGUID && (DateTime.Now - lastClickTime).TotalMilliseconds < 200) {
                    Close();
                }
                lastClickTime = DateTime.Now;

                Select(item.textureGUID);
            }
        }

        // draw texture
        if (item != noneItem) {
            var textureRect = new Rect(position.x, position.y, textureSize.x, textureSize.y);
            var texturesCoords = item.region;

            textureRect = KeepRatio(textureRect, item.pixelsWidth, item.pixelsHeight);

            GUI.DrawTextureWithTexCoords(textureRect, atlas.atlasTexture, texturesCoords);
        }

        // draw label
        var labelRect = new Rect(position.x, position.y + textureSize.y, labelSize.x, labelSize.y);

        if (selectedItemGUID == item.textureGUID) {
            if (EditorGUIUtility.isProSkin) {
                GUI.color = new Color(63 / 255f, 92 / 255f, 133 / 255f);
            } else {
                GUI.color = new Color(63 / 255f, 132 / 255f, 230 / 255f);
            }
            
            GUI.DrawTexture(labelRect, whiteTexture);
            GUI.color = Color.white;
        }

        GUI.Label(labelRect, item.name);

        // update bounds
        bounds = new Vector2(Mathf.Max(bounds.x, labelRect.xMax), Mathf.Max(bounds.y, labelRect.yMax));

        // calculate next position
        Vector2 newPosition = position + new Vector2(tileSize.x + tileMargin.x, 0);
        if (newPosition.x + tileSize.x > Screen.width) {
            newPosition = new Vector2(0, position.y + tileSize.y + tileMargin.y);
        }

        return newPosition;
    }

    private Rect KeepRatio(Rect textureRect, int width, int height) {
        float textureRectRatio = textureRect.width / (float) textureRect.height;
        float targetRatio = width / (float) height;

        if (targetRatio > textureRectRatio) { // texture is wider, rect should be lower
            float targetHeight = textureRect.height / targetRatio;
            return new Rect(textureRect.x, textureRect.y + (textureRect.height - targetHeight) / 2, textureRect.width, targetHeight);
        } else if (targetRatio < textureRectRatio) { // texture is taller, rect should be higher
            float targetWidth = textureRect.width * targetRatio;
            return new Rect(textureRect.x + (textureRect.width - targetWidth) / 2, textureRect.y, targetWidth, textureRect.height);
        } else { // just fune!
            return textureRect;
        }
    }

    void Select(string guid) {
        selectedItemGUID = guid;
        if (changedCallback != null) {
            changedCallback(guid);
        }
        Repaint();
    }

    void DrawInfoPanel() {
        var rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(74), GUILayout.Width(Screen.width));

        string name = "";
        string size = "";
        string path = "";
        
        if (!string.IsNullOrEmpty(selectedItemGUID)) {
            var item = atlas.GetItem(selectedItemGUID);

            if (item != null) {
                name = item.name;
                size = item.pixelsWidth + "x" + item.pixelsHeight;
                path = MadAtlasUtil.GetItemOriginPath(item);

                GUI.DrawTextureWithTexCoords(new Rect(rect.x + 5, rect.y + 5, 64, 64), atlas.atlasTexture, item.region);
            }
        }

        GUILayout.Space(74);
        GUILayout.BeginVertical();

        GUILayout.Label(name);
        GUILayout.Label(size);
        GUILayout.Label(path);

        GUILayout.EndVertical();

        
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Public Static Methods

    public static void Show(MadAtlas atlas, SerializedProperty property) {
        if (property != null) {
            var currentGUID = property.stringValue;
            Show(atlas, currentGUID, (guid) => {
                property.serializedObject.UpdateIfDirtyOrScript();
                property.stringValue = guid;
                property.serializedObject.ApplyModifiedProperties();
            });
        } else {
            Show(atlas, "", null);
        }
    }

    public static void Show(MadAtlas atlas, string currentGUID, Changed changedCallback) {
        //var window = ScriptableObject.CreateInstance<MadAtlasBrowser>();
        var browser = EditorWindow.GetWindow<MadAtlasBrowser>(true, "Atlas Browser", true);
        browser.atlas = atlas;
        browser.changedCallback = changedCallback;
        browser.selectedItemGUID = currentGUID;
    }

    #endregion

    #region Inner and Anonymous Classes
    #endregion

    public delegate void Changed(string guid);
}

#if !UNITY_3_5
} // namespace
#endif