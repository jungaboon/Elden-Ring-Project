// Created by Ronis Vision. All rights reserved
// 18.03.2021.

using System.Collections.Generic;
using RVModules.RVSmartAI.Editor.CustomNodeEditors;
using RVModules.RVSmartAI.Editor.SelectWindows;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.Nodes;
using UnityEditor;
using UnityEngine;

namespace RVModules.RVSmartAI.Editor.CustomInspectors
{
    [CustomEditor(typeof(ConditionNode), false)]
    public class ConditionNodeInspector : SmartAiNodeInspector
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, "inspector");
            var node = target as ConditionNode;

            GUINameAndDesc(node);

            if (GUILayout.Button("Select scorer")) ConditionNodeEditor.SelectScorer(node);

            if (node.scorer != null) DrawScorer(node.AiGraph, node.scorer);

            if (node.selectedElement != null)
            {
                AiTaskInspector(node.selectedElement as AiTask);
            }

            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            serializedObject.ApplyModifiedProperties();
        }
    }
}