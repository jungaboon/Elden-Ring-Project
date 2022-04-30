// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RVModules.RVSmartAI.Editor.SelectWindows;
using RVModules.RVSmartAI.GraphElements;
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
    [XNodeEditor.CustomEditor(typeof(StageNode))]
    public class StageNodeEditor : SmartAiNodeEditor
    {
        #region Fields

        #endregion

        #region Public methods

        //private SerializedObject serializedstage;

        public override void OnBodyGUI()
        {
            if (buttonStyle == null)
                buttonStyle = GUIStyles.GetGUIStyle(0);

            SerializedObject.Update();

            var sn = target as StageNode;
            Undo.RecordObject(target, "inspector");

            var graph = sn.AiGraph;

            if (sn.stage == null) return;

            NodeEditorGUILayout.PortField(sn.GetInputPort("input"), GUILayout.Width(5));

            var expanded = HandleExpandedButton(sn, sn.stage);

            //if (!expanded) return; 

            for (var i = 0; i < sn.stage.utilities.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500));

                var utility = sn.stage.utilities[i];
                if (utility == null)
                {
                    sn.stage.utilities.RemoveAt(i);
                    break;
                }

                Undo.RecordObject(utility, "inspector");

                buttonStyle = GUIHelpers.GuiStyle(0);
                // for debugg vis:
                foreach (var graphWinner in graph.winners)
                {
//                    if (graphWinner.guid == utility.guid)
                    if (graphWinner.GetInstanceID() == utility.GetInstanceID())
                    {
                        buttonStyle = GUIHelpers.GuiDebugStyle(0);
                        break;
                    }
                }

                var c = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0, 0, 0, 0);
                GUILayout.Box(GUIHelpers.utilityIcon(), GUILayout.Height(20), GUILayout.Width(20));
                GUI.backgroundColor = c;

                var utlBtnStg = utility.Name;
                // if this utility is prefab, make text for it blue
                if (!utility.AiGraph.IsRuntimeDebugGraph && !Application.isPlaying)
                    if (PrefabUtility.IsAnyPrefabInstanceRoot(utility.gameObject))
                        buttonStyle = GUIHelpers.GuiStyle(3);
                //utlBtnStg += " (prefab)";

                if (graph.IsRuntimeDebugGraph) utlBtnStg += $" ({Math.Round(utility.lastScore, 2)})";

                var width = 166;
                //if (graph.isRuntimeDebugGraph) width = 184;

                c = GUI.contentColor;
                if (!utility.Enabled) GUI.contentColor = Color.gray;

                if (GUILayout.Button(utlBtnStg, buttonStyle, GUILayout.Width(width)))
                {
                    Selection.objects = new Object[] {sn};
                    SetSelectedElementRetarded(utility, sn);
                }

                GUI.contentColor = c;

                GUILayout.Space(5);
                buttonStyle = GUIHelpers.GuiStyle(0);

                // adding task button
                //if (!graph.isRuntimeDebugGraph)
                AddTaskButton(utility);

                // up down remove buttons for utilities
                GUIHelpers.UpDownRemove(
                    () =>
                    {
                        if (i < 1) return;
                        var nodePort = sn.Outputs.ElementAt(i).Connection;
                        var nodePortOther = sn.Outputs.ElementAt(i - 1).Connection;
                        sn.Outputs.ElementAt(i).ClearConnections();
                        sn.Outputs.ElementAt(i - 1).ClearConnections();
                        if (nodePortOther != null) sn.Outputs.ElementAt(i).Connect(nodePortOther);
                        if (nodePort != null) sn.Outputs.ElementAt(i - 1).Connect(nodePort);

                        sn.stage.utilities.Move(i, MoveDirection.Up);
                    },
                    () =>
                    {
                        if (i >= sn.stage.utilities.Count - 1) return;

                        var nodePort = sn.Outputs.ElementAt(i).Connection;
                        var nodePortOther = sn.Outputs.ElementAt(i + 1).Connection;
                        sn.Outputs.ElementAt(i).ClearConnections();
                        sn.Outputs.ElementAt(i + 1).ClearConnections();
                        if (nodePortOther != null) sn.Outputs.ElementAt(i).Connect(nodePortOther);
                        if (nodePort != null) sn.Outputs.ElementAt(i + 1).Connect(nodePort);

                        sn.stage.utilities.Move(i, MoveDirection.Down);
                    },
                    () =>
                    {
                        Undo.RegisterCompleteObjectUndo(sn.stage, "inspector");
                        for (var j = i; j < sn.stage.utilities.Count; j++)
                        {
                            sn.Outputs.ElementAt(j).ClearConnections();

                            if (j >= sn.stage.utilities.Count - 1) break;

                            var nodePort = sn.Outputs.ElementAt(j + 1).Connection;
                            if (nodePort == null) continue;
                            sn.Outputs.ElementAt(j).Connect(nodePort);
                        }

                        if (utility.Destroy())
                            sn.stage.utilities.Remove(utility);
                    }, !utility.CanBeRemoved());

                NodeEditorGUILayout.PortField(sn.Outputs.ElementAt(i), GUILayout.Width(-5));

                EditorGUILayout.EndHorizontal();

                if (expanded) DrawTasks(utility, sn, utility.tasks);

                if (expanded) EditorGUILayout.Separator();
                //EditorGUILayout.Separator();
            }

            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            PrefabUtility.RecordPrefabInstancePropertyModifications(sn.stage);
            SerializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Not public methods

        public override void OnHeaderGUI()
        {
            var s = target as StageNode;
            if (s.stage == null)
            {
                GUILayout.Label(s.GetType().Name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
                return;
            }

            var c = GUI.contentColor;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(s.gameObject))
                GUI.contentColor = GUIHelpers.GuiStyle(3).normal.textColor;

            var nejm = "";

            if (string.IsNullOrEmpty(s.Name))
                nejm = s.stage.GetType().Name;
            else
                nejm = s.Name;

            if (s.IsRoot) nejm += " (ROOT)";

            GUILayout.Label(nejm, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));

            GUI.contentColor = c;
        }

        #endregion
    }
}