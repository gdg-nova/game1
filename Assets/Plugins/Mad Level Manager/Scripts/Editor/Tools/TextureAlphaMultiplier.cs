/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TextureAlphaMultiplier {

    #region Static Methods
    
    //[MenuItem("Assets/Mad Level Manager/Premultiply Selected Textures")]
    public static void PremultiplyAlphaForSelection() {
        foreach (var obj in Selection.objects) {
            if (obj is Texture2D) {
                var newTexture = PremultiplyAlpha(obj as Texture2D);
                string path = AssetDatabase.GetAssetPath(obj);
                string prePath = PrePath(path);

                var bytes = newTexture.EncodeToPNG();
                GameObject.DestroyImmediate(newTexture);

                File.WriteAllBytes(prePath, bytes);
                AssetDatabase.Refresh();
            }
        }
    }

    public static Texture2D PremultiplyAlpha(Texture2D texture) {
        var output = new Texture2D(
            texture.width, texture.height,
            TextureFormat.ARGB32, false);



        for (int y = 0; y < texture.height; ++y) {
            for (int x = 0; x < texture.width; ++x) {
                Color c = texture.GetPixel(x, y);
                Color nc = new Color(c.r * c.a, c.g * c.a, c.b * c.a, c.a);
                output.SetPixel(x, y, nc);
            }
        }

        return output;
    }

    private static string PrePath(string path) {
        int dot = path.LastIndexOf('.');
        return path.Substring(0, dot) + "_pre" + path.Substring(dot);
    }

    #endregion

}