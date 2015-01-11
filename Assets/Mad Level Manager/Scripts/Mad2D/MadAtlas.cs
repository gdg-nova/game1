/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadAtlas : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Texture2D atlasTexture;
    
    public List<Item> items = new List<Item>();
    
    // guid to item
    private Dictionary<string, Item> map = new Dictionary<string, Item>();

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    public bool AddItem(Item item) {
        if (!map.ContainsKey(item.textureGUID)) {
            items.Add(item);
            map.Add(item.textureGUID, item);
            
            return true;
        } else {
            return false;
        }
    }
    
    public void AddItemRange(IEnumerable<Item> items) {
        foreach (var item in items) {
            AddItem(item);
        }
    }
    
    public Item GetItem(string guid) {
        Refresh();
    
        if (map.ContainsKey(guid)) {
            return map[guid];
        } else {
            return null;
        }
    }
    
    public void ClearItems() {
        items.Clear();
        map.Clear();
    }
    
    public List<Item> ListItems() {
        return items;
    }
    
    public List<string> ListItemNames() {
        var names =
            from item in items
            select item.name;
        return names.ToList();
    }
    
    public List<string> ListItemGUIDs() {
        var guids =
            from item in items
            select item.textureGUID;
        return guids.ToList();
    }
    
    private void Refresh() {
        if (map.Count != items.Count) {
            map.Clear();

            foreach (var item in items) {
                map[item.textureGUID] = item;
            }
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    [System.Serializable]
    public class Item {
        public string name;
        public Rect region;
        public int pixelsWidth, pixelsHeight;
        public string textureGUID;
    }

}

#if !UNITY_3_5
} // namespace
#endif