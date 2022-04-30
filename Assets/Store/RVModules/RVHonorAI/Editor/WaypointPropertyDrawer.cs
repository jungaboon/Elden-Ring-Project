// Created by Ronis Vision. All rights reserved
// 16.01.2020.

using UnityEditor;
using UnityEngine;

namespace RVHonorAI.Editor
{
    [CustomPropertyDrawer(typeof(Waypoint))]
    public class WaypointPropertyDrawer : PropertyDrawer
    {
        #region Public methods

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label.text.Replace("Waypoint", "")));

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            //EditorGUI.indentLevel = 0;
            position.xMin = indent;

            // Calculate rects
            var width = position.width - 55 - indent - position.xMin;
            
            var posRect = new Rect(position.xMin + 55, position.y, width * .45f, position.height);
            var radiusLabelRect = new Rect(posRect.xMax + width * .01f, position.y, width * .1f, position.height);
            var radiusRect = new Rect(radiusLabelRect.xMax + width * .01f, position.y, width * .42f, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(posRect, property.FindPropertyRelative("position"), GUIContent.none);
            EditorGUI.LabelField(radiusLabelRect, "Radius");
            //EditorGUI.PropertyField(radiusRect, property.FindPropertyRelative("radius"), GUIContent.none);
            EditorGUI.Slider(radiusRect, property.FindPropertyRelative("radius"), 0, 50, GUIContent.none);

            //EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        #endregion
    }
}