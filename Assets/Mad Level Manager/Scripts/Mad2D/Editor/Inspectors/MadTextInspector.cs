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

[CustomEditor(typeof(MadText))]
public class MadTextInspector : MadSpriteInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    SerializedProperty panel;
    SerializedProperty font;
    SerializedProperty text;
    SerializedProperty scale;
    SerializedProperty letterSpacing;
    SerializedProperty align;
    SerializedProperty wordWrap;
    SerializedProperty wordWrapLength;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected new void OnEnable() {
        base.OnEnable();

        panel = serializedObject.FindProperty("panel");
        font = serializedObject.FindProperty("font");
        text = serializedObject.FindProperty("text");
        scale = serializedObject.FindProperty("scale");
        letterSpacing = serializedObject.FindProperty("letterSpacing");
        align = serializedObject.FindProperty("align");
        wordWrap = serializedObject.FindProperty("wordWrap");
        wordWrapLength = serializedObject.FindProperty("wordWrapLength");

        showLiveBounds = false;
    }

    public override void OnInspectorGUI() {
        SectionSprite(DisplayFlag.WithoutSize | DisplayFlag.WithoutMaterial | DisplayFlag.WithoutFill);
        
        serializedObject.Update();
        MadGUI.PropertyField(panel, "Panel", MadGUI.ObjectIsSet);
        EditorGUILayout.Space();

        MadGUI.PropertyField(font, "Font", MadGUI.ObjectIsSet);
        EditorGUILayout.LabelField("Text");
        text.stringValue = EditorGUILayout.TextArea(text.stringValue);
        MadGUI.PropertyField(scale, "Scale");
        MadGUI.PropertyField(align, "Align");
        MadGUI.PropertyField(letterSpacing, "Letter Spacing");
        MadGUI.PropertyField(wordWrap, "Word Wrap");
        MadGUI.Indent(() => { 
            MadGUI.PropertyField(wordWrapLength, "Line Length");
        });
        
        serializedObject.ApplyModifiedProperties();
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    List<MadText> TextList() {
        var texts = ((MonoBehaviour) target).GetComponentsInChildren<MadText>();
        return new List<MadText>(texts);
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