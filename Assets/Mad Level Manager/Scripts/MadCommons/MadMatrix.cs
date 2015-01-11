/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

public class MadMatrix {

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

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void Translate(ref Matrix4x4 matrix, float x, float y, float z) {
        Translate(ref matrix, new Vector3(x, y, z));
    }
    
    public static void Translate(ref Matrix4x4 matrix, Vector3 vector) {
        matrix = Matrix4x4.TRS(vector, Quaternion.identity, Vector3.one) * matrix;
    }
    
    public static void Rotate(ref Matrix4x4 matrix, float x, float y, float z) {
        Rotate(ref matrix, Quaternion.Euler(x, y, z));
    }
    
    public static void Rotate(ref Matrix4x4 matrix, Vector3 rotate) {
        Rotate(ref matrix, Quaternion.Euler(rotate));
    }
    
    public static void Rotate(ref Matrix4x4 matrix, Quaternion rotate) {
        matrix = Matrix4x4.TRS(Vector3.zero, rotate, Vector3.one) * matrix;
    }
    
    public static void Scale(ref Matrix4x4 matrix, float x, float y, float z) {
        Scale(ref matrix, new Vector3(x, y, z));
    }
    
    public static void Scale(ref Matrix4x4 matrix, Vector3 scale) {
        matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale) * matrix;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace