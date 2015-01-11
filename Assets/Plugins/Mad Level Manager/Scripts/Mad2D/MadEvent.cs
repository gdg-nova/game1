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

using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadEvent : MonoBehaviour {

    #region Public Fields

    public EventType eventType;

    public string message;
    public GameObject messageReceiver;

    #endregion

    #region Unity Methods

    private void Start() {
        var sprite = GetComponent<MadSprite>();
        if (sprite == null) {
            Debug.LogError("This component must be attached along with MadSprite component!");
            return;
        }

        switch (eventType) {
            case EventType.OnMouseEnter:
                sprite.onMouseEnter += (s) => Invoke();
                break;
            case EventType.OnMouseExit:
                sprite.onMouseExit += (s) => Invoke();
                break;
            case EventType.OnMouseDown:
                sprite.onMouseDown += (s) => Invoke();
                break;
            case EventType.OnMouseUp:
                sprite.onMouseUp += (s) => Invoke();
                break;
            case EventType.OnTap:
                sprite.onTap += (s) => Invoke();
                break;
            case EventType.OnFocusGain:
                sprite.onFocus += (s) => Invoke();
                break;
            case EventType.OnFocusLost:
                sprite.onFocusLost += (s) => Invoke();
                break;
            default:
                Debug.LogError("Unknown event type: " + eventType);
                break;
        }
    }

    #endregion

    #region Private Methods

    private void Invoke() {
        if (messageReceiver != null) {
            messageReceiver.SendMessage(message);
        } else {
            SendMessage(message);
        }
    }
    
    #endregion

    #region Public Static Methods
    #endregion

    #region Inner and Anonymous Classes

    public enum EventType {
        OnMouseEnter,
        OnMouseExit,
        OnMouseDown,
        OnMouseUp,
        OnTap,
        OnFocusGain,
        OnFocusLost,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif