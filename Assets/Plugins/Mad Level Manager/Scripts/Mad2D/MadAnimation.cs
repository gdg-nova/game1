/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadAnimation : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Action onMouseEnter;
    public Action onMouseExit;
    public Action onTouchEnter;
    public Action onTouchExit;
    public Action onFocus;
    public Action onFocusLost;
    
    MadSprite sprite;
    
    Vector3 origPosition, origRotation, origScale;
    Color origTint;
    bool hasOrigs = false;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void Start() {
        sprite = GetComponent<MadSprite>();
        if (sprite != null) {
            sprite.onMouseEnter += (s) => AnimOnMouseEnter();
            sprite.onMouseExit += (s) => AnimOnMouseExit();
            sprite.onTouchEnter += (s) => AnimOnTouchEnter();
            sprite.onTouchExit += (s) => AnimOnTouchExit();
            sprite.onFocus += (s) => PlayOnFocus();
            sprite.onFocusLost += (s) => PlayOnFocusLost();
        } else {
            Debug.LogError("This component must be attached with sprite!", this);
        }
    }
    
    void AnimOnMouseEnter() {
        UpdateOrigs();
        PlayAction(onMouseEnter);
    }
    
    void AnimOnMouseExit() {
        UpdateOrigs();
        PlayAction(onMouseExit);
    }

    void AnimOnTouchEnter() {
        UpdateOrigs();
        PlayAction(onTouchEnter);
    }

    void AnimOnTouchExit() {
        UpdateOrigs();
        PlayAction(onTouchExit);
    }
    
    void PlayOnFocus() {
        UpdateOrigs();
        PlayAction(onFocus);
    }
    
    void PlayOnFocusLost() {
        UpdateOrigs();
        PlayAction(onFocusLost);
    }
    
    void UpdateOrigs() {
        if (!hasOrigs) {
            origPosition = transform.localPosition;
            origRotation = transform.localRotation.eulerAngles;
            origScale = transform.localScale;
            origTint = sprite.tint;
            hasOrigs = true;
        }
    }
    
    void PlayAction(Action action) {
        if (!action.enabled) {
            return;
        }
        
        sprite.AnimMoveTo(origPosition + action.move, action.time, action.easeType);
        sprite.AnimRotateTo(origRotation + action.rotate, action.time, action.easeType);
        sprite.AnimScaleTo(Vector3.Scale(origScale, action.scale), action.time, action.easeType);
        
        if (action.tintEnabled) {
            if (action.tint.useBase) {
                sprite.AnimColorTo(origTint, action.time, action.easeType);
            } else {
                sprite.AnimColorTo(action.tint.color, action.time, action.easeType);
            }
        }

        if (action.playSound != null) {
            AudioSource.PlayClipAtPoint(action.playSound, Camera.main.transform.position, action.playSoundVolume);
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    [System.Serializable]
    public class Action {
        public bool enabled;
        public MadiTween.EaseType easeType = MadiTween.EaseType.easeOutElastic;
        public Vector3 move;
        public Vector3 rotate;
        public Vector3 scale = Vector3.one;
        public float time = 1f;
        public bool tintEnabled = false;
        public Tint tint = new Tint();
        public AudioClip playSound;
        public float playSoundVolume = 1;
        
        [System.Serializable]
        public class Tint {
            public bool useBase = false;
            public Color color = Color.white;
        }
    }
    
}

#if !UNITY_3_5
} // namespace
#endif