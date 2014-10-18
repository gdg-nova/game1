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
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MadDrawCall : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    public Mesh mesh;

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
        var meshFilter = transform.GetComponent<MeshFilter>();
        if (mesh == null) {
            mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSave;
            meshFilter.mesh = mesh;
        }
#if !UNITY_3_5
        mesh.MarkDynamic();
#endif

#if UNITY_4_2
        if (Application.unityVersion.StartsWith("4.2.0")) {
            Debug.LogError("Unity 4.2 comes with terrible bug that breaks down Mad Level Manager rendering process. "
                + "Please upgrade/downgrade to different version. http://forum.unity3d.com/threads/192467-Unity-4-2-submesh-draw-order");
            }
#endif
    }

    void Update() {
    }

    void OnDestroy() {
        if (Application.isEditor) {
            DestroyImmediate(mesh);
        } else {
            Destroy(mesh);
        }
    }

    public void SetMaterials(Material[] materials) {
        var shared = renderer.sharedMaterials;

        if (shared.Length != materials.Length) {
            renderer.sharedMaterials = materials;
            return;
        }

        for (int i = 0; i < shared.Length; ++i) {
            var s = shared[i];
            var m = materials[i];

            if (s != m) {
                renderer.sharedMaterials = materials;
                return;
            }
        }
    }

    public void Destroy() {
        MadGameObject.SetActive(gameObject, false);
        GameObject.DestroyImmediate(gameObject);
        //MadGameObject.SafeDestroy(gameObject);
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    public static MadDrawCall Create() {
#if UNITY_EDITOR
        GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("_draw_call",
 #if MAD_DEBUG
            HideFlags.DontSave,
 #else
            HideFlags.HideAndDontSave,
 #endif
            typeof(MadDrawCall));
#else
        GameObject go = new GameObject("_draw_call");
        go.hideFlags = 
 #if MAD_DEBUG
            HideFlags.DontSave;
 #else
            HideFlags.HideAndDontSave;
#endif

        go.AddComponent<MadDrawCall>();
#endif
        return go.GetComponent<MadDrawCall>();
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif