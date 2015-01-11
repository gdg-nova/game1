/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

public class MadUndo {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void LegacyRegisterSceneUndo(string name) {
#if UNITY_EDITOR && (UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
        Undo.RegisterSceneUndo(name);
#endif
    }
    
    public static void LegacyRegisterUndo(Object obj, string name) {

#if UNITY_EDITOR && (UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
        Undo.RegisterUndo(obj, name);
#endif
    }
    
    public static void DestroyObjectImmediate(GameObject gameObject) {
#if UNITY_EDITOR
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
        GameObject.DestroyImmediate(gameObject);
#else
        Undo.DestroyObjectImmediate(gameObject);
#endif
#endif
    }
    
    public static void RecordObject(Object obj, string name) {
#if UNITY_EDITOR && (!UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2)
        Undo.RecordObject(obj, name);
#endif
    }
    
    public static void RecordObject2(Object obj, string name) {
        RecordObject(obj, name);
        LegacyRegisterUndo(obj, name);
    }
    
    public static void RegisterCreatedObjectUndo(Object obj, string name) {
#if UNITY_EDITOR && (!UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2)
        Undo.RegisterCreatedObjectUndo (obj, name);
#endif
    } 

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace