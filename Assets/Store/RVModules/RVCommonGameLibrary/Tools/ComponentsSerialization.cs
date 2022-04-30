// Created by Ronis Vision. All rights reserved
// 18.09.2021.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using RVModules.RVUtilities.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVCommonGameLibrary.Tools
{
    /// <summary>
    /// todo check if we can remove ths stupid line everywhere &lt;?xml version="1.0" encoding="utf-16"?&gt;
    /// todo add trycatches in critical stuff like writing fields
    /// </summary>
    public static class ComponentsSerialization
    {
        #region Public methods

        /// <summary>
        /// Writes components fields to those matching names from text serialized data
        /// </summary>
        /// <param name="components">Components to set fields to</param>
        /// <param name="filePath">Path with file name</param>
        /// <param name="fieldPredicate">Predicate allowing to selectively skip reading fields. Args: field name, component
        /// Called only when field name and serialized field name are matching</param>
        /// <param name="logFails">Will log all failed field reads (fieldInfo.GetValue)</param>
        public static void ReadComponentsData(Component[] components, string filePath, Func<string, Component, bool> fieldPredicate = null,
            bool logFails = false)
        {
            var content = File.ReadAllText(filePath);
            var dataContent = content.Split(';');

            foreach (var s in dataContent)
            {
                var lines = s.Split(
                    new[] {"\r\n", "\r", "\n"},
                    StringSplitOptions.None
                );
                var propName = lines[0];
                var xmlContentList = lines.ToList();
                xmlContentList.RemoveAt(0);
                var xmlContent = "";
                foreach (var xmlLine in xmlContentList) xmlContent += xmlLine + "\n";
                bool loggedPredicateError = false;

                var isUnityObject = xmlContent.Contains("unity object ");

                foreach (var component in components)
                {
                    if (component == null) continue;
                    Undo.RegisterCompleteObjectUndo(component, "Set component data");

                    foreach (var fieldInfo in component.GetType().GetAllFields())
                    {
                        if (fieldInfo.Name == "m_InstanceID") continue;
                        if (fieldInfo.Name == propName)
                        {
                            // def is true, so when predicate will throw exception, well just ignore it
                            bool predicateResult = true;
                            try
                            {
                                if (fieldPredicate != null)
                                    predicateResult = fieldPredicate.Invoke(fieldInfo.Name, component);
                            }
                            catch (Exception e)
                            {
                                if (!loggedPredicateError)
                                {
                                    loggedPredicateError = true;
                                    Debug.Log($"Predicate error: {e}");
                                }
                            }

                            if (!predicateResult) continue;
                            object deserValue = null;
                            if (!isUnityObject)
                            {
                                try
                                {
                                    // Debug.Log(xmlContent.Substring(0, 4));
                                    if (xmlContent.Substring(0, 4) == "null")
                                    {
                                    }
                                    else
                                        deserValue = XmlDeserialize(xmlContent, fieldInfo.FieldType);
                                }
                                catch (Exception e)
                                {
                                    if (logFails) Debug.Log($"Failed to deserialize field {propName}, content: {xmlContent}, error: {e}");
                                }
                            }
                            else
                            {
                                // to unity object ref try
                                var guid = xmlContentList[0];
                                guid = guid.Replace("unity object ", "");
                                // Debug.Log(guid);
                                if (GlobalObjectId.TryParse(guid, out var unityGuid))
                                {
                                    var unityObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(unityGuid);

                                    if (unityObject != null)
                                    {
                                        SetField(fieldInfo, component, unityObject);
                                        continue;
                                    }
                                }
                            }

                            // if (deserValue != null)
                            // {
                            SetField(fieldInfo, component, deserValue);
                            // }
                        }
                    }

                    PrefabUtility.RecordPrefabInstancePropertyModifications(component);
                }

                // Debug.Log(s);
            }

            void SetField(FieldInfo fieldInfo, Component component, object deserValue)
            {
                try
                {
                    fieldInfo.SetValue(component, deserValue);
                }
                catch (Exception e)
                {
                    if (logFails) Debug.Log($"Failed to set field {fieldInfo.Name}, on component {component.GetType().Name}, error: {e}", component);
                }
            }
        }

        /// <summary>
        /// Serializes array of components into one text file
        /// </summary>
        /// <param name="components"></param>
        /// <param name="filePath"></param>
        /// <param name="fieldPredicate">Predicate allowing to selectively skip serializing fields. Args: field name, component</param>
        /// <param name="logFails">Will log all failed field reads (fieldInfo.GetValue) and serializations</param>
        /// <param name="serializeOnlyUnitySerialized">If true, only public and fields with SerializeField attribute will get serialzied</param>
        public static void SerializeComponents(Component[] components, string filePath, Func<string, Component, bool> fieldPredicate = null,
            bool logFails = false, bool serializeOnlyUnitySerialized = true)
        {
            var serializedGameObjectData = "";

            foreach (var component in components)
            {
                if (component == null) continue;
                foreach (var fieldInfo in component.GetType().GetAllFields())
                {
                    if (fieldPredicate != null && !fieldPredicate.Invoke(fieldInfo.Name, component)) continue;
                    if (fieldInfo.Name == "m_InstanceID") continue;

                    if (!serializeOnlyUnitySerialized || (!fieldInfo.IsPublic && fieldInfo.GetCustomAttribute<SerializeField>() == null))
                    {
                        continue;
                    }

                    var serField = $"{fieldInfo.Name}\n";

                    // Debug.Log(fieldInfo.FieldType);
                    object fieldValue = null;

                    try
                    {
                        fieldValue = fieldInfo.GetValue(component);
                        if (fieldValue is IntPtr) continue;
                    }
                    catch (Exception e)
                    {
                        if (logFails)
                            Debug.Log($"Failed to get value of {fieldInfo.Name} on component {component.GetType().Name}, error: {e.Message}", component);
                        continue;
                    }


                    if (fieldValue is Object unityObject)
                    {
                        serField += "unity object ";

                        // Debug.Log($"field is of unity object type");
                        if (unityObject == null)
                            continue;
                        else
                            serField += GlobalObjectId.GetGlobalObjectIdSlow(unityObject);
                    }
                    else
                    {
                        try
                        {
                            if (fieldValue == null)
                            {
                                serField += "null";
                            }
                            else
                                serField += SerializeObject(fieldValue);
                        }
                        catch (Exception e)
                        {
                            if (logFails)
                                Debug.Log($"Failed to serialize {fieldInfo.Name} on component {component.GetType().Name}, error: {e.Message}", component);
                            continue;
                        }
                    }

                    // Debug.Log(serField);
                    serializedGameObjectData += serField;
                    serializedGameObjectData += ";";
                }

                File.WriteAllText(filePath, serializedGameObjectData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static object XmlDeserialize(string toDeserialize, Type _type)
        {
            var xmlSerializer = new XmlSerializer(_type);
            using (var textReader = new StringReader(toDeserialize))
            {
                return xmlSerializer.Deserialize(textReader);
            }
        }

        public static string SerializeObject<T>(T toSerialize)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        #endregion
    }
}