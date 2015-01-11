/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadLevelManager {

public class MadScreen {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Properties
    // ===========================================================
    
    // TODO: Unity 4.3 may have that issue fixed
    
    public static int width {
        get {
            #if UNITY_EDITOR
            string[] res = UnityStats.screenRes.Split('x');
            return int.Parse(res[0]);
            #else
            return Screen.width;
            #endif
        }
    }
    
    public static int height {
        get {
            #if UNITY_EDITOR
            string[] res = UnityStats.screenRes.Split('x');
            return int.Parse(res[1]);
            #else
            return Screen.height;
            #endif
        }
    }

    // ===========================================================
    // Methods
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace