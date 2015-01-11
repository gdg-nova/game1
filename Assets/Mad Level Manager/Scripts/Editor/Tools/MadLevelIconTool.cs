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

public class MadLevelIconTool : ScriptableWizard {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadPanel panel;
    public new string name = "icon template";
    public Texture2D texture;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
  
    void OnWizardUpdate() {
        if (panel == null) {
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
        var icon = panel.CreateChild<MadLevelIcon>(name);
        icon.transform.localPosition = Vector3.zero;
        icon.transform.localScale = Vector3.one;
        icon.texture = texture;

        Selection.activeGameObject = icon.gameObject;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    public static void Display() {
        var wizard = ScriptableWizard.DisplayWizard<MadLevelIconTool>("Create Icon", "Create");
        wizard.panel = MadPanel.UniqueOrNull();

        if (Selection.activeObject is Texture2D) {
            wizard.texture = Selection.activeObject as Texture2D;
            wizard.OnWizardUpdate();
        }
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif