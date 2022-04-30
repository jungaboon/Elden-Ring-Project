// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Linq;
using RVModules.RVUtilities.Reflection;
using UnityEditor;
using UnityEngine;

namespace RVModules.RVSmartAI.Editor.SelectWindows
{
    /// <summary>
    /// Ta klasa tak musi byc, bo nie mozna wziac typu dynamicnznie z typ w OnGUI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectWindowBase : EditorWindow
    {
        #region Fields

        public Type[] types;

        public Action<Type> onSelectedItem;

        private string searchText = "";

        #endregion

        #region Properties

        public virtual string Title { get; }

        #endregion

        #region Not public methods

        protected virtual void OnEnable()
        {
            // allow for only one opened window
            var windows = Resources.FindObjectsOfTypeAll(GetType());
            foreach (var w in windows)
                if (w != this)
                    ((EditorWindow) w).Close();

            ShowUtility();

            titleContent = new GUIContent
            {
                text = Title
            };
            
            types = ReflectionHelper.GetDerivedTypes(GetTypes());
        }

        //protected virtual Type[] GetTypes() => ReflectionHelper.GetDerivedTypes(typeof(T));
        protected virtual Type GetTypes() => typeof(Component);

        private void OnGUI()
        {
            Repaint();
            searchText = EditorGUILayout.TextField(searchText);
            var t = types.OrderBy(_type => _type.Name);
            foreach (var type in t)
            {
                var nameToDisplay = NameToDisplay(type);
                if (nameToDisplay.ToUpper().Contains("obsolete".ToUpper())) continue;
                if (!nameToDisplay.ToUpper().Contains(searchText.ToUpper())) continue;
                if (!GUILayout.Button("  " + ObjectNames.NicifyVariableName(nameToDisplay), GUIHelpers.GuiStyle(4))) continue;
                onSelectedItem?.Invoke(type);
                Close();
            }
        }

        protected virtual string NameToDisplay(Type type) => type.Name;

        #endregion
    }
}