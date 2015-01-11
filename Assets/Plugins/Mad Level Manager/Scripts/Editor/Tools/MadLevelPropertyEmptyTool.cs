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

public class MadLevelPropertyEmptyTool : ScriptableWizard {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadLevelIcon icon;
    public new string name = "empty_property";

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
        
        isValid = true;
    }
    
    void OnWizardCreate() {
        var property = icon.CreateChild<MadLevelProperty>(name);
        
        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;
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