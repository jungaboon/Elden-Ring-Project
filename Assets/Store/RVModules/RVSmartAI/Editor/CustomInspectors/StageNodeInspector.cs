// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System.Collections.Generic;
using System.Linq;
using RVModules.RVSmartAI.Editor.SelectWindows;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Utilities;
using RVModules.RVSmartAI.Nodes;
using RVModules.RVUtilities.Editor;
using RVModules.RVUtilities.Extensions;
using RVModules.RVUtilities.Reflection;
using UnityEditor;
using UnityEngine;

namespace RVModules.RVSmartAI.Editor.CustomInspectors
{
    [CustomEditor(typeof(StageNode), true)]
    public class StageNodeInspector : SmartAiNodeInspector
    {
        #region Fields

        #endregion

        #region Public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, "inspector");

            var sn = target as StageNode;
            Undo.RecordObject(sn.stage, "inspector");

            if (sn.selectedElement != null)
            {
                DrawSpecificElementInspector(sn.selectedElement);
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                PrefabUtility.RecordPrefabInstancePropertyModifications(sn.stage);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            GUINameAndDesc(sn);

            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            PrefabUtility.RecordPrefabInstancePropertyModifications(sn.stage);
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Not public methods

        private void DrawSpecificElementInspector(AiGraphElement _element)
        {
            Undo.RecordObject(_element, "inspector");
            var utility = _element as AiUtility;
            if (utility != null) AiUtilityInspector(utility);

            var task = _element as AiTask;
            if (task != null) AiTaskInspector(task);
        }

        private void AiUtilityInspector(AiUtility _utility)
        {
            GUIHelpers.GUIDrawNameAndDescription(_utility, _utility.ToString(), _utility.Name, _utility.Description, out var name, out var desc);
            _utility.Name = name;
            _utility.Description = desc;

            GUIHelpers.GUIDrawFields(_utility);

            if (_utility is FixedScoreAiUtility)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            var scorers = _utility.scorers;
            var graph = _utility.AiGraph;
            var parentGraphElement = _utility as AiGraphElement;

            DrawScorers(graph, parentGraphElement, scorers);

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}