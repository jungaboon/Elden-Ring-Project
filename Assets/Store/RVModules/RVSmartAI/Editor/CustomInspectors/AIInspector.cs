// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using RVModules.RVUtilities.Editor;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RVModules.RVSmartAI.Editor.CustomInspectors
{
    [CanEditMultipleObjects] [UnityEditor.CustomEditor(typeof(Ai))]
    public class AIInspector : UnityEditor.Editor
    {
        #region Fields

        private SerializedProperty graphProp;

        private SerializedProperty contextProviderProp;

//        private SerializedProperty stepsPerUpdateProp;
        private SerializedProperty updateFrequencyProp;
        private SerializedProperty dontHideInHierarchyProp;
        private SerializedProperty secondaryGraphs;
        private SerializedProperty instantiatedSecondaryGraphs;
        private Ai ai;

        private static bool advancedFold;

        #endregion

        #region Public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //EditorGUILayout.LabelField("RV Smart AI", GUIHelpers.GuiStyle(0));

            if (!Application.isPlaying)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(graphProp);
                if (EditorGUI.EndChangeCheck())
                    if (graphProp.objectReferenceValue != null)
                        if (AiGraphEditor.CheckIfIsPartOfPrefab(graphProp.objectReferenceValue as AiGraph, true) == false)
                        {
                            Debug.LogError("Using non-prefab graphs is not allowed. Assign graph prefab here", target);
                            graphProp.objectReferenceValue = null;
                        }

                if (ai.MainAiGraph != null && GUILayout.Button("Open graph"))
                {
                    NodeEditorWindow.OpenWithGraph(ai.MainAiGraph);
                    AiGraphEditor.LoadGraphPrefabContents(ai.MainAiGraph.gameObject);
                }

                EditorGUILayout.PropertyField(contextProviderProp, true);

                advancedFold = EditorGUILayout.Foldout(advancedFold, "Advanced");
                if (advancedFold)
                {
                    var useCustomLoadBalancerConfigProp = serializedObject.FindProperty("useCustomLoadBalancerConfig");
                    if (useCustomLoadBalancerConfigProp.boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideGraphVariablesForNestedGraphs"));
                        EditorGUILayout.PropertyField(dontHideInHierarchyProp);
                        EditorGUILayout.PropertyField(useCustomLoadBalancerConfigProp);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("mainGraphLbc"), true);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideGraphVariablesForNestedGraphs"));
                        EditorGUILayout.PropertyField(dontHideInHierarchyProp);
                        
                        EditorGUILayout.PropertyField(useCustomLoadBalancerConfigProp);
                        var scalableAi = serializedObject.FindProperty("scalableLoadBalancing");
                        EditorGUILayout.PropertyField(scalableAi);
                        int maxAllowedFreq = updateFrequencyProp.intValue;
                        
                        if (scalableAi.boolValue)
                        {
                            var serializedProperty = serializedObject.FindProperty("maxAllowedUpdateFrequency");
                            maxAllowedFreq = serializedProperty.intValue;
                            EditorGUILayout.PropertyField(serializedProperty);
                        }

                        EditorGUILayout.IntSlider(updateFrequencyProp, 1, 8);
                        
                        var expand = serializedObject.FindProperty("expandPerfInfo");
                        expand.boolValue = EditorGUILayout.Foldout(expand.boolValue, "Performance info");
                        if (expand.boolValue) GUIHelpers.AiLbPerfInfo(maxAllowedFreq, updateFrequencyProp.intValue, scalableAi.boolValue);
                    }

//                    EditorGUILayout.HelpBox(
//                        "How many steps will graph do in one update. One step is going from one stage node to another. " +
//                        "Think of this as AI speed of thinking vs performance (more steps -> faster AI). For debugging purposes you will " +
//                        "almost always want to have it at 1, so you can easily observe AI decision process step by step.",
//                        MessageType.Info);
//                    EditorGUILayout.IntSlider(stepsPerUpdateProp, 1, 20);
//                    EditorGUILayout.HelpBox(
//                        "By default runtime instances of graphs are hidden. Sometimes it may be usefull to not hide them, for example if " +
//                        "you want to save whole graph with runtime changes as prefab.",
//                        MessageType.Info);
                }

                EditorGUILayout.PropertyField(secondaryGraphs, true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("references"), true);
            }
            else
            {
                if(Application.isPlaying && !ai.IsInitialized)
                {
                    serializedObject.ApplyModifiedProperties();
                    return;
                }

                if (GUILayout.Button("Debug graph " + ai.MainAiGraph.name))
                    NodeEditorWindow.OpenWithGraph(ai.MainAiGraph);

//                EditorGUILayout.LabelField("Debug info");
//                EditorGUILayout.BeginHorizontal("box");
//                EditorGUILayout.LabelField("Current node: " + ai.CurrentNode);
//                EditorGUILayout.EndHorizontal();
//                EditorGUILayout.BeginHorizontal("box");
//                EditorGUILayout.LabelField("Last utility: " + ai.LastUtility);
//                EditorGUILayout.EndHorizontal();
//                EditorGUILayout.BeginHorizontal("box");
//                EditorGUILayout.LabelField("Last task: " + ai.LastTask);
//                EditorGUILayout.EndHorizontal();

                ShowGraphVariables(ai.MainGraphAiVariables);

                if (ai.SecondaryGraphs.Length > 0)
                {
                    EditorGUILayout.LabelField("Secondary graphs");
                    EditorHelpers.WrapInBox(() =>
                    {
                        for (var i = 0; i < instantiatedSecondaryGraphs.arraySize; i++)
                        {
                            var sGraph = instantiatedSecondaryGraphs.GetArrayElementAtIndex(i);
                            EditorGUILayout.PropertyField(sGraph);
                            if (GUILayout.Button("Debug graph " + ai.SecondaryGraphs[i].name)) NodeEditorWindow.OpenWithGraph(ai.SecondaryGraphs[i]);
                            ShowGraphVariables(ai.SecondaryGraphs[i].GraphAiVariables);
                        }
                    }, 0);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// todo To consider: also show real-time graph vars in custom vars inspector at runtime
        /// </summary>
        /// <param name="_aiVariables"></param>
        private static void ShowGraphVariables(AiVariables _aiVariables)
        {
            if (_aiVariables == null) return;
            var allVariablesAsStrings = _aiVariables.GetAllVariablesAsStrings();
            if (allVariablesAsStrings.Length == 0) return;

            EditorGUILayout.LabelField("Variables");
            EditorHelpers.WrapInBox(() =>
            {
                foreach (var allVariable in allVariablesAsStrings) EditorGUILayout.LabelField(allVariable);
            }, 0);
        }

        #endregion

        #region Not public methods

        private void OnEnable()
        {
            ai = target as Ai;
            graphProp = serializedObject.FindProperty("aiGraph");
            contextProviderProp = serializedObject.FindProperty("contextProvider");
//            stepsPerUpdateProp = serializedObject.FindProperty("graphStepsPerUpdate");
            dontHideInHierarchyProp = serializedObject.FindProperty("dontHideInHierarchy");
            updateFrequencyProp = serializedObject.FindProperty("updateFrequency");
            secondaryGraphs = serializedObject.FindProperty("secondaryGraphs");
            instantiatedSecondaryGraphs = serializedObject.FindProperty("instantiatedSecondaryGraphs");
        }

        #endregion
    }
}