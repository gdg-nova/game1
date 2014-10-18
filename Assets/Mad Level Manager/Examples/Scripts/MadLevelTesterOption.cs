/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

// this is example code of how use Mad Level Manager code in your game
public class MadLevelTesterOption : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public int points; // number of points that this option gives
    
    bool completed = false;

    MadLevelTesterController controller;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<MadLevelTesterController>();

        var sprite = GetComponent<MadSprite>();

        // connect to sprite onMouseDown && onTap event
        sprite.onMouseUp += sprite.onTap = (s) => {
        
            // star_1, star_2 and start_3 properties are names or game objects
            // that are nested under every icon in the level select screen
            
            // note that if you've gained 3 stars already there's no possibility to lost them

            // If you want to create a sprite that should change depending on property value (like medals)
            // here you can see how to do that. There's property called "medal" and it's set to bronze, silver or gold
            // depending on which medal should be gained. Please look for medals example scene
        
            // first star gained when points number is at least 100
            if (points >= 100) {
                EarnStar("star_1");
                EarnMedal("bronze");
                MarkLevelCompleted();
            }
            
            // second star gained when points number is at least 150
            if (points >= 150) {
                EarnStar("star_2");
                EarnMedal("silver");
                MarkLevelCompleted();
            }
            
            // third star gained when points number is at least 200
            if (points >= 200) {
                EarnStar("star_3");
                EarnMedal("gold");
                MarkLevelCompleted();
            }
            
            // play animation
            controller.PlayFinishAnimation(sprite, completed);
            StartCoroutine(WaitForAnimation());
        };
    }
    
    void EarnStar(string name) {
        // set level boolean sets level property of type boolean
        MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, name, true);
    }

    void EarnMedal(string name) {
        MadLevelProfile.SetLevelString(MadLevel.currentLevelName, "medal", name);
    }
    
    void MarkLevelCompleted() {
        // sets the level completed
        // by default next level will be unlocked
        MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
        
        // manual unlocking looks like this:
        // MadLevelProfile.SetLocked("level_name", false);
        
        // remembering that level is completed
        completed = true;
    }
    
    IEnumerator WaitForAnimation() {
        yield return new WaitForSeconds(2.2f); // animation lasts 2 second
        
        if (completed) {
            if (MadLevel.hasExtension && MadLevel.CanContinue()) { // check if this level has extension and can continue it
                // then continue
                MadLevel.Continue();
            } else if (MadLevel.HasNextInGroup(MadLevel.Type.Level)) { // if not extension, check if there is next level of type level
                // load it
                MadLevel.LoadNextInGroup(MadLevel.Type.Level);
            } else { // otherwise load level select screen
                controller.LoadLevelSelectScreen();
            }
        } else {
            // if not completed go back to the menu
            controller.LoadLevelSelectScreen();
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}