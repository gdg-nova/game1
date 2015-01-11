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

public class MadGameObject : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    // activation methods because method how unity handles object activation changed in 4.0
    
    public static void SetActive(GameObject go, bool active) {
#if UNITY_3_5
        go.SetActiveRecursively(active);
#else
        go.SetActive(active);
#endif
    }
    
    public static bool IsActive(GameObject go) {
#if UNITY_3_5
        return go.active;
#else
        return go.activeInHierarchy;
#endif    
    }
    
    public static void SafeDestroy(Object go) {
        if (Application.isPlaying) {
            GameObject.Destroy(go);
        } else {
            GameObject.DestroyImmediate(go);
        }
    }
    
    public static bool AnyNull(params object[] objects) {
        foreach (object obj in objects) {
            if (obj == null) {
                return true;
            }
        }
        
        return false;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif