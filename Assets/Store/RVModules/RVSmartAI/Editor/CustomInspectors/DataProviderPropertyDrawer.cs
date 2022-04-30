// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Linq;
using System.Reflection;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Editor.SelectWindows;
using RVModules.RVUtilities.Editor;
using RVModules.RVUtilities.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.Editor.CustomInspectors
{
    [CustomPropertyDrawer(typeof(DataProviderBase), true)]
    public class DataProviderPropertyDrawer : PropertyDrawer
    {
        #region Public methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
           var obj = property.serializedObject.targetObject;
            var field = obj.GetType().GetField(property.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);


            EditorHelpers.WrapInBox(() =>
            {
                var provider = field.GetValue(obj) as Object;
                var desc = (provider as DataProviderBase)?.Description;

                EditorGUILayout.BeginHorizontal();

                var tooltipAttribute = field.GetCustomAttribute<TooltipAttribute>();
                label.tooltip = tooltipAttribute != null ? tooltipAttribute.tooltip : "";
                
                GUILayout.Label(label);

                var buttonText = $"{ObjectNames.NicifyVariableName(field.GetValue(obj)?.GetType().Name).Replace("Provider", "")}";
                if (field.GetValue(obj) as Object == null) buttonText = "Add data provider";
                
                if (GUILayout.Button(buttonText)) AssignNewProvider(field, obj);

                if (provider != null)
                {
                    if (field.GetCustomAttributes(typeof(OptionalDataProvider)).Any() && GUILayout.Button("X"))
                    {
                        DataProviderBase.DestroyReferencedDataProviders(provider as Component);
                        Object.DestroyImmediate(provider);
                        field.SetValue(provider, null);
                    }

                    EditorGUILayout.EndHorizontal();
                    if (!string.IsNullOrEmpty(desc)) EditorGUILayout.HelpBox(desc, MessageType.Info);
                    GUIHelpers.GUIDrawFields(provider);
                }
                else
                {
                    if (field.GetCustomAttributes(typeof(OptionalDataProvider)).Any() == false)
                    {
                        CreateDefaultDataProviderIfExist(obj, field);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            });

//            EditorGUI.EndProperty();
        }

        #endregion

        #region Not public methods

        private static void AssignNewProvider(FieldInfo field, Object obj) =>
            DataProviderWindow(obj, field, () =>
            {
                var old = field.GetValue(obj) as MonoBehaviour;
                if (obj != null)
                {
                    DataProviderBase.DestroyReferencedDataProviders(old);
                    Object.DestroyImmediate(old);
                }
            });

        private static void CreateDefaultDataProviderIfExist(Object obj, FieldInfo field)
        {
            var assignableDataProviders = ReflectionHelper.GetDerivedTypes(typeof(DataProviderBase))
                .Where(_type => field.FieldType.IsAssignableFrom(_type)).ToArray();

            Type firsType = null;
            Type defaultType = null;
            foreach (var assignableDataProvider in assignableDataProviders)
            {
                if (assignableDataProvider.Name.ToUpper().Contains("default".ToUpper()))
                {
                    defaultType = assignableDataProvider;
                    break;
                }

                if (firsType == null) firsType = assignableDataProvider;
            }

            if (defaultType != null)
            {
                CreateDataProvider(obj, field, null, defaultType);
                return;
            }

            if (firsType != null) CreateDataProvider(obj, field, null, firsType);
        }

        private static void DataProviderWindow(Object obj, FieldInfo field, Action onCreate)
        {
            var w = ScriptableObject.CreateInstance<DataProvidersWindow>();
            w.types = w.types.Where(_type => field.FieldType.IsAssignableFrom(_type)).ToArray();
            try
            {
                w.titleContent = new GUIContent($"Select data provider of type {w.types[0].BaseType.BaseType.GetGenericArguments()[0].Name}");
            }
            catch (Exception e)
            {
                w.titleContent = new GUIContent("Select data provider");
            }

            w.onSelectedItem = _type => { CreateDataProvider(obj, field, onCreate, _type); };
        }

        private static void CreateDataProvider(Object obj, FieldInfo field, Action onCreate, Type _type)
        {
            Undo.RecordObject(obj, "inspector");
            onCreate?.Invoke();
            var newProvider = Undo.AddComponent((obj as Component).gameObject, _type);
            Undo.RecordObject(obj, "inspector");
            field.SetValue(obj, newProvider);
        }

        #endregion
    }
}