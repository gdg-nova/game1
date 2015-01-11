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

public class MadText : MadSprite {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadFont font;
    public string text = "";

    public Align align = Align.Left;
    
    public float scale = 24;
    public float letterSpacing = 1;

    public bool wordWrap = false;
    public float wordWrapLength = 1000;

    private int hash;

    private int linesCount;
    private Rect bounds;

    private List<string> lines = new List<string>();
    private List<float> lineWidths = new List<float>();

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void Update() {
        base.Update();
    }

    public override Rect GetBounds() {
        return bounds;
    }

    // ===========================================================
    // Methods
    // ===========================================================

    private void UpdateTextIfNeeded() {
        int h = MadHashCode.FirstPrime;
        h += MadHashCode.Add(h, text);
        h += MadHashCode.Add(h, wordWrap);
        h += MadHashCode.Add(h, wordWrapLength);
        h += MadHashCode.Add(h, scale);

        if (h != hash) {
            UpdateText();
            hash = h;
        }
    }

    private void UpdateText() {
        lineWidths.Clear();
        lines.Clear();

        if (!CanDraw()) {
            linesCount = 0;
            bounds = new Rect();
            return;
        }

        string[] tempLines = text.Split('\n');
        linesCount = 0;

        float totalWidth = 0;
        for (int i = 0; i < tempLines.Length; ++i) {
            string line = tempLines[i];
            float lineWidth = LineWidth(line);

            if (wordWrap && lineWidth > wordWrapLength) {
                List<string> wrap = WordWrap(line, lineWidth);

                for (int j = 0; j < wrap.Count; ++j) {
                    lines.Add(wrap[j]);
                    float width = LineWidth(wrap[j]);
                    lineWidths.Add(width);
                    totalWidth = Mathf.Max(totalWidth, width);
                    linesCount++;
                }
            } else {
                lines.Add(line);
                totalWidth = Mathf.Max(totalWidth, lineWidth);
                lineWidths.Add(lineWidth);
                linesCount++;
            }
            
        }

        float totalHeight = scale * linesCount;

        var rawBounds = new Rect(0, 0, totalWidth, totalHeight);

        UpdatePivotPoint();
        var start = PivotPointTranslate(new Vector2(0, 0), rawBounds);
        var end = PivotPointTranslate(new Vector2(totalWidth, totalHeight), rawBounds);

        bounds = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);

        UpdateCollider();
    }

    private void UpdateCollider() {
        var box = GetComponent<BoxCollider>();
        if (box != null) {
            box.center = bounds.center;
            box.size = new Vector3(bounds.width, bounds.height, 0.01f);
        }
    }

    private float LineWidth(string text) {
        float lineWidth = 0;

        foreach (char c in text) {
            var glyph = font.GlyphFor(c);
            if (glyph == null) {
                Debug.LogWarning("No glyph found for '" + c + "' (code " + (int) c + ")");
                continue;
            }

            float xAdvance;
            GlyphBounds(glyph, out xAdvance);
            lineWidth += xAdvance;
        }

        return lineWidth;
    }

    private List<string> WordWrap(string text, float lineWidth) {
        string[] words = text.Split(' ');
        float totalWidth = 0;
        float spaceWidth = LineWidth(" ");

        //Debug.Log(lineWidth);

        int firstWord = 0;
        List<string> lines = new List<string>();

        for (int i = 0; i < words.Length; ++i) {
            float wordWidth = LineWidth(words[i]);

            if (totalWidth + spaceWidth + wordWidth > wordWrapLength) {
                if (i != firstWord) {
                    var line = Join(words, firstWord, i - firstWord, " ");
                    lines.Add(line);
                    firstWord = i;
                    totalWidth = wordWidth;
                } else {
                    var line = Join(words, firstWord, 1, " ");
                    lines.Add(line);
                    firstWord = i + 1;
                    totalWidth = 0;
                }
                
            } else {
                totalWidth += spaceWidth + wordWidth;
            }
        }

        if (firstWord < words.Length) {
            var line = Join(words, firstWord, words.Length - firstWord, " ");
            lines.Add(line);
        }

        return lines;
    }

    private string Join(string[] words, int index, int count, string glue) {
        if (count <= 0) {
            Debug.LogError("Illegal argument: " + count);
            return "";
        }

        var builder = new System.Text.StringBuilder(count * 7);
        for (int i = 0; i < count; ++i) {
            if (i > 0) {
                builder.Append(glue);
            }
            builder.Append(words[index + i]);
        }

        return builder.ToString();
    }

    private Rect GlyphBounds(MadFont.Glyph g, out float xAdvance) {
        float realScale = scale;
        float xOffset = 0, yOffset = 0;

        float baseScale = font.data.infoSize / (float) font.data.commonScaleH;
        realScale = g.height / baseScale * scale;
        xOffset = g.xOffset / baseScale * scale;
        yOffset = g.yOffset / baseScale * scale; // wrong?
        xAdvance = g.xAdvance / baseScale * scale * font.textureAspect;

        float w = (realScale / g.height) * g.width;

        return new Rect(xOffset, yOffset, w * font.textureAspect, realScale);
    }
    
    public override bool CanDraw() {
        return font != null && !string.IsNullOrEmpty(text);
    }
    
    public override Material GetMaterial() {
        return font.material;
    }
          
    public override void DrawOn(ref MadList<Vector3> vertices, ref MadList<Color32> colors, ref MadList<Vector2> uv,
                ref MadList<int> triangles, out Material material) {

        UpdateTextIfNeeded();
        UpdatePivotPoint();
        
        var matrix = TransformMatrix();
        var bounds = GetBounds();
        
        material = font.material;
        float x = 0;
        float y = linesCount * scale - scale;

        for (int i = 0; i < linesCount; ++i) {
            string text = lines[i];
            float lineWidth = lineWidths[i];

            switch (align) {
                case Align.Left:
                    x = 0;
                    break;
                case Align.Center:
                    x = (bounds.width - lineWidth) / 2;
                    break;
                case Align.Right:
                    x = bounds.width - lineWidth;
                    break;
                default:
                    Debug.LogError("Unknown align: " + align);
                    x = 0;
                    break;
            }

            for (int j = 0; j < text.Length; ++j) {
                char c = text[j];

                int offset = vertices.Count;

                var glyph = font.GlyphFor(c);
                if (glyph == null) {
                    Debug.LogWarning("Glyph not found: '" + c + "' (code " + (int) c + ")");
                    continue;
                }

                //            float w = (scale / glyph.height) * glyph.width * font.textureAspect;
                float xAdvance;
                var gBounds = GlyphBounds(glyph, out xAdvance);

                if (c != ' ') { // render anything but space
                    float left = x + gBounds.x;
                    float bottom = y + gBounds.y;
                    float right = left + gBounds.width;
                    float top = bottom + gBounds.height;

                    vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(left, bottom, 0), bounds)));
                    vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(left, top, 0), bounds)));
                    vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(right, top, 0), bounds)));
                    vertices.Add(matrix.MultiplyPoint(PivotPointTranslate(new Vector3(right, bottom, 0), bounds)));

                    colors.Add(tint);
                    colors.Add(tint);
                    colors.Add(tint);
                    colors.Add(tint);

                    uv.Add(new Vector2(glyph.uMin, glyph.vMin));
                    uv.Add(new Vector2(glyph.uMin, glyph.vMax));
                    uv.Add(new Vector2(glyph.uMax, glyph.vMax));
                    uv.Add(new Vector2(glyph.uMax, glyph.vMin));


                    triangles.Add(0 + offset);
                    triangles.Add(1 + offset);
                    triangles.Add(2 + offset);

                    triangles.Add(0 + offset);
                    triangles.Add(2 + offset);
                    triangles.Add(3 + offset);

                    //                x += gBounds.width + letterSpacing;
                } else {
                    //                x += gBounds.width; // no letter spacing for blank characters
                }

                x += xAdvance;
            }

            // end of line
            y -= scale;
            
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

    public enum Align {
        Left,
        Center,
        Right,
    }

}

#if !UNITY_3_5
} // namespace
#endif