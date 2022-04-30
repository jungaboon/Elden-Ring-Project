// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RVModules.RVSmartAI.Editor
{
    public static class RuntimeDebugGraph
    {
        #region Public methods

        [InitializeOnLoadMethod]
        public static void AddOpenGraphCallbackToSelectionChange() =>
            Selection.selectionChanged += () =>
            {
                var selectedGraph = Selection.activeObject as AiGraph;
                if (selectedGraph == null) return;

                if (AssetDatabase.Contains(selectedGraph) || !Application.isPlaying)
                    return;

                // if you want to use one window 
                //NodeEditorWindow w = GetWindow(typeof(NodeEditorWindow), false, "SmartAIGraph", true) as NodeEditorWindow;
                // 
                var w = GetDebugWindow();
                if (w == null) w = ScriptableObject.CreateInstance<NodeEditorWindow>();

                w.Show();
                w.Focus();
                AiGraphEditor.AssignGraphForCurrentNodeEditorWindow(selectedGraph);
            };

        [InitializeOnLoadMethod]
        public static void CloseRuntimeGraphs() => EditorApplication.playModeStateChanged += CloseRuntimeGraphs;

        #endregion

        #region Not public methods

        private static NodeEditorWindow GetDebugWindow() =>
            Resources.FindObjectsOfTypeAll<NodeEditorWindow>().FirstOrDefault(e =>
            {
                var g = e.graph as AiGraph;
                if (g == null) return false;
                return g.IsRuntimeDebugGraph;
            });

        // closes debug window efter exiting from play mode
        private static void CloseRuntimeGraphs(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode) return;
            if (GetDebugWindow() == null) return;
            AiGraphEditor.AssignGraphForCurrentNodeEditorWindow();
            //GetDebugWindow().graph = null;
        }

        #endregion
    }
}