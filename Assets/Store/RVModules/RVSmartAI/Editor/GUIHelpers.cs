// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.Editor
{
    public static class GUIHelpers
    {
        #region Fields

        private static int buttonSize = 18;

        #endregion

        #region Public methods

        public static void UpDownRemove(Action _up, Action _down, Action _remove, bool _nonActive = false)
        {
            var c = GUI.color;

            // up
            if (GUILayout.Button(UpButton(), GuiStyle(1), GUILayout.MaxWidth(buttonSize), GUILayout.MaxHeight(buttonSize)))
                _up();
            // down
            if (GUILayout.Button(DownButton(), GuiStyle(1), GUILayout.MaxWidth(buttonSize), GUILayout.MaxHeight(buttonSize)))
                _down();

            if (_nonActive) GUI.color = Color.gray;

            // remove
            if (GUILayout.Button(RemoveButton(), GuiStyle(1), GUILayout.MaxWidth(buttonSize), GUILayout.MaxHeight(buttonSize)) && !_nonActive)
                _remove();

            if (_nonActive)
                GUI.color = c;
        }

        public static Texture utilityIcon() => Resources.Load<Texture>("gui/utilityIcon");

        public static Texture ActionIcon() => Resources.Load<Texture>("gui/actionIcon");

        public static Texture RemoveButton() => Resources.Load<Texture>("gui/removeButton");

        public static Texture UpButton() => Resources.Load<Texture>("gui/upButton");

        public static Texture DownButton() => Resources.Load<Texture>("gui/DownButton");

        public static Texture AddTaskButton() => Resources.Load<Texture>("gui/AddTaskButton");

        public static GUIStyle GuiStyle(int _id) => GUIStyles.GetGUIStyle(_id);

        public static GUIStyle GuiDebugStyle(int _id) => GUIStyles.GetGUIDebugStyle(_id);

        public static void GUIDrawNameAndDescription(Object target, string title, SerializedProperty nameProp, SerializedProperty descProp, out string desc)
        {
            Undo.RecordObject(target, "inspector");

            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(title), new GUIStyle {fontSize = 15, fontStyle = FontStyle.Bold});
            EditorGUILayout.Separator();

            if (nameProp != null) EditorGUILayout.PropertyField(nameProp);

            EditorGUILayout.LabelField("Description");
            desc = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.MinHeight(50));

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
        }

        public static void GUIDrawNameAndDescription(Object target, string title, string nameProp, string descProp, out string _name, out string desc)
        {
            Undo.RecordObject(target, "inspector");

            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(title), new GUIStyle {fontSize = 15, fontStyle = FontStyle.Bold});
            EditorGUILayout.Separator();

            _name = EditorGUILayout.TextField("Name", nameProp);

            EditorGUILayout.LabelField("Description");
            desc = EditorGUILayout.TextArea(descProp, GUILayout.MinHeight(50));

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Automatic undo and dirty handling - Unity proper way if serialisation allows for it
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_serializedObject"></param>
        public static void GUIDrawFields(Object _object)
        {
            if (_object == null) return;
            var serializedObject = new SerializedObject(_object);

            serializedObject.Update();
            //SmartAiExposeField exposeFieldattribute = null;
            foreach (var fieldInfo in _object.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var draw = true;

                foreach (var fieldInfoAttribute in fieldInfo.CustomAttributes)
                {
                    if (fieldInfoAttribute.AttributeType == typeof(SmartAiHideInInspector) || fieldInfoAttribute.AttributeType == typeof(HideInInspector))
                    {
                        draw = false;
                        break;
                    }

                    if (fieldInfo.Name == "lastValue" && !Application.isPlaying)
                    {
                        draw = false;
                        break;
                    }

//                    if (fieldInfoAttribute.AttributeType == typeof(SmartAiExposeField))
//                    {
//                        exposeFieldattribute = fieldInfo.GetCustomAttribute(typeof(SmartAiExposeField)) as SmartAiExposeField;
//                        draw = true;
//                        break;
//                    }
                }

                if (!draw) continue;

                var p = serializedObject.FindProperty(fieldInfo.Name);
                if (p == null) continue;

//                if (!string.IsNullOrEmpty(exposeFieldattribute.description))
//                {
//                    //EditorGUILayout.BeginVertical("box");
//                    EditorGUILayout.LabelField(exposeFieldattribute.description);
//                    EditorGUILayout.PropertyField(p, true);
//                    //EditorGUILayout.EndVertical();
//                }
//                else
                EditorGUILayout.PropertyField(p, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static void GUIDrawFieldsAll(Object _object)
        {
            if (_object == null) return;
            var serializedObject = new SerializedObject(_object);

            serializedObject.Update();

            var firstprop = serializedObject.GetIterator();
            firstprop.Next(true);

//            foreach (var prop in firstprop.GetChildren())
//            {
//                EditorGUILayout.PropertyField(prop, true);
//            }

            var prop = serializedObject.GetIterator();
            //EditorGUILayout.PropertyField(prop, true);

            //var lastProp = prop.Copy();
            prop.NextVisible(true);
            //EditorGUILayout.PropertyField(prop, true);

            while (prop.NextVisible(false))
                //if (lastProp == prop) continue;
                EditorGUILayout.PropertyField(prop, true);

            serializedObject.ApplyModifiedProperties();
        }
        
         private static int simCount = 50;
        
        public static void AiLbPerfInfo(int maxAllwdFreq, int updateFreq, bool _scalable)
        {
//            EditorGUILayout.HelpBox("Performance info", MessageType.Info);
            if (_scalable) EditorGUILayout.LabelField($"Number of AIs before throttling: {maxAllwdFreq / updateFreq}");

            simCount = EditorGUILayout.IntSlider("AI count", simCount, 1, 2000);
            float realFreq = updateFreq;
            if (_scalable && realFreq * simCount > maxAllwdFreq) realFreq = 1.0f * maxAllwdFreq / simCount;

            EditorGUILayout.LabelField(realFreq > 1.0f
                ? $"Update frequency per AI: {realFreq}"
                : $"Update frequency per AI: once every {1.0f / realFreq} seconds");

            EditorGUILayout.LabelField($"Total AI updates per second: {realFreq * simCount}");
            EditorGUILayout.LabelField($"Total AI updates per frame @60fps: {realFreq * simCount / 60}");
            EditorGUILayout.LabelField($"Total AI updates per frame @30fps: {realFreq * simCount / 30}");

//            var ratingString = "";
//            var suitableString = "";
//            var perfRatiing = realFreq * simCount;
//            perfRatiing += simCount * .35f;
//
//            if (perfRatiing < 100)
//            {
//                ratingString = "Very low";
//                suitableString = "Suitable for mobile";
//            }
//            else if (perfRatiing < 200)
//            {
//                ratingString = "Low";
//                suitableString = "Suitable for high end mobile";
//            }
//            else if (perfRatiing < 450)
//            {
//                ratingString = "Medium";
//                suitableString = "Suitable for desktop, consoles";
//            }
//            else if (perfRatiing < 800)
//            {
//                ratingString = "High";
//                suitableString = "Suitable for high end desktop";
//            }
//            else if (perfRatiing < 1200)
//            {
//                ratingString = "Very high";
//                suitableString = "Suitable for top end desktop";
//            }
//            else
//            {
//                ratingString = "Very high";
//                suitableString = "Unsuitable";
//            }
//
//            EditorGUILayout.LabelField($"Expected {ratingString.ToLower()} CPU load, {suitableString.ToLower()}");
        }

        #endregion
    }
}