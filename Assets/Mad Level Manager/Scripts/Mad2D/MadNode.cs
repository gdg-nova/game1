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

public class MadNode : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // tells if this node is managed by other script. This means that
    // it should not be edited by user
    [HideInInspector]
    public bool managed;
    
    // Tells that MadNode is currently creating instance.
    // This flag is used to prevent error messages that rely on
    // things like transform position in hierarchy (which is usually
    // wrong right after Instantiate()).
    public static bool Instantiating {
        get; private set;
    }
  
    MadRootNode Root {
        get {
            if (this is MadRootNode) {
                return (MadRootNode) this;
            } else {
                var c = this.transform;
                do {
                    c = c.parent;
                    var rootNode = c.GetComponent<MadRootNode>();
                    if (rootNode != null) {
                        return rootNode;
                    }
                } while (c.parent != null);
                
                Debug.LogError("Missing root node?!");
                
                return null;
            }
        }
    }
                    
    // ===========================================================
    // Constructors
    // ===========================================================
    
//    public Node(GameObject go) {
//        this.gameObject = go;
//    }

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
//    public Node CreateChild(string name) {
//        var go = new GameObject(name);
//        gameObject.transform.parent = go.transform;
//        return new Node(go);
//    }
    
    public T CreateChild<T>(string name) where T : MadNode {
        GameObject go = null;
    
        try {
            Instantiating = true;
            go = new GameObject(name);
            go.transform.parent = gameObject.transform;
            
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            
        } finally {
            Instantiating = false;
        }
        
        var component = go.AddComponent<T>();
        return component;
    }
    
    public T CreateChild<T>(string name, T template) where T : MadNode {
        return (T) CreateChild(name, template.gameObject);
    }
    
    public MadNode CreateChild(string name, GameObject template) {
        GameObject go = null;
        try {
            Instantiating = true;
            go = GameObject.Instantiate(template) as GameObject;
            go.transform.parent = gameObject.transform;
            go.name = name;
        } finally {
            Instantiating = false;
        }
        var node = go.GetComponent<MadNode>();
        
        if (node == null) {
            Debug.LogError("Template doesn't have Node component");
            return null;
        }
        
        return node;
    }
    
    public T FindParent<T>() where T : Component {
        var c = this.transform.parent;
        while (c != null) {
            var comp = c.GetComponent<T>();
            if (comp != null) {
                return comp;
            }
            
            c = c.parent;
        }
        
        return null;
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