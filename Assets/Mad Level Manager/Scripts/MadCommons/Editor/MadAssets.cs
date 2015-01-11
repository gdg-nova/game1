/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MadLevelManager {

public class MadAssets : MonoBehaviour {

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
    // Static Methods
    // ===========================================================
    
    public static string[] ListAllScenes() {
        List<string> output = ListFiles(Application.dataPath, "*.unity");
        output = output.ConvertAll((input) => input.Substring(Application.dataPath.Length + 1));
        return output.ToArray();
    }
    
    public static string[] ListAllProjectFiles() {
        List<string> output = ListFiles(Application.dataPath, "*");
        return output.ToArray();
    }
    
    static List<string> ListFiles(string dir, string searchPattern) {
        List<string> output = new List<string>();
    
        foreach (string d in Directory.GetDirectories(dir)) {
            output.AddRange(ListFiles(d, searchPattern));
        }
        
        foreach (string f in Directory.GetFiles(dir, searchPattern)) {
            output.Add(FixSlashes(f));
        }
        
        return output;
    }
    
    public static string FixSlashes(string path) {
        return path.Replace("\\", "/");
    }

    public static void CreateAsset<T>() where T : ScriptableObject {
        CreateAsset<T>(typeof(T).ToString());
    }
            
    // thanks to http://www.jacobpennock.com/Blog/?p=670
    public static void CreateAsset<T>(string baseName) where T : ScriptableObject {
        T asset = ScriptableObject.CreateInstance<T> ();
        
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")  {
            path = "Assets";
        } else if (System.IO.Path.GetExtension (path) != "")  {
            path = path.Replace (System.IO.Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
        }
        
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + baseName + ".asset");
        
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    
    // path like "Assets/scene.unity"
    public static bool Exists(string path) {
        return File.Exists(path);
    }

    public static T TryLoadComponent<T>(string guid) where T : Component {
        var go = TryLoadGameObject(guid);
        if (go == null) {
            return null;
        }

        return go.GetComponent<T>();
    }

    public static GameObject TryLoadGameObject(string guid) {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(assetPath)) {
            return null;
        }

        var go = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
        return go;
    }
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace