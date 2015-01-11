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

public class MadAnimRotate : MadAnim {

    #region Public Fields

    public ValueType rotateFrom = ValueType.Current;
    public Vector3 rotateFromValue;

    public ValueType rotateTo = ValueType.Value;
    public Vector3 rotateToValue;

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
        originLocal = transform.localRotation.eulerAngles;
    }

    protected override void StartAnim() {
        startLocal = transform.localRotation.eulerAngles;
    }

    protected override void Anim(float progress) {
        var from = GetFrom();
        var to = GetTo();

        float deltaX = Mathf.DeltaAngle(from.x, to.x);
        float deltaY = Mathf.DeltaAngle(from.y, to.y);
        float deltaZ = Mathf.DeltaAngle(from.z, to.z);

        float x = easingFunction.Invoke(from.x, from.x + deltaX, progress);
        float y = easingFunction.Invoke(from.y, from.y + deltaY, progress);
        float z = easingFunction.Invoke(from.z, from.z + deltaZ, progress);

        var result = new Vector3(x, y, z);
        transform.localRotation = Quaternion.Euler(result);
    }

    private Vector3 GetFrom() {
        return GetLocalRotation(rotateFrom, rotateFromValue);
    }

    private Vector3 GetTo() {
        return GetLocalRotation(rotateTo, rotateToValue);
    }

    private Vector3 GetLocalRotation(ValueType valueType, Vector3 modifier) {
        switch (valueType) {
            case ValueType.Current:
                return startLocal;
            case ValueType.Origin:
                return originLocal;
            case ValueType.Value:
                return modifier;
            case ValueType.CurrentAdd:
                return new Vector3(startLocal.x + modifier.x, startLocal.y + modifier.y, startLocal.z + modifier.z);
            case ValueType.OriginAdd:
                return new Vector3(originLocal.x + modifier.x, originLocal.y + modifier.y, originLocal.z + modifier.z);
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
        CurrentAdd,
        OriginAdd,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif