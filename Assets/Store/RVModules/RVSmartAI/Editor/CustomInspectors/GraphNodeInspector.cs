// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using RVModules.RVSmartAI.Nodes;
using UnityEditor;

namespace RVModules.RVSmartAI.Editor.CustomInspectors
{
    [CustomEditor(typeof(GraphNode), true)]
    public class GraphNodeInspector : UnityEditor.Editor
    {
        #region Fields

        protected SerializedProperty nameProp;
        protected SerializedProperty descProp;

        #endregion

        #region Public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, "inspector");

            var gn = target as GraphNode;

            EditorGUI.BeginChangeCheck();
            GUIHelpers.GUIDrawNameAndDescription(target, gn.GetType().Name, nameProp, descProp, out var desc);
            if (EditorGUI.EndChangeCheck()) gn.UpdateGameObjectName();

            gn.Description = desc;

            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Not public methods

        private void OnEnable()
        {
            try
            {
                nameProp = serializedObject.FindProperty("_name");
                descProp = serializedObject.FindProperty("description");
            }
            catch
            {
            }
        }

        #endregion
    }
}