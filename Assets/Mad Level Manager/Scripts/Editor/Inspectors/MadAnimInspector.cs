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

public abstract class MadAnimInspector : Editor {

    #region Fields

    SerializedProperty animationName;
    SerializedProperty easing;
    SerializedProperty duration;
    SerializedProperty delay;
    SerializedProperty offset;
    SerializedProperty wrapMode;
    SerializedProperty queue;
    SerializedProperty playOnAwake;
    SerializedProperty destroyObjectOnFinish;
    SerializedProperty sendMessageOnFinish;
    SerializedProperty messageReceiver;
    SerializedProperty messageName;
    SerializedProperty playAnimationOnFinish;
    SerializedProperty playAnimationOnFinishName;
    SerializedProperty playAnimationOnFinishFromTheBeginning;

    #endregion

    #region Methods

    protected virtual void OnEnable() {
        animationName = serializedObject.FindProperty("animationName");
        easing = serializedObject.FindProperty("easing");
        duration = serializedObject.FindProperty("duration");
        delay = serializedObject.FindProperty("delay");
        offset = serializedObject.FindProperty("offset");
        wrapMode = serializedObject.FindProperty("wrapMode");
        queue = serializedObject.FindProperty("queue");
        playOnAwake = serializedObject.FindProperty("playOnAwake");
        destroyObjectOnFinish = serializedObject.FindProperty("destroyObjectOnFinish");
        sendMessageOnFinish = serializedObject.FindProperty("sendMessageOnFinish");
        messageReceiver = serializedObject.FindProperty("messageReceiver");
        messageName = serializedObject.FindProperty("messageName");
        playAnimationOnFinish = serializedObject.FindProperty("playAnimationOnFinish");
        playAnimationOnFinishName = serializedObject.FindProperty("playAnimationOnFinishName");
        playAnimationOnFinishFromTheBeginning = serializedObject.FindProperty("playAnimationOnFinishFromTheBeginning");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();

        MadGUI.PropertyField(animationName, "Animation Name");
        EditorGUILayout.Space();

        DrawInspector();
        EditorGUILayout.Space();

        MadGUI.PropertyField(duration, "Duration");
        MadGUI.PropertyField(delay, "Delay");
        MadGUI.PropertyFieldSlider(offset, 0, 1, "Offset");
        EditorGUILayout.Space();

        MadGUI.PropertyFieldEnumPopup(easing, "Easing");
        MadGUI.PropertyFieldEnumPopup(wrapMode, "Wrap Mode");
        MadGUI.PropertyField(queue, "Queue");
        EditorGUILayout.Space();

        MadGUI.PropertyField(playOnAwake, "Play On Awake");
        MadGUI.PropertyField(destroyObjectOnFinish, "Destroy On Finish");
        MadGUI.PropertyField(sendMessageOnFinish, "Send Message On Finish");

        if (sendMessageOnFinish.boolValue) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(messageReceiver, "Receiver", MadGUI.ObjectIsSet);
                MadGUI.PropertyField(messageName, "Method Name", MadGUI.StringNotEmpty);
            }
        }

        MadGUI.PropertyField(playAnimationOnFinish, "Play Animation On Finish");

        if (playAnimationOnFinish.boolValue) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(playAnimationOnFinishName, "Name", MadGUI.StringNotEmpty);
                MadGUI.PropertyField(playAnimationOnFinishFromTheBeginning, "From The Beginning");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void PropertyFieldAnimationName(SerializedProperty animationName, string label) {
        //string nameBefore = animationName.stringValue;
        MadGUI.PropertyField(animationName, "Animation Name");
        //string nameAfter = animationName.stringValue;

        //if (nameBefore != nameAfter) {
        //    UpdateAnimationName(nameBefore, nameAfter);
        //}
    }

    //private void UpdateAnimationName(string nameBefore, string nameAfter) {
    //    var animators = (target as Component).GetComponents<MadAnimator>();
    //    foreach (var a in animators) {
    //        bool u1 = false, u2 = false;
    //        u1 = UpdateAnimationName(nameBefore, nameAfter,
    //             a.onFocus, a.onFocusLost, a.onMouseEnter, a.onMouseExit, a.onTouchEnter, a.onTouchExit);
    //        if (a is MadLevelAnimator) {
    //            u2 = UpdateAnimationName(nameBefore, nameAfter,
    //                ((MadLevelAnimator) a).delayModifiers, ((MadLevelAnimator) a).offsetModifiers);
    //        }

    //        if (u1 || u2) {
    //            EditorUtility.SetDirty(a);
    //        }
    //    }
    //}

    //private bool UpdateAnimationName(string nameBefore, string nameAfter, params MadAnimator.Action[] actions) {
    //    bool updated = false;

    //    foreach (var action in actions) {
    //        foreach (var pa in action.playAnimations) {
    //            if (pa.name == nameBefore) {
    //                pa.name = nameAfter;
    //                updated = true;
    //            }
    //        }

    //        foreach (var sa in action.stopAnimations) {
    //            if (sa.name == nameBefore) {
    //                sa.name = nameAfter;
    //                updated = true;
    //            }
    //        }
    //    }

    //    return updated;
    //}

    //private bool UpdateAnimationName(string nameBefore, string nameAfter, params List<MadLevelAnimator.Modifier>[] modifiers) {
    //    bool updated = false;

    //    foreach (var modifierList in modifiers) {
    //        foreach (var modifier in modifierList) {
    //            if (modifier.animationName == nameBefore) {
    //                modifier.animationName = nameAfter;
    //                updated = true;
    //            }
    //        }
    //    }

    //    return updated;
    //}

    protected abstract void DrawInspector();

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif