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
public class MadBigMeshRenderer : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    MadPanel panel;
    
    // helpers for decrasing GC activity
    MadList<Vector3> vertices = new MadList<Vector3>();
    MadList<Color32> colors = new MadList<Color32>();
    MadList<Vector2> uv = new MadList<Vector2>();
    MadList<MadList<int>> triangleList = new MadList<MadList<int>>();

    MadObjectPool<MadList<int>> trianglesPool = new MadObjectPool<MadList<int>>(32);

    public MadDrawCall drawCall;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        if (drawCall == null) {
            drawCall = MadDrawCall.Create();
            drawCall.gameObject.layer = gameObject.layer;

            MadTransform.SetLocalScale(drawCall.transform, transform.lossyScale);
        }

        panel = GetComponent<MadPanel>();

        if (drawCall != null) {
            MadGameObject.SetActive(drawCall.gameObject, true);
        }
    }

    void OnDisable() {
        if (drawCall != null) {
            MadGameObject.SetActive(drawCall.gameObject, false);
        }
    }

    void Start() {
    }

    void Update() {
        MadTransform.SetLocalScale(drawCall.transform, transform.lossyScale);

        drawCall.transform.position = transform.position;
        drawCall.transform.rotation = transform.rotation;

        drawCall.gameObject.layer = gameObject.layer;
    }

    void LateUpdate() {
        if (panel == null) {
            panel = GetComponent<MadPanel>();
        }

        var mesh = drawCall.mesh;

        mesh.Clear();

        var visibleSprites = VisibleSprites(panel.sprites);
        SortByGUIDepth(visibleSprites);
        var batchedSprites = Batch(visibleSprites);

        Material[] materials = new Material[batchedSprites.Count];

        mesh.subMeshCount = batchedSprites.Count;

        for (int i = 0; i < batchedSprites.Count; ++i) {
            List<MadSprite> sprites = batchedSprites[i];

            MadList<int> triangles;

            if (!trianglesPool.CanTake()) {
                trianglesPool.Add(new MadList<int>());
            }

            triangles = trianglesPool.Take();

            for (int j = 0; j < sprites.Count; ++j) {
                var sprite = sprites[j];
                Material material;
                sprite.DrawOn(ref vertices, ref colors, ref uv, ref triangles, out material);
                materials[i] = material;
            }
        
            triangles.Trim();
            triangleList.Add(triangles);
        }

        vertices.Trim();
        colors.Trim();
        uv.Trim();
        triangleList.Trim();

        mesh.vertices = vertices.Array;
        mesh.colors32 = colors.Array;
        mesh.uv = uv.Array;

        // excluding for metro, because of a bug
#if !UNITY_METRO
        mesh.RecalculateNormals();
#endif

        for (int i = 0; i < triangleList.Count; ++i) {
            MadList<int> triangles = triangleList[i];
            mesh.SetTriangles(triangles.Array, i);
            triangles.Clear();
            trianglesPool.Release(triangles);
        }

        //renderer.sharedMaterials = materials;
        drawCall.SetMaterials(materials);

        vertices.Clear();
        colors.Clear();
        uv.Clear();
        triangleList.Clear();
    }

    void OnDestroy() {
        if (drawCall != null) {
            drawCall.Destroy();
        }
    }

    List<MadSprite> VisibleSprites(ICollection<MadSprite> sprites) {
        List<MadSprite> output = new List<MadSprite>();
        
        foreach (var sprite in sprites) {
            bool active = 
#if UNITY_3_5
        sprite.gameObject.active;
#else
        sprite.gameObject.activeInHierarchy;
#endif
            if (active && sprite.visible && sprite.tint.a != 0 && sprite.CanDraw()) {
                output.Add(sprite);
            }
        }
        
        return output;
    }
    
    void SortByGUIDepth(List<MadSprite> sprites) {
        sprites.Sort((x, y) => x.guiDepth.CompareTo(y.guiDepth));
    }
    
    List<List<MadSprite>> Batch(List<MadSprite> sprites) {
        var output = new List<List<MadSprite>>();
        
        int count = sprites.Count;
        List<MadSprite> batched = null;
        for (int i = 0; i < count; ++i) {
            var currentSprite = sprites[i];
            if (batched == null) {
                batched = new List<MadSprite>();
            } else if (!CanBatch(currentSprite, batched[batched.Count - 1])) {
                output.Add(batched);
                batched = new List<MadSprite>();
            }
            
            batched.Add(currentSprite);
        }
        
        if (batched != null) {
            output.Add(batched);
        }
        
        return output;
    }
    
    bool CanBatch(MadSprite a, MadSprite b) {
        var materialA = a.GetMaterial();
        var materialB = b.GetMaterial();
        
        return materialA.Equals(materialB);
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