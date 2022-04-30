// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVUtilities.Editor
{
    public static class EditorHelpers
    {
        #region Public methods

        /// <summary>
        /// Zero is smallest (will still make space)
        /// </summary>
        public static void VerticalSpace(int size) => GUILayoutUtility.GetRect(0, size);

        public static void DrawProperties(Dictionary<string, SerializedProperty> _dictionary)
        {
            foreach (var serializedProperty in _dictionary)
                EditorGUILayout.PropertyField(serializedProperty.Value, true);
        }

        public static void AddProperties(Dictionary<string, SerializedProperty> _dictionary, SerializedObject _serializedObject, params string[] _propNames)
        {
            foreach (var propName in _propNames) AddProperty(_dictionary, propName, _serializedObject);
        }

        public static void AddProperty(Dictionary<string, SerializedProperty> _dictionary, string _propName, SerializedObject _serializedObject)
        {
            var property = _serializedObject.FindProperty(_propName);
            if (property == null) return;
            _dictionary.Add(_propName, property);
        }

        public static ReorderableList InitReorderableList(SerializedProperty _serializedProperty, SerializedObject _serializedObject)
        {
            var reorderableList = new ReorderableList(_serializedObject, _serializedProperty, true, false, true, true);
            reorderableList.drawElementCallback += (_rect, _index, _active, _focused) =>
            {
                EditorGUI.PropertyField(_rect, _serializedProperty.GetArrayElementAtIndex(_index), new GUIContent($"Waypoint {_index + 1}"), true);
            };
            reorderableList.onSelectCallback += _list => { };
            return reorderableList;
        }


        public static void ChangeGuiColorsTemporarily(Action _action, params GuiColorChange[] colorChanges)
        {
            foreach (var guiColorChange in colorChanges)
                switch (guiColorChange.type)
                {
                    case GuiColorType.Color:
                        guiColorChange.orgColor = GUI.color;
                        GUI.color = guiColorChange.color;
                        break;
                    case GuiColorType.Content:
                        guiColorChange.orgColor = GUI.contentColor;
                        GUI.contentColor = guiColorChange.color;
                        break;
                    case GuiColorType.Background:
                        guiColorChange.orgColor = GUI.backgroundColor;
                        GUI.backgroundColor = guiColorChange.color;
                        break;
                    case GuiColorType.LabelNormalText:
                        guiColorChange.orgColor = EditorStyles.label.normal.textColor;
                        EditorStyles.label.normal.textColor = guiColorChange.color;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            //EditorStyles.label.normal.textColor = Color.white;
            try
            {
                _action.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            foreach (var guiColorChange in colorChanges)
                switch (guiColorChange.type)
                {
                    case GuiColorType.Color:
                        GUI.color = guiColorChange.orgColor;
                        break;
                    case GuiColorType.Content:
                        GUI.contentColor = guiColorChange.orgColor;
                        break;
                    case GuiColorType.Background:
                        GUI.backgroundColor = guiColorChange.orgColor;
                        break;
                    case GuiColorType.LabelNormalText:
                        EditorStyles.label.normal.textColor = guiColorChange.orgColor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        public static void WrapInBox(Action _action, int indentationLevel = 1, int internalSpaces = 1, int externalSpacesAfter = 0)
        {
            for (var i = 0; i < indentationLevel; i++) EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical("box");
            for (var i = 0; i < internalSpaces; i++) VerticalSpace(0);
            _action();
            for (var i = 0; i < internalSpaces; i++) VerticalSpace(0);
            EditorGUILayout.EndVertical();
            for (var i = 0; i < externalSpacesAfter; i++) VerticalSpace(0);
            for (var i = 0; i < indentationLevel; i++) EditorGUI.indentLevel--;
        }

        public static void WrapInWindow(Action _action, string _windowTitle = "", int indentationLevel = 1, int internalSpaces = 1, int externalSpacesAfter = 0)
        {
            for (var i = 0; i < indentationLevel; i++) EditorGUI.indentLevel++;
            GUILayout.BeginVertical(_windowTitle, "window");
            for (var i = 0; i < internalSpaces; i++) VerticalSpace(0);
            _action();
            for (var i = 0; i < internalSpaces; i++) VerticalSpace(0);
            GUILayout.EndVertical();
            for (var i = 0; i < externalSpacesAfter; i++) VerticalSpace(0);
            for (var i = 0; i < indentationLevel; i++) EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Instantiates prefab with connection to it, no matter if it's prefab instance, or prefab asset, or not prefab at all
        /// </summary>
        public static GameObject InstantiatePrefab(GameObject prefab)
        {
            GameObject charGo = null;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(prefab))
            {
                if (!EditorUtility.IsPersistent(prefab))
                    charGo = PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(prefab)) as GameObject;
                else
                    charGo = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            }
            else if (PrefabUtility.IsPartOfPrefabAsset(prefab))
                charGo = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            else
                charGo = Object.Instantiate(prefab);

            return charGo;
        }

        /// <summary>
        /// Draws all serialized properties
        /// </summary>
        /// <param name="_object">Unity object from which we want to draw props</param>
        /// <param name="_propPredicate">Optional predicate. Can be used to filter which props are drawn, return false if you dont want to draw property
        /// You can also add your own gui code like some info box and shit</param>
        public static void DrawAllSerializedProperties(Object _object, Func<SerializedProperty, bool> _propPredicate = null, bool enterChildren = false)
        {
            var so = new SerializedObject(_object);
            so.Update();

            var prop = so.GetIterator();
            prop.Next(true);

            while (prop.NextVisible(enterChildren))
            {
                if (prop.displayName == "Script") continue;
                if (_propPredicate != null)
                    if (!_propPredicate.Invoke(prop))
                        continue;

                EditorGUILayout.PropertyField(prop, true);
            }

            so.ApplyModifiedProperties();
            PrefabUtility.RecordPrefabInstancePropertyModifications(_object);
        }

        public static void ChangeSerializedPropertyValue(Object _object, string property, Action<SerializedProperty> _action)
        {
            var so = new SerializedObject(_object);
            so.Update();

            var prop = so.FindProperty(property);
            if (prop == null) return;
            _action(prop);
            so.ApplyModifiedProperties();
        }

        public static void ChangeSerializedPropertyValue(Object _object, string property, object _value)
        {
            var so = new SerializedObject(_object);
            so.Update();

            var prop = so.FindProperty(property);
            if (prop == null) return;
            if (_value is float floatValue) prop.floatValue = floatValue;
            if (_value is int intValue) prop.intValue = intValue;
            if (_value is string stringValue) prop.stringValue = stringValue;
            if (_value is Object objectValue) prop.objectReferenceValue = objectValue;
            if (_value is Vector3 vector3Value) prop.vector3Value = vector3Value;
            so.ApplyModifiedProperties();
        }

        #endregion
    }

    public enum GuiColorType
    {
        Color,
        Content,
        Background,
        LabelNormalText
    }

    public class GuiColorChange
    {
        #region Fields

        public GuiColorType type;
        public Color color;
        public Color orgColor;

        #endregion

        public GuiColorChange(GuiColorType _type, Color _color)
        {
            type = _type;
            color = _color;
        }
    }
}