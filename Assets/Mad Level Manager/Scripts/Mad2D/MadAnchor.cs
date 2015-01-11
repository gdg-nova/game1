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
 
[ExecuteInEditMode]   
public class MadAnchor : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Mode mode = Mode.ScreenAnchor;
    
    // anchoring to screen position
    public Position position;
    
    // anchoring to object position
    public GameObject anchorObject;
    public Camera anchorCamera;
    
    MadRootNode _root;
    MadRootNode root {
        get {
            if (_root == null) {
                _root = MadTransform.FindParent<MadRootNode>(transform);
            }
            
            return _root;
        }
    }

    private MadPanel panel {
        get {
            if (_panel == null) {
                _panel = MadTransform.FindParent<MadPanel>(transform);
                if (_panel == null) {
                    Debug.LogError("Anchor can be set only under the panel", this);
                }
            }

            return _panel;
        }
    }

    private MadPanel _panel;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
        // do nothing
    }
    
    public void Update() {
        switch (mode) {
            case Mode.ScreenAnchor:
                UpdateScreenAnchor();
                break;
            case Mode.ObjectAnchor:
                UpdateObjectAnchor();
                break;
            default:
                MadDebug.Assert(false, "Unknown mode: " + mode);
                break;
        }
    }
    
    void UpdateScreenAnchor() {
        var input = FromPosition(position);
        MadTransform.SetPosition(transform, input);
    }
    
    Vector3 FromPosition(Position position) {
        float x = 0, y = 0;
    
        switch (position) {
            case Position.Left:
                x = 0;
                y = 0.5f;
                break;
            case Position.Top:
                y = 1;
                x = 0.5f;
                break;
            case Position.Right:
                x = 1;
                y = 0.5f;
                break;
            case Position.Bottom:
                y = 0;
                x = 0.5f;
                break;
            case Position.TopLeft:
                x = 0;
                y = 1;
                break;
            case Position.TopRight:
                x = 1;
                y = 1;
                break;
            case Position.BottomRight:
                x = 1;
                y = 0;
                break;
            case Position.BottomLeft:
                x = 0;
                y = 0;
                break;
            case Position.Center:
                x = 0.5f;
                y = 0.5f;
                break;
            default:
                MadDebug.Assert(false, "Unknown option: " + position);
                break;
        }
        
        var pos = root.ScreenGlobal(x, y);
        return pos;
    }
    
    void UpdateObjectAnchor() {
        if (anchorObject == null) {
            return;
        }
        
        Camera camera = anchorCamera;
        if (camera == null) {
            if (Application.isPlaying) {
                MadDebug.LogOnce("Anchor camera not set. Using main camera.", this);
            }
            camera = Camera.main;
            if (camera == null) {
                Debug.LogWarning("There's no camera tagged as MainCamera on this scene. Please make sure that there is one or assign a custom camera to this anchor object.", this);
                return;
            }
        }

        var pos = panel.WorldToPanel(camera, anchorObject.transform.position);

        MadTransform.SetPosition(transform, pos);
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum Mode {
        ScreenAnchor,
        ObjectAnchor,
    }
    
    public enum Position {
        Left,
        Top,
        Right,
        Bottom,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
        Center,
    }

}

#if !UNITY_3_5
} // namespace
#endif