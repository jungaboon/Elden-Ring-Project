// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using XNode;
using XNodeEditor;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.Editor.CustomInspectors
{
    [UnityEditor.CustomEditor(typeof(AiGraph))]
    public class AiGraphInspector : UnityEditor.Editor
    {
        #region Fields

        //private SerializedProperty graphNameProp;
        private SerializedProperty graphDescriptionProp;
        private SerializedProperty graphVariablesProp;

        #endregion

        #region Public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, "inspector");

            var graph = target as AiGraph;

            foreach (var graphNode in graph.nodes) Undo.RecordObject(graphNode, "inspector");

            // because prefab workflows, now opening graph instances should not be possible
//            if (GUILayout.Button("Open graph"))
//            {
//                graph.UpdateAiGraphForAllElements();
//                NodeEditorWindow.OpenWithGraph(graph);
//                //NodeEditorWindow.current.Focus();
//            }

            GUIHelpers.GUIDrawNameAndDescription(graph, graph.GetType().Name, null, graphDescriptionProp, out var desc);
            graph.description = desc;

            EditorGUILayout.PropertyField(graphVariablesProp);

            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            foreach (var graphNode in graph.nodes)
                PrefabUtility.RecordPrefabInstancePropertyModifications(graphNode);

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Not public methods

        private void OnEnable()
        {
            //graphNameProp = serializedObject.FindProperty("name");
            graphDescriptionProp = serializedObject.FindProperty("description");
            graphVariablesProp = serializedObject.FindProperty("graphAiVariables");
        }

        [MenuItem("RVSmartAI/Open SmartAi graph window")]
        private static void OpenSmartAiGraphWindow()
        {
            if (Selection.activeGameObject != null)
            {
                var graph = Selection.activeGameObject.GetComponent<INodeGraph>();
                if (graph == null)
                {
                    Debug.LogError("Select SmartAi graph first");
                    return;
                }

                NodeEditorWindow.OpenWithGraph(graph);
                AiGraphEditor.LoadGraphPrefabContents(Selection.activeGameObject);
                return;
            }

            Debug.LogError("Select SmartAi graph first");
        }

        [MenuItem("RVSmartAI/Create new AiGraph")]
        private static void CreateNewAiGraph() =>
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateGraphPrefab>(), "New AiGraph", null,
                null /*SmartAiSettings.prefabPath*/);

        #endregion

        private class CreateGraphPrefab : EndNameEditAction
        {
            #region Public methods

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var go = new GameObject("AiGraph");
                var graph = go.AddComponent<AiGraph>();
                var graphVars = go.AddComponent<AiVariables>();
                graph.GraphAiVariables = graphVars;

                var path = pathName + ".prefab";
                Object asset = null;

                try
                {
                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.UserAction);
                    asset = AssetDatabase.LoadAssetAtPath<Object>(pathName + ".prefab");
                }
                catch (Exception e)
                {
                    DestroyImmediate(go);
                    Debug.LogError(e);
                    return;
                }

                ProjectWindowUtil.ShowCreatedAsset(asset);
                DestroyImmediate(go);
                OpenSmartAiGraphWindow();
            }

            #endregion
        }
    }
}