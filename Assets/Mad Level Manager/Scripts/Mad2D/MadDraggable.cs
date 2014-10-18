/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;
using System;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

// Handles drag event and stops at selected positions
[ExecuteInEditMode]
public class MadDraggable : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadiTween.EaseType moveEasingType = MadiTween.EaseType.easeOutQuad;
    public float moveEasingDuration = 0.5f;
    
    
    protected Vector2 lastPosition;
    protected Vector2 interiaForce = Vector2.zero;
    
    LinkedList<Vector2> lastDeltas = new LinkedList<Vector2>();
    int lastDeltasCount = 5;

    // distance what mouse/finger did from the beggining of drag action
    protected float dragDistance;    
    // distance under what dragging will be not treated as drag action
    protected float deadDistance = 50;
    
    protected Vector2 cachedCamPos;
    
    // estaminated stop position if there was no force after drag stop
    protected Vector2 estaminatedPos;
    
    Touch? singleTouch = null;

#pragma warning disable 0414
    bool singleTouchEnded = false; // set to true if ended in this frame
#pragma warning restore 0414
    protected List<Touch> multiTouches = new List<Touch>();
    
    // amount of time from the last touch/click
    protected float lastTouchTime { get; set; }
    protected Vector2 lastTouchCameraPos { get; set; }
    
    // deprecated
    [SerializeField] DragMode dragMode = DragMode.DragStop;
    [SerializeField] Rect freeDragArea = new Rect(-200, -200, 400, 400);
    [SerializeField] bool allowScaling = false;
    [SerializeField] float scaleMax = 2;
    [SerializeField] float scaleMin = 0.25f;
    
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY    
    protected float lastDoubleTouchDistance = 0;
#endif
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    public bool dragging {
        get; protected set;
    }
    
    protected Vector2 cameraPos {
        get {
            return -transform.localPosition;
        }
        
        set {
            transform.localPosition = -value;
        }
    }

    // gets the position progress (0.0 - 1.0)
    public virtual Vector2 progress { get { throw new NotImplementedException(); } }

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected virtual void OnEnable() {
        Upgrade();
        if (Time.timeScale == 0) {
            Debug.LogWarning("Level selector may not work when Time.timeScale == 0. Setting it to 1.");
            Time.timeScale = 1;
        }
    }
    
    void Upgrade() {
        if (this.GetType() == typeof(MadDraggable)) {
            Debug.Log("Upgrading Draggable object... Please save your scene afterwards.");
            switch (dragMode) {
                case DragMode.Free:
                    var free = gameObject.AddComponent<MadFreeDraggable>() as MadFreeDraggable;
                    #pragma warning disable 618
                    free.dragArea = freeDragArea;
                    free.scaling = allowScaling;
                    #pragma warning restore 618
                    free.scalingMax = scaleMax;
                    free.scalingMin = scaleMin;
                    break;
                case DragMode.DragStop:
                    gameObject.AddComponent<MadDragStopDraggable>();
                    // nothing to move
                    break;
            }
            
            DestroyImmediate(this);
        }
    }
  
    protected virtual void Start() {
        cachedCamPos = cameraPos;
        lastTouchTime = -1000; // prevent touch time on start
    }
    
    bool addInteriaForce;
    
    protected virtual void Update() {
        UpdateTouchClassification();
        
        if (IsTouchingSingle()) {
            addInteriaForce = true;
        } else {
            addInteriaForce = false;
        }
    }
    
    protected virtual void FixedUpdate() {
        if (addInteriaForce) {
            ComputeInteriaForce();
        }
    }
    
    protected virtual void LateUpdate() {
        if (IsTouchingSingle()) {
            lastTouchTime = Time.time;
            lastTouchCameraPos = cameraPos;
        }
    }
    
    /// <summary>
    /// Decides on current touch phase and classification (single or multiple touches)
    /// </summary>
    void UpdateTouchClassification() {
        singleTouchEnded = false;

        foreach (Touch touch in Input.touches) {
            switch (touch.phase) {
                case TouchPhase.Began:
                    if (singleTouch == null && multiTouches.Count == 0) {
                        singleTouch = touch;
                    } else {
                        if (singleTouch != null) {
                            multiTouches.Add(singleTouch.Value);
                        }
                        multiTouches.Add(touch);
                        
                        singleTouch = null;
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    // not passing last touch makes sprites touch actions work
                    if (singleTouch != null) {
                        singleTouch = null;
                    } else {
                        multiTouches.Clear();
                    }
                    singleTouchEnded = true;
                    break;
                default:
                    if (singleTouch != null && singleTouch.Value.fingerId == touch.fingerId) {
                        singleTouch = touch;
                    } else {
                        for (int i = 0; i < multiTouches.Count; ++i) {
                            var multiTouch = multiTouches[i];
                            if (multiTouch.fingerId == touch.fingerId) {
                                multiTouches[i] = touch;
                            }
                        }
                    }
                    break;
            }
        }
    }
    
    
    
#region Touch
    protected bool IsTouchingSingle() {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
    if (!Application.isEditor) {
        return singleTouch != null;
    } else {
        return Input.GetMouseButton(0);
    }
#else
    return Input.GetMouseButton(0);
#endif
    }
    
    protected bool IsTouchingJustStarted() {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
        if (!Application.isEditor) {
            if (!IsTouchingSingle()) {
                return false;
            }
            
            return singleTouch.Value.phase == TouchPhase.Began;
        } else {
            return Input.GetMouseButtonDown(0);
        }
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    protected bool IsTouchingJustEnded() {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
        if (!Application.isEditor) {
            return singleTouchEnded;
        } else {
            return Input.GetMouseButtonUp(0);
        }
#else
        return Input.GetMouseButtonUp(0);
#endif
    }
    
    protected Vector2 TouchPosition() {
        MadDebug.Assert(IsTouchingSingle(), "Not touching anything");
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
        if (!Application.isEditor) {
            return singleTouch.Value.position;
        } else {
            return Input.mousePosition;
        }
#else
        return Input.mousePosition;
#endif
            
    }
    
    
#endregion
    
    protected void RegisterDelta(Vector2 delta) {
        lastDeltas.AddLast(delta / Time.deltaTime);
        dragDistance += delta.magnitude;
        if (lastDeltas.Count > lastDeltasCount) {
            lastDeltas.RemoveFirst();
        }
    }
    
    protected void Clear() {
        lastDeltas.Clear();
        dragDistance = 0;
        interiaForce = Vector3.zero;
        estaminatedPos = cameraPos;
    }
    
    void ComputeInteriaForce() {
        if (lastDeltas.Count > 0) {
            interiaForce = Vector2.zero;
        
            foreach (var delta in lastDeltas) {
                interiaForce += delta;
            }
            
            interiaForce /= lastDeltas.Count;
        }
        
        estaminatedPos = cameraPos + interiaForce * 0.12f;
    }
    
    protected Vector2 Ease(MadiTween.EaseType type, Vector2 start, Vector2 end, float percentage) {
        var fun = MadiTween.GetEasingFunction(type);
        float x = fun(start.x, end.x, percentage);
        float y = fun(start.y, end.y, percentage);
        
        return new Vector2(x, y);
    }
    
    protected Vector3 Ease(MadiTween.EaseType type, Vector3 start, Vector3 end, float percentage) {
        var fun = MadiTween.GetEasingFunction(type);
        float x = fun(start.x, end.x, percentage);
        float y = fun(start.y, end.y, percentage);
        float z = fun(start.z, end.z, percentage);
        
        return new Vector3(x, y, z);
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    private enum DragMode {
        Free,
        DragStop,
    }

    /// <summary>
    /// Defines easing function for drag actions
    /// </summary>    
//    public abstract class EasingFunction {
//        public Vector2 startValue;
//        public Vector2 endValue;
//        public Vector2 currentValue;
//        public float duration;
//        
//        public virtual void Start() {}
//        public virtual bool Update() { return false; }
//    }
//    
//    public class EasingNone : EasingFunction {
//        public override bool Update() { return true; }
//    }
//    
//    public class EasingITween : EasingFunction {
//        public MadiTween.EaseType easeType;
//        
//        private MadiTween.EasingFunction function;
//        private float timeAccum;
//        
//        public EasingITween(MadiTween.EaseType easeType, float duration) {
//            this.easeType = easeType;
//            this.duration = duration;
//        }
//    
//        public override void Start() {
//            function = MadiTween.GetEasingFunction(easeType);
//        }
//        
//        public override bool Update() {
//            timeAccum += Time.deltaTime;
//            if (timeAccum >= duration) {
//                return true; // end
//            }
//            
//            float percentage = timeAccum / duration;
//            
//            float x = function(startValue.x, endValue.x, percentage);
//            float y = function(startValue.y, endValue.y, percentage);
//            
//            currentValue = new Vector2(x, y);
//            
//            return false;
//        }
//    }
    
}

#if !UNITY_3_5
} // namespace
#endif