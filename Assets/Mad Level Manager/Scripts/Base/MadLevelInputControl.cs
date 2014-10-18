/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelInputControl : MonoBehaviour {

    #region Fields
    
    public InputMode inputMode = InputMode.KeyCodes;

    public KeyCode keycodeLeft = KeyCode.LeftArrow;
    public KeyCode keycodeRight = KeyCode.RightArrow;
    public KeyCode keycodeUp = KeyCode.UpArrow;
    public KeyCode keycodeDown = KeyCode.DownArrow;
    public KeyCode keycodeEnter = KeyCode.Return;

    public string axisHorizontal = "Horizontal";
    public string axisVertical = "Vertical";
    public string axisEnter = "Fire1";

    public TraverseRule traverseRule = new DirectionTraverseRule();

    public ActivateOnStart activateOnStart = ActivateOnStart.LastCompleted;

    public bool onlyOnMobiles = false;

    public bool repeat = true;

    public float repeatInterval = 0.5f;

    private bool isMobile;

    private float lastActionTime;

    private bool keyDown;

    #endregion

    #region Properites

    private bool isLeft {
        get {
            switch (inputMode) {
                case InputMode.KeyCodes:
                    return Input.GetKey(keycodeLeft);
                case InputMode.InputAxes:
                    return Input.GetAxis(axisHorizontal) < 0;
                default:
                    Debug.LogError("Unknown input mode: " + inputMode);
                    return false;
            }
        }
    }

    private bool isRight {
        get {
            switch (inputMode) {
                case InputMode.KeyCodes:
                    return Input.GetKey(keycodeRight);
                case InputMode.InputAxes:
                    return Input.GetAxis(axisHorizontal) > 0;
                default:
                    Debug.LogError("Unknown input mode: " + inputMode);
                    return false;
            }
        }
    }

    private bool isDown {
        get {
            switch (inputMode) {
                case InputMode.KeyCodes:
                    return Input.GetKey(keycodeDown);
                case InputMode.InputAxes:
                    return Input.GetAxis(axisVertical) < 0;
                default:
                    Debug.LogError("Unknown input mode: " + inputMode);
                    return false;
            }
        }
    }

    private bool isUp {
        get {
            switch (inputMode) {
                case InputMode.KeyCodes:
                    return Input.GetKey(keycodeUp);
                case InputMode.InputAxes:
                    return Input.GetAxis(axisVertical) > 0;
                default:
                    Debug.LogError("Unknown input mode: " + inputMode);
                    return false;
            }
        }
    }

    private bool isEnter {
        get {
            switch (inputMode) {
                case InputMode.KeyCodes:
                    return Input.GetKeyDown(keycodeEnter);
                case InputMode.InputAxes:
                    return Input.GetAxis(axisEnter) > 0;
                default:
                    Debug.LogError("Unknown input mode: " + inputMode);
                    return false;
            }
        }
    }

    #endregion

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
        var layout = MadLevelLayout.current;
        if (layout.twoStepActivationType == MadLevelAbstractLayout.TwoStepActivationType.Disabled) {
            Debug.LogError("Input controller cannot work when two step activation is disabled! Please enable it!", this);
            MadGameObject.SetActive(gameObject, false);
            return;
        }

        if (layout.twoStepActivationType == MadLevelAbstractLayout.TwoStepActivationType.OnlyOnMobiles && !onlyOnMobiles) {
            Debug.LogError("Two step activation is set to work on mobiles, but input controler is not!", this);
            MadGameObject.SetActive(gameObject, false);
            return;
        }

        isMobile = SystemInfo.deviceType == DeviceType.Handheld;

        DoActivateOnStart();
    }

    private void DoActivateOnStart() {
        var layout = MadLevelLayout.current;
        MadLevelIcon icon = null;

        switch (activateOnStart) {
            case ActivateOnStart.First:
                icon = layout.GetFirstIcon();
                break;
            case ActivateOnStart.LastCompleted:
                icon = layout.GetLastCompletedIcon();
                break;
            case ActivateOnStart.LastUnlocked:
                icon = layout.GetLastUnlockedIcon();
                break;
            case ActivateOnStart.PreviouslyPlayedOrFirst:
                icon = layout.GetIcon(MadLevel.lastPlayedLevelName);
                break;
            case ActivateOnStart.PreviouslyPlayedOrLastCompleted:
                icon = layout.GetIcon(MadLevel.lastPlayedLevelName);
                if (icon == null) {
                    icon = layout.GetLastCompletedIcon();
                }
                break;
            case ActivateOnStart.PreviouslyPlayedOrLastUnlocked:
                icon = layout.GetIcon(MadLevel.lastPlayedLevelName);
                if (icon == null) {
                    icon = layout.GetLastUnlockedIcon();
                }
                break;
            default:
                Debug.LogError("Unknown activateOnStart: " + activateOnStart);
                break;
        }

        if (icon == null) {
            icon = layout.GetFirstIcon();
        }

        if (icon != null && !icon.hasFocus) {
            Activate(icon);
        }
    }

    void Update() {
        if (onlyOnMobiles && !isMobile) {
            return;
        }

        MadLevelIcon newIcon = null;
        var activeIcon = ActiveIcon();

        if ((isLeft || isRight || isUp || isDown) && activeIcon == null) {
            var layout = MadLevelLayout.current;
            var firstIcon = layout.GetFirstIcon();

            Activate(firstIcon);
        } else {

            if (isLeft && traverseRule.canGoLeft) {
                if (CanExecuteAction()) {
                    newIcon = traverseRule.LeftIcon(activeIcon);
                    keyDown = true;
                    lastActionTime = Time.time;
                }
            } else if (isRight && traverseRule.canGoRight) {
                if (CanExecuteAction()) {
                    newIcon = traverseRule.RightIcon(activeIcon);
                    keyDown = true;
                    lastActionTime = Time.time;
                }
            } else if (isUp && traverseRule.canGoUp) {
                if (CanExecuteAction()) {
                    newIcon = traverseRule.TopIcon(activeIcon);
                    keyDown = true;
                    lastActionTime = Time.time;
                }
            } else if (isDown && traverseRule.canGoDown) {
                if (CanExecuteAction()) {
                    newIcon = traverseRule.BottomIcon(activeIcon);
                    keyDown = true;
                    lastActionTime = Time.time;
                }
            } else if (isEnter) {
                Activate(activeIcon);
            } else {
                keyDown = false;
            }

            if (newIcon != null) {
                Activate(newIcon);
            }
        }
        
    }

    private bool CanExecuteAction() {
        if (!keyDown) {
            return true;
        } else if (!repeat) {
            return false;
        } else if (lastActionTime + repeatInterval <= Time.time) {
            return true;
        } else {
            return false;
        }
    }

    private MadLevelIcon ActiveIcon() {
        var layout = MadLevelLayout.current;
        var activeIcon = layout.GetActiveIcon();
        return activeIcon;
    }
    
    void ActivateCurrentLevel() {
        var layout = MadLevelLayout.current;
        var activeIcon = layout.GetActiveIcon();
        Activate(activeIcon);
    }
    
    void Activate(MadLevelIcon icon) {
        if (icon == null) {
            return;
        }

        var layout = MadLevelLayout.current;
        layout.Activate(icon);
        if (layout is MadLevelFreeLayout) {
            var free = layout as MadLevelFreeLayout;
            free.LookAtIcon(icon, MadiTween.EaseType.easeOutCubic, 1);
        } else if (layout is MadLevelGridLayout) {
            var grid = layout as MadLevelGridLayout;
            grid.LookAtIconAnimate(icon);
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

    public abstract class TraverseRule {
        public readonly int capFlags;

        public TraverseRule(params Capability[] capabilities) {
            for (int i = 0; i < capabilities.Length; ++i) {
                capFlags |= (int) capabilities[i];
            }
        }

        public bool canGoLeft {
            get {
                return (capFlags & (int) Capability.CanGoLeft) != 0;
            }
        }

        public bool canGoRight {
            get {
                return (capFlags & (int) Capability.CanGoRight) != 0;
            }
        }

        public bool canGoUp {
            get {
                return (capFlags & (int) Capability.CanGoUp) != 0;
            }
        }

        public bool canGoDown {
            get {
                return (capFlags & (int) Capability.CanGoDown) != 0;
            }
        }

        public abstract MadLevelIcon LeftIcon(MadLevelIcon current);
        public abstract MadLevelIcon TopIcon(MadLevelIcon current);
        public abstract MadLevelIcon RightIcon(MadLevelIcon current);
        public abstract MadLevelIcon BottomIcon(MadLevelIcon current);

        [System.Flags]
        public enum Capability {
            CanGoLeft = 1,
            CanGoRight = 2,
            CanGoUp = 4,
            CanGoDown = 8,
            CanGoAnywhere = 15,
        }
    }

    public class SimpleTraverseRule : TraverseRule {

        public SimpleTraverseRule() 
            : base(Capability.CanGoLeft, Capability.CanGoRight) {
        }

        public override MadLevelIcon LeftIcon(MadLevelIcon current) {
            var layout = MadLevelLayout.current;
            return layout.GetPreviousIcon(current);
        }

        public override MadLevelIcon RightIcon(MadLevelIcon current) {
            var layout = MadLevelLayout.current;
            return layout.GetNextIcon(current);
        }

        public override MadLevelIcon TopIcon(MadLevelIcon current) {
            throw new System.NotImplementedException();
        }

        public override MadLevelIcon BottomIcon(MadLevelIcon current) {
            throw new System.NotImplementedException();
        }

    }

    public class DirectionTraverseRule : TraverseRule {

        //public float searchDistance = 200;

        public DirectionTraverseRule()
            : base(Capability.CanGoAnywhere) {
        }

        public override MadLevelIcon LeftIcon(MadLevelIcon current) {
            return FindBest(current, Vector3.left, 45);
        }

        public override MadLevelIcon RightIcon(MadLevelIcon current) {
            return FindBest(current, Vector3.right, 45);
        }

        public override MadLevelIcon TopIcon(MadLevelIcon current) {
            return FindBest(current, Vector3.up, 45);
        }

        public override MadLevelIcon BottomIcon(MadLevelIcon current) {
            return FindBest(current, Vector3.down, 45);
        }

        public MadLevelIcon FindBest(MadLevelIcon origin, Vector2 direction, float toleranceAngle) {
            List<MadLevelIcon> found = FindAll(origin, direction, toleranceAngle);
            if (found.Count == 0) {
                return null;
            } else if (found.Count == 1) {
                return found[0];
            } else {
                // find the best candidate based on distance and angle
                float weight = float.MaxValue;
                MadLevelIcon bestIcon = null;

                for (int i = 0; i < found.Count; ++i) {
                    var icon = found[i];
                    var originToIconVec = icon.transform.position - origin.transform.position;
                    float angle = Vector2.Angle(direction, originToIconVec);
                    float distance = originToIconVec.magnitude;

                    float currentWeight = distance + (distance * (angle / toleranceAngle));
                    if (currentWeight < weight) {
                        bestIcon = icon;
                        weight = currentWeight;
                    }
                }

                return bestIcon;
            }
        }

        public List<MadLevelIcon> FindAll(MadLevelIcon origin, Vector2 direction, float toleranceAngle) {
            List<MadLevelIcon> output = new List<MadLevelIcon>();
            
            var layout = MadLevelLayout.current;
            var allIcons = layout.GetAllIcons();

            for (int i = 0; i < allIcons.Length; ++i) {
                var icon = allIcons[i];
                if (icon == origin) {
                    continue;
                }

                Vector2 vec = icon.transform.position - origin.transform.position;
                if (Vector2.Angle(direction, vec) <= toleranceAngle) {
                    output.Add(icon);
                }
            }

            return output;
        }
    }

    public enum InputMode {
        KeyCodes,
        InputAxes,
    }

    public enum ActivateOnStart {
        First,
        LastUnlocked,
        LastCompleted,
        PreviouslyPlayedOrFirst,
        PreviouslyPlayedOrLastUnlocked,
        PreviouslyPlayedOrLastCompleted,
    }

}

#if !UNITY_3_5
} // namespace
#endif