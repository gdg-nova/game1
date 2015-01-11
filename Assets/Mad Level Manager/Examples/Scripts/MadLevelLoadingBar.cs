/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

public class MadLevelLoadingBar : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    private MadLevelLoadingScreen loadingScreen;
    private MadSprite bar;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
        loadingScreen = FindObjectOfType(typeof(MadLevelLoadingScreen)) as MadLevelLoadingScreen;
        bar = GetComponent<MadSprite>();
    }

    void Update() {
        bar.fillValue = loadingScreen.progress;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}