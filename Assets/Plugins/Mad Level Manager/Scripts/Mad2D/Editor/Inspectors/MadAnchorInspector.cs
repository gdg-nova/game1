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
 
[CustomEditor(typeof(MadAnchor))]
public class MadAnchorInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    public override void OnInspectorGUI() {
        DrawInspector(serializedObject);
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void DrawInspector(SerializedObject so) {
        so.Update();
        
        var mode = so.FindProperty("mode");
        var position = so.FindProperty("position");
        var anchorObject = so.FindProperty("anchorObject");
        var anchorCamera = so.FindProperty("anchorCamera");
        
        MadGUI.PropertyField(mode, "Mode");
        switch ((MadAnchor.Mode) mode.enumValueIndex) {
            case MadAnchor.Mode.ObjectAnchor:
                MadGUI.PropertyField(anchorObject, "Anchor Object", MadGUI.ObjectIsSet);
                MadGUI.PropertyField(anchorCamera, "Anchor Camera", property => property.objectReferenceValue != null || HasMainCamera());

                if (!HasMainCamera()) {
                    GUIMissingMainCameraWarning();
                } else if (anchorCamera.objectReferenceValue == null) {
                    GUIMainCameraWillBeUsed();
                }
                break;
                
            case MadAnchor.Mode.ScreenAnchor:
                MadGUI.PropertyField(position, "Position");
                break;
                
            default:
                MadDebug.Assert(false, "Unknown mode: " + (MadAnchor.Mode) mode.enumValueIndex);
                break;
        }
        
        so.ApplyModifiedProperties();
    }

    private static bool HasMainCamera() {
        return Camera.main != null;
    }

    private static void GUIMissingMainCameraWarning() {
        MadGUI.Warning("There's no camera tagged as MainCamera on this scene. " +
                       "Please make sure that there is one or choose any other camera using the field above.");
    }

    private static void GUIMainCameraWillBeUsed() {
        MadGUI.Info("MainCamera is used by default.");
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif