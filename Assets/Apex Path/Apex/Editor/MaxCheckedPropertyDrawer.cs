namespace Apex.Editor
{
    using Apex.Utilities;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(MaxCheckAttribute))]
    public class MaxCheckedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = this.attribute as MaxCheckAttribute;

            EditorGUI.PropertyField(position, property, label);

            if (property.propertyType == SerializedPropertyType.Float)
            {
                var val = property.floatValue;
                if (val > attrib.max)
                {
                    property.floatValue = attrib.max;
                }
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                var val = property.intValue;
                if (val > attrib.max)
                {
                    property.intValue = (int)attrib.max;
                }
            }
            else
            {
                EditorGUI.LabelField(position, "The max check attribute is only valid on int or float fields.");
            }
        }
    }
}
