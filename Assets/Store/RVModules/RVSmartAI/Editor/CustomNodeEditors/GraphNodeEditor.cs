// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System.Linq;
using RVModules.RVSmartAI.Nodes;
using RVModules.RVUtilities;
using RVModules.RVUtilities.Editor;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace RVModules.RVSmartAI.Editor.CustomNodeEditors
{
    [XNodeEditor.CustomEditor(typeof(GraphNode))]
    public class GraphNodeEditor : NodeEditor
    {
        #region Fields

        private GUIStyle buttonStyle;

        #endregion

        #region Public methods

        //private SerializedObject serializedstage;
        public override int GetWidth() => 300;

        public override void AddContextMenuItems(GenericMenu menu)
        {
            var sn = target as GraphNode;

            var graph = sn.AiGraph;
            if (graph == null) return;

            // Custom sctions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is INode)
            {
                var node = Selection.activeObject as INode;
                //NodeEditorWindow.AddCustomContextMenuItems(menu, node);
                //menu.AddSeparator("");
                menu.AddItem(new GUIContent("Select corresponding game object"), false, SelectGoOfGraphElement);
            }
        }

        public override Color GetTint()
        {
            var s = target as GraphNode;
            var graph = s.AiGraph;
            if (graph == null) return base.GetTint();
            if (graph.lastNode == null) return base.GetTint();
            if (graph.lastNode == target)
                return Color.cyan;
            return base.GetTint();
        }

        public override void OnBodyGUI()
        {
            if (buttonStyle == null)
                buttonStyle = GUIStyles.GetGUIStyle(0);

            SerializedObject.Update();

            var graphNode = target as GraphNode;
            Undo.RecordObject(target, "inspector");

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            NodeEditorGUILayout.PortField(graphNode.GetInputPort("input"), GUILayout.Width(5));
            GUILayout.FlexibleSpace();
            //NodeEditorGUILayout.PortField(graphNode.Outputs.ElementAt(0), GUILayout.Width(-5));
            GUILayout.EndHorizontal();

            if (!Application.isPlaying)
            {
                EditorHelpers.ChangeGuiColorsTemporarily(
                    () => { EditorGUILayout.PropertyField(SerializedObject.FindProperty("graphReference"), new GUIContent("Graph"), true); },
                    new GuiColorChange(GuiColorType.Color, Color.white),
                    new GuiColorChange(GuiColorType.Background, Utilities.RonisVisionOrange),
                    new GuiColorChange(GuiColorType.LabelNormalText, Utilities.RonisVisionOrange));
            }
            else
            {
                if (graphNode.graphReference != null)
                    EditorHelpers.ChangeGuiColorsTemporarily(() =>
                        {
                            if (GUILayout.Button($"Enter {graphNode.graphReference.name} ->")) NodeEditorWindow.OpenWithGraph(graphNode.graphReference);
                        },
                        new GuiColorChange(GuiColorType.Background, Utilities.RonisVisionOrange));
            }
        }

        public override void OnHeaderGUI()
        {
            var s = target as GraphNode;
//            if (s.stage == null)
//            {
//                GUILayout.Label(s.GetType().Name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
//                return;
//            }

            var c = GUI.contentColor;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(s.gameObject))
                GUI.contentColor = GUIHelpers.GuiStyle(3).normal.textColor;

            string nejm;

            if (string.IsNullOrEmpty(s.Name))
                nejm = "Graph node";
            else
                nejm = s.Name;

            GUILayout.Label(nejm, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));

            GUI.contentColor = c;
        }

        #endregion

        #region Not public methods

        private void SelectGoOfGraphElement()
        {
            var sn = target as GraphNode;
            var aiGraph = sn.AiGraph;

            var selectedGraphElement = aiGraph.GetSelectedGraphElement();
            Selection.activeGameObject = selectedGraphElement.gameObject;
        }

        #endregion
    }
}