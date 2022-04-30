// Created by Ronis Vision. All rights reserved
// 19.03.2021.

using System;
using System.Collections;
using System.Collections.Generic;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Stages;
using RVModules.RVSmartAI.GraphElements.Utilities;
using UnityEngine;
using XNode;

namespace RVModules.RVSmartAI.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable] public abstract class StageNode : SmartAiNode
    {
        #region Fields

        public Stage stage;

        // workaround for clear instance ports issue
        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output1;

        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output2;

        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output3;

        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output4;

        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output5;

        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output6;

        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output7;

        [Input]
        public AiUtility input;

        #endregion

        #region Properties

        public override int OutputsCount => stage.utilities.Count;

        // this is not needed here, but it's IAiGraphElement member 
        public override IContext Context
        {
            get => stage.Context;
            set => stage.Context = value;
        }

        public override IList ChildGraphElements => stage.ChildGraphElements;

        #endregion

        #region Public methods

        public override void AssignSubSelement(IAiGraphElement _aiGraphElement)
        {
            var s = _aiGraphElement as Stage;
            if (s != null)
            {
                if (stage != null) DestroyImmediate(stage.gameObject);
                stage = s;
                return;
            }

            stage.AssignSubSelement(_aiGraphElement);
            if (!Application.isPlaying) return;
            _aiGraphElement.Context = Context;
        }

        public override void RemoveSubElement(IAiGraphElement _aiGraphElement, bool _destroyGameObject) =>
            stage.RemoveSubElement(_aiGraphElement, _destroyGameObject);

        public override void RemoveAllSubElements(bool _destroyGameObjects) => stage.RemoveAllSubElements(_destroyGameObjects);

        public override Type[] GetAssignableSubElementTypes() => stage.GetAssignableSubElementTypes();

        public override IAiGraphElement[] GetAllGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            list.AddRange(stage.GetAllGraphElements());
            return list.ToArray();
        }

        public override IAiGraphElement[] GetChildGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            list.AddRange(stage.GetChildGraphElements());
            return list.ToArray();
        }

        public void PassGraphReferenceToAllElements()
        {
            stage.AiGraph = AiGraph;
            foreach (var allGraphElement in stage.GetAllGraphElements())
                allGraphElement.AiGraph = AiGraph;
        }

        #endregion

        public int GetIndexOfUtility(AiUtility _utility)
        {
            for (var i = 0; i < stage.utilities.Count; i++)
            {
                var stageUtility = stage.utilities[i];
                if (stageUtility == _utility) return i;
            }

            return -1;
        }

        #region Not public methods

        // called by OnEnable in MonoNode
        protected override void Init()
        {
            base.Init();
#if UNITY_EDITOR
            if (stage == null) stage = CreateStage(AiGraph);
#endif
            PassGraphReferenceToAllElements();

            // if there is no stage it means deserialization failed!
            //if (stage == null) return;
#if UNITY_EDITOR
            if (stage.utilities.Count == 0)
            {
                AddFixedScoreUtility();
                stage.utilities[0].Name = "Default utility";
            }
#endif
        }

#if UNITY_EDITOR
        protected abstract Stage CreateStage(AiGraph _aiGraph);
#endif

        #endregion

#if UNITY_EDITOR
        [ContextMenu("Add sum utility")]
        public void AddSumUtility()
        {
            AiGraph = graph as AiGraph;
            var s = (SumAiUtility) AiGraph.CreateNewElement(typeof(SumAiUtility), this);
            stage.AssignSubSelement(s);
        }

        [ContextMenu("Add all or nothing utility")]
        public void AddAllOrNothingUtility()
        {
            AiGraph = graph as AiGraph;
            var s = (AllOrNothingAiUtility) AiGraph.CreateNewElement(typeof(AllOrNothingAiUtility), this);
            stage.AssignSubSelement(s);
        }

        [ContextMenu("Add fixed score utility")]
        public void AddFixedScoreUtility()
        {
            AiGraph = graph as AiGraph;
            var s = (FixedScoreAiUtility) AiGraph.CreateNewElement(typeof(FixedScoreAiUtility), this);
            stage.AssignSubSelement(s);
        }
#endif
    }
}