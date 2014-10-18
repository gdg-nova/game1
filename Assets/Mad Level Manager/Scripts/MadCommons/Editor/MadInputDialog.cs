/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadInputDialog : EditorWindow {

    #region Fields

    private string description;
    private string textValue;
    private Callback callback;

    private string okLabel;
    private string cancelLabel;

    private bool positionSet = false;

    private bool allowEmpty;

    #endregion

    #region Unity Methods

    void OnGUI() {
        var size = EditorGUILayout.BeginVertical();

        GUILayout.Label(description);

        GUI.SetNextControlName("TextField");
        MadGUI.Validate(() => {
            if (allowEmpty || !string.IsNullOrEmpty(textValue)) {
                return true;
            } else {
                return false;
            }
        }, () => {
            textValue = EditorGUILayout.TextField(textValue);
        });

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        MadGUI.ConditionallyEnabled(allowEmpty || !string.IsNullOrEmpty(textValue), () => {
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(okLabel)) {
                Close();
                callback(textValue);
            }
            GUI.backgroundColor = Color.white;
        });

        if (GUILayout.Button(cancelLabel)) {
            Close();
            callback(null);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        if (!positionSet && Event.current.type == EventType.Repaint) {
            position = new Rect(position.x, position.y, size.width, size.height);
            positionSet = true;
            GUI.FocusControl("TextField");
        }

        if (allowEmpty || !string.IsNullOrEmpty(textValue)) {
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) {
                Close();
                callback(textValue);
            }
        }
    }
    
    #endregion

    #region Static Methods

    private static void Show(Builder builder) {
        //var editor = ScriptableObject.CreateInstance<MadInputDialog>();

        var editor = EditorWindow.GetWindow<MadInputDialog>(true, builder.title, true);
        editor.description = builder.description;
        editor.textValue = builder.defaultValue;
        editor.callback = builder.callback;

        editor.okLabel = builder.okLabel;
        editor.cancelLabel = builder.cancelLabel;

        editor.allowEmpty = builder.allowEmpty;

        //editor.ShowPopup();
    }

    #endregion

    #region Inner Types

    public class Builder {
        public string title;
        public string description;
        public Callback callback;

        public string okLabel = "OK";
        public string cancelLabel = "Cancel";
        public string defaultValue = "";

        public bool allowEmpty = false;

        public Builder(string title, string description, Callback callback) {
            this.title = title;
            this.description = description;
            this.callback = callback;
        }

        public void BuildAndShow() {
            MadInputDialog.Show(this);
        }
    }

    public delegate void Callback(string result);

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif