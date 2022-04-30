// Created by Ronis Vision. All rights reserved
// 18.03.2021.

using System;
using System.Linq;
using RVModules.RVSmartAI.Editor.SelectWindows;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.Nodes;
using RVModules.RVUtilities;
using RVModules.RVUtilities.Editor;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.Editor.CustomNodeEditors
{
    [XNodeEditor.CustomEditor(typeof(ConditionNode))]
    public class ConditionNodeEditor : SmartAiNodeEditor
    {
        public override int GetWidth() => 240;

        public override void OnBodyGUI()
        {
            if (buttonStyle == null)
                buttonStyle = GUIStyles.GetGUIStyle(0);

            SerializedObject.Update();

            var node = target as SmartAiNode;
            var conditionNode = target as ConditionNode;

            Undo.RecordObject(target, "inspector");

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

//            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            NodeEditorGUILayout.PortField(node.GetInputPort("input"), GUILayout.Width(-5));

            GUILayout.Space(25);

            EditorGUILayout.LabelField(conditionNode.scorer?.ToString(), GUIHelpers.GuiStyle(0));
            
//            GUILayout.FlexibleSpace();
            HandleExpandedButton(conditionNode, conditionNode);
            GUILayout.EndHorizontal();

            var width = 66;

//            EditorGUILayout.EndHorizontal();

            var guiStyle = GUIHelpers.GuiStyle(0);
            if (node.AiGraph.winNodes.Contains(node) && conditionNode.lastWinner == 0) guiStyle = GUIHelpers.GuiDebugStyle(0);

            EditorGUILayout.BeginHorizontal();
            EditorHelpers.ChangeGuiColorsTemporarily(() =>
                    GUILayout.Box(GUIHelpers.utilityIcon(), GUILayout.Height(20), GUILayout.Width(20)),
                new GuiColorChange(GuiColorType.Background, new Color(0, 0, 0, 0)));
            if (GUILayout.Button("True", guiStyle, GUILayout.Width(width))) conditionNode.addingFalseTask = false;
            GUILayout.FlexibleSpace();
            AddTaskButton(node, () => conditionNode.addingFalseTask = false);
            NodeEditorGUILayout.PortField(node.Outputs.ElementAt(0), GUILayout.Width(-5));
            EditorGUILayout.EndHorizontal();
            if (node.expanded) DrawTasks(conditionNode, conditionNode, conditionNode.trueTasks);

            if (node.AiGraph.winNodes.Contains(node) && conditionNode.lastWinner == 1) guiStyle = GUIHelpers.GuiDebugStyle(0);
            else guiStyle = GUIHelpers.GuiStyle(0);

            EditorGUILayout.BeginHorizontal();
            EditorHelpers.ChangeGuiColorsTemporarily(() =>
                    GUILayout.Box(GUIHelpers.utilityIcon(), GUILayout.Height(20), GUILayout.Width(20)),
                new GuiColorChange(GuiColorType.Background, new Color(0, 0, 0, 0)));
            if (GUILayout.Button("False", guiStyle, GUILayout.Width(width))) conditionNode.addingFalseTask = true;
            GUILayout.FlexibleSpace();
            AddTaskButton(node, () => conditionNode.addingFalseTask = true);
            NodeEditorGUILayout.PortField(node.Outputs.ElementAt(1), GUILayout.Width(-5));
            EditorGUILayout.EndHorizontal();
            if (node.expanded) DrawTasks(conditionNode, conditionNode, conditionNode.falseTasks);

//            GUI.contentColor = c;

            GUILayout.Space(5);
            buttonStyle = GUIHelpers.GuiStyle(0);
        }

        // moved to inspector
//        protected override void AddCustomSmartAiContextMenuItems(GenericMenu _genericMenu)
//        {
//            _genericMenu.AddItem(new GUIContent("Select scorer"), false, SelectScorer);
//        }

        public static void SelectScorer(IAiGraphElement _node)
        {
            var e = ScriptableObject.CreateInstance<SelectScorerWindow>();
            e.onSelectedItem = _s =>
            {
                Undo.RecordObject(_node as Object, "inspector");
                // create it
                var newScorer = _node.AiGraph.CreateNewElement(_s, _node) as AiScorer;
                // assign
                _node.AssignSubSelement(newScorer);
            };
        }
    }
}