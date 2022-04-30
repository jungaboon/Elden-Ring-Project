// Created by Ronis Vision. All rights reserved
// 12.04.2021.

using System;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// Stick this on a method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public InspectorButtonAttribute()
        {
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Object), true)]
    public  class EditorButton : Editor
    {
        #region Public methods

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mono = target as Object;
            if (mono as ScriptableObject == null && mono as MonoBehaviour == null) return;

            var methods = mono.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                            BindingFlags.NonPublic)
                .Where(o => Attribute.IsDefined(o, typeof(InspectorButtonAttribute)));
            
            foreach (var methodInfo in methods)
                if (GUILayout.Button(methodInfo.Name))
                    methodInfo.Invoke(mono, null);
        }

        #endregion
    }
#endif
}