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

public class MadAnimMove : MadAnim {

    #region Public Fields

    public ValueType moveFrom = ValueType.Current;
    public Vector3 moveFromPosition;

    public ValueType moveTo = ValueType.Local;
    public Vector3 moveToPosition;

    #endregion

    #region Private Fields

    private Vector3 originWorld;
    private Vector3 originLocal;

    private Vector3 startWorld;
    private Vector3 startLocal;

    private MadiTween.EasingFunction easingFunction {
        get {
            if (_easingFunction == null) {
                _easingFunction = MadiTween.GetEasingFunction(easing);
            }

            return _easingFunction;
        }
    }
    private MadiTween.EasingFunction _easingFunction;

    #endregion

    #region Methods

    protected override void Start() {
        base.Start();
    }

    public override void UpdateOrigin() {
        base.UpdateOrigin();
        originWorld = transform.position;
        originLocal = transform.localPosition;
    }

    protected override void StartAnim() {
        startWorld = transform.position;
        startLocal = transform.localPosition;
    }

    protected override void Anim(float progress) {
        var from = GetFrom();
        var to = GetTo();

        float x = easingFunction.Invoke(from.x, to.x, progress);
        float y = easingFunction.Invoke(from.y, to.y, progress);
        float z = easingFunction.Invoke(from.z, to.z, progress);

        var result = new Vector3(x, y, z);
        transform.localPosition = result;
    }

    private Vector3 GetFrom() {
        return GetPosition(moveFrom, moveFromPosition);
    }

    private Vector3 GetTo() {
        return GetPosition(moveTo, moveToPosition);
    }

    private Vector3 GetPosition(ValueType valueType, Vector3 modifier) {
        switch (valueType) {
            case ValueType.Origin:
                return originLocal;
            case ValueType.Current:
                return startLocal;
            case ValueType.Local:
                return modifier;
            case ValueType.World:
                if (transform.parent != null) {
                    return transform.parent.InverseTransformPoint(modifier);
                } else {
                    return modifier;
                }
            case ValueType.LocalOriginAdd:
                return originLocal + modifier;
            case ValueType.LocalCurrentAdd:
                return startLocal + modifier;
            case ValueType.WorldOriginAdd:
                if (transform.parent != null) {
                    return transform.parent.InverseTransformPoint(originWorld + modifier);
                } else {
                    return originWorld + modifier;
                }
            case ValueType.WorldCurrentAdd:
                if (transform.parent != null) {
                    return transform.parent.InverseTransformPoint(startWorld + modifier);
                } else {
                    return startWorld + modifier;
                }
            default:
                Debug.LogError("Unknown option: " + valueType);
                return startLocal;
        }
    }

    #endregion

    #region Inner Types

    public enum ValueType {
        Origin,
        Current,
        Local,
        World,
        LocalOriginAdd,
        LocalCurrentAdd,
        WorldOriginAdd,
        WorldCurrentAdd,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif