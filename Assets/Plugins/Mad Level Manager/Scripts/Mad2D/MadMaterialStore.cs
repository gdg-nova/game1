/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
// helps to manage material creation and sharing   
public class MadMaterialStore : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    Dictionary<MaterialKey, Material> materials = new Dictionary<MaterialKey, Material>();
    int nextVariation = 1;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    void OnDestroy() {
        // destroy all material
        foreach (var material in materials.Values) {
            DestroyImmediate(material);
        }
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    public Material CreateUnique(Texture texture, string shaderName, out int variation) {
        variation = nextVariation++;
        return CreateMaterial(texture, shaderName, variation);
    }
    
    public Material MaterialFor(Texture texture, string shaderName) {
        return MaterialFor(texture, shaderName, 0);
    }
    
    public Material MaterialFor(Texture texture, string shaderName, int variation) {
        if (texture == null) {
            Debug.LogError("null texture", this);
            return null;
        }
        
        if (shaderName == null) {
            Debug.LogError("null shader name", this);
            return null;
        }
        
        var key = new MaterialKey(texture, shaderName, variation);
        if (materials.ContainsKey(key)) {
            return materials[key];
        } else {
            return CreateMaterial(texture, shaderName, 0);
        }
    }
    
    Material CreateMaterial(Texture texture, string shaderName, int variation) {
        var key = new MaterialKey(texture, shaderName, variation);
    
        Shader shader = Shader.Find(shaderName);
        if (shader == null) {
            Debug.LogError("Shader not found: " + shaderName);
            return null;
        }
        
        var material = new Material(shader);
        material.mainTexture = texture;
        material.hideFlags = HideFlags.DontSave;
        
        materials.Add(key, material);
        
        return material;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    class MaterialKey {
        Texture texture;
        string shaderName;
        int variation;
        
        public MaterialKey(Texture texture, string shaderName, int variation) {
            this.texture = texture;
            this.shaderName = shaderName;
            this.variation = variation;
        }
        
        public override bool Equals(System.Object obj) {
            if (obj == null || !(obj is MaterialKey)) {
                return false;
            } else {
                var other = obj as MaterialKey;
                return texture == other.texture && shaderName == other.shaderName && variation == other.variation;
            }
        }
        
        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 23 + texture.GetHashCode();
            hash = hash * 23 + shaderName.GetHashCode();
            hash = hash * 23 + variation.GetHashCode();
            return hash;
        }
    }

}

#if !UNITY_3_5
} // namespace
#endif