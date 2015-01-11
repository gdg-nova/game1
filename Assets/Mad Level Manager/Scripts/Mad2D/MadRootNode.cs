/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[ExecuteInEditMode]   
public class MadRootNode : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public ResizeMode resizeMode = ResizeMode.FixedSize;
    
    public int manualHeight = 720;
    public int minimumHeight = 320;
    public int maximumHeight = 1536;
    
    // ===========================================================
    // Fields
    // ===========================================================
    
    public float pixelSize {
        get {
            if (resizeMode == ResizeMode.FixedSize) {
                return Screen.height / (float) manualHeight;
            } else {
                return 0.5f;
            }
        }
    }
    
    public float screenHeight {
        get {
            switch (resizeMode) {
                case ResizeMode.FixedSize:
                    return manualHeight;
                case ResizeMode.PixelPerfect:
                    return MadScreen.height;
                default:
                    Debug.Log("Unknown resize mode: " + resizeMode);
                    return 128;
            }
        }
    }
    
    public float screenWidth {
        get {
            int w = MadScreen.width;
            int h = MadScreen.height;
        
            switch (resizeMode) {
                case ResizeMode.FixedSize:
                    return manualHeight * (w / (float) h);
                case ResizeMode.PixelPerfect:
                    return w;
                default:
                    Debug.Log("Unknown resize mode: " + resizeMode);
                    return 128;
            }
        }
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        UpdateScale();
    }
    
    void Start() {
        UpdateScale();
    }
    
    void Update() {
        UpdateScale();
    }
    
    void UpdateScale() {
        float scale;
        
        switch (resizeMode) {
            case ResizeMode.FixedSize:
                scale = 1f / manualHeight * 2;
                break;
            case ResizeMode.PixelPerfect:
                scale = 1f / Mathf.Clamp(screenHeight, minimumHeight, maximumHeight) * 2;
                break;
            default:
                Debug.LogError("Unknown option: " + resizeMode);
                scale = 1f / manualHeight * 2;
                break;
        }

        MadTransform.SetLocalScale(transform, scale);
    }
    
    public Vector3 ScreenToLocal(Vector3 v) {
        return new Vector3(
            v.x / transform.localScale.x,
            v.y / transform.localScale.y,
            v.z
        );
    }
    
    // converts normalized screen coordinates (0,0 - bottom left) to world global
    public Vector3 ScreenGlobal(float x, float y) {
        x = x * 2 - 1;
        y = y * 2 - 1;
        
        float ratio = screenWidth / (float) screenHeight;
        var pos = transform.position + new Vector3(x * ratio, y, transform.position.z);
        return pos;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum ResizeMode {
        PixelPerfect,
        FixedSize,
    }
            
}
 
#if !UNITY_3_5   
} // namespace
#endif