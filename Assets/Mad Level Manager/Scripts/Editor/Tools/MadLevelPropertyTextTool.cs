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

public class MadLevelPropertyTextTool : ScriptableWizard {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadLevelIcon icon;
    public new string name = "text_property";
    public MadFont font;

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
        
        if (font == null) {
            isValid = false;
            return;
        }
        
        isValid = true;
    }
    
    void OnWizardCreate() {
        var property = icon.CreateChild<MadLevelProperty>(name);
        var text = property.gameObject.AddComponent<MadText>();
        
        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;
        
        text.font = font;
        text.text = "Sample Text";
        text.guiDepth = icon.guiDepth + 1;
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