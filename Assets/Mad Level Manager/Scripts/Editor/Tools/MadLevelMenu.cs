/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {
 
// this class holds most of menu entries   
public class MadLevelMenu : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================
    
    private const string HomePage =
        "http://madlevelmanager.madpixelmachine.com/documentation.html";

    private const string Themes =
        "http://madlevelmanager.madpixelmachine.com/themes.html";

    // ===========================================================
    // Fields
    // ===========================================================
 
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    [MenuItem("Tools/Mad Level Manager/Initialize", false, 100)]
    static void InitTool() {
        EditorWindow.GetWindow<MadLevelInitTool>(false, "Init Tool", true);
    }
    
    [MenuItem ("Tools/Mad Level Manager/Create Font", false, 120)]
    static void CreateFont() {
        MadFontBuilder.CreateFont();
    }
    
    [MenuItem("Tools/Mad Level Manager/Create Atlas", false, 121)]
    static void CreateAtlas() {
        MadAtlasBuilder.CreateAtlas();
    }
    
    [MenuItem ("Tools/Mad Level Manager/Create UI/Sprite", false, 140)]
    [MenuItem ("GameObject/Create Other/Mad Level Manager/UI Sprite", false, 10000)]
    static void CreateSprite() {
        var sprite = MadTransform.CreateChild<MadSprite>(ActiveParentOrPanel(), "sprite");
        Selection.activeGameObject = sprite.gameObject;
    }
    
    [MenuItem ("Tools/Mad Level Manager/Create UI/Text", false, 141)]
    [MenuItem ("GameObject/Create Other/Mad Level Manager/UI Text", false, 10001)]
    static void CreateText() {
        var text = MadTransform.CreateChild<MadText>(ActiveParentOrPanel(), "text");
        Selection.activeGameObject = text.gameObject;
    }
    
    [MenuItem ("Tools/Mad Level Manager/Create UI/Anchor", false, 142)]
    [MenuItem ("GameObject/Create Other/Mad Level Manager/UI Anchor", false, 10002)]
    static void CreateAnchor() {
        var anchor = MadTransform.CreateChild<MadAnchor>(ActiveParentOrPanel(), "Anchor");
        Selection.activeGameObject = anchor.gameObject;
    }
    
    [MenuItem("Tools/Mad Level Manager/Create UI/Background", false, 149)]
    static void CreateBackground() {
        var freeLayout = GameObject.FindObjectOfType(typeof(MadLevelFreeLayout));
        if (freeLayout != null) {
            EditorUtility.DisplayDialog(
                "Create Background",
                "Free layout has different method of creating backgrounds (this is temporary and will be fixed in future versions). "
                + "You should look for a 'Background Texture' field in the free layout inspector.", "OK");
            Selection.activeObject = freeLayout;
            return;
        }

        MadLevelBackgroundTool.ShowWindow();
    }
    
    static Transform ActiveParentOrPanel() {
        Transform parentTransform = null;

        var transforms = Selection.transforms;
        if (transforms.Length > 0) {
            var firstTransform = transforms[0];
            if (firstTransform.GetComponent<MadPanel>() != null || MadTransform.FindParent<MadPanel>(firstTransform) != null) {
                parentTransform = firstTransform;
            }
        }

        if (parentTransform == null) {
            var selected = Selection.activeGameObject;
            Transform selectedTransform = null;

            if (selected != null) {
                selectedTransform = selected.transform;
            }

            var panel = MadPanel.FirstOrNull(selectedTransform);
            if (panel != null) {
                parentTransform = panel.transform;
            }
        }

        return parentTransform;
    }
    
    [MenuItem("Tools/Mad Level Manager/Create Level Icon", false, 150)]      
    static void CreateIcon() {
        MadLevelIconTool.Display();
    }
    
    //[MenuItem("Tools/Mad Level Manager/Create Level Property/Empty", false, 155)]
    //static void CreateEmptyProperty() {
    //    ScriptableWizard.DisplayWizard<MadLevelPropertyEmptyTool>("Create Empty Property", "Create");
    //}
    
    //[MenuItem("Tools/Mad Level Manager/Create Level Property/Sprite", false, 160)]
    //static void CreateSpriteProperty() {
    //    ScriptableWizard.DisplayWizard<MadLevelPropertySpriteTool>("Create Sprite Property", "Create");
    //}
    
    //[MenuItem("Tools/Mad Level Manager/Create Level Property/Text", false, 165)]
    //static void CreateTextProperty() {
    //    ScriptableWizard.DisplayWizard<MadLevelPropertyTextTool>("Create Text Property", "Create");
    //}

    [MenuItem("Tools/Mad Level Manager/Animations/Add Animator", false, 155)]
    static void AddAnimator() {
        var go = Selection.activeGameObject;
        if (!CheckAnimationValidObject(go)) {
            return;
        }

        if (go.GetComponent<MadLevelIcon>() != null) {
            go.AddComponent<MadLevelAnimator>();
        } else if (go.GetComponent<MadSprite>() != null) {
            go.AddComponent<MadAnimator>();
        } else {
            EditorUtility.DisplayDialog("Cannot Add Animator", "Animator can be added only to level icons or sprites.", "OK");
        }
    }

    [MenuItem("Tools/Mad Level Manager/Animations/Add Move Animation", false, 170)]
    static void AddMoveAnimation() {
        var go = Selection.activeGameObject;
        if (!CheckAnimationValidObject(go)) {
            return;
        }

        go.AddComponent<MadAnimMove>();
    }

    [MenuItem("Tools/Mad Level Manager/Animations/Add Rotate Animation", false, 171)]
    static void AddRotateAnimation() {
        var go = Selection.activeGameObject;
        if (!CheckAnimationValidObject(go)) {
            return;
        }

        go.AddComponent<MadAnimRotate>();
    }

    [MenuItem("Tools/Mad Level Manager/Animations/Add Scale Animation", false, 172)]
    static void AddScaleAnimation() {
        var go = Selection.activeGameObject;
        if (!CheckAnimationValidObject(go)) {
            return;
        }

        go.AddComponent<MadAnimScale>();
    }

    [MenuItem("Tools/Mad Level Manager/Animations/Add Color Animation", false, 173)]
    static void AddColorAnimation() {
        var go = Selection.activeGameObject;
        if (!CheckAnimationValidObject(go)) {
            return;
        }

        go.AddComponent<MadAnimColor>();
    }

    static bool CheckAnimationValidObject(GameObject go) {
        if (go == null) {
            EditorUtility.DisplayDialog("Cannot Add Animation", "Please select object with MadSprite or MadLevelIcon component attached.", "OK");
            return false;
        }

        return true;
    }
    
    [MenuItem("Tools/Mad Level Manager/Grid Layout/Create", false, 200)]
    static void CreateGridLayout() {
        var panel = MadPanel.UniqueOrNull();
        if (panel != null) {
            MadLevelGridTool.CreateUnderPanel(panel);
        } else {
            ScriptableWizard.DisplayWizard<MadLevelGridTool>("Create Grid Layout", "Create");
        }
    }

    [MenuItem("Tools/Mad Level Manager/Grid Layout/Add Input Control", false, 300)]
    [MenuItem("Tools/Mad Level Manager/Free Layout/Add Input Control", false, 300)]
    static void CreateKeyboardControl() {
        var inputControl = MadTransform.CreateChild<MadLevelInputControl>(null, "Input Control");
        Selection.activeGameObject = inputControl.gameObject;
    }

    [MenuItem("Tools/Mad Level Manager/Grid Layout/Create Bullets", false, 301)]
    static void CreateBullets() {
        var panel = MadPanel.UniqueOrNull();
        var bullets = MadLevelGridBulletsTool.Create(panel);
        Selection.activeObject = bullets.gameObject;
    }

    [MenuItem("Tools/Mad Level Manager/Free Layout/Create", false, 200)]
    static void CreateFreeLayout() {
        var panel = MadPanel.UniqueOrNull();
        if (panel != null) {
            MadLevelFreeTool.CreateUnderPanel(panel);
        } else {
            ScriptableWizard.DisplayWizard<MadLevelFreeTool>("Create Free Layout", "Create");
        }
    }

    [MenuItem("Tools/Mad Level Manager/Prepare for UnityScript and Boo", false, 899)]
    static void MoveAssetsTool() {
        MadLevelMoveAssetsTool.Execute();
    }
    
    [MenuItem("Tools/Mad Level Manager/Profile Tool", false, 900)]
    static void ProfileTool() {
        EditorWindow.GetWindow<MadLevelProfileTool>(false, "Profile Tool", true);
    }

    [MenuItem("Tools/Mad Level Manager/Profile Browser", false, 901)]
    static void ProfileBrowser() {
        MadLevelProfileBrowser.Show();
    }

    [MenuItem("Tools/Mad Level Manager/Extension Editor", false, 902)]
    static void ExtensionEditor() {
        MadLevelExtensionEditor.Show(MadLevel.activeConfiguration);
    }
    
    [MenuItem("Tools/Mad Level Manager/Select Active Configuration", false, 903)]
    static void SelectActiveConfiguration() {
        var active = MadLevelConfiguration.FindActive();
        if (active == null) {
            EditorUtility.DisplayDialog("Not Found", "No active configuration found.", "OK");
        } else {
//            EditorGUIUtility.PingObject(active);
            Selection.activeObject = active;
        }
    }

    [MenuItem("Tools/Mad Level Manager/Get Additional Themes", false, 999)]
    static void OpenThemes() {
        Application.OpenURL(Themes);
    }

    [MenuItem("Tools/Mad Level Manager/Manual", false, 1000)]
    static void OpenHomepage() {
        Application.OpenURL(HomePage);
    }

    [MenuItem("Tools/Mad Level Manager/About", false, 1001)]
    static void OpenAbout() {
        string trialInfo = "";

        if (MadTrialEditor.isTrialVersion) {
            if (MadTrialEditor.expired) {
                trialInfo = "\n\nYour evaluation period has expired!";
            } else {
                int daysLeft = MadTrialEditor.DaysLeft();
                trialInfo = "\n\nYour have " + daysLeft + " evaluation days left.";
            }
            
        }

        EditorUtility.DisplayDialog("Mad Level Manager",
            "Copyright (c) Mad Pixel Machine\nVersion: 2.2.2a\n\nhttp://madlevelmanager.madpixelmachine.com/" + trialInfo,
            "OK");
    }

#if !true
    [MenuItem("Tools/Mad Level Manager/Debug/Reset Update Data", false, 10000)]
    static void ResetUpdateData() {
        MadLevelUpdater.ResetUpdateData();
    }
#endif

    #if TRIAL

    [MenuItem("Tools/Mad Level Manager/Request Evaluation Key", false, 10001)]
    public static void Extend() {
        MadTrialEditor.RequestExtend("Mad Level Manager");
    }

    #endif
    
    //
    // validators
    //
    
    [MenuItem("Tools/Mad Level Manager/Create Sprite", true)]
    [MenuItem("Tools/Mad Level Manager/Create Level Icon", true)]
    [MenuItem("Tools/Mad Level Manager/Create Property/Empty", true)]
    [MenuItem("Tools/Mad Level Manager/Create Property/Sprite", true)]
    [MenuItem("Tools/Mad Level Manager/Create Property/Text", true)]
    [MenuItem("Tools/Mad Level Manager/Create Grid Layout", true)]
    [MenuItem("Tools/Mad Level Manager/Create Free Layout", true)]
    [MenuItem("Tools/Mad Level Manager/Grid Layout/Create Bullets", true)]
    static bool ValidateHasPanel() {
        return MadPanel.UniqueOrNull() != null;
    }

    [MenuItem("Tools/Mad Level Manager/Grid Layout/Create Bullets", true)]
    static bool ValidateHasGridLayout() {
        var panel = MadPanel.UniqueOrNull();
        if (panel == null) {
            return false;
        }

        var gridLayout = MadTransform.FindChild<MadLevelGridLayout>(panel.transform);
        return gridLayout != null;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

}