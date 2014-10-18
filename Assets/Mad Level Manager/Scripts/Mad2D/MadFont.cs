
/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
 
[ExecuteInEditMode]   
public class MadFont : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // build properties
    public InputType inputType = InputType.TextureAndGlyphList;
    
    // common properties
    public Texture2D texture;
    public bool forceWhite; // force white color rendering
    
    // texture and glyph list properties
    public string glyphs;
    public int linesCount = 1;
    public float fillFactorTolerance = 0.01f;

    // glyph designer properties
    public TextAsset fntFile;
    
    public CreateStatus createStatus = default(CreateStatus);
    
    public bool created;
    
    public Material material;
    public string dimensions;
    
    private MadFontData _data;
    public MadFontData data {
        get {
            if (_data == null || dirty) {
                _data = MadFontData.Parse(dimensions, texture);
                dirty = false;
            }
            
            return _data;
        }
    }
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    public float textureAspect {
        get {
            return material.mainTexture.width / (float) material.mainTexture.height;
        }
    }
    
    public bool initialized {
        get; private set;
    }
    
    public bool dirty {
        get; set;
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    public override int GetHashCode() {
        var hash = new MadHashCode();
        hash.Add(texture);
        hash.Add(glyphs);
        hash.Add(linesCount);
        hash.Add(fillFactorTolerance);
        hash.Add(createStatus);
        hash.Add(created);
        hash.Add(material);
        hash.Add(dimensions);
        
        return hash.GetHashCode();
    }

    public Glyph GlyphFor(char c) {
        if (!data.charTable.ContainsKey(c)) {
            if (c == ' ') {
                return Space();
            } else {
                return null;
            }
        }
        
        var ch = data.charTable[c];
        var g = new Glyph();
        
        g.x = ch.x / (float) data.commonScaleW;
        g.y = 1 * ((ch.y /*+ ch.height*/) / (float) data.commonScaleH);
        g.width = ch.width / (float) data.commonScaleW;
        g.height = ch.height / (float) data.commonScaleH;
        g.xAdvance = ch.xAdvance / (float) data.commonScaleW;
        g.xOffset = ch.xOffset / (float) data.commonScaleW;
        g.yOffset = (data.infoSize - (ch.yOffset + ch.height)) / (float) data.commonScaleH;
        
        g.widthPx = ch.width;
        g.heightPx = ch.height;
        
        return g;
    }
    
    Glyph Space() {
        return GlyphFor('-') ?? GlyphFor('1'); // hack
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public class Glyph {
        // from top-left
        public float x, y, width, height;
        
        
        public int widthPx, heightPx;
        public float xAdvance;
        public float xOffset, yOffset;
        
        public float uMin {get {return x;}}
        public float uMax {get {return x + width;}}
        public float vMin {get {return 1 - (y + height);}}
        public float vMax {get {return 1 - (y);}}
        
        public override string ToString ()
        {
            return string.Format("[glyph x={0}, y={1}, width={2}, height={3}]", x, y, width, height);
        }
        
    }

    public enum CreateStatus {
        None,
        Ok,
        TooMuchGlypsDefined,
        TooMuchGlypsFound,
    }
    
    public enum InputType {
        TextureAndGlyphList,
        Bitmap,
    }
}

#if !UNITY_3_5
} // namespace
#endif
