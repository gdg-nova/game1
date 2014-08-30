namespace Apex.Editor
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Apex.Common;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(AttributePropertyAttribute))]
    public class AttributePropertyDrawer : PropertyDrawer
    {
        private static Type _attributesEnumType;
        private static bool _lookupComplete;
        private static bool _enumFound;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_lookupComplete && _attributesEnumType == null)
            {
                var asm = Assembly.GetAssembly(typeof(EntityAttributesEnumAttribute));
                _attributesEnumType = asm.GetTypes().Where(t => t.IsEnum && Attribute.IsDefined(t, typeof(EntityAttributesEnumAttribute)) && t != typeof(DefaultEntityAttributesEnum)).FirstOrDefault();

                if (_attributesEnumType == null)
                {
                    _enumFound = false;
                    _attributesEnumType = typeof(DefaultEntityAttributesEnum);
                }
                else
                {
                    _enumFound = true;
                }

                _lookupComplete = true;
            }

            if (!_enumFound)
            {
                EditorGUILayout.HelpBox("To enable attribute specific behaviours, create an entity attribute enum and decorate it with the EntityAttributesEnum.", MessageType.Info);
                return;
            }

            var attrib = this.attribute as AttributePropertyAttribute;

            var currentValue = property.intValue;
            var curEnumVal = (Enum)Enum.ToObject(_attributesEnumType, property.intValue);

            if (!string.IsNullOrEmpty(attrib.label))
            {
                label.text = attrib.label;
            }

            var newValRaw = EditorGUI.EnumMaskField(position, label, curEnumVal) as IConvertible;

            var newVal = newValRaw.ToInt32(null);
            if (newVal != currentValue)
            {
                property.intValue = newVal;
            }
        }
    }
}
