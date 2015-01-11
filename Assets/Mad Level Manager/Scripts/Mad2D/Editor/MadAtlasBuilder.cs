/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadAtlasBuilder : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    public static void CreateAtlas() {
        var textures = Selection
            .GetFiltered(typeof(Texture2D), SelectionMode.Assets)
            .Cast<Texture2D>()
            .OrderByDescending(t => t.width * t.height)
            .ToArray();

        if (textures.Length == 0) {
            EditorUtility.DisplayDialog("Create Atlas", "No textures selected. Please select at least one texture.", "OK");
            return;
        }
            
        CreateAtlas(textures);
    }

    private static IEnumerable<Texture2D> MakeReadable(IEnumerable<Texture2D> textures) {
        List<Texture2D> modified = new List<Texture2D>();

        foreach (var texture in textures) {
            var path = AssetDatabase.GetAssetPath(texture);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null) {
                if (!importer.isReadable) {
                    importer.isReadable = true;
                    AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
                    modified.Add(texture);
                }
            }
        }

        return modified;
    }

    private static void RevertReadable(IEnumerable<Texture2D> textures) {
        foreach (var texture in textures) {
            var path = AssetDatabase.GetAssetPath(texture);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null) {
                importer.isReadable = false;
                AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
            }
        }
    }

    public static MadAtlas CreateAtlas(Texture2D[] textures) {
        var modified = MakeReadable(textures);

        try {
            var saveFolder = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(textures[0]));
            var prefabPath = EditorUtility.SaveFilePanel("Save atlas", saveFolder, "New Texture Atlas", "prefab");
            if (string.IsNullOrEmpty(prefabPath)) {
                return null;
            }

            prefabPath = MadPath.MakeRelative(prefabPath);

            var texturePath = System.IO.Path.ChangeExtension(prefabPath, "png");
            List<MadAtlas.Item> items = new List<MadAtlas.Item>();

            PackTextures(textures, texturePath, ref items);

            var go = new GameObject() { name = System.IO.Path.GetFileNameWithoutExtension(prefabPath) };
            var atlas = go.AddComponent<MadAtlas>();
            atlas.atlasTexture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;

            atlas.AddItemRange(items);

            var prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
            prefab.name = atlas.name;
            var prefabGo = PrefabUtility.ReplacePrefab(go, prefab);
            DestroyImmediate(go);

            AssetDatabase.Refresh();

            return prefabGo.GetComponent<MadAtlas>();
        } finally {
            RevertReadable(modified);
            AssetDatabase.Refresh();
        }
    }
    
    public static void AddToAtlas(MadAtlas atlas, Texture2D texture) {
        AddToAtlas(atlas, new Texture2D[] { texture });
    }
    
    public static void AddToAtlas(MadAtlas atlas, Texture2D[] textures) {
        List<MadAtlas.Item> liveItems = LiveItems(atlas);
        List<Texture2D> allTextures = new List<Texture2D>();

        allTextures.AddRange(from i in liveItems select MadAtlasUtil.GetItemOrigin(i));
        allTextures.AddRange(textures);

        var modified = MakeReadable(allTextures);
        try {
            string atlasTexturePath = AssetDatabase.GetAssetPath(atlas.atlasTexture);
            PackTextures(allTextures.ToArray(), atlasTexturePath, ref liveItems);

            atlas.ClearItems();
            atlas.AddItemRange(liveItems);

            EditorUtility.SetDirty(atlas);
        } finally {
            RevertReadable(modified);
            AssetDatabase.Refresh();
        }
    }
    
    public static void RemoveFromAtlas(MadAtlas atlas, MadAtlas.Item item) {
        var liveItems = LiveItems(atlas);
        var newItems = (from el in liveItems where el != item select el).ToList();
        
        atlas.ClearItems();
        
        var allTextures = from el in newItems select MadAtlasUtil.GetItemOrigin(el);
        
        string atlasTexturePath = AssetDatabase.GetAssetPath(atlas.atlasTexture);
        PackTextures(allTextures.ToArray(), atlasTexturePath, ref newItems);
        
        atlas.ClearItems();
        atlas.AddItemRange(newItems);
    }
    
    private static List<MadAtlas.Item> LiveItems(MadAtlas atlas) {
        return (from item in atlas.items where MadAtlasUtil.GetItemOrigin(item) != null select item).ToList();
    }
    
    private static void PackTextures(Texture2D[] textures, string path, ref List<MadAtlas.Item> items) {
        int padding = 2;
        
        var atlasTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        var rects = atlasTexture.PackTextures(textures, padding, 4096);

        if (atlasTexture.format != TextureFormat.ARGB32) {
            // need to rewrite texture to a new one
            var newAtlasTexture = new Texture2D(atlasTexture.width, atlasTexture.height, TextureFormat.ARGB32, false);
            newAtlasTexture.SetPixels32(atlasTexture.GetPixels32());
            newAtlasTexture.Apply();
            DestroyImmediate(atlasTexture);
            atlasTexture = newAtlasTexture;
        }

        var bytes = atlasTexture.EncodeToPNG();
        DestroyImmediate(atlasTexture);
        
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
        
        for (int i = 0; i < textures.Length; ++i) {
            var texture = textures[i];
            var region = rects[i];
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
            var item = (from el in items where el.textureGUID == guid select el).FirstOrDefault();
            
            if (item != null) {
                item.region = region;
            } else {
                item = CreateItem(texture, region);
                items.Add(item);
            }
        }

        // set texture max size to 4086
        var importer = TextureImporter.GetAtPath(path) as TextureImporter;
        importer.maxTextureSize = 4086;
        importer.isReadable = true;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
    
    private static MadAtlas.Item CreateItem(Texture2D texture, Rect region) {
        var item = new MadAtlas.Item();
        
        item.name = texture.name;
        item.pixelsWidth = texture.width;
        item.pixelsHeight = texture.height;
        item.region = region;
        item.textureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
        
        return item;
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