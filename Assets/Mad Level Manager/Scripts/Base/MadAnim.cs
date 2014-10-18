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

public abstract class MadAnim : MonoBehaviour {

    #region Public Fields

    public string animationName = "New Animation";

    public MadiTween.EaseType easing = MadiTween.EaseType.easeOutCubic;

    // duration if this animation
    public float duration = 1;

    // the time after the animation will start in seconds
    public float delay = 0;

    // sets the animation offset at start, 0 - 1
    public float offset = 0;

    public WrapMode wrapMode = WrapMode.Once;

    public bool queue = false;

    public bool playOnAwake = false;

    public bool destroyObjectOnFinish;

    public bool sendMessageOnFinish;

    public GameObject messageReceiver;

    public string messageName;

    public bool playAnimationOnFinish;

    public string playAnimationOnFinishName;

    public bool playAnimationOnFinishFromTheBeginning;

    #endregion

    #region Private Fields

    private float delayTime;

    private float playTime;

    private bool firstFrame = true;

    private bool startAnimInvoked;

    private bool hasOrigin = false;

    private string animationQueue;

    #endregion

    #region Properties

    public bool isPlaying {
        get;
        private set;
    }

    public bool isDelaying { get; private set; }

    #endregion

    #region Public Methods

    public void Play() {
        if (isPlaying) {
            return;
        }

        if (delayTime >= delay) {
            TryStartPlaying();
        } else {
            isDelaying = true;
        }

        firstFrame = true;
    }

    public void PlayNow() {
        if (isPlaying) {
            return;
        }

        isDelaying = false;
        firstFrame = true;
        TryStartPlaying();
    }

    public void Stop() {
        isPlaying = false;
    }

    public void Reset() {
        //Anim(0);
        delayTime = 0;
        playTime = offset * duration;
        startAnimInvoked = false;
    }

    #endregion

    #region Other Methods

    protected abstract void Anim(float progress);

    protected virtual void Start() {
        if (!hasOrigin) {
            UpdateOrigin();
        }

        if (playOnAwake) {
            Play();
        }
    }

    private void TryStartPlaying() {

        var otherAnimation = OtherAnimationPlaying();

        if (otherAnimation != null) {
            if (!queue) {
#if !true
                //Debug.Log("Stopping animation " + otherAnimation + " " + otherAnimation.animationName
                //    + " for " + this + " " + animationName, this);
#endif
                otherAnimation.Stop();
                isPlaying = true;
            } else {
                if (!string.IsNullOrEmpty(animationQueue)) {
                    Debug.LogWarning("Animation queue cannot contain more than one animation. "
                        + "Please review your animations density.");
                }

                otherAnimation.animationQueue = animationName;
                isPlaying = false;
            }
        } else {
            isPlaying = true;
        }
    }

    private void Update() {
        if (firstFrame) {
            firstFrame = false;
        } else {
            if (isDelaying) {
                delayTime += Time.deltaTime;

                if (delayTime >= delay) {
                    isDelaying = false;
                    playTime += delayTime - delay;

                    TryStartPlaying();
                }
            } else if (isPlaying) {
                playTime += Time.deltaTime;
            }
        }

        if (isPlaying) {
            if (!startAnimInvoked) {
                StartAnim();
                startAnimInvoked = true;
            }

            switch (wrapMode) {
                case WrapMode.Once:
                    AnimWrapOnce(playTime);
                    break;

                case WrapMode.Loop:
                    AnimLoop(playTime);
                    break;

                case WrapMode.PingPong:
                    AnimPingPong(playTime);
                    break;

                case WrapMode.ClampForever:
                    AnimClampForever(playTime);
                    break;

                default:
                    Debug.LogError("Unknown wrap mode: " + wrapMode);
                    break;
            }
            
        }
    }

    private MadAnim OtherAnimationPlaying() {
        return PlayingAnimation(gameObject, GetType());
    }

    private void AnimWrapOnce(float animTime) {
        if (animTime < duration) {
            float progress = animTime / duration;
            Anim(progress);
        } else if (animTime > duration) {
            Finish();
        }
    }

    private void AnimLoop(float animTime) {
        animTime %= duration;

        float progress = animTime / duration;
        Anim(progress);
    }

    private void AnimPingPong(float animTime) {
        animTime %= (duration * 2);

        float progress = animTime / duration;
        if (progress > 1) {
            progress = 2 - progress;
        }
        Anim(progress);
    }

    private void AnimClampForever(float animTime) {
        float progress = Mathf.Clamp(animTime / duration, 0, 1);
        Anim(progress);
    }

    protected abstract void StartAnim();

    public virtual void UpdateOrigin() {
        hasOrigin = true;
    }

    private void Finish() {
        Anim(1);

        if (sendMessageOnFinish) {
            GameObject receiver = gameObject;
            if (messageReceiver != null) {
                receiver = messageReceiver;
            }

            receiver.SendMessage(messageName);
        }

        if (playAnimationOnFinish && !string.IsNullOrEmpty(playAnimationOnFinishName)) {
            MadAnim.PlayAnimation(gameObject, playAnimationOnFinishName, playAnimationOnFinishFromTheBeginning);
        }

        if (destroyObjectOnFinish) {
            MadGameObject.SafeDestroy(gameObject);
        }
        isPlaying = false;

        if (!string.IsNullOrEmpty(animationQueue)) {
            PlayAnimationNow(gameObject, animationQueue);
        }
    }

    #endregion

    #region Static Methods

    public static int PlayAnimation(GameObject gameObject, string animationName, bool fromTheBeginning = false) {
        var animations = FindAnimations(gameObject, animationName);
        for (int i = 0; i < animations.Count; ++i) {
            var anim = animations[i];
            if (fromTheBeginning) {
                anim.Reset();
            }

            anim.Play();
        }

        return animations.Count;
    }

    public static int PlayAnimationNow(GameObject gameObject, string animationName, bool fromTheBeginning = false) {
        var animations = FindAnimations(gameObject, animationName);
        for (int i = 0; i < animations.Count; ++i) {
            var anim = animations[i];
            if (fromTheBeginning) {
                anim.Reset();
            }

            anim.PlayNow();
        }

        return animations.Count;
    }

    public static int StopAnimation(GameObject gameObject, string animationName) {
        var animations = FindAnimations(gameObject, animationName);
        for (int i = 0; i < animations.Count; ++i) {
            animations[i].Stop();
        }

        return animations.Count;
    }

    public static List<MadAnim> FindAnimations(GameObject gameObject, string name) {
        var animations = gameObject.GetComponents<MadAnim>();
        var query = from anim in animations where anim.animationName == name select anim;
        return query.ToList();
    }

    public static List<MadAnim> AllAnimations(GameObject gameObject) {
        return gameObject.GetComponents<MadAnim>().ToList();
    }

    public static MadAnim PlayingAnimation(GameObject gameObject, Type type) {
        var animations = gameObject.GetComponents(type);
        for (int i = 0; i < animations.Length; ++i) {
            var animation = animations[i] as MadAnim;
            if (animation.isPlaying) {
                return animation;
            }
        }

        return null;
    }

    public static T PlayingAnimation<T>(GameObject gameObject) where T : MadAnim {
        var animations = gameObject.GetComponents<T>();
        for (int i = 0; i < animations.Length; ++i) {
            var animation = animations[i];
            if (animation.isPlaying) {
                return animation;
            }
        }

        return null;
    }

    #endregion

    #region Types

    public enum WrapMode {
        Once,
        Loop,
        PingPong,
        ClampForever,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif