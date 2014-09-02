namespace Apex.Editor
{
    using Apex.Utilities;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(MinCheckAttribute))]
    public class MinCheckedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as MinCheckAttribute;

            EditorGUI.PropertyField(position, property, label);

            if (property.propertyType == SerializedPropertyType.Float)
            {
                var val = property.floatValue;
                if (val < attrib.min)
                {
                    property.floatValue = attrib.min;
                }
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                var val = property.intValue;
                if (val < attrib.min)
                {
                    property.intValue = (int)attrib.min;
                }
            }
            else
            {
                EditorGUI.LabelField(position, "The min check attribute is only valid on int or float fields.");
            }
        }
    }
}
