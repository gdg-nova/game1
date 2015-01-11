/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadPath : MonoBehaviour {

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

    public static string MakeRelative(string path) {
        var current = CurrentDirectory();
        path = FixSlashes(path);
        
        if (path.StartsWith(current)) {
            path = path.Substring(current.Length + 1);
        }
        
        return path;
    }
    
    public static string FixSlashes(string path) {
        return path.Replace('\\', '/');
    }
    
    public static string CurrentDirectory() {
        var dir = Directory.GetCurrentDirectory();
        return FixSlashes(dir);
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