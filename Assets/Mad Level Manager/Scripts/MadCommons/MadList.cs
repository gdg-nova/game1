/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadList<T> {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    T[] arr;
    int size;

    // ===========================================================
    // Constructors
    // ===========================================================
    
    public MadList() : this(32) {
    }
    
    public MadList(int capacity) {
        arr = new T[capacity];
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    public T[] Array {
        get { return arr; }
        set { arr = value; }
    }
    
    public int Count {
        get { return size; }
    }

    public void Add(T e) {
        EnsureCapacity(size + 1);
        arr[size] = e;
        size++;
    }
    
    public T this[int index] {
        get {
            CheckRange(index);
            return arr[index];
        }
        
        set {
            CheckRange(index);
            arr[index] = value;
        }
    }
    
    void CheckRange(int index) {
        if (index >= size) {
            throw new IndexOutOfRangeException("index " + index + " out of range (size = " + size + ")");
        }
    }
    
    public void Clear() {
        size = 0;
    }
    
    public void Trim() {
        if (size != arr.Length) {
            System.Array.Resize(ref arr, size);
        }
    }
    
    void EnsureCapacity(int targetSize) {
        if (arr.Length < targetSize) {
            System.Array.Resize(ref arr, Mathf.Min(targetSize * 2, 1024 * 1024));
        }
    }

    public int FindIndex(T obj) {
        for (int i = 0; i < size; ++i) {
            var el = arr[i];


            if (el == null && obj == null) {
                return i;
            } else if (el != null && el.Equals(obj)) {
                return i;
            }
        }

        return -1;
    }

    public bool Contains(T obj) {
        return FindIndex(obj) != -1;
    }

    public bool Remove(T obj) {
        var index = FindIndex(obj);
        if (index == -1) {
            return false;
        }

        RemoveAt(index);

        return true;
    }

    public void RemoveAt(int index) {
        CheckRange(index);

        if (size > index + 1) {
            ShiftLeft(index + 1);
        }

        size--;
    }

    private void ShiftLeft(int firstIndex) {
        for (int i = firstIndex; i < size; ++i) {
            Copy(i, i - 1);
        }
    }

    private void Copy(int from, int to) {
        arr[to] = arr[from];
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