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

public class MadLevelAnimator : MadAnimator {

    #region Public Fields

    public Action onLevelLocked = new Action();
    public Action onLevelUnlocked = new Action();

    public List<Modifier> delayModifiers = new List<Modifier>();
    public List<Modifier> offsetModifiers = new List<Modifier>();

    public bool startupScaleForce = false;

    public ApplyMethod startupPositionApplyMethod = ApplyMethod.DoNotChange;
    public Vector3 startupPosition = new Vector3(0, 0, 0);

    public ApplyMethod startupRotationApplyMethod = ApplyMethod.DoNotChange;
    public Vector3 startupRotation = new Vector3(0, 0, 0);

    public ApplyMethod startupScaleApplyMethod = ApplyMethod.DoNotChange;
    public Vector3 startupScale = new Vector3(1, 1, 1);

    #endregion

    #region Private Fields

    private bool modifiersApplied = false;

    #endregion

    #region Public Properties
    #endregion

    #region Slots

    void OnEnable() {
        if (!modifiersApplied) {
            ApplyModifiers();
            modifiersApplied = true;
        }
    }

    private void ApplyModifiers() {
        var icon = GetComponent<MadLevelIcon>();

        if (icon == null) {
            return;
        }

        for (int i = 0; i < delayModifiers.Count; ++i) {
            var modifier = delayModifiers[i];
            modifier.Execute(icon, (a) => a.delay, (a, v) => a.delay = v);
        }

        for (int i = 0; i < offsetModifiers.Count; ++i) {
            var modifier = offsetModifiers[i];
            modifier.Execute(icon, (a) => a.offset, (a, v) => a.offset = v);
        }
    }

    protected override void Start() {
        base.Start();

        UpdateAnimOrigins();
        ApplyStartupPosition();
        ApplyStartupRotation();
        ApplyStartupScale();

        var icon = GetComponent<MadLevelIcon>();
        if (icon != null && !icon.isTemplate) {
            if (icon.locked) {
                onLevelLocked.Execute(gameObject);
            } else {
                onLevelUnlocked.Execute(gameObject);
            }

        }
    }

    void Update() {
    }

    #endregion

    #region Private Methods

    private void UpdateAnimOrigins() {
        var anims = GetComponents<MadAnim>();
        foreach (var anim in anims) {
            anim.UpdateOrigin();
        }
    }

    private void ApplyStartupPosition() {
        var newPosition = ApplyValue(startupPositionApplyMethod, transform.localPosition, startupPosition);
        transform.localPosition = newPosition;
    }

    private void ApplyStartupRotation() {
        var newRotation = ApplyValue(startupRotationApplyMethod, transform.localRotation.eulerAngles, startupRotation);
        transform.localRotation = Quaternion.Euler(newRotation);
    }

    private void ApplyStartupScale() {
        var newScale = ApplyValue(startupScaleApplyMethod, transform.localScale, startupScale);
        transform.localScale = newScale;
    }

    private Vector3 ApplyValue(ApplyMethod method, Vector3 originalValue, Vector3 applyValue) {
        switch (method) {
            case ApplyMethod.DoNotChange:
                // do nothing
                return originalValue;

            case ApplyMethod.Add:
                return originalValue + applyValue;

            case ApplyMethod.Multiply:
                return new Vector3(
                    originalValue.x * applyValue.x,
                    originalValue.y * applyValue.y,
                    originalValue.z * applyValue.z);

            case ApplyMethod.Set:
                return startupScale;

            default:
                Debug.LogError("Unknown apply method: " + method);
                return originalValue;
        }
    }

    #endregion

    #region Public Static Methods
    #endregion

    #region Inner and Anonymous Classes

    [Serializable]
    public class Modifier {
        public string animationName;

        public ModifierFunc modifierFunction = ModifierFunc.Predefined;
        public Operator baseOperator = Operator.Add;
        public Value firstParameter = Value.LevelIndex;
        public Operator valueOperator = Operator.Multiply;
        public float secondParameter = 1;

        [NonSerialized]
        public ModifierFunction customModifierFunction;

        public void Execute(MadLevelIcon icon, ValueGetter getter, ValueSetter setter) {
            var animations = MadAnim.FindAnimations(icon.gameObject, animationName);
            for (int i = 0; i < animations.Count; ++i) {
                var animation = animations[i];

                float baseValue = getter(animation);

                switch (modifierFunction) {
                    case ModifierFunc.Custom:
                        setter(animation, customModifierFunction(icon));
                        break;

                    case ModifierFunc.Predefined:
                        float firstParameter = GetFirstParameterValue(icon);
                        float rightSideValue = Compute(firstParameter, secondParameter, valueOperator);
                        float leftSideValue = Compute(baseValue, rightSideValue, baseOperator);
                        setter(animation, leftSideValue);
                        break;

                    default:
                        Debug.LogError("Uknown modifier function:" + modifierFunction);
                        setter(animation, baseValue);
                        break;
                }
            }
        }

        private float GetFirstParameterValue(MadLevelIcon icon) {
            var layout = MadLevelLayout.current;
            var gridLayout = layout as MadLevelGridLayout;

            switch (firstParameter) {
                case Value.LevelIndex:
                    return icon.levelIndex;

                case Value.GridLevelPageIndex: {
                        if (gridLayout == null) {
                            return icon.levelIndex;
                        }

                        int iconsPerPage = gridLayout.gridWidth * gridLayout.gridHeight;
                        return (icon.levelIndex % iconsPerPage);
                    }

                case Value.GridRow: {
                        if (gridLayout == null) {
                            return icon.levelIndex;
                        }

                        int iconsPerPage = gridLayout.gridWidth * gridLayout.gridHeight;
                        int index = icon.levelIndex % iconsPerPage;

                        int x = index / gridLayout.gridWidth;
                        return x;
                    }

                case Value.GridColumn: {
                        if (gridLayout == null) {
                            return icon.levelIndex;
                        }

                        int iconsPerPage = gridLayout.gridWidth * gridLayout.gridHeight;
                        int index = icon.levelIndex % iconsPerPage;

                        int x = index % gridLayout.gridWidth;
                        return x;
                    }

                default:
                    Debug.LogError("Unknown value: " + firstParameter);
                    return 0;
                    
            }
        }

        private float Compute(float first, float second, Operator op) {
            switch (op) {
                case Operator.Add:
                    return first + second;
                case Operator.Subtract:
                    return first - second;
                case Operator.Multiply:
                    return first * second;
                case Operator.Divide:
                    return first / second;
                case Operator.Modulo:
                    return first % second;
                default:
                    Debug.LogError("Unknown operator: " + op);
                    return first;
            }
        }

        #region Types

        public delegate void Executor(MadAnim animation, float modifier);

        public delegate float ValueGetter(MadAnim animation);
        public delegate void ValueSetter(MadAnim animation, float value);

        public enum Operator {
            Add,
            Subtract,
            Multiply,
            Divide,
            Modulo,
        }

        public enum Value {
            LevelIndex,
            GridLevelPageIndex,
            GridRow,
            GridColumn,
        }

        public enum ModifierFunc {
            Custom,
            Predefined,
        }

	    #endregion
    }

    public delegate float ModifierFunction(MadLevelIcon icon);

    public enum ApplyMethod {
        DoNotChange,
        Add,
        Multiply,
        Set,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif