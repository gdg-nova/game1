/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;
using System.IO;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[CustomEditor(typeof(MadFont))]   
public class MadFontInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty inputType;
    
    // common properties
    SerializedProperty texture;
    SerializedProperty forceWhite;
    
    // texture and glyph list properties
    SerializedProperty glyphs;
    SerializedProperty linesCount;
    SerializedProperty fillFactorTolerance;
    
    // glyph designer properties
    SerializedProperty fntFile;
    
    
    MadFont script;

    private Color primaryColor = Color.white;
    private Color secondaryColor = Color.black;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        script = target as MadFont;
        if (script.created) {
            primaryColor = script.material.GetColor("_PrimaryColor");
            secondaryColor = script.material.GetColor("_SecondaryColor");
        }
    
        inputType = serializedObject.FindProperty("inputType");
        
        texture = serializedObject.FindProperty("texture");
        forceWhite = serializedObject.FindProperty("forceWhite");
        
        glyphs = serializedObject.FindProperty("glyphs");
        linesCount = serializedObject.FindProperty("linesCount");
        fillFactorTolerance = serializedObject.FindProperty("fillFactorTolerance");
        
        fntFile = serializedObject.FindProperty("fntFile");
    }

    public override void OnInspectorGUI() {
        GUICreator(script.created);
        
        switch (script.createStatus) {
            case MadFont.CreateStatus.None:
                MadGUI.Warning("This font is not yet created. Please fill above and click on the Create button.");
                break;
            case MadFont.CreateStatus.TooMuchGlypsDefined:
                MadGUI.Error("There more glyphs declared than found in the texture.");
                break;
            case MadFont.CreateStatus.TooMuchGlypsFound:
                MadGUI.Error("There more glyphs found in the texture than declared.");
                break;
            case MadFont.CreateStatus.Ok:
                // nothing wrong
                break;
            default:
                MadDebug.Assert(false, "Unknown create status: " + script.createStatus);
                break;
        }
        
//        MadGUI.Info("This field is read-only");
//        EditorGUILayout.TextArea(font.dimensions);
    }
    
    void GUICreator(bool created) {
        serializedObject.Update();
        MadGUI.PropertyField(inputType, "Input Type");
        
        bool canCreate = false;
        
        switch ((MadFont.InputType) inputType.enumValueIndex) {
            case MadFont.InputType.TextureAndGlyphList:
                canCreate = GUITextureAndGlyphListCreator();
                break;
            case MadFont.InputType.Bitmap:
                canCreate = GUIBitmapFontCreator();
                break;
        }
        
        MadGUI.PropertyField(forceWhite, "Force White Color", "Forces this font to be rendered with white color only.");
        MadGUI.ConditionallyEnabled(!forceWhite.boolValue, () => {
            EditorGUI.BeginChangeCheck();
            primaryColor = EditorGUILayout.ColorField("Primary Color", primaryColor);
            secondaryColor = EditorGUILayout.ColorField("Primary Color", secondaryColor);
            if (EditorGUI.EndChangeCheck()) {
                if (created) {
                    SetColors();
                }
            }
        });
        
        serializedObject.ApplyModifiedProperties();
        
        GUI.enabled = canCreate;
        if (GUILayout.Button(created ? "Recreate" : "Create")) {
            var builder = new MadFontBuilder(script);
            builder.white = forceWhite.boolValue;
            builder.Build();

            if (!forceWhite.boolValue && script.created) {
                SetColors();
            }
        }
        GUI.enabled = true;
    }

    private void SetColors() {
        script.material.SetColor("_PrimaryColor", primaryColor);
        script.material.SetColor("_SecondaryColor", secondaryColor);
    }
    
    bool GUIBitmapFontCreator() {
        MadGUI.Info(
            "The bitmap font creation mostly takes place outside Unity. "
            + "For this purpose you can use BMFont on Windows, Glyph Designer, or bmGlyph on Mac.\n\n"
            + "These tools will create a texture and FNT file that you should drag into the field below. "
            + "Please note that you should change FNT file extension to TXT.");
    
        MadGUI.PropertyField(texture, "Font Texture");
        MadGUI.PropertyField(fntFile, "FNT File");
        
        return script.texture != null && script.fntFile != null;
    }
    
    bool GUITextureAndGlyphListCreator() {
        MadGUI.PropertyField(texture, "Font Texture");
        EditorGUILayout.BeginHorizontal();
        MadGUI.PropertyField(glyphs, "Glyphs");
        if (GUILayout.Button("Load", GUILayout.Width(50))) {
            string filePath = EditorUtility.OpenFilePanel(
                "Select Glyps File", System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)), "txt");
            
            if (!string.IsNullOrEmpty(filePath)) {
                var text = System.IO.File.ReadAllText(filePath);
                text = text.Trim();
                glyphs.stringValue = text;
            }
        }
        EditorGUILayout.EndHorizontal();
        MadGUI.PropertyField(linesCount, "Lines Count");
        MadGUI.PropertyField(fillFactorTolerance, "Fill Tolerance",
                             "How tolerant will be when looking for spaces between glyphs.");
        
        return script.texture != null && !string.IsNullOrEmpty(script.glyphs) && script.linesCount > 0;
    }
    
    public override bool HasPreviewGUI() {
        MadFont font = target as MadFont;
        return font.material != null;
    }
    
    public override void OnPreviewGUI(Rect rect, GUIStyle background) {
        Graphics.DrawTexture(
            rect,
            script.material.mainTexture,
            new Rect(0, 0, 1, 1),
            0, 0, 0, 0,
            Color.white,
            script.material);
            
        foreach (var c in script.data.charTable.Keys) {
            if (c == ' ') {
                continue; // ignore space
            }
        
            var glyph = script.GlyphFor(c);
        
            var glyphRect = new Rect(
                rect.x + glyph.x * rect.width,
                rect.y + glyph.y * rect.height,
                glyph.width * rect.width,
                glyph.height * rect.height);
                
            DrawRect(glyphRect, Color.green);
        }
    }
    
    void DrawRect(Rect rect, Color color) {
        MadLevelManager.MadDrawing.DrawLine(
            new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin), color, 1, false);
        MadLevelManager.MadDrawing.DrawLine(
            new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax), color, 1, false);
        MadLevelManager.MadDrawing.DrawLine(
            new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMin, rect.yMax), color, 1, false);
        MadLevelManager.MadDrawing.DrawLine(
            new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMin, rect.yMin), color, 1, false);
    }
    
//    void Upgrade() {
//        string input = script.dimensions;
//        Texture2D texture = script.texture;
//        
//        MadDebug.Assert(!string.IsNullOrEmpty(input) && texture != null, "Invalid input");
//        
//        var texturePath = AssetDatabase.GetAssetPath(texture);
//        var importer = TextureImporter.GetAtPath(texturePath) as TextureImporter;
//        
//        importer.get
//    }

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