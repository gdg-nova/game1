/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelLoadingScreen : MonoBehaviour {

    #region Constants

    #endregion


    #region Public Fields

    /// <summary>
    /// Number of frame that will trigger the level loading start.
    /// It may be important to delay the level loading, because of other scripts that are present on the loading scene.
    /// They may need one or two frames to setup the graphics.
    /// 
    /// If your scripts needs more or less time, you can try to modify this parameter.
    /// </summary>
    public int notAsyncLoadingStartFrame = 3;

    public bool asyncLoading;

    public string testModeLevelToLoad = "";


    [NonSerialized]
    public MadLevelConfiguration.Level nextLevel;


    [NonSerialized]
    public AsyncOperation asyncOperation;

    #endregion

    #region Private Methods

    // current frame number
    private int frameNumber = 0;

    private bool testMode;

    #endregion

    #region Public Properties

    public float progress {
        get {
            if (asyncOperation != null) {
                return asyncOperation.progress;
            } else {
                return 0;
            }
        }
    }

    public bool isDone {
        get {
            if (asyncOperation != null) {
                return asyncOperation.isDone;
            } else {
                return false;
            }
        }
    }

    public bool isTestMode {
        get {
            return testMode;
        }
    }

	#endregion


    #region Unity Methods

    private void Start() {
#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1)
        if (!Application.HasProLicense()) {
            asyncLoading = false;
        }
#endif

        if (!MadLevel.hasExtension) {
            InitTestMode();
        }

        InitFinalize();
    }

    private void Update() {
        frameNumber++;

        if (!asyncLoading) {
            if (frameNumber >= notAsyncLoadingStartFrame) {
                if (MadLevel.hasExtension && MadLevel.CanContinue()) {
                    MadLevel.Continue();
                } else {
                    Debug.LogWarning("Level loading screen is meant to be in extension as 'before' scene.");
                    MadLevel.LoadNext();
                }
            }
        }
        
    }

    #endregion

    #region Private Methods

    private void InitTestMode() {
        Debug.Log("Initializing test mode");
        testMode = true;

        if (string.IsNullOrEmpty(testModeLevelToLoad)) {
            Debug.LogError("Test level name not set");
        } else {
            nextLevel = MadLevel.activeConfiguration.FindLevelByName(testModeLevelToLoad);
            if (nextLevel == null) {
                Debug.LogError("Cannot find level with name " + testModeLevelToLoad);
            }
        }
    }

    private void InitFinalize() {
        if (asyncLoading) {
            if (MadLevel.hasExtension && MadLevel.CanContinue()) {
                asyncOperation = MadLevel.ContinueAsync();
            } else {
                Debug.LogWarning("Level loading screen is meant to be in extension as 'before' scene.");
                asyncOperation = MadLevel.LoadNextAsync();
            }
        }
    }

    #endregion

}

#if !UNITY_3_5
} // namespace
#endif