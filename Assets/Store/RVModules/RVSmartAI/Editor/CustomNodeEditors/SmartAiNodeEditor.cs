// Created by Ronis Vision. All rights reserved
// 18.03.2021.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RVModules.RVSmartAI.Editor.SelectWindows;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Stages;
using RVModules.RVSmartAI.GraphElements.Utilities;
using RVModules.RVSmartAI.Nodes;
using RVModules.RVUtilities.Extensions;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using XNode;
using XNodeEditor;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.Editor.CustomNodeEditors
{
    public class SmartAiNodeEditor : NodeEditor
    {
        protected GUIStyle buttonStyle;
        protected static IAiGraphElement graphElementClipboard;
        public override int GetWidth() => 300;

        public override Color GetTint()
        {
            var s = target as SmartAiNode;
            var graph = s.AiGraph;
            if (graph == null) return base.GetTint();
            if (graph.lastNode == null) return base.GetTint();
            if (graph.lastNode == target)
                return Color.cyan;
            return base.GetTint();
        }

        public override void OnHeaderGUI()
        {
            var s = target as SmartAiNode;
//            if (s.stage == null)
//            {
//                GUILayout.Label(s.GetType().Name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
//                return;
//            }

            var c = GUI.contentColor;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(s.gameObject))
                GUI.contentColor = GUIHelpers.GuiStyle(3).normal.textColor;

            var nejm = "";

            nejm = string.IsNullOrEmpty(s.Name) ? s.GetType().Name : s.Name;

            if (s.IsRoot) nejm += " (ROOT)";

            GUILayout.Label(nejm, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));

            GUI.contentColor = c;
        }

        protected void SelectGoOfGraphElement()
        {
            var sn = target as SmartAiNode;
            var aiGraph = sn.AiGraph;

            var selectedGraphElement = aiGraph.GetSelectedGraphElement();
            Selection.activeGameObject = selectedGraphElement.gameObject;
        }

        protected bool GetSelectedGraphElement(out AiGraph aiGraph, out IAiGraphElement selectedGraphElement)
        {
            var sn = target as SmartAiNode;
            aiGraph = sn.AiGraph;

            selectedGraphElement = aiGraph.GetSelectedGraphElement();
            if (selectedGraphElement == null)
            {
                Debug.Log("Select graph element first");
                return true;
            }

            return false;
        }

        protected static async void SetSelectedElementRetarded(AiGraphElement _element, SmartAiNode _ownerNode)
        {
            await Task.Delay(5);
            _ownerNode.selectedElement = _element;
        }

        [InitializeOnLoadMethod]
        private static void OnNodeSelected() =>
            Selection.selectionChanged += () =>
            {
                var s = Selection.objects.FirstOrDefault();
                if (s == null) return;
                var sn = s as SmartAiNode;
                if (sn == null) return;
                sn.selectedElement = null;
            };

        protected virtual void AddCustomSmartAiContextMenuItems(GenericMenu _genericMenu)
        {
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            var sn = target as SmartAiNode;

            var graph = sn.AiGraph;
            if (graph == null) return;

            // Custom sctions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is INode)
            {

                var node = Selection.activeObject as INode;
                NodeEditorWindow.AddCustomContextMenuItems(menu, node);

                AddCustomSmartAiContextMenuItems(menu);

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Copy selected graph element"), false, CopySelectedGraphElement);
                menu.AddItem(new GUIContent("Duplicate selected graph element"), false, DuplicateSelectedGraphElement);
                if (graphElementClipboard != null) menu.AddItem(new GUIContent("Paste copied graph element"), false, PasteGraphElement);
                menu.AddItem(new GUIContent("Select corresponding game object"), false, SelectGoOfGraphElement);
                menu.AddItem(new GUIContent("Make selected graph element prefab"), false, MakeSelectedGraphElementPrefab);

                if (graph.GetSelectedGraphElement() == null) return;

                var selectedGo = graph.GetSelectedGraphElement().gameObject;

                if (PrefabUtility.IsAnyPrefabInstanceRoot(selectedGo))
                    menu.AddItem(new GUIContent("Unpack selected graph element prefab"), false,
                        () => PrefabUtility.UnpackPrefabInstance(
                            selectedGo,
                            PrefabUnpackMode.OutermostRoot,
                            InteractionMode.UserAction));

                if (PrefabUtility.IsPartOfAnyPrefab(graph.GetSelectedGraphElement().gameObject))
                    menu.AddItem(new GUIContent("Show selected graph element prefab"), false,
                        () => Selection.activeObject =
                            AssetDatabase.LoadAssetAtPath<Object>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedGo)));
            }
        }

        private void DuplicateSelectedGraphElement()
        {
            if (GetSelectedGraphElement(out var aiGraph, out var selectedGraphElement)) return;
            AiGraphEditor.ValidateAndAddToGraphNewElement(aiGraph, selectedGraphElement.gameObject, selectedGraphElement.GetParentGraphElement());
        }

        private void CopySelectedGraphElement()
        {
            if (GetSelectedGraphElement(out var aiGraph, out var selectedGraphElement)) return;

            graphElementClipboard = selectedGraphElement;
            Debug.Log($"{graphElementClipboard} copied into clipboard. It will be lost on serialization.", graphElementClipboard?.gameObject);
        }

        private void PasteGraphElement()
        {
            if (GetSelectedGraphElement(out var aiGraph, out var selectedGraphElement)) return;

            AiGraphEditor.ValidateAndAddToGraphNewElement(aiGraph, graphElementClipboard.gameObject, selectedGraphElement);
        }

        private void MakeSelectedGraphElementPrefab()
        {
            if (GetSelectedGraphElement(out var aiGraph, out var selectedGraphElement)) return;

            var path = "";
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                path = AssetDatabase.GetAssetPath(NodeEditorWindow.current.lastOpenedSmartAiGraphId);
            else
                path = PrefabStageUtility.GetCurrentPrefabStage().prefabAssetPath;

            if (string.IsNullOrEmpty(path)) path = Application.dataPath + "\\";
            else
                path = new FileInfo(path).Directory.FullName + "\\";

            path = path + (aiGraph.name + "_" + selectedGraphElement.Name + ".prefab").GetWithoutIllegal();

            var goPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
                selectedGraphElement.gameObject, path, InteractionMode.UserAction);
            Selection.activeObject = goPrefab;
        }

        protected virtual void AddTaskButton(IAiGraphElement graphElement, Action onClicked = null)
        {
            if (GUILayout.Button(GUIHelpers.AddTaskButton(), GUIHelpers.GuiStyle(1), GUILayout.MaxWidth(18),
                GUILayout.MaxHeight(18)) /*&& !dontAllowAddTask*/)
            {
                onClicked?.Invoke();
                AddTask(graphElement);
                //e.onSelectedGameObject += _o => { graph.ValidateAndAddToGraphNewElement(_o, utility); };
            }
        }

        protected virtual void AddTask(IAiGraphElement graphElement)
        {
            var e = ScriptableObject.CreateInstance<SelectTaskWindow>();
            e.onSelectedItem = _aiTask =>
            {
                Undo.RecordObject(graphElement as Object, "inspector");
                // create it
                var newTask = graphElement.AiGraph.CreateNewElement(_aiTask, graphElement) as AiTask;
                // assign
                graphElement.AssignSubSelement(newTask);
            };
        }

        protected void DrawTasks(IAiGraphElement taskOwner, SmartAiNode sn, List<AiTask> _tasks)
        {
            var graph = taskOwner.AiGraph;

            Color c;
            for (var taskId = 0; taskId < _tasks.Count; taskId++)
            {
                var task = _tasks[taskId];
                if (task == null) continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                c = GUI.backgroundColor;

                GUI.backgroundColor = new Color(0, 0, 0, 0);
                GUILayout.Box(GUIHelpers.ActionIcon(), GUILayout.Height(20), GUILayout.Width(20));
                GUI.backgroundColor = c;

                c = GUI.contentColor;

                if (!task.Enabled || !taskOwner.Enabled) GUI.contentColor = Color.gray;

                if (PrefabUtility.IsAnyPrefabInstanceRoot(task.gameObject))
                    buttonStyle = GUIHelpers.GuiStyle(3);

                var taskText = task.Name;
                if (graph.IsRuntimeDebugGraph && task.taskScorers.Count > 0) taskText += $" ({Math.Round(task.lastScore, 2)})";

                if (graph.IsRuntimeDebugGraph && task.IsRunningTask && task.IsExecuting) buttonStyle = GUIHelpers.GuiDebugStyle(0);

                if (GUILayout.Button(taskText, buttonStyle))
                {
                    Selection.objects = new Object[] {sn};
                    SetSelectedElementRetarded(task, sn);
                }

                GUI.contentColor = c;
                buttonStyle = GUIStyles.GetGUIStyle(0);

                GUILayout.FlexibleSpace();

                var allowDestroy = task.CanBeRemoved();

                GUIHelpers.UpDownRemove(() => { _tasks.Move(taskId, MoveDirection.Up); },
                    () => { _tasks.Move(taskId, MoveDirection.Down); },
                    () =>
                    {
                        Undo.RegisterCompleteObjectUndo(taskOwner as Object, "inspector");
                        _tasks.Remove(task);
                        task.Destroy();
                    }, !allowDestroy);

                EditorGUILayout.EndHorizontal();
            }
        }

        protected virtual bool HandleExpandedButton(SmartAiNode sn, Object _objectToUndo)
        {
            var expanded = sn.expanded;

            Undo.RecordObject(_objectToUndo, "inspector");

            var buttonTexture = GUIHelpers.DownButton();
            if (sn.expanded) buttonTexture = GUIHelpers.UpButton();

            if (GUI.Button(new Rect(GetWidth() -34, 35, 15, 15), buttonTexture, GUIHelpers.GuiStyle(1))) sn.expanded = !expanded;
//            if (GUILayout.Button(buttonTexture, GUIHelpers.GuiStyle(1))) sn.expanded = !expanded;
            return expanded;
        }
    }
}