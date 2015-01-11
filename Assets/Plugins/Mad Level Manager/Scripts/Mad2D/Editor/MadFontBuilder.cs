/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadFontBuilder {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public bool white { get; set; }
    
    MadFont font;
    
    Color[] pixels;
    float lineHeight;
    
    char glyphChar;
    float glyphX, glyphY;
    float glyphWidth, glyphHeight;
    StringBuilder fontString = new StringBuilder();

    // ===========================================================
    // Constructors
    // ===========================================================
    
    public MadFontBuilder(MadFont font) {
        this.font = font;
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    public void Build() {
        switch (font.inputType) {
            case MadFont.InputType.TextureAndGlyphList:
                BuildLegacy();
                break;
            case MadFont.InputType.Bitmap:
                BuildBitmapFont();
                break;
        }    
    
        
    }
    
    void BuildBitmapFont() {
        // try to parse first time
        MadFontData.Parse(font.fntFile.text, font.texture);
        // if parsing succeed, assign file contents to dimensions string
        font.dimensions = font.fntFile.text;
        
        font.createStatus = MadFont.CreateStatus.Ok;
        
        BuildFinalize();
    }
    
    void BuildLegacy() {
        var texturePath = AssetDatabase.GetAssetPath(font.texture);
        var importer = TextureImporter.GetAtPath(texturePath) as TextureImporter;
        
        bool wasReadable = importer.isReadable;
        if (!importer.isReadable) {
            // making texture readable for one moment
            importer.isReadable = true;
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
        }
        
        try {
            BuildWhenReadable();
        } finally {
            
            // if texture wasn't readable then I will set is as not readable again
            if (!wasReadable) {
                importer.isReadable = false;
                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            }
        }
    }
    
    void BuildWhenReadable() {
        var texturePath = AssetDatabase.GetAssetPath(font.texture);
        var importer = TextureImporter.GetAtPath(texturePath) as TextureImporter;
    
        if (!SupportedTextureFormat(importer)) {
            if (EditorUtility.DisplayDialog(
                "Unsupported Texture Format",
                "Unsupported texture format. Needs to be ARGB32, RGBA32, RGB24, Alpha8 or DXT. "
                    + "Change to ARGB32 now?",
                "Yes", "No")) {
                importer.textureFormat = TextureImporterFormat.ARGB32;
                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            }
        }
        
        font.createStatus = MadFont.CreateStatus.Ok;
    
        pixels = font.texture.GetPixels();
        lineHeight = font.texture.height / font.linesCount;
        
        bool atGlyph = false;
        int currentGlyphIndex = -1;
        float fillFactor = 0;
        int fillFactorCount = 0;
        
        for (int line = 0; line < font.linesCount; ++line) {
            int y = (int) Mathf.Floor(line * lineHeight);
            int x = 0;
        
            for (; x < font.texture.width; ++x) {
                if (!IsEmpty(line, x)) {
                    if (!atGlyph) {
                        atGlyph = true;
                        fillFactor = 0;
                        fillFactorCount = 0;
                        
                        if (font.glyphs.Length == currentGlyphIndex + 1) {
                            Debug.LogError("No more glyphs! Have you defined all glyphs?");
                            font.createStatus = MadFont.CreateStatus.TooMuchGlypsFound;
                            break;
                        }
                        
                        glyphChar = font.glyphs[++currentGlyphIndex];
                        glyphX = x / (float) font.texture.width;
                        glyphY = y / (float) font.texture.height;
                        glyphHeight = lineHeight / font.texture.height;
                    }
                    
                    fillFactor += FillFactor(line, x);
                    fillFactorCount++;
                } else { // IsEmpty
                    if (atGlyph) {
                    
                        // add glyph only if its fill factor is big enough
                        fillFactor /= fillFactorCount;
                        if (fillFactor >= font.fillFactorTolerance) {
                            glyphWidth = (x / (float) font.texture.width) - glyphX;
                            AddGlyph();
                        } else {
                            currentGlyphIndex--;
                        }
                        
                        atGlyph = false;
                    }
                }
            }
            
            if (atGlyph) {
                glyphWidth = ((x - 1) / (float) font.texture.width) - glyphX;
                AddGlyph();
                atGlyph = false;
            }
        }
        
        // check correctness
        if (currentGlyphIndex + 1 < font.glyphs.Length) {
            Debug.LogWarning(
                string.Format("Not all glyphs used (used={0}, expected={1})",
                currentGlyphIndex + 1, font.glyphs.Length));
                font.createStatus = MadFont.CreateStatus.TooMuchGlypsDefined;
        }
        
        font.dimensions = fontString.ToString();
        BuildFinalize();
    }
    
    void AddGlyph() {
        if (fontString.Length == 0) {
            fontString.AppendLine("1");
        }
        fontString.AppendLine(glyphChar + " " + glyphX + " " + glyphY + " " + glyphWidth + " " + glyphHeight);
    }
    
    void BuildFinalize() {
        string texturePath = AssetDatabase.GetAssetPath(font.texture);
        var splitPath = texturePath.Split('.');
        string baseName = string.Join(".", splitPath, 0, splitPath.Length - 1);
        
        if (font.material == null) {
            var material = new Material(Shader.Find(
                white ? MadLevelManager.MadShaders.FontWhite : MadLevelManager.MadShaders.Font));
            AssetDatabase.CreateAsset(material, baseName + ".mat");
            font.material = material;
        } else {
            font.material.shader = Shader.Find(
                white ? MadLevelManager.MadShaders.FontWhite : MadLevelManager.MadShaders.Font);
        }
        
        font.material.mainTexture = font.texture;
        
        font.dirty = true;
        font.created = true;
        EditorUtility.SetDirty(font);
    }
    
    bool IsEmpty(int line, int x) {
        int fromY = (int) Mathf.Floor(line * lineHeight);
        int toY = (int) Mathf.Ceil((line + 1) * lineHeight);
        for (int y = fromY; y < toY; ++y) {
            var color = pixels[Index(x, y)];
            if (color.a > 0.0f) {
                return false;
            }
        }
        
        return true;
    }
    
    // finds out fill factor for this column
    // from 0 - nothing to 1 - full fill
    float FillFactor(int line, int x) {
        float pixelFactor = 1f / lineHeight;
        float fillFactor = 0;
        
        int fromY = (int) Mathf.Floor(line * lineHeight);
        int toY = (int) Mathf.Ceil((line + 1) * lineHeight);
        for (int y = fromY; y < toY; ++y) {
            var color = pixels[Index(x, y)];
            fillFactor += color.a * pixelFactor;
        }
        
        return fillFactor;
    }
    
    int Index(int x, int y) {
        return (font.texture.height - y - 1) * font.texture.width + x;
    }
    
    bool SupportedTextureFormat(TextureImporter importer) {
        var format = importer.textureFormat;
        return format == TextureImporterFormat.ARGB32
            || format == TextureImporterFormat.RGBA32
            || format == TextureImporterFormat.RGB24
            || format == TextureImporterFormat.Alpha8
            || format == TextureImporterFormat.DXT1
            || format == TextureImporterFormat.DXT5;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void CreateFont() {
        string saveFolder = "Assets";
        if (Selection.activeObject != null) {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(assetPath)) {
                saveFolder = System.IO.Path.GetDirectoryName(assetPath);
            }
        }

        var assetPathAndName = EditorUtility.SaveFilePanel("Save Font", saveFolder, "New Font", "prefab");
        if (string.IsNullOrEmpty(assetPathAndName)) {
            return;
        }

        var go = new GameObject();
        go.AddComponent<MadFont>();

        assetPathAndName = MadPath.MakeRelative(assetPathAndName);
        
        var prefab = PrefabUtility.CreatePrefab(assetPathAndName, go);
        GameObject.DestroyImmediate(go);
        
        Selection.activeObject = prefab;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif