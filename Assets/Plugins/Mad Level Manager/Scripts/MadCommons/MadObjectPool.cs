/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadLevelManager {

public class MadObjectPool<T> {

    private List<Item> list;
    
    public MadObjectPool(int capacity) {
        list = new List<Item>(capacity);
    }

    public void Add(T obj) {
        list.Add(new Item(obj));
    }

    public bool CanTake() {
        return FindFree() != -1;
    }

    public T Take() {
        int index = FindFree();
        if (index == -1) {
            return default(T);
        }

        var item = list[index];
        item.free = false;
        return item.obj;
    }

    public void Release(T obj) {
        int index = Find(obj);
        var item = list[index];

        if (item.free) {
            Debug.LogError("Item is already free");
            return;
        }

        item.free = true;
    }

    private int FindFree() {
        int c = list.Count;
        for (int i = 0; i < c; ++i) {
            if (list[i].free) {
                return i;
            }
        }

        return -1;
    }

    private int Find(T obj) {
        int c = list.Count;
        for (int i = 0; i < c; ++i) {
            if ((object) list[i].obj == (object) obj) {
                return i;
            }
        }

        return -1;
    }

    private class Item {
        public bool free = true;
        public T obj;

        public Item(T obj) {
            this.obj = obj;
        }
    }
}

} // namespace
