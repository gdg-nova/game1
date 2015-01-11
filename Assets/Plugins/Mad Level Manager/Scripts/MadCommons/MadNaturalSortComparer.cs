/*
* Copyright (c) James McCormack
* http://zootfroot.blogspot.com/2009/09/natural-sort-compare-with-linq-orderby.html
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadNaturalSortComparer : IComparer<string>, IDisposable {

    private readonly bool isAscending;
 
    public MadNaturalSortComparer(bool inAscendingOrder = true)
    {
        this.isAscending = inAscendingOrder;
    }
 
    #region IComparer<string> Members
 
    public int Compare(string x, string y)
    {
        throw new NotImplementedException();
    }
 
    #endregion
 
    #region IComparer<string> Members
 
    int IComparer<string>.Compare(string x, string y)
    {
        if (x == y)
            return 0;
 
        string[] x1, y1;
 
        if (!table.TryGetValue(x, out x1))
        {
            x1 = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
            table.Add(x, x1);
        }
 
        if (!table.TryGetValue(y, out y1))
        {
            y1 = Regex.Split(y.Replace(" ", ""), "([0-9]+)");
            table.Add(y, y1);
        }
 
        int returnVal;
 
        for (int i = 0; i < x1.Length && i < y1.Length; i++)
        {
            if (x1[i] != y1[i])
            {
                returnVal = PartCompare(x1[i], y1[i]);
                return isAscending ? returnVal : -returnVal;
            }
        }
 
        if (y1.Length > x1.Length)
        {
            returnVal = 1;
        }
        else if (x1.Length > y1.Length)
        { 
            returnVal = -1; 
        }
        else
        {
            returnVal = 0;
        }
 
        return isAscending ? returnVal : -returnVal;
    }
 
    private static int PartCompare(string left, string right)
    {
        int x, y;
        if (!int.TryParse(left, out x))
            return left.CompareTo(right);
 
        if (!int.TryParse(right, out y))
            return left.CompareTo(right);
 
        return x.CompareTo(y);
    }
 
    #endregion
 
    private Dictionary<string, string[]> table = new Dictionary<string, string[]>();
 
    public void Dispose()
    {
        table.Clear();
        table = null;
    }
}

#if !UNITY_3_5
} // namespace
#endif