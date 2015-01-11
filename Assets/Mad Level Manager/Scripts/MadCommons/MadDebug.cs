/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

public class MadDebug {

    // ===========================================================
    // Constants
    // ===========================================================
    
    public const string internalPostfix =
        "\nThis is an internal error. Please report this to support@madpixelmachine.com";

    // ===========================================================
    // Fields
    // ===========================================================
    
    static HashSet<string> messages = new HashSet<string>();
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static void Assert(bool condition, string message) {
//        System.Diagnostics.Debug.Assert(condition, text);
        if (!condition) {
            throw new AssertException(message);
        }
    }
    
    public static void Log(string message) {
        Log(message, null);
    }
    
    public static void Log(string message, UnityEngine.Object context) {
#if MAD_DEBUG
        if (context != null) {
            Debug.Log(message, context);
        } else {
            Debug.Log(message);
        }
#endif
    }
    
    public static void LogOnce(string message) {
        LogOnce(message, null);
    }
    
    public static void LogOnce(string message, UnityEngine.Object context) {
        if (messages.Contains(message)) {
            return;
        }
        
        messages.Add(message);
        Debug.Log(message, context);
    }
    
    public static void LogWarningOnce(string message) {
        LogWarningOnce(message, null);
    }
    
    public static void LogWarningOnce(string message, UnityEngine.Object context) {
        if (messages.Contains(message)) {
            return;
        }
        
        messages.Add(message);
        Debug.LogWarning(message, context);
    }
    
    public static void LogErrorOnce(string message) {
        LogErrorOnce(message, null);
    }
    
    public static void LogErrorOnce(string message, UnityEngine.Object context) {
        if (messages.Contains(message)) {
            return;
        }
        
        messages.Add(message);
        Debug.LogError(message, context);
    }

    public static void Internal(string message) {
        Debug.LogError(message + internalPostfix);
    }
    
    public static void Internal(string message, Object context) {
        Debug.LogError(message + internalPostfix, context);
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public class AssertException : System.Exception {
        public AssertException(string message) : base(message) { }
    }

}

} // namespace