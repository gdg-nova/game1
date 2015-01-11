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

public class MadLevelInitTool : MadInitTool {

    // ===========================================================
    // Constants
    // ===========================================================

    const string IconPrefabGUID = "d26447d96e726434490779b5a3fcab28";
    const string SlideLeftPrefabGUID = "bf4251ffacf0daa46bd260901b6b77ee";
    const string SlideRightPrefabGUID = "640571df6b5f5244ea807ef008ac9985";

    // ===========================================================
    // Fields
    // ===========================================================
    
    Layout layout;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected override void OnFormGUI() {
        layout = (Layout) EditorGUILayout.EnumPopup("Screen Layout", layout);
    }

    protected override void AfterCreate(MadRootNode root) {
        root.gameObject.AddComponent<MadLevelRoot>();

        MadLevelIcon icon;
        MadSprite slideLeft, slideRight;

        switch (layout) {
            case Layout.Grid:
                InitTemplates(root, out icon, out slideLeft, out slideRight);
                CreateGrid(root, icon, slideLeft, slideRight);
                break;
            case Layout.Free:
                InitTemplates(root, out icon, out slideLeft, out slideRight);
                CreateFree(root, icon);
                break;
            case Layout.None:
                // do not add anything on the scene
                break;
            default:
                Debug.LogError("Unknown layout: " + layout);
                break;
        }

    }

    // ===========================================================
    // Methods
    // ===========================================================

    void CreateGrid(MadRootNode root, MadLevelIcon icon, MadSprite slideLeft, MadSprite slideRight) {
        var panel = MadTransform.FindChild<MadPanel>(root.transform);
        var gridLayout = MadLevelGridTool.CreateUnderPanel(panel);

        gridLayout.iconTemplate = icon;
        gridLayout.leftSlideSprite = slideLeft;
        gridLayout.rightSlideSprite = slideRight;
        gridLayout.dirty = true;
    }

    void CreateFree(MadRootNode root, MadLevelIcon icon) {
        var panel = MadTransform.FindChild<MadPanel>(root.transform);
        var freeLayout = MadLevelFreeTool.CreateUnderPanel(panel);

        freeLayout.iconTemplate = icon;
        freeLayout.dirty = true;
    }

    void InitTemplates(MadRootNode root, out MadLevelIcon icon, out MadSprite slideLeftSprite,
            out MadSprite slideRightSprite) {
        var panel = MadTransform.FindChild<MadPanel>(root.transform);
        var templates = MadTransform.CreateChild(panel.transform, "Templates");

        GameObject iconPrefab = MadAssets.TryLoadGameObject(IconPrefabGUID);
        GameObject slideLeftPrefab = MadAssets.TryLoadGameObject(SlideLeftPrefabGUID);
        GameObject slideRightPrefab = MadAssets.TryLoadGameObject(SlideRightPrefabGUID);

        if (MadGameObject.AnyNull(iconPrefab, slideLeftPrefab, slideRightPrefab)) {
            Debug.LogWarning("I cannot find all needed prefabs to create example templates. Have you removed Mad Level "
            + "Manager example prefabs?");
        }

        if (iconPrefab != null) {
            var obj = PrefabUtility.InstantiatePrefab(iconPrefab) as GameObject;
            obj.transform.parent = templates.transform;
            obj.name = "icon";

            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector2(-400, 150);
            icon = obj.GetComponent<MadLevelIcon>();
        } else {
            icon = null;
        }

        if (slideLeftPrefab != null) {
            var slide = PrefabUtility.InstantiatePrefab(slideLeftPrefab) as GameObject;
            slide.transform.parent = templates.transform;
            slide.name = "slide left";

            slide.transform.localScale = Vector3.one;
            slide.transform.localPosition = new Vector2(-400, 0);
            slideLeftSprite = slide.GetComponent<MadSprite>();
        } else {
            slideLeftSprite = null;
        }

        if (slideRightPrefab != null) {
            var slide = PrefabUtility.InstantiatePrefab(slideRightPrefab) as GameObject;
            slide.transform.parent = templates.transform;
            slide.name = "slide right";

            slide.transform.localScale = Vector3.one;
            slide.transform.localPosition = new Vector2(-400, -150);
            slideRightSprite = slide.GetComponent<MadSprite>();
        } else {
            slideRightSprite = null;
        }

        MadGameObject.SetActive(templates, false);
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

    public enum Layout {
        Grid,
        Free,
        None,
    }

}

#if !UNITY_3_5
} // namespace
#endif