/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelTesterController : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadText levelNameText;
    public MadText argumentsText;
    public MadText backToMenu;
    public MadText levelCompletedText;
    public MadText levelNotCompletedText;
    public MadSprite[] other;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
        levelNameText.text = "Level Name: '" + MadLevel.currentLevelName + "'";
        
        if (!string.IsNullOrEmpty(MadLevel.arguments)) {
            argumentsText.text = "Arguments: " + MadLevel.arguments;
        }
        
        backToMenu.onMouseDown += backToMenu.onTap = (sprite) => {
            LoadLevelSelectScreen();
        };
    }

    // loads level select screen or tries to found one
    public void LoadLevelSelectScreen() {
        // load Level Select if exists
        if (MadLevel.activeConfiguration.FindLevelByName("Level Select") != null) {
            MadLevel.LoadLevelByName("Level Select");
        } else {
            // if not, load first level of type other in this group
            string groupName = MadLevel.currentGroupName;
            var g = MadLevel.activeConfiguration.FindGroupByName(groupName);

            var query = from level
                in MadLevel.activeConfiguration.levels
                        where level.groupId == g.id && level.type == MadLevel.Type.Other
                        orderby level.order
                        select level;
            var levelFound = query.FirstOrDefault();

            if (levelFound != null) {
                MadLevel.LoadLevelByName(levelFound.name);
            } else {
                Debug.LogError("Cannot found level to get back to :-(");
            }
        }
    }

    public void PlayFinishAnimation(MadSprite chosenSprite, bool completed) {
        levelNameText.eventFlags = MadSprite.EventFlags.None;
        argumentsText.eventFlags = MadSprite.EventFlags.None;
        backToMenu.eventFlags = MadSprite.EventFlags.None;
    
        Color transparent = new Color(1, 1, 1, 0);
        Color opaque = new Color(1, 1, 1, 1);
        
        levelNameText.AnimColorTo(transparent, 1, MadiTween.EaseType.linear);
        argumentsText.AnimColorTo(transparent, 1, MadiTween.EaseType.linear);
        backToMenu.AnimColorTo(transparent, 1, MadiTween.EaseType.linear);
        
        if (completed) {
            levelCompletedText.tint = transparent;
            levelCompletedText.visible = true;
            levelCompletedText.AnimColorTo(opaque, 1, MadiTween.EaseType.linear);
        } else {
            levelNotCompletedText.tint = transparent;
            levelNotCompletedText.visible = true;
            levelNotCompletedText.AnimColorTo(opaque, 1, MadiTween.EaseType.linear);
        }
        
        foreach (var sprite in other) {
            var children = MadTransform.FindChildren<MadSprite>(sprite.transform);
            
            sprite.eventFlags = MadSprite.EventFlags.None;
            foreach (var s in children) {
                s.eventFlags = MadSprite.EventFlags.None;
            }
        
            if (sprite != chosenSprite) {
                sprite.AnimColorTo(transparent, 1, MadiTween.EaseType.linear);
                
                foreach (var s in children) {
                    s.AnimColorTo(transparent, 1, MadiTween.EaseType.linear);
                }
            }
        }
        
        chosenSprite.AnimMoveTo(new Vector3(), 1, MadiTween.EaseType.easeOutSine);
            
        MadiTween.ScaleTo(chosenSprite.gameObject, MadiTween.Hash(
            "scale", new Vector3(7, 7, 7),
            "time", 0.5f,
            "easetype", MadiTween.EaseType.easeInQuint,
            "delay", 1.5f
        ));
        
        MadiTween.ValueTo(chosenSprite.gameObject, MadiTween.Hash(
            "from", chosenSprite.tint,
            "to", transparent,
            "time", 0.5f,
            "onupdate", "OnTintChange",
            "easetype", MadiTween.EaseType.easeInQuint,
            "delay", 1.5f
        ));
            
        foreach (var s in MadTransform.FindChildren<MadSprite>(chosenSprite.transform)) {
            MadiTween.ValueTo(s.gameObject, MadiTween.Hash(
                "from", s.tint,
                "to", transparent,
                "time", 0.5f,
                "onupdate", "OnTintChange",
                    "easetype", MadiTween.EaseType.easeInQuint,
                "delay", 1.5f
            ));
        }
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