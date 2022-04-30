// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVUtilities.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionHelper
    {
        #region Public methods

        public static void CopyFields(object source, object target, bool logAssignments = false, bool copyReferenceTypeFields = false, Type dontGoAboveThisTypeIncluded = null)
        {
            if (source == null || target == null) throw new NullReferenceException();

            var sourceFields = source.GetType().GetAllFields(dontGoAboveThisTypeIncluded);
            var targetFields = target.GetType().GetAllFields(dontGoAboveThisTypeIncluded);

            foreach (var targetField in targetFields)
            {
                foreach (var sourceField in sourceFields)
                {
                    if (sourceField.Name != targetField.Name || !targetField.FieldType.IsValueType && !copyReferenceTypeFields) continue;
                    targetField.SetValue(target, sourceField.GetValue(source));
                    if (logAssignments) Debug.Log($"Copied field {targetField.Name} from {source} to {target}", target as Object);
                }
            }
        }
#if UNITY_EDITOR
        
        /// <summary>
        /// editor only !!!
        /// </summary>
        public static void CopyFieldsOfUnityObject(Object source, Object target, bool logAssignments = false, bool copyReferenceTypeFields = false)
        {
            UnityEditor.Undo.RecordObject(target, $"Copy fields from {source} to {target}");
            CopyFields(source, target, logAssignments, copyReferenceTypeFields, typeof(Object));
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }
        /// <summary>
        /// editor only !!!
        /// </summary>
        public static void CopyGameObjectAllComponentsFields(GameObject source, GameObject target, bool logAssignments = false, bool copyReferenceTypeFields = false)
        {
            var targetComponents = target.GetComponents<Component>();

            foreach (var targetComponent in targetComponents)
            {
                var equivalentSourceComp = source.GetComponent(targetComponent.GetType());
                if (equivalentSourceComp == null) continue;
                CopyFieldsOfUnityObject(equivalentSourceComp, targetComponent, logAssignments,copyReferenceTypeFields);
            }
        }
#endif
        
        /// <summary>
        /// Returns all fields from type, also private from all base classes
        /// </summary>
        public static IList<FieldInfo> GetAllFields(this Type type, Type dontGoAboveThisTypeIncluded = null, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if(type.BaseType == null || dontGoAboveThisTypeIncluded == type) return new List<FieldInfo>();
            
            var list = type.BaseType.GetAllFields(dontGoAboveThisTypeIncluded, flags);
            // in order to avoid duplicates, force BindingFlags.DeclaredOnly
            foreach (var f in type.GetFields(flags | BindingFlags.DeclaredOnly)) list.Add(f);
            return list;
        }
        
        /// <summary> Get FieldInfo of a field, including those that are private and/or inherited </summary>
        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            // If we can't find field in the first run, it's probably a private field in a base class.
            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // Search base classes for private fields only. Public fields are found above
            if (field == null)
                field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return field;
        }

        public static Type GetTypeByName(string _name, Type[] _allTypes = null)
        {
            if (_allTypes == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes();
                    if (TypeByName(types, out var type)) return type;
                }
            }
            else
            {
                if (TypeByName(_allTypes, out var type)) return type;
            }

            return null;

            bool TypeByName(Type[] types, out Type type)
            {
                type = null;
                foreach (var type1 in types)
                    if (type1.Name == _name)
                    {
                        type = type1;
                        return true;
                    }

                return false;
            }
        }

        public static object CreateObjectFromName(string _className)
        {
            var t = GetTypeByName(_className);
            return Activator.CreateInstance(t);
        }

        /// <summary> Get all classes deriving from baseType via reflection </summary>
        public static Type[] GetDerivedTypes(Type baseType)
        {
            var types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t) && t != baseType).ToArray())
                types.Add(type);
            return types.ToArray();
        }

        public static Type[] GetDerivedGenericTypes(Type baseType)
        {
            var types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                types.AddRange(assembly.GetTypes().Where(t => t != baseType && IsAssignableToGenericType(t, baseType)));
            return types.ToArray();
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public static string ExtractString(string s, string tag)
        {
            // You should check for errors in real-world code, omitted for brevity
            var startTag = "<" + tag + ">";
            var startIndex = s.IndexOf(startTag) + startTag.Length;
            var endIndex = s.IndexOf("</" + tag + ">", startIndex);
            return s.Substring(startIndex, endIndex - startIndex);
        }

        public static Type[] GetTypesFromAllAssemblies()
        {
            var types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) types.AddRange(assembly.GetTypes());

            return types.ToArray();
        }

        #endregion

        #region other

        public class ReflectionSearchIgnoreAttribute : Attribute
        {
        }

        /// <summary>
        /// Gets all non-abstract types extending the given base type
        /// </summary>
        public static Type[] getSubTypes(Type baseType) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName.Contains("Assembly"))
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(T =>
                        T.IsClass && !T.IsAbstract
                                  && T.IsSubclassOf(baseType)
                                  && !T.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any())
                ).ToArray();

        /// <summary>
        /// Gets all non-abstract types extending the given base type and with the given attribute
        /// </summary>
        public static Type[] getSubTypes(Type baseType, Type hasAttribute) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName.Contains("Assembly"))
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(T =>
                        T.IsClass && !T.IsAbstract
                                  && T.IsSubclassOf(baseType)
                                  && T.GetCustomAttributes(hasAttribute, false).Any()
                                  && !T.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any())
                ).ToArray();

        /// <summary>
        /// Returns all fields that should be serialized in the given type
        /// </summary>
        public static FieldInfo[] getSerializedFields(Type type) =>
            type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field =>
                    field.IsPublic && !field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Any()
                    || field.GetCustomAttributes(typeof(SerializeField), true).Any()
                    && !field.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any())
                .ToArray();

        /// <summary>
        /// Returns all fields that should be serialized in the given type, minus the fields declared in or above the given base type
        /// </summary>
        public static FieldInfo[] getSerializedFields(Type type, Type hiddenBaseType) =>
            type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field =>
                    (hiddenBaseType == null || !field.DeclaringType.IsAssignableFrom(hiddenBaseType))
                    && (field.IsPublic && !field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Any()
                        || field.GetCustomAttributes(typeof(SerializeField), true).Any()
                        && !field.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any()))
                .ToArray();

        /// <summary>
        /// Gets all fields in the classType of the specified fieldType
        /// </summary>
        public static FieldInfo[] getFieldsOfType(Type classType, Type fieldType) =>
            classType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field =>
                    field.FieldType == fieldType || field.FieldType.IsSubclassOf(fieldType)
                    && !field.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any())
                .ToArray();

        #endregion
    }
}