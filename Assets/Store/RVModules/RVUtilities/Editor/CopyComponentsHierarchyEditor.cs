// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEditor;
using UnityEngine;

namespace RVModules.RVUtilities.Editor
{
    [CustomEditor(typeof(CopyComponentsHierarchy))]
    public class CopyComponentsHierarchyEditor : UnityEditor.Editor
    {
        #region Fields

        private CopyComponentsHierarchy tool;

        #endregion

        #region Public methods

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            tool = target as CopyComponentsHierarchy;

            if (GUILayout.Button("Copy components")) tool.Work();
        }

        #endregion
    }
}