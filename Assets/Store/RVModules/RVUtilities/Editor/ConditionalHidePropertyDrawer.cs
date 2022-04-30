// Created by Ronis Vision. All rights reserved
// 07.04.2021.

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RVModules.RVUtilities.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        #region Public methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var condHAtt = (ConditionalHideAttribute) attribute;
            var enabled = GetConditionalHideAttributeResult(condHAtt, property);

            var wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (!condHAtt.hideInInspector || enabled) EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var condHAtt = (ConditionalHideAttribute) attribute;
            var enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (!condHAtt.hideInInspector || enabled)
                return EditorGUI.GetPropertyHeight(property, label);
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        #endregion

        #region Not public methods

        private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
        {
            var enabled = true;

            var propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
            string conditionPath;

            if (!string.IsNullOrEmpty(condHAtt.conditionalSourceField))
            {
                //Get the full relative property path of the sourcefield so we can have nested hiding
                conditionPath = propertyPath.Replace(property.name, condHAtt.conditionalSourceField); //changes the path to the conditionalsource property path
                var sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

                if (sourcePropertyValue != null) enabled = CheckPropertyType(sourcePropertyValue);
                else
                {
                    var propertyInfo = property.serializedObject.targetObject.GetType().GetProperty(conditionPath,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(property.serializedObject.targetObject);
                        enabled = CheckPropertyType(value);
                    }
                }
            }

            if (!string.IsNullOrEmpty(condHAtt.conditionalSourceField2))
            {
                conditionPath =
                    propertyPath.Replace(property.name,
                        condHAtt.conditionalSourceField2); //changes the path to the conditionalsource property path
                var sourcePropertyValue2 = property.serializedObject.FindProperty(conditionPath);

                if (sourcePropertyValue2 != null) enabled = enabled && CheckPropertyType(sourcePropertyValue2);
                else
                {
                    var propertyInfo = property.serializedObject.targetObject.GetType().GetProperty(conditionPath,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(property.serializedObject.targetObject);
                        enabled = CheckPropertyType(value);
                    }
                }
            }

            if (condHAtt.inverse) enabled = !enabled;

            return enabled;
        }

        private static bool CheckPropertyType(object val)
        {
            if (val is bool b) return b;
            return true;
        }

        private bool CheckPropertyType(SerializedProperty sourcePropertyValue)
        {
            switch (sourcePropertyValue.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return sourcePropertyValue.boolValue;
                case SerializedPropertyType.ObjectReference:
                    return sourcePropertyValue.objectReferenceValue != null;
                default:
                    Debug.LogError("Data type of the property used for conditional hiding [" +
                                   sourcePropertyValue.propertyType + "] is currently not supported");
                    return true;
            }
        }

        #endregion
    }
}