/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MadLevelManager;
using System.Linq;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[ExecuteInEditMode]   
public class MadLevelBackground : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    public MadDraggable draggable;    
    
    public int startDepth = -20;
    
    public bool ignoreYMovement;
    public bool ignoreXMovement;
    
    [NonSerializedAttribute]
    public List<MadLevelBackgroundLayer> layers = new List<MadLevelBackgroundLayer>();

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    public Vector2 UserPosition {
        get {
            if (draggable == null) {
                return Vector2.zero;
            } else {
                var locPos = draggable.transform.localPosition;
                float x = !ignoreXMovement ? locPos.x : 0;
                float y = !ignoreYMovement ? locPos.y : 0;
                
                return new Vector2(x, y);
            }
        }
    }

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        UpdateLayers();
    }
    
    void Update() {
        if (Application.isEditor) {
            UpdateLayers();
        }
        UpdateDepth();
    }
    
    void UpdateLayers() {
        layers.Clear();
        
        layers.AddRange(MadTransform.FindChildren<MadLevelBackgroundLayer>(transform));
        layers = layers.OrderBy(o => o.name).ToList(); // sort by name
        
        foreach (var l in layers) {
            var sprite = l.GetComponent<MadSprite>();
            sprite.hideFlags = HideFlags.HideInInspector;
        }
    }
    
    public void UpdateDepth() {
        int depth = startDepth;
    
        foreach (var layer in layers) {
            var sprite = layer.GetComponent<MadSprite>();
            sprite.guiDepth = depth++;
            layer.Update();
        }
    }
    
    public void RemoveLayer(MadLevelBackgroundLayer layer) {
        MadGameObject.SafeDestroy(layer.gameObject);
        layers.Remove(layer);
    }
    
    public int IndexOf(MadLevelBackgroundLayer layer) {
        return layers.IndexOf(layer);
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
}

#if !UNITY_3_5
} // namespace
#endif