/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelPropertySpriteTool : ScriptableWizard {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadLevelIcon icon;
    public new string name = "sprite_property";
    public Texture2D texture;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
  
    void OnWizardUpdate() {
        if (icon == null) {
            isValid = false;
            return;
        }
        
        if (string.IsNullOrEmpty(name)) {
            isValid = false;
            return;
        }
        
        if (texture == null) {
            isValid = false;
            return;
        }
        
        isValid = true;
    }
    
    void OnWizardCreate() {
        var property = icon.CreateChild<MadLevelProperty>(name);
        var sprite = property.gameObject.AddComponent<MadSprite>();
        
        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;
        
        sprite.texture = texture;
        sprite.ResizeToTexture();
        sprite.guiDepth = icon.guiDepth + 1;
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