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

public class MadAnimColor : MadAnim {

    #region Public Fields

    public ValueType colorFrom = ValueType.Current;
    public Color colorFromValue = Color.white;

    public ValueType colorTo = ValueType.Value;
    public Color colorToValue = Color.white;

    #endregion

    #region Private Fields

    private Color origin;
    private Color start;

    private MadiTween.EasingFunction easingFunction;

    private MadSprite sprite;

    #endregion

    #region Methods

    protected override void Start() {
        sprite = GetComponent<MadSprite>();
        if (sprite == null) {
            Debug.Log("Anim Color component requires MadSprite component!", this);
            return;
        }

        origin = sprite.tint;

        easingFunction = MadiTween.GetEasingFunction(easing);

        base.Start();
    }

    protected override void StartAnim() {
        start = sprite.tint;
    }

    protected override void Anim(float progress) {
        var from = GetFrom();
        var to = GetTo();

        float r = easingFunction.Invoke(from.r, to.r, progress);
        float g = easingFunction.Invoke(from.g, to.g, progress);
        float b = easingFunction.Invoke(from.b, to.b, progress);
        float a = easingFunction.Invoke(from.a, to.a, progress);

        var result = new Color(r, g, b, a);
        sprite.tint = result;
    }

    private Color GetFrom() {
        return GetColor(colorFrom, colorFromValue);
    }

    private Color GetTo() {
        return GetColor(colorTo, colorToValue);
    }

    private Color GetColor(ValueType valueType, Color modifier) {
        switch (valueType) {
            case ValueType.Origin:
                return origin;
            case ValueType.Current:
                return start;
            case ValueType.Value:
                return modifier;
            default:
                Debug.LogError("Unknown option: " + valueType);
                return start;
        }
    }

    #endregion

    #region Inner Types

    public enum ValueType {
        Origin,
        Current,
        Value,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif