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

[ExecuteInEditMode]
public class MadFreeDraggable : MadDraggable {

    #region Public Fields

    public Bounds dragBounds = new Bounds(Vector3.zero, new Vector3(400, 400));

    public ScaleMode scaleMode;

    public float scalingMax = 2;
    public float scalingMin = 0.25f;
    
    public bool moveEasing = true;
    public bool scaleEasing = true;
    public MadiTween.EaseType scaleEasingType = MadiTween.EaseType.easeOutQuad;
    public float scaleEasingDuration = 0.5f;

    #endregion

    #region Private Fields

    private Vector3 scaleSource;
    private Vector3 scaleTarget;
    private float scaleStartTime;

    // current move anim
    private bool moveAnim;
    private Vector3 moveAnimStartPosition;
    private Vector3 moveAnimEndPosition;
    private float moveAnimStartTime;
    private float moveAnimDuration;
    private MadiTween.EaseType moveAnimEaseType;

    #endregion

    #region Deprecated Fields
    [Obsolete("Use dragBounds.")]
    public Rect dragArea = new Rect(0, 0, 0, 0);

    [Obsolete("Use scaleMode.")]
    public bool scaling = false;
    #endregion

    #region Properties

    public override Vector2 progress {
        get {
            var rootNode = MadTransform.FindParent<MadRootNode>(transform);

            var areaBottomLeft = new Vector2(dragBounds.min.x, dragBounds.min.y);
            var areaTopRight = new Vector2(dragBounds.max.x, dragBounds.max.y);

            var screenBottomLeft = transform.InverseTransformPoint(rootNode.ScreenGlobal(0, 0));
            var screenTopRight = transform.InverseTransformPoint(rootNode.ScreenGlobal(1, 1));

            float screenW = screenTopRight.x - screenBottomLeft.x;
            float screenH = screenTopRight.y - screenBottomLeft.y;

            float areaW = areaTopRight.x - areaBottomLeft.x;
            float areaH = areaTopRight.y - areaBottomLeft.y;

            return new Vector2(
                (screenBottomLeft.x - areaBottomLeft.x) / (areaW - screenW), 
                (screenBottomLeft.y - areaBottomLeft.y) / (areaH - screenH));
        }
    }

    #endregion

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnValidate() {
        scalingMin = Mathf.Min(scalingMin, scalingMax);
        scalingMax = Mathf.Max(scalingMin, scalingMax);
    }
    
    void OnDrawGizmosSelected() {
        var center = transform.TransformPoint(dragBounds.center);

        var topRight = transform.TransformPoint(new Vector3(dragBounds.max.x, dragBounds.max.y, 0.01f));
        var bottomLeft = transform.TransformPoint(new Vector3(dragBounds.min.x, dragBounds.min.y, 0.01f));
        
        var extends = new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, extends);
    }

    protected override void OnEnable() {
        base.OnEnable();

        Upgrade();
    }

    void Upgrade() {
#pragma warning disable 618
        var dragArea = this.dragArea;
        bool scaling = this.scaling;
#pragma warning restore 618

        if (dragArea.width != 0) {
            dragBounds = new Bounds(dragArea.center, new Vector2(dragArea.xMax - dragArea.xMin, dragArea.yMax - dragArea.yMin));
#pragma warning disable 618
            this.dragArea = new Rect(0, 0, 0, 0);
#pragma warning restore 618
        }

        if (scaling) {
            scaleMode = ScaleMode.Free;
#pragma warning disable 618
            this.scaling = false;
#pragma warning restore 618
        }
    }

    protected override void Start() {
        base.Start();
        scaleSource = scaleTarget = transform.localScale;

        StartScaleMode();
    }

    private void StartScaleMode() {
        if (scaleMode != ScaleMode.FitToAreaWidth && scaleMode != ScaleMode.FitToAreaHeight) {
            return;
        }

        var root = MadTransform.FindParent<MadRootNode>(transform);

        Vector3 scale;

        switch (scaleMode) {
            case ScaleMode.FitToAreaWidth:
                float width = dragBounds.size.x;
                float screenWidth = root.screenWidth;
                scale = Vector3.one * screenWidth / width;
                break;
            case ScaleMode.FitToAreaHeight:
                float height = dragBounds.size.y;
                float screenHeight = root.screenHeight;
                scale = Vector3.one * screenHeight / height;
                break;
            default:
                Debug.Log("Unknown scale mode: " + scaleMode);
                scale = Vector3.one;
                break;
        }

        transform.localScale = scale;
    }

    protected override void Update() {
        if (!Application.isPlaying) {
            return;
        }

        base.Update();
        
        cachedCamPos = cameraPos;
        
        UpdateMoving();
        
        if (!IsTouchingSingle()) {
            dragging = false;
            
            if (scaleMode == ScaleMode.Free) {
                float scaleModifier = ScaleModifier();
                if (scaleModifier != 0) {
                    scaleSource = transform.localScale;
                    scaleTarget += scaleTarget * scaleModifier;
                    scaleTarget = ClampLocalScale(scaleTarget);
                    scaleStartTime = Time.time;
                }
            }
            
            float timeDiff = Time.time - lastTouchTime;
            if (moveEasing && timeDiff < moveEasingDuration && !moveAnim) {
                MoveToLocal(estaminatedPos, moveEasingType, moveEasingDuration);
            } else {
                Clear();
            }

            cameraPos = cachedCamPos;
            ClampPosition();
            
        } else { // toching single (drag)
            StopMoving();
        
            Vector2 lastCamPos = cachedCamPos;
            var touchPos = TouchPosition();
            
            if (IsTouchingJustStarted()) {
                lastPosition = touchPos;
            } else {
                cachedCamPos -= touchPos - lastPosition;
                lastPosition = touchPos;
            }
            
            RegisterDelta(cachedCamPos - lastCamPos);
            
            if (dragDistance > deadDistance) {
                dragging = true;
                cameraPos = cachedCamPos;
                ClampPosition();
            }
        }
        
        if (scaleMode == ScaleMode.Free) {
            float timeDiff = Time.time - scaleStartTime;
            if (scaleEasing && timeDiff < scaleEasingDuration) {
                transform.localScale = Ease(scaleEasingType, scaleSource, scaleTarget, timeDiff / scaleEasingDuration);
            } else {
                transform.localScale = scaleTarget;
            }
        }
    }
    
    void UpdateMoving() {
        if (!moveAnim) {
            return;
        }
        
        if (moveAnimStartTime + moveAnimDuration > Time.time) {
            float percentage = (Time.time - moveAnimStartTime) / moveAnimDuration;
            cachedCamPos = Ease(moveAnimEaseType, moveAnimStartPosition, moveAnimEndPosition, percentage);
        } else {
            cachedCamPos = moveAnimEndPosition;
            moveAnim = false;
        }
 }
    
    public void MoveToLocal(Vector2 position) {
        MoveToLocal(position, default(MadiTween.EaseType), 0);
    }
    
    public void MoveToLocal(Vector2 position, MadiTween.EaseType easeType, float time) {
        if (time == 0) {
            cameraPos = MadMath.ClosestPoint(dragBounds, position);
            moveAnim = false;
        } else {
            moveAnimStartPosition = cachedCamPos;
            moveAnimEndPosition = position;
            moveAnimStartTime = Time.time;
            moveAnimDuration = time;
            moveAnimEaseType = easeType;
            moveAnim = true;
        }
    }
    
    void StopMoving() {
        moveAnim = false;
    }
    
    void ClampPosition() {
        var position = cameraPos;
        var rootNode = MadTransform.FindParent<MadRootNode>(transform);
        
        var areaBottomLeft = new Vector2(dragBounds.min.x, dragBounds.min.y);
        var areaTopRight = new Vector2(dragBounds.max.x, dragBounds.max.y);
        
        var screenBottomLeft = transform.InverseTransformPoint(rootNode.ScreenGlobal(0, 0));
        var screenTopRight = transform.InverseTransformPoint(rootNode.ScreenGlobal(1, 1));
        
        float deltaLeft = screenBottomLeft.x - areaBottomLeft.x;
        float deltaRight = screenTopRight.x - areaTopRight.x;
        float deltaTop = screenTopRight.y - areaTopRight.y;
        float deltaBottom = screenBottomLeft.y - areaBottomLeft.y;
        
        // apply scale because transform matrix does not contain it
        float scale = transform.localScale.x;
        deltaLeft *= scale;
        deltaRight *= scale;
        deltaTop *= scale;
        deltaBottom *= scale;
        
        if (dragBounds.size.x < (screenTopRight.x - screenBottomLeft.x)) { // drag area smaller
            position.x = (areaTopRight.x + areaBottomLeft.x) / 2;
        } else if (deltaLeft < 0) {
            position.x -= deltaLeft;
        } else if (deltaRight > 0) {
            position.x -= deltaRight;
        }
        
        if (dragBounds.size.y < (screenTopRight.y - screenBottomLeft.y)) {
            position.y = (areaBottomLeft.y + areaTopRight.y) / 2;
        } else if (deltaBottom < 0) {
            position.y -= deltaBottom;
        } else if (deltaTop > 0) {
            position.y -= deltaTop;
        }

        cameraPos = position;

        // fixing position flicker if fit to area width or height (a little hack)
        switch (scaleMode) {
            case ScaleMode.FitToAreaWidth:
                cameraPos = new Vector2(transform.position.x + dragBounds.center.x * transform.localScale.x, cameraPos.y);
                break;
            case ScaleMode.FitToAreaHeight:
                cameraPos = new Vector2(cameraPos.x, transform.position.y + dragBounds.center.y * transform.localScale.y);
                break;
        }
    }
    
    Vector3 ClampLocalScale(Vector3 scale) {
        if (scale.x < scalingMin) {
            return new Vector3(scalingMin, scalingMin, scalingMin);
        } else if (scale.x > scalingMax) {
            return new Vector3(scalingMax, scalingMax, scalingMax);
        } else {
            return scale;
        }
    }
    
    float ScaleModifier() {
        #if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
        if (!Application.isEditor) {
            if (multiTouches.Count == 2) {
                var firstPosition = multiTouches[0].position;
                var secondPosition = multiTouches[1].position;
                
                Vector2 moveVec = (secondPosition - firstPosition);
                float currentDistance = moveVec.magnitude;
                
                if (lastDoubleTouchDistance != 0) {
                    float delta = lastDoubleTouchDistance - currentDistance;
                    lastDoubleTouchDistance = currentDistance;
                    
                    // calculte scale speed by touch angle
                    var moveVecNorm = moveVec.normalized;
                    float speed =
                        Screen.width * Mathf.Abs(moveVecNorm.x) + Screen.height * Mathf.Abs(moveVecNorm.y);
                    
                    return -delta * 2 / speed;
                } else {
                    lastDoubleTouchDistance = currentDistance;
                }
                
            } else {
                lastDoubleTouchDistance = 0;
                return 0;
            }
            return 0;
        } else {
            return Input.GetAxis("Mouse ScrollWheel");
        }
        #else
        return Input.GetAxis("Mouse ScrollWheel");
        #endif
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

    public enum ScaleMode {
        None,
        FitToAreaWidth,
        FitToAreaHeight,
        Free,
    }

}

#if !UNITY_3_5
} // namespace
#endif