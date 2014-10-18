/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//#if !UNITY_3_5
namespace MadLevelManager {
//#endif

[System.Serializable]
public class MadLevelExtension {

    #region Public Fields

    public string name = "";
    public string guid = "0";

    public List<MadLevelScene> scenesBefore = new List<MadLevelScene>();
    public List<MadLevelScene> scenesAfter = new List<MadLevelScene>();

    #endregion

    #region Public Methods

    public MadLevelExtension(string name) {
        this.name = name;
        guid = System.Guid.NewGuid().ToString();
    }

    public void Load(MadLevelConfiguration.Level level) {
        MadLevel.currentExtension = this;
        MadLevel.currentExtensionProgress = 0;

        if (scenesBefore.Count != 0) {
            var scene = scenesBefore[0];
            MadLevel.lastPlayedLevelName = MadLevel.currentLevelName;
            MadLevel.currentLevelName = level.name;
            scene.Load();
        } else {
            level.Load();
        }
    }

    public AsyncOperation LoadAsync(MadLevelConfiguration.Level level) {
        MadLevel.currentExtension = this;
        MadLevel.currentExtensionProgress = 0;

        if (scenesBefore.Count != 0) {
            var scene = scenesBefore[0];
            MadLevel.lastPlayedLevelName = MadLevel.currentLevelName;
            MadLevel.currentLevelName = level.name;
            return scene.LoadAsync();
        } else {
            return level.LoadAsync();
        }
    }

    public void Continue(MadLevelScene currentLevel, int progress) {
        if (!CheckCanContinue(currentLevel, progress)) {
            return;
        }

        var scenes = ScenesInOrder(currentLevel);
        var scene = scenes[progress + 1];

        MadLevel.currentExtensionProgress = progress + 1;
        scene.Load();
    }

    public AsyncOperation ContinueAsync(MadLevelScene currentLevel, int progress) {
        if (!CheckCanContinue(currentLevel, progress)) {
            return null;
        }

        var scenes = ScenesInOrder(currentLevel);
        var scene = scenes[progress + 1];

        MadLevel.currentExtensionProgress = progress + 1;
        return scene.LoadAsync();
    }

    private bool CheckCanContinue(MadLevelScene currentLevel, int progress) {
        if (!CanContinue(currentLevel, progress)) {
            Debug.LogError(
                "Cannot continue the level, this is the last scene. Please use MadLevel.CanContinue() to check "
                + "if level can be continued. Now you have to use MadLevel.LoadNext(), MadLevel.LoadLevelByName() or similar.");
            return false;
        }

        return true;
    }

    public bool CanContinue(MadLevelScene currentLevel, int progress) {
        var scenes = ScenesInOrder(currentLevel);
        if (scenes.Count > progress + 1) {
            return true;
        }

        return false;
    }

    private List<MadLevelScene> ScenesInOrder(MadLevelScene currentLevel) {
        List<MadLevelScene> scenes = new List<MadLevelScene>();
        scenes.AddRange(scenesBefore);
        scenes.Add(currentLevel);
        scenes.AddRange(scenesAfter);

        return scenes;
    }

    #endregion

    internal bool IsValid() {
        foreach (var s in scenesBefore) {
            if (!s.IsValid()) {
                return false;
            }
        }

        foreach (var s in scenesAfter) {
            if (!s.IsValid()) {
                return false;
            }
        }

        return true;
    }
}

//#if !UNITY_3_5
} // namespace
//#endif