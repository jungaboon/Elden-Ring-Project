// Created by Ronis Vision. All rights reserved
// 19.03.2021.

using System;
using System.Collections;
using System.Collections.Generic;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Utilities;
using UnityEngine;
using XNode;

namespace RVModules.RVSmartAI.Nodes
{
    
    /// <summary>
    /// Simple yes/no type node with one scorer and always two output ports - true if scored 0 and false otherwise 
    /// </summary>
    [CreateNodeMenu("Create condition node")] [Serializable]
    public class ConditionNode : SmartAiNode
    {
        #region Fields

        [SerializeField]
        public AiScorer scorer;

        public List<AiTask> trueTasks = new List<AiTask>();
        public List<AiTask> falseTasks = new List<AiTask>();

        /// <summary>
        /// for internal use
        /// </summary>
        [HideInInspector]
        public int lastWinner = -1;

        /// <summary>
        /// for internal use
        /// </summary>
        [HideInInspector]
        public bool addingFalseTask;

        [Input]
        public AiUtility input;

        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output1;

        [Output(connectionType: ConnectionType.Override)]
        public AiUtility output2;

        #endregion

        #region Inherited members 

        public override IList ChildGraphElements
        {
            get
            {
                var arr = new ArrayList();
                if (scorer != null) arr.Add(scorer);

                arr.AddRange(trueTasks);
                arr.AddRange(falseTasks);
                return arr;
            }
        }

        public override void AssignSubSelement(IAiGraphElement _aiGraphElement)
        {
            var aiScorer = _aiGraphElement as AiScorer;
            var aiTask = _aiGraphElement as AiTask;
            var aiUtility = _aiGraphElement as AiUtility;

            if (aiScorer != null)
            {
                if (scorer != null) DestroyImmediate(scorer.gameObject);

                scorer = aiScorer;
            }
            else if (aiTask != null)
            {
                if (addingFalseTask)
                    falseTasks.Add(aiTask);
                else
                    trueTasks.Add(aiTask);
            }
            else if (aiUtility != null)
            {
                foreach (var aiUtilityTask in aiUtility.tasks)
                {
                    if (aiUtilityTask == null) continue;
                    aiUtilityTask.transform.SetParent(transform);
                    if (addingFalseTask)
                        falseTasks.Add(aiUtilityTask);
                    else
                        trueTasks.Add(aiUtilityTask);
                }
                DestroyImmediate(aiUtility.gameObject);
            }
            else return;

            _aiGraphElement.AiGraph = AiGraph;
            if (!Application.isPlaying) return;
            _aiGraphElement.Context = Context;
        }

        public override void RemoveSubElement(IAiGraphElement _aiGraphElement, bool _destroyGameObject)
        {
            if (scorer == _aiGraphElement) RemoveElement(_aiGraphElement, _destroyGameObject);

            if (_aiGraphElement is AiTask task)
            {
                if (trueTasks.Contains(task))
                {
                    RemoveElement(_aiGraphElement, _destroyGameObject);
                    trueTasks.Remove(task);
                }

                if (falseTasks.Contains(task))
                {
                    RemoveElement(_aiGraphElement, _destroyGameObject);
                    falseTasks.Remove(task);
                }
            }
        }

        public override IAiGraphElement[] GetAllGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            if (scorer != null) list.Add(scorer);

            foreach (var ge in trueTasks) list.AddRange(ge.GetAllGraphElements());
            foreach (var ge in falseTasks) list.AddRange(ge.GetAllGraphElements());

            return list.ToArray();
        }

        public override IAiGraphElement[] GetChildGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            if (scorer != null) list.Add(scorer);

            list.AddRange(trueTasks);
            list.AddRange(falseTasks);

            return list.ToArray();
        }

        public override Type[] GetAssignableSubElementTypes() => new[] {typeof(AiScorer), typeof(AiTask), typeof(AiUtility)};

        public override int OutputsCount => 2;

        #endregion
    }
}