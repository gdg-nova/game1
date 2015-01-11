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
 
[RequireComponent(typeof(MadSprite))]   
public class MadLevelBackgroundLayer : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================
    
    public const float ScrollSpeedMultiplier = 0.01f;

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Texture2D texture;
    public Color tint = Color.white;
    
    public Vector2 scale = Vector2.one;
    public ScaleMode scaleMode;
    public Align align = Align.Middle;

    public bool repeatX = true;
    public bool repeatY = false;

    public float fillMarginLeft = -2, fillMarginTop = -2, fillMarginRight = -2, fillMarginBottom = -2;
    
    // how big can be the stretch
    public bool dontStretch = true;
    
    public Vector2 position = Vector2.zero;
    
    public float followSpeed = 1;
    public Vector2 scrollSpeed; // texture lengths per second
    
    Vector2 scrollAccel;
    
    MadRootNode _root;
    MadRootNode root {
        get {
            if (_root == null) {
                _root = MadTransform.FindParent<MadRootNode>(transform);
            }
            
            return _root;
        }
    }
    
    MadLevelBackground _parent;
    public MadLevelBackground parent {
        get {
            if (_parent == null) {
                _parent = MadTransform.FindParent<MadLevelBackground>(transform);
            }
            
            return _parent;
        }
    }
    
    
    MadSprite _sprite;
    MadSprite sprite {
        get {
            if (_sprite == null) {
                _sprite = GetComponent<MadSprite>();
            }
            
            return _sprite;
        }
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    void Start() {
        SetDirty();
    }
    
    public void SetDirty() {
        sprite.texture = texture;
        sprite.tint = tint;
        
        int index = parent.IndexOf(this);
        name = string.Format("{0:D2} layer ({1})", index, texture != null ? texture.name : "empty");
    }
    
    public void Cleanup() {
        if (sprite != null) {
            if (Application.isPlaying) {
                Destroy(gameObject);
            } else {
                MadUndo.DestroyObjectImmediate(gameObject);
            }
        }
    }
    
    public void Update() {
    
        if (sprite == null || sprite.texture == null) {
            return;
        }

        float screenLeft = 0;
        float screenBottom = 0;
        float screenRight = root.screenWidth;
        float screenTop = root.screenHeight;

        // margins can be only applied on fill and with no repeat
        if (scaleMode == ScaleMode.Fill && !repeatX && !repeatY) {
            screenLeft = fillMarginLeft;
            screenBottom = fillMarginBottom;
            screenRight -= fillMarginRight;
            screenTop -= fillMarginTop;
        }

        float screenWidth = screenRight - screenLeft;
        float screenHeight = screenTop - screenBottom;
        
        float scaleX = scale.x;
        float scaleY = scale.y;
        
        float spriteWidth = screenWidth;
        float spriteHeight = screenHeight;
        
        switch (scaleMode) {
            case ScaleMode.Fill:
            	if (repeatX && repeatY) {
					scaleX = sprite.texture.width / screenWidth;
					scaleY = sprite.texture.height / screenHeight;
            	} else if (repeatX || repeatY || dontStretch) {
					scaleX = (screenHeight / (float) sprite.texture.height)
						* (sprite.texture.width / (float) screenWidth);
					scaleY = (screenWidth / (float) sprite.texture.width)
						* (sprite.texture.height / (float) screenHeight);
				}
				break;
            case ScaleMode.Manual:
                spriteHeight = sprite.texture.height * scaleY;
                scaleX *= sprite.texture.width / (float) screenWidth;
                break;
        }
        
		bool stretchX = true;
		bool stretchY = true;
		
		if (dontStretch && !repeatX && !repeatY) {
			float ratio =
				(sprite.texture.width / (float) sprite.texture.height) / (screenWidth / screenHeight);
		
			if (ratio > 1) {
				stretchX = false;
			} else if (ratio < 1) {
				stretchY = false;
			}
		}
        
        // scale to fill whole screen, but keep the scale set by user
        sprite.size = new Vector2(spriteWidth, spriteHeight);
        
        if (scaleMode == ScaleMode.Manual) {
            switch (align) {
                case Align.None:
                    sprite.transform.localPosition =
                        new Vector3(0, Screen.height * position.y, 0);
                    break;
            
                case Align.Middle:
                    sprite.transform.localPosition =
                        new Vector3(0, position.y, 0);
                    break;
                
                case Align.Bottom:
                    sprite.transform.localPosition =
                        new Vector3(
                            0,
                            screenHeight * (- 0.5f) + position.y + (0.5f * scaleY * sprite.texture.height),
                            0);
                    break;
                    
                case Align.Top:
                    sprite.transform.localPosition =
                        new Vector3(
                            0,
                            screenHeight * (0.5f) + position.y + (-0.5f * scaleY * sprite.texture.height),
                            0);
                    break;
            }
        } else { // ScaleMode.Fill
            sprite.transform.localPosition = new Vector3(0, 0, 0);
        }
        
        // set proper repeat
        float rx = 1, ry = 1;
        
//        sprite.textureRepeat = new Vector2(rx * (1 / scaleX), ry * (1 / scaleY));
		sprite.textureRepeat = new Vector2(
			repeatX || !stretchX ? rx * (1 / scaleX) : rx,
			repeatY || !stretchY ? ry * (1 / scaleY) : ry);
        
        // follow draggable
        var locPos = parent.UserPosition;
        float dx = -locPos.x * followSpeed;
        float dy = -locPos.y * followSpeed;

        dx /= root.screenWidth;
        dy /= root.screenHeight;
        
        float offsetX = 0;
        float offsetY = 0;
        
        if (!stretchX) {
			offsetX = -(sprite.textureRepeat.x - 1) / 2;
        } else if (!stretchY) {
        	offsetY = -(sprite.textureRepeat.y - 1) / 2;
        }
        
//        sprite.textureOffset = new Vector2(dx * (1 / scaleX) + position.x, dy * (1 / scaleY) - position.y);
			sprite.textureOffset = new Vector2(
				repeatX || !stretchX ? dx * (1 / scaleX) + position.x + offsetX : dx,
				repeatY || !stretchY ? dy * (1 / scaleY) - position.y + offsetY : dy);
        
        if (scrollSpeed != Vector2.zero) {
            scrollAccel +=
                new Vector2(
                    scrollSpeed.x * ScrollSpeedMultiplier / scale.x,
                    scrollSpeed.y * ScrollSpeedMultiplier / scale.y)
                * Time.deltaTime;
                
            scrollAccel = new Vector2(scrollAccel.x % 1, scrollAccel.y % 1);
            sprite.textureOffset += scrollAccel;
        }

        UpdateFillMargin();
    }

    void UpdateFillMargin() {
        if (scaleMode == ScaleMode.Fill && !repeatX && !repeatY) {
            // pivot point is center here, so it is treated as is
            sprite.transform.localPosition += new Vector3(
                fillMarginLeft - fillMarginRight,
                fillMarginBottom - fillMarginTop, 0)
                / 2;
        }
    }
       
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum ScaleMode {
        Manual,
        Fill,
    }
    
    public enum Align {
        None,
        Top,
        Middle,
        Bottom,
    }

}

#if !UNITY_3_5
} // namespace
#endif