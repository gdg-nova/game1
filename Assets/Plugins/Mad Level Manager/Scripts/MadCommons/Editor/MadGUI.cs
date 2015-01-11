/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MadLevelManager {

public class MadGUI {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static Color Darker(Color color) {
        return new Color(color.r * 0.9f, color.g * 0.9f, color.b * 0.9f);
    }
    
    public static Color Brighter(Color color) {
        return new Color(color.r * 1.1f, color.g * 1.1f, color.b * 1.1f);
    }
    
    public static Color ToggleColor(Color color, ref bool last) {
        if (last) {
            last = false;
            return Darker(color);
        } else {
            last = true;
            return color;
        }
    }
    
    public static bool Button(string label) {
        return Button(label, GUI.color);
    }

    public static bool Button(string label, Color color, params GUILayoutOption[] options) {
        var prevColor = GUI.backgroundColor;
        GUI.backgroundColor = color;

        GUILayout.BeginHorizontal(options);
        GUILayout.Space(15 * EditorGUI.indentLevel);
        bool state = GUILayout.Button(label, options);
        GUILayout.EndHorizontal();

        GUI.backgroundColor = prevColor;

        return state;
    }

    public static void Space(float width, float height) {

        if (height > 0) {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(height);
            EditorGUILayout.EndVertical();
        }

        if (width > 0) {
            GUILayout.Space(width);
        }
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
#region ScrollableList
    public class ScrollableList<T> where T : ScrollableListItem {
        List<T> items;
    
        public string label = "Scrollable List";
        public int height = 200;
        public int spaceAfter = 0;
        
        public bool selectionEnabled = false;
        public string emptyListMessage = "No elements!";
        
        RunnableVoid1<T> _selectionCallback = (arg1) => {};
        public RunnableVoid1<T> selectionCallback {
            get { return _selectionCallback; }
            set { _selectionCallback += value; }
        }

        public HashSet<System.Type> acceptDropTypes = new HashSet<System.Type>();

        public event RunnableVoid2<int, Object> dropCallback;
        
        T _selectedItem;
        public T selectedItem {
            get { return _selectedItem; }
            set { 
                DeselectAll();
                _selectedItem = value;
                
                if (value != null) {
                    value.selected = true;
                    selectionCallback(value);
                }
            }
        }
        
        Vector2 position;
        GUISkin skin;
        
        T scrollToItem;
        
        public ScrollableList(List<T> items) {
            this.items = items;
        }

        public void Draw() {
            if (skin == null) {
                skin = Resources.Load("GUISkins/editorSkin", typeof(GUISkin)) as GUISkin;
            }

            var allRect = EditorGUILayout.BeginVertical();

            bool toggleColor = false;
            Color baseColor = GUI.color;

            GUILayout.Label(label);
            position = EditorGUILayout.BeginScrollView(
                position, false, true, GUILayout.Height(height));

            float y = 0;

            foreach (var item in items) {
                var itemRect = EditorGUILayout.BeginHorizontal();
                if (scrollToItem == item && Event.current.type == EventType.Repaint) {
                    if (y - position.y < 0) { // item above
                        position.y = y;
                    } else if (y - position.y + itemRect.height + 2 >= allRect.height) { // item below ( + 2 - hack)
                        position.y = y - (allRect.height - itemRect.height * 2);
                    }

                    scrollToItem = null;
                }

                if (selectionEnabled) {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                        if (itemRect.Contains(Event.current.mousePosition)) {
                            selectedItem = item;
                        }
                    }
                }

                // component value based on skin
                float c = EditorGUIUtility.isProSkin ? 0 : 1;

                GUI.color = toggleColor ? Color.clear : new Color(c, c, c, 0.2f);
                toggleColor = !toggleColor;
                if (item.selected) {
                    GUI.color = toggleColor ? new Color(c, c, c, 0.4f) : new Color(c, c, c, 0.6f);
                }

                GUI.Box(itemRect, "", skin.box);
                GUI.color = baseColor;

                EditorGUILayout.BeginVertical();
                item.OnGUI();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                y += itemRect.height;
            }

            if (items.Count == 0) {
                GUILayout.Label(emptyListMessage);
            }

            GUILayout.FlexibleSpace();

            if (spaceAfter != 0) {
                GUILayout.Space(spaceAfter);
            }

            EditorGUILayout.EndScrollView();

            GUI.color = baseColor;

            EditorGUILayout.EndVertical();

            Event evt = Event.current;
            switch (evt.type) {
                case EventType.MouseDrag:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!allRect.Contains(evt.mousePosition)) {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences) {
                            if (acceptDropTypes.Contains(draggedObject.GetType())) {
                                AcceptDrag(items.Count, draggedObject);
                            }
                        }
                    }
                    break;
            }
        }

        private void AcceptDrag(int index, Object obj) {
            if (dropCallback != null) {
                dropCallback(index, obj);
            }
        }

        void DeselectAll() {
            foreach (var i in items) {
                i.selected = false;
            }
        }
        
        public void ScrollToItem(T item) {
            scrollToItem = item;
            //Draw();
        }
    };
    
#endregion

#region ArrayList

    public class ArrayList<T> where T : class, new() {
    
        public static readonly RunnableVoid1<SerializedProperty> PropertyRenderer = (prop) =>
        {
            MadGUI.PropertyField(prop, "");
        };
    
        List<T> items;
        SerializedProperty arrayProperty;
        
        RunnableGeneric1<T, T> genericRenderer;
        RunnableVoid1<SerializedProperty> propertyRenderer;
        
		public string emptyLabel = "Use the 'Add' button to add items";
		public string addLabel = "Add";

        public bool drawSeparator = false;
        public bool drawOrderButtons = true;
        
        public RunnableGeneric0<T> createFunctionGeneric = () => { return new T(); };
        public RunnableVoid1<SerializedProperty> createFunctionProperty = (element) => {
            // when creating new array element like this, the color will be initialized with
            // (0, 0, 0, 0) - zero aplha. This may be confusing for end user so this workaround looks
            // for color fields and sets them to proper values                  
            var enumerator = element.GetEnumerator();
            while (enumerator.MoveNext()) {
                var el = enumerator.Current as SerializedProperty;
                if (el.type == "ColorRGBA") {
                    el.colorValue = Color.black;
                }
            }
        };
        
        public RunnableVoid0 beforeAdd;
        public RunnableVoid1<T> beforeRemove;
        
        public RunnableVoid1<T> onAdd;
        public RunnableVoid1<T> onRemove;
        
        public ArrayList(SerializedProperty arrayProperty, RunnableVoid1<SerializedProperty> renderer) {
            this.arrayProperty = arrayProperty;
            propertyRenderer = renderer;
        }

        public ArrayList(List<T> items, RunnableGeneric1<T, T> renderer) {
            this.items = items;
            genericRenderer = renderer;
        }
        
        // items accessors
        int ItemCount() {
            if (items != null) {
                return items.Count;
            } else {
                return arrayProperty.arraySize;
            }
        }
        
        T ItemAt(int index) {
            if (items != null) {
                return items[index];
            } else {
                return arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as T;
            }
        }
        
        void RemoveItem(int index) {
            if (items != null) {
                items.RemoveAt(index);
            } else {
                arrayProperty.DeleteArrayElementAtIndex(index);
            }
        }
        
        void AddItem() {
            if (beforeAdd != null) {
                beforeAdd();
            }
        
            if (items != null) {
                items.Add(createFunctionGeneric());
            } else {
                arrayProperty.arraySize++;
                var prop = arrayProperty.GetArrayElementAtIndex(ItemCount() - 1);
                createFunctionProperty(prop);
            }

            if (onAdd != null) {
                var item = ItemAt(ItemCount() - 1);
                onAdd(item);
            }
        }
        
        void RenderItem(int index) {
            if (items != null) {
                items[index] = genericRenderer(items[index]);
            } else {
                propertyRenderer(arrayProperty.GetArrayElementAtIndex(index));
            }
        }
        
        public bool Draw() {
            EditorGUI.BeginChangeCheck();
            if (ItemCount() == 0) {
                GUILayout.Label("   " + emptyLabel);
            } else {
                if (drawSeparator) {
                    Separator();
                }
                int removeIndex = -1;
                
                int count = ItemCount();
                for (int i = 0; i < count; ++i) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    RenderItem(i);
                    EditorGUILayout.EndVertical();

                    GUI.backgroundColor = Color.yellow;

                    if (drawOrderButtons) {

                        string upLabel;
                        string downLabel;

#if !UNITY_3_5
                    upLabel = "\u25B2";
                    downLabel = "\u25BC";
#else
                        upLabel = "U";
                        downLabel = "D";
#endif
                        GUI.enabled = CanMoveDown(i);
                        if (GUILayout.Button(downLabel, GUILayout.ExpandWidth(false))) {
                            MoveDown(i);
                        }

                        GUI.enabled = CanMoveUp(i);
                        if (GUILayout.Button(upLabel, GUILayout.ExpandWidth(false))) {
                            MoveUp(i);
                        }

                        GUI.enabled = true;
                    }

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.ExpandWidth(false))) {
                        removeIndex = i;
                    }
                    GUI.backgroundColor = Color.white;
                    
                    EditorGUILayout.EndHorizontal();
                    
                    if (i + 1 > count) {
                        EditorGUILayout.Space();
                    }
                    if (drawSeparator) {
                        Separator();
                    }
                }
                
                if (removeIndex != -1) {
                    T item = null;
                    if (beforeRemove != null || beforeAdd != null) {
                        item = ItemAt(removeIndex);
                    }

                    if (beforeRemove != null) {
                        beforeRemove(item);
                    }

                    RemoveItem(removeIndex);

                    if (onRemove != null) {
                        onRemove(item);
                    }
                }
            }

            GUI.backgroundColor = Color.green;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(addLabel, GUILayout.ExpandWidth(false))) {
                AddItem();
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            
            return EditorGUI.EndChangeCheck();
        }

        private bool CanMoveUp(int i) {
            return i > 0;
        }

        private bool CanMoveDown(int i) {
            return i < ItemCount() - 1;
        }

        private void MoveUp(int i) {
            if (items != null) {
                T tmp = items[i];
                items[i] = items[i - 1];
                items[i - 1] = tmp;
            } else {
                arrayProperty.MoveArrayElement(i, i - 1);
            }
            
        }

        private void MoveDown(int i) {
            if (items != null) {
                T tmp = items[i];
                items[i] = items[i + 1];
                items[i + 1] = tmp;
            } else {
                arrayProperty.MoveArrayElement(i, i + 1);                
            }
        }
    }

#endregion

#region Wrappers
    public static void Box(RunnableVoid0 runnable) {
        Box("", runnable);
    }
    
    public static void Box(string label, RunnableVoid0 runnable) {
        BeginBox(label);
        runnable();
        EndBox();
    }
          
    public static void BeginBox() {
        BeginBox("");
    }
    
    public static void BeginBox(string label) {
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < EditorGUI.indentLevel; ++i) {
            EditorGUILayout.Space();
        }
        var rect = EditorGUILayout.BeginVertical();
        
        GUI.Box(rect, GUIContent.none);
        if (!string.IsNullOrEmpty(label)) {
            GUILayout.Label(label, "BoldLabel");
        }
        
        EditorGUILayout.Space();
    }
    
    public static void EndBox() {
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
    
    public static void Indent(RunnableVoid0 runnable) {
        EditorGUI.indentLevel++;
        runnable();
        EditorGUI.indentLevel--;
    }

    public static void Indent(int value, RunnableVoid0 runnable) {
        EditorGUI.indentLevel += value;
        runnable();
        EditorGUI.indentLevel -= value;
    }

    public static void IndentBox(RunnableVoid0 runnable) {
        IndentBox("", runnable);
    }

    public static IndentC Indent(int strength = 1) {
        return new IndentC(strength);
    }
    
    public static void IndentBox(string label, RunnableVoid0 runnable) {
        Box(label, () => {
            Indent(() => {
                runnable();
            });
        });
    }
    
    public static void Disabled(RunnableVoid0 runnable) {
        ConditionallyEnabled(false, runnable);
    }

    public static EnableC EnabledIf(bool condition) {
        return new EnableC(condition);
    }
    
    public static void ConditionallyEnabled(bool enabled, RunnableVoid0 runnable) {
        bool prevState = GUI.enabled;
        GUI.enabled = enabled;
        runnable();
        GUI.enabled = prevState;
    }
    
    public static void LineHelp(ref bool state, string helpMessage, RunnableVoid0 runnable) {
        EditorGUILayout.BeginHorizontal();
        runnable();
        state = GUILayout.Toggle(state, "?", "Button", GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();
        if (state) {
            MadGUI.Message("Help:\n" + helpMessage, MessageType.Info);
        }
    }
#endregion
#region Messages
    public static bool InfoFix(string message) {
        return MessageFix(message, MessageType.Info);
    }
    
    public static bool InfoFix(string message, string fixMessage) {
        return MessageFix(message, fixMessage, MessageType.Info);
    }
          
    public static bool WarningFix(string message) {
        return MessageFix(message, MessageType.Warning);
    }

    public static bool WarningFix(string message, string fixMessage) {
        return MessageFix(message, fixMessage, MessageType.Warning);
    }
    
    public static bool ErrorFix(string message) {
        return MessageFix(message, MessageType.Error);
    }
    
    public static bool ErrorFix(string message, string fixMessage) {
        return MessageFix(message, fixMessage, MessageType.Error);
    }
    
    public static bool MessageFix(string message, MessageType messageType) {
        return MessageWithButton(message, "Fix it", messageType);
    }
    
    public static bool MessageFix(string message, string fixMessage, MessageType messageType) {
        return MessageWithButton(message, fixMessage, messageType);
    }
    
    public static bool MessageWithButton(string message, string buttonLabel, MessageType messageType) {
        EditorGUILayout.HelpBox(message, messageType);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUI.backgroundColor = Color.cyan;
        bool result = GUILayout.Button(buttonLabel);
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        return result;
    }
    
    public static int MessageWithButtonMulti(string message, MessageType messageType, params string[] buttonLabel) {
        EditorGUILayout.HelpBox(message, messageType);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        int result = -1;
        for (int i = 0; i < buttonLabel.Length; ++i) {
            if (GUILayout.Button(buttonLabel[i])) {
                result = i;
            }
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        return result;
    }

    public static void Warning(string message) {
        Message(message, MessageType.Warning);
    }
    
    public static void Info(string message) {
        Message(message, MessageType.Info);
    }
    
    public static void Error(string message) {
        Message(message, MessageType.Error);
    }
    
    public static void Message(string message, MessageType messageType) {
        EditorGUILayout.HelpBox(message, messageType);
    }
#endregion

#region Properties

    public static bool Validate(Validator0 validator, RunnableVoid0 runnable) {
        bool valid = true;
    
        Color prevColor = GUI.color;
        if (GUI.enabled && !validator()) {
            GUI.color = Color.red;
            valid = false;
        }
        
        runnable();
        
        if (!valid) {
            GUI.color = prevColor;
        }
        
        return valid;
    }

    public static void PropertyField(SerializedProperty obj, string label) {
        PropertyField(obj, label, (s) => true);
    }
    
    public static void PropertyField(SerializedProperty obj, string label, Validator validator) {
        Validate(() => validator(obj), () => {
            EditorGUILayout.PropertyField(obj, new GUIContent(label));
        });
    }
    
    public static void PropertyField(SerializedProperty obj, string label, string tooltip) {
        PropertyField(obj, label, tooltip, (s) => true);
    }
    
    public static void PropertyField(SerializedProperty obj, string label, string tooltip, Validator validator) {
        Validate(() => validator(obj), () => {
            EditorGUILayout.PropertyField(obj, new GUIContent(label, tooltip));
        });
    }
    
    public static void PropertyFieldSlider(SerializedProperty obj, float leftValue, float rightValue, string label) {
        obj.floatValue = EditorGUILayout.Slider(label, obj.floatValue, leftValue, rightValue);
    }
    
    public static void PropertyFieldVector2(SerializedProperty obj, string label) {
        obj.vector2Value = EditorGUILayout.Vector2Field(label, obj.vector2Value);
    }
    
    public static void PropertyFieldVector2Compact(SerializedProperty obj, string label) {
        PropertyFieldVector2Compact(obj, label, 0);
    }
    
    public static void PropertyFieldVector2Compact(SerializedProperty obj, string label, int labelWidth) {
        EditorGUILayout.BeginHorizontal();
        Vector2 v = obj.vector2Value;
        float x = v.x;
        float y = v.y;
        
        EditorGUI.BeginChangeCheck();
        if (labelWidth > 0) {
            GUILayout.Label(label, GUILayout.Width(labelWidth));
        } else {
            GUILayout.Label(label);
        }
        LookLikeControls(15);
        x = EditorGUILayout.FloatField("X", x);
        y = EditorGUILayout.FloatField("Y", y);
        LookLikeControls(0);
        
        if (EditorGUI.EndChangeCheck()) {
            obj.vector2Value = new Vector2(x, y);
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void PropertyFieldEnumPopup(SerializedProperty obj, string label, params GUILayoutOption[] options) {
        var names = obj.enumNames;
        // split names by camel-case
        for (int i = 0; i < names.Length; ++i) {
            string name = names[i];
            var newName = new StringBuilder();
            
            for (int j = 0; j < name.Length; ++j) {
                bool first = j == 0;
                char p = first ? ' ' : name[j - 1];
                char c = name[j];
                if (
                    (!first && char.IsUpper(c) && !char.IsUpper(p)) // upper case check
                    ||
                    (!first && char.IsNumber(c) && !char.IsNumber(p)) // number check
                    ) {
                    newName.Append(" ");
                }
                
                newName.Append(c);
            }
            
            names[i] = newName.ToString();
        }
        
        int selectedIndex = obj.enumValueIndex;
        int newIndex = EditorGUILayout.Popup(label, selectedIndex, names, options);
        
        if (selectedIndex != newIndex) {
            obj.enumValueIndex = newIndex;
        }
    }

    public static int DynamicPopup(int selectedIndex, string label, int count, RunnableGeneric1<string, int> visitor) {

        if (selectedIndex >= count) {
            selectedIndex = 0;
        } else if (selectedIndex < 0) {
            selectedIndex = 0;
        }

        string[] labels = new string[count];

        for (int i = 0; i < count; ++i) {
            string l = visitor(i);
            labels[i] = l;
        }

        return EditorGUILayout.Popup(label, selectedIndex, labels);
    }

    public static int DynamicPopup(Rect rect, int selectedIndex, string label, int count, RunnableGeneric1<string, int> visitor) {

        if (selectedIndex >= count) {
            selectedIndex = 0;
        } else if (selectedIndex < 0) {
            selectedIndex = 0;
        }

        string[] labels = new string[count];

        for (int i = 0; i < count; ++i) {
            string l = visitor(i);
            labels[i] = l;
        }

        return EditorGUI.Popup(rect, label, selectedIndex, labels);
    }

    public static Object SceneField(Object obj, string label) {
        obj = EditorGUILayout.ObjectField(label, obj, typeof(UnityEngine.Object), false);
        if (!CheckAssetIsScene(obj)) {
            obj = null;
        }

        return obj;
    }

    private static bool CheckAssetIsScene(UnityEngine.Object obj) {
        if (obj == null) {
            return false;
        }
    
        string path = AssetDatabase.GetAssetPath(obj);
        if (!path.EndsWith(".unity")) {
            EditorUtility.DisplayDialog(
                "Only scene allowed",
                "You can only drop scene files into this field.",
                "OK");
                
            return false;
        }
        
        return true;
    }
    
    public static void LookLikeControls(int labelWidth) {
        LookLikeControls(labelWidth, 0);
    }
    
    public static void LookLikeControls(int labelWidth, int fieldWidth) {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
        EditorGUIUtility.LookLikeControls(labelWidth, fieldWidth);
#else
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUIUtility.fieldWidth = fieldWidth;
#endif
    }
    
    public static void PropertyFieldToggleGroup(SerializedProperty obj, string label, RunnableVoid0 runnable) {
        obj.boolValue = EditorGUILayout.BeginToggleGroup(label, obj.boolValue);
        
        runnable();
        EditorGUILayout.EndToggleGroup();
    }
    
    public static void PropertyFieldToggleGroup2(SerializedProperty obj, string label, RunnableVoid0 runnable) {
        obj.boolValue = EditorGUILayout.Toggle(label, obj.boolValue);
        
        bool savedState = GUI.enabled;
        GUI.enabled = obj.boolValue;
        runnable();
        GUI.enabled = savedState;
    }
    
    public static void PropertyFieldToggleGroupInv2(SerializedProperty obj, string label, RunnableVoid0 runnable) {
        obj.boolValue = !EditorGUILayout.Toggle(label, !obj.boolValue);
        
        bool savedState = GUI.enabled;
        GUI.enabled = !obj.boolValue;
        runnable();
        GUI.enabled = savedState;
    }

    public static void PropertyFieldObjectsPopup<T>(Object target, string label, ref T selectedObject, List<T> objects,
        bool allowEditWhenDisabled) where T : UnityEngine.Object {
        
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
    
    public static bool Foldout(string name, bool defaultState) {
        bool state = EditorPrefs.GetBool(name, defaultState);
        
        bool newState = EditorGUILayout.Foldout(state, name);
        if (newState != state) {
            EditorPrefs.SetBool(name, newState);
        }
        
        return newState;
    }
    
    public static void Separator() {
        var rect = EditorGUILayout.BeginHorizontal();
        int indentPixels = (EditorGUI.indentLevel + 1) * 10 - 5;
        GUI.Box(new Rect(rect.xMin, rect.yMin, rect.width - indentPixels, 1), "");
        EditorGUILayout.EndHorizontal();
    }
#endregion

    #region Delegates
    
    public delegate void RunnableVoid0();
    public delegate void RunnableVoid1<T>(T arg1);
    public delegate void RunnableVoid2<T1, T2>(T1 arg1, T2 arg2);
    public delegate T RunnableGeneric0<T>();
    public delegate T RunnableGeneric1<T, T1>(T1 arg1);
    public delegate bool Validator(SerializedProperty property);
    public delegate bool Validator0();

    #endregion

    #region Types
    
    public abstract class ScrollableListItem {
        public bool selected;
        
        public abstract void OnGUI();
    }
    
    public class ScrollableListItemLabel : ScrollableListItem {
        public string label;
        
        public ScrollableListItemLabel(string label) {
            this.label = label;
        }
        
        public override void OnGUI() {
            EditorGUILayout.LabelField(label);
        }
    }
    
    public static Validator StringNotEmpty = (SerializedProperty s) => {
        return !string.IsNullOrEmpty(s.stringValue);
    };
    
    public static Validator ObjectIsSet = (SerializedProperty s) => {
        return s.objectReferenceValue != null;
    };

    public class IndentC : System.IDisposable {
        private readonly int strength;

        public IndentC(int strength) {
            this.strength = strength;
            EditorGUI.indentLevel += strength;
        }

        public void Dispose() {
            EditorGUI.indentLevel -= strength;
        }
    }

    public class EnableC : System.IDisposable {
        private bool previousState;

        public EnableC(bool condition) {
            previousState = GUI.enabled;
            GUI.enabled = condition;
        }

        public void Dispose() {
            GUI.enabled = previousState;
        }
    }

    #endregion
}

} // namespace