/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadAnimator : MonoBehaviour {

    #region Public Fields

    public Action onMouseEnter = new Action();
    public Action onMouseExit = new Action();
    public Action onTouchEnter = new Action();
    public Action onTouchExit = new Action();
    public Action onFocus = new Action();
    public Action onFocusLost = new Action();

    #endregion

    #region Slots

    protected virtual void Start() {
        var sprite = GetComponent<MadSprite>();
        if (sprite != null) {
            sprite.onMouseEnter += (s) => onMouseEnter.Execute(gameObject);
            sprite.onMouseExit += (s) => onMouseExit.Execute(gameObject);
            sprite.onTouchEnter += (s) => onTouchEnter.Execute(gameObject);
            sprite.onTouchExit += (s) => onTouchExit.Execute(gameObject);
            sprite.onFocus += (s) => onFocus.Execute(gameObject);
            sprite.onFocusLost += (s) => onFocusLost.Execute(gameObject);
        } else {
            Debug.LogError("This component must be attached with sprite!", this);
        }
    }

    void Update() {
    }

    #endregion

    #region Inner and Anonymous Classes

    [Serializable]
    public class Action {
        public List<AnimationRef> playAnimations = new List<AnimationRef>();
        public List<AnimationRef> stopAnimations = new List<AnimationRef>();
        public bool stopAllAnimations;

        public void Execute(GameObject parent) {
            if (stopAllAnimations) {
                StopAllAnimations(parent);
            } else {
                StopAnimations(parent);
            }

            PlayAnimations(parent);
        }

        private void PlayAnimations(GameObject parent) {
            for (int i = 0; i < playAnimations.Count; ++i) {
                var anim = playAnimations[i];
                string name = anim.name;
                bool reset = anim.fromTheBeginning;

                int playedCount = MadAnim.PlayAnimation(parent, name, reset);
                if (playedCount == 0) {
                    Debug.LogWarning("There's no animation with name '" + name + "'.");
                }
            }
        }

        private void StopAnimations(GameObject parent) {
            for (int i = 0; i < stopAnimations.Count; ++i) {
                var anim = stopAnimations[i];
                string name = anim.name;
                //bool reset = anim.fromTheBeginning;

                int playedCount = MadAnim.StopAnimation(parent, name);
                if (playedCount == 0) {
                    Debug.LogWarning("There's no animation with name '" + name + "'.");
                }
            }
        }

        private void StopAllAnimations(GameObject parent) {
            var animations = MadAnim.AllAnimations(parent);
            for (int i = 0; i < animations.Count; ++i) {
                var animation = animations[i];
                animation.Stop();
            }
        }
    }

    [Serializable]
    public class AnimationRef {
        public string name;
        public bool fromTheBeginning = true;
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif