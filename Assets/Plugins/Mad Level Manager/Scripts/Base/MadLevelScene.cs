/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadLevelManager {

[System.Serializable]
public class MadLevelScene {

    #region Private Fields

    [SerializeField]
    private UnityEngine.Object _sceneObject;

    [SerializeField]
    private string _scenePath;

    [SerializeField]
    private string _sceneName;

    // deprecated fields
    [SerializeField]
    private string scene = "";

    #endregion

    #region Properties

    public UnityEngine.Object sceneObject {
        get {
            return _sceneObject;
        }
        set {
            if (!Application.isEditor) {
                Debug.LogError("This method has no effect when calling from play mode");
            }

#if UNITY_EDITOR
            if (_sceneObject == value) {
                return;
            }

            _sceneObject = value;

            UpdateScenePath();
            UpdateSceneName();
#endif
        }
    }

    public string scenePath {
        get {
            UpdateScenePath();
            return _scenePath;
        }
    }

    public string sceneName {
        get {
            UpdateSceneName();
            return _sceneName;
        }
    }

    #endregion

    #region Public Methods

    public virtual void Load() {
        Application.LoadLevel(sceneName);
    }

    public virtual AsyncOperation LoadAsync() {
        return Application.LoadLevelAsync(sceneName);
    }

    public void Upgrade() {
#if UNITY_EDITOR
        // moves scene paths to into scenes
        if (!string.IsNullOrEmpty(scene)) {
            var obj = AssetDatabase.LoadAssetAtPath("Assets/" + scene, typeof(UnityEngine.Object));
            if (obj != null) {
                sceneObject = obj;
                scene = "";
            }
        }
#endif
    }

    public virtual bool IsValid() {
        return sceneObject != null;
    }

    #endregion

    #region Private Methods

    private void UpdateScenePath() {
#if UNITY_EDITOR
        if (sceneObject != null) {
            _scenePath = AssetDatabase.GetAssetPath(sceneObject);
        }
#endif
    }

    private void UpdateSceneName() {
#if UNITY_EDITOR
        string path = scenePath;
        if (!string.IsNullOrEmpty(path)) {
            string basename = scenePath.Substring(_scenePath.LastIndexOf('/') + 1);
            _sceneName = basename.Substring(0, basename.IndexOf('.'));
        }
#endif
    }

    #endregion

}

}