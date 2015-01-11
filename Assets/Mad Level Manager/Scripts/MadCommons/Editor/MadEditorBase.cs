/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

/*
MadEditorBase is base class for editor classes for Mad Pixel Machine products.
To avoid conflicts it will be kept in different namespaces.

Changelog:
[06/29/2013]
- Changing array size by property.arraySize instead of property.InsertArrayElementAtIndex(property.arraySize);
    The second one caused unity to hand in 3.5.5
- BeginBox() label inside the box and bold
- Added PropertyFieldObjectsPopup()

[06/25/2013]
- Removed method that was specialized with EnergyBarToolkit
- Better indent support for BeginBox/EndBox

*/

public class MadEditorBase : Editor {

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
    
    protected void Separator() {
        var rect = EditorGUILayout.BeginHorizontal();
        int indentPixels = (EditorGUI.indentLevel + 1) * 10 - 5;
        GUI.Box(new Rect(indentPixels, rect.yMin, rect.width - indentPixels, rect.height), "");
        EditorGUILayout.EndHorizontal();
    }

    protected void PropertyFieldWithChildren(string name) {
        var property = serializedObject.FindProperty(name);
        EditorGUILayout.BeginVertical();
        do {
            if (property.propertyPath != name && !property.propertyPath.StartsWith(name + ".") ) {
                break;
            }  
            EditorGUILayout.PropertyField(property);
        } while (property.NextVisible(true));
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
 
    
//    private bool ex = false;
    
    protected void PropertyField(SerializedProperty obj, string label) {
        EditorGUILayout.PropertyField(obj, new GUIContent(label));
    }
    
    protected void PropertyField(SerializedProperty obj, string label, string tooltip) {
        EditorGUILayout.PropertyField(obj, new GUIContent(label, tooltip));
    }
    
    protected void PropertyFieldVector2(SerializedProperty obj, string label) {
        obj.vector2Value = EditorGUILayout.Vector2Field(label, obj.vector2Value);
    }
    
    protected void PropertyFieldToggleGroup(SerializedProperty obj, string label, Runnable0 runnable) {
        obj.boolValue = EditorGUILayout.BeginToggleGroup(label, obj.boolValue);
        
        runnable();
        EditorGUILayout.EndToggleGroup();
    }
    
    protected void PropertyFieldToggleGroup2(SerializedProperty obj, string label, Runnable0 runnable) {
        obj.boolValue = EditorGUILayout.Toggle(label, obj.boolValue);
        
        bool savedState = GUI.enabled;
        GUI.enabled = obj.boolValue;
        runnable();
        GUI.enabled = savedState;
    }
    
    protected void PropertyFieldToggleGroupInv2(SerializedProperty obj, string label, Runnable0 runnable) {
        obj.boolValue = !EditorGUILayout.Toggle(label, !obj.boolValue);
        
        bool savedState = GUI.enabled;
        GUI.enabled = !obj.boolValue;
        runnable();
        GUI.enabled = savedState;
    }
    
    protected void PropertyFieldObjectsPopup<T>(string label, ref T selectedObject, List<T> objects,
        bool allowEditWhenDisabled) where T : Component {
        
        bool active = allowEditWhenDisabled || 
#if UNITY_3_5
        ((MonoBehaviour) target).gameObject.active;
#else
        ((MonoBehaviour) target).gameObject.activeInHierarchy;
#endif
        
        bool guiEnabledPrev = GUI.enabled;
        GUI.enabled = active && guiEnabledPrev;
    
        if (GUI.enabled) {
            int index = 0;
            List<string> names = objects.ConvertAll((T input) => input.name);
            
            if (selectedObject != null) {
                T so = selectedObject;
                int foundIndex = objects.FindIndex((obj) => obj == so);
                if (foundIndex != -1) {
                    index = foundIndex + 1;
                }
            }
            
            names.Insert(0, "--");
            
            index = EditorGUILayout.Popup(label, index, names.ToArray());
            
            if (index == 0) {
                if (selectedObject != null) {
                    selectedObject = null;
                    EditorUtility.SetDirty(target);
                }
            } else {
                var newObject = objects[index - 1];
                if (selectedObject != newObject) {
                    selectedObject = newObject;
                    EditorUtility.SetDirty(target);
                }
            }
        } else {
            if (selectedObject == null) {
                EditorGUILayout.Popup(label, 0, new string[] {"--"});
            } else {
                EditorGUILayout.Popup(label, 0, new string[] {selectedObject.name});
            }
        }
        
        GUI.enabled = guiEnabledPrev;
    }
    
    protected bool Foldout(string name, bool defaultState) {
        bool state = EditorPrefs.GetBool(name, defaultState);
        
        bool newState = EditorGUILayout.Foldout(state, name);
        if (newState != state) {
            EditorPrefs.SetBool(name, newState);
        }
        
        return newState;
    }
    
    protected Vector2 RountToInt(Vector2 v) {
        var x = Mathf.RoundToInt(v.x);
        var y = Mathf.RoundToInt(v.y);
        return new Vector2(x, y);
    }
    
    protected Vector3 RountToInt(Vector3 v) {
        var x = Mathf.RoundToInt(v.x);
        var y = Mathf.RoundToInt(v.y);
        var z = Mathf.RoundToInt(v.z);
        return new Vector3(x, y, z);
    }
    
    protected void ArrayList(SerializedProperty property, string title) {
        ArrayList(property, title, (prop) => { PropertyField(prop, ""); });
    }
    
    protected void ArrayList(SerializedProperty property, string title, Runnable1<SerializedProperty> renderer) {
        if (Foldout(title, false)) {
            MadGUI.Indent(() => {
                if (property.arraySize == 0) {
                    GUILayout.Label("   Use 'Add' button to add items");
                } else {
                    int arrSize = property.arraySize;
                    Separator();
                    for (int i = 0; i < arrSize; ++i) {
                        var go = property.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal();
                        
                        EditorGUILayout.BeginVertical();
                        renderer(go);
                        EditorGUILayout.EndVertical();
                        
                        GUI.color = Color.red;
                        if (GUILayout.Button("X", GUILayout.ExpandWidth(false))) {
                            property.DeleteArrayElementAtIndex(i);
                            arrSize--;
                        }
                        GUI.color = Color.white;
                        EditorGUILayout.EndHorizontal();
                        
                        if (i + 1 < arrSize) {
                            EditorGUILayout.Space();
                        }
                        Separator();
                    }
                }
                
                GUI.color = Color.green;
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add", GUILayout.ExpandWidth(false))) {
                    property.arraySize++;
                    
                    // when creating new array element like this, the color will be initialized with
                    // (0, 0, 0, 0) - zero aplha. This may be confusing for end user so this workaround looks
                    // for color fields and sets them to proper values                  
                    var element = property.GetArrayElementAtIndex(property.arraySize - 1);                  
                    var enumerator = element.GetEnumerator();
                    while (enumerator.MoveNext()) {
                        var el = enumerator.Current as SerializedProperty;
                        if (el.type == "ColorRGBA") {
                            el.colorValue = Color.black;
                        }
                    }
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            });
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    protected delegate void Runnable0();
    protected delegate void Runnable1<T>(T t);
    protected delegate T Runnable<T>();
    protected delegate void Updater<T1, T2>(ref T1 o, T2 after);

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace