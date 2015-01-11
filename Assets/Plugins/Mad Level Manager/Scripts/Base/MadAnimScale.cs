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

public class MadAnimScale : MadAnim {

    #region Public Fields

    public ValueType scaleFrom = ValueType.Current;
    public Vector3 scaleFromValue;

    public ValueType scaleTo = ValueType.Value;
    public Vector3 scaleToValue;

    #endregion

    #region Private Fields

    private Vector3 originLocal;

    //private Vector3 startWorld;
    private Vector3 startLocal;

    private MadiTween.EasingFunction easingFunction;

    #endregion

    #region Methods

    protected override void Start() {
        easingFunction = MadiTween.GetEasingFunction(easing);

        base.Start();
    }

    public override void UpdateOrigin() {
        base.UpdateOrigin();
        originLocal = transform.localScale;
    }

    protected override void StartAnim() {
        startLocal = transform.localScale;
    }

    protected override void Anim(float progress) {
        var from = GetFrom();
        var to = GetTo();

        float x = easingFunction.Invoke(from.x, to.x, progress);
        float y = easingFunction.Invoke(from.y, to.y, progress);
        float z = easingFunction.Invoke(from.z, to.z, progress);

        var result = new Vector3(x, y, z);
        transform.localScale = result;
    }

    private Vector3 GetFrom() {
        return GetLocalScale(scaleFrom, scaleFromValue);
    }

    private Vector3 GetTo() {
        return GetLocalScale(scaleTo, scaleToValue);
    }

    private Vector3 GetLocalScale(ValueType valueType, Vector3 modifier) {
        switch (valueType) {
            case ValueType.Current:
                return startLocal;
            case ValueType.Origin:
                return originLocal;
            case ValueType.Value:
                return modifier;
            case ValueType.CurrentMultiply:
                return new Vector3(startLocal.x * modifier.x, startLocal.y * modifier.y, startLocal.z * modifier.z);
            case ValueType.OriginMultiply:
                return new Vector3(originLocal.x * modifier.x, originLocal.y * modifier.y, originLocal.z * modifier.z);
            default:
                Debug.LogError("Unknown option: " + valueType);
                return startLocal;
        }
    }

    #endregion

    #region Inner Types

    public enum ValueType {
        Current,
        Origin,
        Value,
        CurrentMultiply,
        OriginMultiply,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif