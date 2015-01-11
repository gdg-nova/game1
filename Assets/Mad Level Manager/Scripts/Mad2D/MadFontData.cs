/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MadLevelManager;
using System;
using System.Globalization;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

// BMFont format reader/writer
public class MadFontData {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Dictionary<char, Char> charTable = new Dictionary<char, Char>();
    public Dictionary<char, Dictionary<char, Kerning>> kerningTable = new Dictionary<char, Dictionary<char, Kerning>>();
    
    public string infoFace;
    public int infoSize;
    public bool infoBold;
    public bool infoItalic;
    public string infoCharset;
    public bool infoUnicode;
    public int infoStretchH;
    public bool infoSmooth;
    public bool infoAA;
    public int[] infoPadding;
    public int[] infoSpacing;
    
    public int commonLineHeight;
    public int commonBase;
    public int commonScaleW;
    public int commonScaleH;
    public int commonPages;
    public bool commonPacked;
    
    public int pageId;
    public string pageFile;
    
    public int charsCount;
    
    public int kerningCount;
    
    // parse helpers
    string lineType;
    Dictionary<string, string> lineMap = new Dictionary<string, string>();
    
    // ===========================================================
    // Constructors
    // ===========================================================
    
    private MadFontData() {
    }
    
    public static MadFontData Parse(string text, Texture2D texture) {
        var data = new MadFontData();
        if (text.StartsWith("1")) {
            data.DoParseLegacy(text, texture);
        } else {
            data.DoParse(text);
        }
        return data;
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    void DoParse(string text) {
        string[] lines = text.Split('\n');
        foreach (var line in lines) {
            if (string.IsNullOrEmpty(line)) {
                continue;
            }
            
            UseLine(line);
            
            switch (lineType) {
                case "info":
                    ParseInfo();
                    break;
                case "common":
                    ParseCommon();
                    break;
                case "page":
                    ParsePage();
                    break;
                case "chars":
                    ParseChars();
                    break;
                case "char":
                    ParseChar();
                    break;
                case "kernings":
                    ParseKernings();
                    break;
                case "kerning":
                    ParseKerning();
                    break;
                default:
                    throw new MadDebug.AssertException("Unknown line type: " + lineType);
            }
            
        }
    }
    
    void UseLine(string line) {
        lineMap.Clear();
    
        bool hasType = false;
        bool inCitation = false;
        int startIndex = 0;
        
        line = line.TrimEnd();
        
        for (int i = 0; i < line.Length; ++i) {
            char c = line[i];
            switch (c) {
                case '"':
                    inCitation = !inCitation;
                    break;
                case ' ':
                    if (!inCitation) {
                        string el = line.Substring(startIndex, i - startIndex);
                    
                        if (!hasType) {
                            lineType = el;
                            hasType = true;
                        } else {
                            ReadPair(el);
                        }
                        startIndex = i + 1;
                    }
                    break;
                default:
                    // do nothing
                    break;
            }
        }
        
        // last element
        string e = line.Substring(startIndex);
        ReadPair(e);
    }
    
    void ReadPair(string pair) {
        if (!string.IsNullOrEmpty(pair)) {
            int delimiter = pair.IndexOf("=");
            MadDebug.Assert(delimiter != -1, "Delimiter not found");
            
            var key = pair.Substring(0, delimiter);
            var value = pair.Substring(delimiter + 1);
            
            lineMap[key] = value;
        }
    }
    
    void ParseInfo() {
        infoFace = GetString("face");
        infoSize = GetInt("size");
        infoBold = GetBool("bold");
        infoItalic = GetBool("italic");
        infoCharset = GetString("charset");
        infoUnicode = GetBool("unicode");
        infoStretchH = GetInt("stretchH");
        infoSmooth = GetBool("smooth");
        infoAA = GetBool("aa");
        infoPadding = GetIntArray("padding");
        infoSpacing = GetIntArray("spacing");
    }
    
    void ParseCommon() {
        commonLineHeight = GetInt("lineHeight");
        commonBase = GetInt("base");
        commonScaleW = GetInt("scaleW");
        commonScaleH = GetInt("scaleH");
        commonPages = GetInt("pages");
        commonPacked = GetBool("packed");
    }
    
    void ParsePage() {
        pageId = GetInt("id");
        pageFile = GetString("file");
    }
    
    void ParseChars() {
        charsCount = GetInt("count");
    }
    
    void ParseChar() {
        var c = new Char();
        c.c = (char) GetInt("id");
        c.x = GetInt("x");
        c.y = GetInt("y");
        c.width = GetInt("width");
        c.height = GetInt("height");
        c.xOffset = GetInt("xoffset");
        c.yOffset = GetInt("yoffset");
        c.xAdvance = GetInt("xadvance");
        c.page = GetInt("page");
        c.chnl = GetInt("chnl");
        
        charTable.Add(c.c, c);
    }
    
    void ParseKernings() {
        kerningCount = GetInt("count");
    }
    
    void ParseKerning() {
        var k = new Kerning();
        k.first = (char) GetInt("first");
        k.second = (char) GetInt("second");
        k.amount = GetInt("amount");
        
        Dictionary<char, Kerning> valueDict;
        if (kerningTable.ContainsKey(k.first)) {
            valueDict = kerningTable[k.first];
        } else {
            valueDict = new Dictionary<char, Kerning>();
            kerningTable[k.first] = valueDict;
        }
        
        valueDict.Add(k.second, k);
    }
    
    private string GetString(string key) {
        string value = GetValue(key);
        MadDebug.Assert(value != null, "Key " + key + " not found");
        
        MadDebug.Assert(value[0] == '"' && value[value.Length - 1] == '"', "Key " + key + " not a string");
        return value.Substring(1, value.Length - 2);
    }
    
    private bool GetBool(string key) {
        return GetInt(key) > 0;
    }
    
    private int GetInt(string key) {
        string value = GetValue(key);
        MadDebug.Assert(value != null, "Key " + key + " not found");
        
        int parsed;
        MadDebug.Assert(int.TryParse(value, out parsed), "Key " + key + "not a int");
        
        return parsed;
    }
    
    private int[] GetIntArray(string key) {
        string value = GetValue(key);
        MadDebug.Assert(value != null, "Key " + key + " not found");
        string[] elements = value.Split(',');
        
        int[] result = new int[elements.Length];
        for (int i = 0; i < elements.Length; ++i) {
            result[i] = int.Parse(elements[i]);
        }
        
        return result;
    }
    
    private string GetValue(string key) {
        if (lineMap.ContainsKey(key)) {
            return lineMap[key];
        } else {
            return null;
        }
    }
    
    private void DoParseLegacy(string data, Texture2D texture) {
        charTable.Clear();
    
        int texWidth = texture.width;
        int texHeight = texture.height;
        
        commonScaleW = texWidth;
        commonScaleH = texHeight;
    
        string[] lines = data.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; ++i) {
            string line = lines[i];
            if (string.IsNullOrEmpty(line)) {
                continue;
            }
            
            var parts = line.Split(' ');
            char c = parts[0][0];

            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
            float width = float.Parse(parts[3], CultureInfo.InvariantCulture);
            float height = float.Parse(parts[4], CultureInfo.InvariantCulture);
            
            var ch = new Char();
            ch.c = c;
            ch.x = (int) (x * texWidth);
            ch.y = (int) (y * texHeight);
            ch.width = (int) (width * texWidth);
            ch.height = (int) (height * texHeight);
            ch.xAdvance = (int) (width * texWidth + 3);
            
            if (!charTable.ContainsKey(c)) {
                charTable.Add(c, ch);
            }
            
            infoSize = ch.height; // all characters shares the same height
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

    public class Char {
        public char c;
        public int x, y, width, height;
        public int xOffset, yOffset;
        public int xAdvance;
        public int page;
        public int chnl;
    }
    
    public class Kerning {
        public char first;
        public char second;
        public int amount;
    }
}

#if !UNITY_3_5
} // namespace
#endif