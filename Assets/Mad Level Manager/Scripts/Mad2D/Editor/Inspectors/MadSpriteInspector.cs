/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[CustomEditor(typeof(MadSprite))]   
public class MadSpriteInspector : Editor {

    #region Constants
    
    private static readonly Color BoundsColor = new Color(1f, 0f, 1f, 1f);

    #endregion

    #region Fields
    
    private SerializedProperty panel;
    private SerializedProperty visible;
    private SerializedProperty pivotPoint;
    private SerializedProperty customPivotPoint;
    private SerializedProperty tint;
    private SerializedProperty inputType;
    private SerializedProperty texture;
    private SerializedProperty textureAtlas;
    private SerializedProperty textureAtlasSpriteGUID;
    private SerializedProperty textureOffset;
    private SerializedProperty textureRepeat;
    private SerializedProperty hasPremultipliedAlpha;
    private SerializedProperty guiDepth;
    private SerializedProperty fillType;
    private SerializedProperty fillValue;
    private SerializedProperty radialFillOffset;
    private SerializedProperty radialFillLength;

    private SerializedProperty hasLiveBounds;
    private SerializedProperty liveLeft;
    private SerializedProperty liveBottom;
    private SerializedProperty liveRight;
    private SerializedProperty liveTop;
    
    protected MadSprite sprite;

    private Texture2D gridTexture;
    private Texture2D whiteTexture;

    protected bool showLiveBounds = true;

    #endregion

    #region Unity Methods
    
    protected void OnEnable() {
        sprite = target as MadSprite;

        panel = serializedObject.FindProperty("panel");
        visible = serializedObject.FindProperty("visible");
        pivotPoint = serializedObject.FindProperty("pivotPoint");
        customPivotPoint = serializedObject.FindProperty("customPivotPoint");
        tint = serializedObject.FindProperty("tint");
        inputType = serializedObject.FindProperty("inputType");
        texture = serializedObject.FindProperty("texture");
        textureAtlas = serializedObject.FindProperty("textureAtlas");
        textureAtlasSpriteGUID = serializedObject.FindProperty("textureAtlasSpriteGUID");
        textureOffset = serializedObject.FindProperty("textureOffset");
        textureRepeat = serializedObject.FindProperty("textureRepeat");
        hasPremultipliedAlpha = serializedObject.FindProperty("hasPremultipliedAlpha");
        guiDepth = serializedObject.FindProperty("guiDepth");
        fillType = serializedObject.FindProperty("fillType");
        fillValue = serializedObject.FindProperty("fillValue");
        radialFillOffset = serializedObject.FindProperty("radialFillOffset");
        radialFillLength = serializedObject.FindProperty("radialFillLength");

        hasLiveBounds = serializedObject.FindProperty("hasLiveBounds");
        liveLeft = serializedObject.FindProperty("liveLeft");
        liveBottom = serializedObject.FindProperty("liveBottom");
        liveRight = serializedObject.FindProperty("liveRight");
        liveTop = serializedObject.FindProperty("liveTop");

        gridTexture = Resources.Load("Mad2D/Textures/grid", typeof(Texture2D)) as Texture2D;
        whiteTexture = Resources.Load("Textures/white", typeof(Texture2D)) as Texture2D;

        
    }
    
    public override void OnInspectorGUI() {
        SectionSprite();
    }

    public override bool HasPreviewGUI() {
        switch (sprite.inputType) {
            case MadSprite.InputType.SingleTexture:
                return sprite.texture != null && showLiveBounds;
            case MadSprite.InputType.TextureAtlas:
                return sprite.textureAtlas != null && sprite.textureAtlas.GetItem(sprite.textureAtlasSpriteGUID) != null && showLiveBounds;
            default:
                Debug.LogError("Unknown input type: " + sprite.inputType);
                return false;
        }
        
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background) {
        DrawLiveBoundsPreview(r);
    }

    #endregion

    #region Methods
    
    protected void SectionSprite() {
        SectionSprite(DisplayFlag.None);
    }
    
    protected void SectionSprite(DisplayFlag flags) {
        serializedObject.Update();
        GUIDepthCheck();

        MadGUI.PropertyField(panel, "Panel", MadGUI.ObjectIsSet);
        EditorGUILayout.Space();

        MadGUI.PropertyField(visible, "Visible");
    
        if ((flags & DisplayFlag.WithoutMaterial) == 0) {
            MadGUI.PropertyFieldEnumPopup(inputType, "Input Type");
            MadGUI.Indent(() => {
            
                switch (sprite.inputType) {
                    case MadSprite.InputType.SingleTexture:
                        MadGUI.PropertyField(texture, "Texture", MadGUI.ObjectIsSet);
                        MadGUI.Indent(() => {
                            MadGUI.PropertyFieldVector2(textureRepeat, "Repeat");
                            MadGUI.PropertyFieldVector2(textureOffset, "Offset");
                        });
                        break;
                    case MadSprite.InputType.TextureAtlas:
                        MadGUI.PropertyField(textureAtlas, "Texture Atlas", MadGUI.ObjectIsSet);
                        
                        if (sprite.textureAtlas != null) {
                            MadAtlasUtil.AtlasField(textureAtlasSpriteGUID, sprite.textureAtlas, "Sprite");
                        }
                        
                        break;
                    default:
                        Debug.LogError("Unknown input type: " + sprite.inputType);
                        break;
                }
            
            });
            
        }

        MadGUI.PropertyField(hasPremultipliedAlpha, "Has Pre-Alpha");
        
        MadGUI.PropertyField(tint, "Tint");
        
        EditorGUILayout.Space();
        
        if ((flags & DisplayFlag.WithoutSize) == 0) {
            EditorGUILayout.Space();
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button(new GUIContent("Resize To Texture",
                "Resizes this sprite to match texture size"))) {
                MadUndo.RecordObject2(sprite, "Resize To Texture");
                sprite.ResizeToTexture();
                EditorUtility.SetDirty(sprite);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.Space();
        }
        
        EditorGUILayout.Space();
        
        MadGUI.PropertyField(pivotPoint, "Pivot Point");
        if (sprite.pivotPoint == MadSprite.PivotPoint.Custom) {
            MadGUI.Indent(() => {
                MadGUI.PropertyFieldVector2(customPivotPoint, "Custom Pivot Point");
            });
        }

        MadGUI.PropertyField(guiDepth, "GUI Depth");
        
        EditorGUILayout.Space();
        
        if ((flags & DisplayFlag.WithoutFill) == 0) {
            MadGUI.PropertyField(fillType, "Fill Type");
            EditorGUILayout.Slider(fillValue, 0, 1, "Fill Value");
            
            if (sprite.fillType == MadSprite.FillType.RadialCCW || sprite.fillType == MadSprite.FillType.RadialCW) {
                MadGUI.PropertyFieldSlider(radialFillOffset, -1, 1, "Offset");
                MadGUI.PropertyFieldSlider(radialFillLength, 0, 1, "Length");
            }
        }

        if (showLiveBounds) {
            GUILayout.Label("Sprite Border", "HeaderLabel");
            EditorGUILayout.Space();
            if (sprite.CanDraw()) {
                FieldLiveBounds();
            } else {
                MadGUI.Info("More settings will be available when the sprite texture or atlas is set.");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void FieldLiveBounds() {

        int texWidth = sprite.currentTextureWidth;
        int texHeight = sprite.currentTextureHeight;

        MadGUI.PropertyField(hasLiveBounds, "Has Border");

        GUI.backgroundColor = Color.yellow;
        MadGUI.Indent(() => {
            MadGUI.LookLikeControls(0, 40);
            FieldLiveBounds(liveLeft, texWidth, "Left", 0, liveRight.floatValue);
            FieldLiveBounds(liveTop, texHeight, "Top", liveBottom.floatValue, 1);
            FieldLiveBounds(liveRight, texWidth, "Right", liveLeft.floatValue, 1);
            FieldLiveBounds(liveBottom, texHeight, "Bottom", 0, liveTop.floatValue);
            MadGUI.LookLikeControls(0);
        });
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (MadGUI.Button("Reset")) {
            ResetLiveBounds();
        }
        if (MadGUI.Button("Compute")) {
            MadUndo.RecordObject2(sprite, "Set Live Bounds");
            ComputeLiveBounds();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ResetLiveBounds() {
        liveLeft.floatValue = 0;
        liveBottom.floatValue = 0;
        liveTop.floatValue = 1;
        liveRight.floatValue = 1;
    }

    private Rect DrawLiveBoundsPreview(Rect rect) {
        DrawGrid(rect);

        switch (sprite.inputType) {
            case MadSprite.InputType.SingleTexture: {
                var textureRect = KeepRatio(sprite.texture.width, sprite.texture.height, rect);
                GUI.color = sprite.tint;
                GUI.DrawTexture(textureRect, sprite.texture);
                GUI.color = Color.white;

                DrawLiveBounds(textureRect);
            }
                break;

            case MadSprite.InputType.TextureAtlas: {
                var item = sprite.textureAtlas.GetItem(sprite.textureAtlasSpriteGUID);
                if (item != null) {
                    var textureRect = KeepRatio(item.pixelsWidth, item.pixelsHeight, rect);
                    GUI.DrawTextureWithTexCoords(textureRect, sprite.textureAtlas.atlasTexture, item.region);

                    DrawLiveBounds(textureRect);
                }
            }
                break;
        }
        

        
        return rect;
    }

    private Rect KeepRatio(int textureWidth, int textureHeight, Rect rect) {
        float targetRatio = textureWidth / (float) textureHeight;
        float rectRatio = rect.width / rect.height;

        if (rectRatio > targetRatio) { // rect is wider than the texture, correct width
            float width = rect.height * targetRatio;
            return new Rect(rect.x + (rect.width - width) / 2, rect.y, width, rect.height);
        } else if (rectRatio < targetRatio) { // rect is higher than the texture, correct height
            float height = rect.width / targetRatio;
            return new Rect(rect.x, rect.y + (rect.height - height) / 2, rect.width, height);
        } else {
            return rect;
        }
    }

    private void DrawGrid(Rect rect) {
        float tx = rect.width / gridTexture.width;
        float ty = rect.height / gridTexture.height;

        GUI.DrawTextureWithTexCoords(rect, gridTexture, new Rect(0, 0, tx, ty));
    }

    private void DrawLiveBounds(Rect rect) {
        DrawLiveBoundHoriz(rect, sprite.liveTop);
        DrawLiveBoundHoriz(rect, sprite.liveBottom);
        DrawLiveBoundVert(rect, sprite.liveLeft);
        DrawLiveBoundVert(rect, sprite.liveRight);
    }

    private void DrawLiveBoundHoriz(Rect rect, float val) {
        float top = rect.y + rect.height * (1 - val);
        DrawRect(new Rect(rect.x, top, rect.width, 1), BoundsColor);
    }

    private void DrawLiveBoundVert(Rect rect, float val) {
        float left = rect.x + rect.width * val;
        DrawRect(new Rect(left, rect.y, 1, rect.height), BoundsColor);
    }

    private void DrawRect(Rect rect, Color color) {
        Graphics.DrawTexture(rect, whiteTexture, new Rect(0, 0, 1, 1), 0, 0, 0, 0, color);
    }

    private void FieldLiveBounds(SerializedProperty property, int width, string label, float min, float max) {
        int value = (int) (property.floatValue * width);
        EditorGUI.BeginChangeCheck();
        value = EditorGUILayout.IntField(label, value);
        if (EditorGUI.EndChangeCheck()) {
            property.floatValue = Mathf.Clamp(value / (float) width, min, max);
        }
    }

    private void ComputeLiveBounds() {

        bool changed = SetReadable(sprite.currentTexture, true);
        try {
            sprite.RecalculateLiveBounds();
            EditorUtility.SetDirty(sprite);
        } finally {
            if (changed) {
                SetReadable(sprite.currentTexture, false);
            }
        }

    }

    private bool SetReadable(Texture2D texture, bool readable) {
        var assetPath = AssetDatabase.GetAssetPath(texture);

        var importer = TextureImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer.isReadable != readable) {
            importer.isReadable = readable;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            return true;
        }

        return false;
    }

    private void GUIDepthCheck() {
        var sprites = SharingDepthWithOtherTexturesCount();
        if (sprites.Count > 0) {
            bool select = MadGUI.WarningFix("This sprite is have the same GUI depth as " + sprites.Count + " other sprites with other textures on the scene. "
                + " This may lead to situations when something is randomly disappearing.", "Select These Sprites");
            if (select) {
                Select(sprites);
            }

            EditorGUILayout.Space();
        }
    }

    // searches for other sprites with other textures with the same depth
    private List<MadSprite> SharingDepthWithOtherTexturesCount() {
        List<MadSprite> result = new List<MadSprite>();

        var panel = sprite.panel;
        if (panel == null) {
            return result;
        }

        foreach (var s in panel.sprites) {
            if (s == sprite) {
                continue;
            }

            if (s.guiDepth == sprite.guiDepth) {
                if (s.inputType == sprite.inputType) {
                    switch (s.inputType) {
                        case MadSprite.InputType.SingleTexture:
                            if (s.texture != sprite.texture) {
                                result.Add(s);
                            }
                            break;

                        case MadSprite.InputType.TextureAtlas:
                            // checking guid is sufficient. I don't need to check atlas texture
                            if (s.textureAtlasSpriteGUID != sprite.textureAtlasSpriteGUID) {
                                result.Add(s);
                            }
                            break;
                    }
                }
            }
        }

        return result;
    }

    private void Select(List<MadSprite> sprites) {
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (var sprite in sprites) {
            gameObjects.Add(sprite.gameObject);
        }

        Selection.objects = gameObjects.ToArray();
    }

    #endregion

    #region Inner Types
    
    [Flags]
    protected enum DisplayFlag {
        None = 0,
        WithoutSize = 1,
        WithoutMaterial = 2,
        WithoutFill = 4,
    }

    #endregion

}

#if !UNITY_3_5
} // namespace
#endif