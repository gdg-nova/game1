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

[CustomEditor(typeof(MadAnimator))]
public class MadAnimatorInspector : Editor {

    #region Fields

    SerializedProperty onMouseEnter;
    SerializedProperty onMouseExit;
    SerializedProperty onTouchEnter;
    SerializedProperty onTouchExit;
    SerializedProperty onFocus;
    SerializedProperty onFocusLost;

    List<string> animationNames = new List<string>();

    #endregion

    #region Methods
    
    protected virtual void OnEnable() {
        onMouseEnter = serializedObject.FindProperty("onMouseEnter");
        onMouseExit = serializedObject.FindProperty("onMouseExit");
        onTouchEnter = serializedObject.FindProperty("onTouchEnter");
        onTouchExit = serializedObject.FindProperty("onTouchExit");
        onFocus = serializedObject.FindProperty("onFocus");
        onFocusLost = serializedObject.FindProperty("onFocusLost");
    }

    public override void OnInspectorGUI() {
        RefreshAnimationNames();

        serializedObject.UpdateIfDirtyOrScript();

        GUILayout.Label("Events", "HeaderLabel");
        EditorGUILayout.Space();

        using (MadGUI.Indent()) {
            GUIEvents();
        }

        GUIElements();

        serializedObject.ApplyModifiedProperties();
    }


    protected virtual void GUIElements() {
        // empty
    }

    protected virtual void GUIEvents() {
        GUIEvent("On Mouse Enter", onMouseEnter);
        GUIEvent("On Mouse Exit", onMouseExit);
        GUIEvent("On Touch Enter", onTouchEnter);
        GUIEvent("On Touch Exit", onTouchExit);
        GUIEvent("On Focus", onFocus);
        GUIEvent("On Focus Lost", onFocusLost);
    }

    private void RefreshAnimationNames() {
        animationNames.Clear();

        var anims = (target as Component).GetComponents<MadAnim>();
        foreach (var anim in anims) {
            if (!animationNames.Contains(anim.name)) {
                animationNames.Add(anim.animationName);
            }
        }

        animationNames.Sort();
    }

    protected void GUIEvent(string name, SerializedProperty property) {
        int playCount, stopCount;
        bool stopAll;

        AnalyzeAction(property, out playCount, out stopCount, out stopAll);

        bool colorize = playCount > 0 || stopCount > 0 || stopAll;

        if (colorize) {
            GUI.color = Color.yellow;
        }

        if (MadGUI.Foldout(name, false)) {

            GUI.color = Color.white;

            using (MadGUI.Indent()) {
                GUIAction(property);
            }
        }

        GUI.color = Color.white;
    }

    private void AnalyzeAction(SerializedProperty action, out int play, out int stop, out bool stopAll) {
        var playAnimations = action.FindPropertyRelative("playAnimations");
        var stopAnimations = action.FindPropertyRelative("stopAnimations");
        var stopAllAnimations = action.FindPropertyRelative("stopAllAnimations");

        play = playAnimations.arraySize;
        stop = stopAnimations.arraySize;
        stopAll = stopAllAnimations.boolValue;
    }

    protected void GUIAction(SerializedProperty action) {
        var playAnimations = action.FindPropertyRelative("playAnimations");
        var stopAnimations = action.FindPropertyRelative("stopAnimations");
        var stopAllAnimations = action.FindPropertyRelative("stopAllAnimations");

        EditorGUILayout.LabelField("Play Animations");
        using (MadGUI.Indent()) {
            GUIAnimationRefList(playAnimations, true);
        }

        MadGUI.PropertyFieldToggleGroup2(stopAllAnimations, "Stop All Animations", () => {
            GUI.enabled = !GUI.enabled;
            EditorGUILayout.LabelField("Stop Animations");
            using (MadGUI.Indent()) {
                GUIAnimationRefList(stopAnimations, false);
            }
        });
    }

    protected void GUIAnimationRefList(SerializedProperty list, bool showFromTheBeginning) {
        var alist = new MadGUI.ArrayList<MadAnimator.AnimationRef>(list, (p) => {
            GUIAnimationRef(p, showFromTheBeginning);
        });
        alist.Draw();
    }

    protected void GUIAnimationRef(SerializedProperty animationRef, bool showFromTheBeginning) {
        var name = animationRef.FindPropertyRelative("name");
        GUIAnimationName(name);
        //MadGUI.PropertyField(name, "Name");

        if (showFromTheBeginning) {
            using (MadGUI.Indent()) {
                var fromTheBeginning = animationRef.FindPropertyRelative("fromTheBeginning");
                MadGUI.PropertyField(fromTheBeginning, "From The Start");
            }
        }
    }

    protected void GUIAnimationName(SerializedProperty animationName) {
        int MissingAnimationNameNum = animationNames.Count + 1;
        bool animationNameMissing = false;

        string selectedAnimationName = animationName.stringValue;
        int animationNameNum = animationNames.FindIndex((s) => s == selectedAnimationName) + 1;

        if (!string.IsNullOrEmpty(selectedAnimationName) && animationNameNum == 0) {
            // there was animation name, but it's not available now
            animationNameNum = MissingAnimationNameNum;
            animationNameMissing = true;
            GUI.backgroundColor = Color.red;
        }

        EditorGUI.BeginChangeCheck();

        animationNameNum = MadGUI.DynamicPopup(animationNameNum, "Name", animationNames.Count + 1 + (animationNameMissing ? 1 : 0), (index) => {
            if (index == 0) {
                return "(none)";
            } else if (index == MissingAnimationNameNum) {
                return selectedAnimationName + " (missing)";
            } else {
                return animationNames[index - 1];
            }
        });

        GUI.backgroundColor = Color.white;

        if (animationNameMissing) {
            MadGUI.Warning("The animation with this name cannot be found.");
        }

        if (EditorGUI.EndChangeCheck()) {
            if (animationNameNum == 0) {
                animationName.stringValue = "";
            } else if (animationNameNum == MissingAnimationNameNum) {
                // do nothing
            } else {
                animationName.stringValue = animationNames[animationNameNum - 1];
            }
        }
    }

    #endregion

}

#if !UNITY_3_5
} // namespace
#endif