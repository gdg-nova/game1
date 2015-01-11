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

public class MadLevelGridTool : ScriptableWizard {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadPanel panel;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnWizardUpdate() {
        errorString = "";
        isValid = panel != null;
    }
    
    void OnWizardOtherButton() {
        OnWizardCreate();
    }
    
    void OnWizardCreate() {
        CreateUnderPanel(panel);
    }
    
    public static MadLevelGridLayout CreateUnderPanel(MadPanel panel) {
        MadUndo.LegacyRegisterSceneUndo("Creating Grid Layout");
        var child = MadTransform.CreateChild<MadLevelGridLayout>(panel.transform, "Grid Layout");
        child.setupMethod = MadLevelGridLayout.SetupMethod.Generate;
        Selection.activeObject = child;
        
        return child;
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
