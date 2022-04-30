// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RVModules.RVSmartAI.GraphElements.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable] public abstract class AiUtility : AiGraphElement
    {
        #region Fields

//        [HideInInspector]
//        public string guid;

        [SmartAiHideInInspector]
        public List<AiScorer> scorers = new List<AiScorer>();

        /// <summary>
        /// for debugging only
        /// </summary>
        [SmartAiHideInInspector]
        public float lastScore;

        [SmartAiHideInInspector]
        public List<AiTask> tasks = new List<AiTask>();

        #endregion

        #region Properties

        public override IList ChildGraphElements
        {
            get
            {
                var list = new List<IAiGraphElement>();
                list.AddRange(scorers);
                list.AddRange(tasks);
                return list;
            }
        }

        #endregion

        #region Public methods

        public override Type[] GetAssignableSubElementTypes() => new[] {typeof(AiScorer), typeof(AiTask)};

        public override void RemoveAllSubElements(bool _destroyGos)
        {
            // needs to have copy so we wont modify collection we foreach on
            var children = ChildGraphElements.Cast<object>().ToArray();
            foreach (var childGraphElement in children)
            {
                if (scorers.Contains(childGraphElement)) scorers.Remove((AiScorer) childGraphElement);
                if (tasks.Contains(childGraphElement)) tasks.Remove((AiTask) childGraphElement);
                RemoveSubElement((IAiGraphElement) childGraphElement, _destroyGos);
            }
        }

        public sealed override IAiGraphElement[] GetAllGraphElements()
        {
            var list = new List<IAiGraphElement> {this};

            tasks = tasks.Where(_t => _t != null).ToList();
            foreach (var aiTask in tasks) list.AddRange(aiTask.GetAllGraphElements());

            scorers = scorers.Where(_s => _s != null).ToList();
            foreach (var aiScorer in scorers)
                list.AddRange(aiScorer.GetAllGraphElements());

            return list.ToArray();
        }

        public sealed override IAiGraphElement[] GetChildGraphElements()
        {
            var list = new List<IAiGraphElement> {this};

            // return all tasks
            tasks = tasks.Where(_t => _t != null).ToList();
            foreach (var aiTask in tasks) list.Add(aiTask);

            // and scorers
            scorers = scorers.Where(_s => _s != null).ToList();
            foreach (var aiScorer in scorers)
                list.Add(aiScorer);

            return list.ToArray();
        }

        public override void RemoveNulls()
        {
            base.RemoveNulls();
            scorers = scorers.Where(_scorer => _scorer != null).ToList();
            tasks = tasks.Where(_task => _task != null).ToList();
        }

        public override void AssignSubSelement(IAiGraphElement _aiGraphElement)
        {
            var t = _aiGraphElement as AiTask;
            if (t != null)
            {
                AssignTask(t);
                base.AssignSubSelement(_aiGraphElement);
                return;
            }

            var s = _aiGraphElement as AiScorer;
            if (s != null)
            {
                AssignScorer(s);
                base.AssignSubSelement(_aiGraphElement);
            }
        }

        public abstract float Score(float _deltaTime);

        #endregion

        #region Not public methods

        protected virtual void AssignScorer(AiScorer _scorer)
        {
            _scorer.AiGraph = AiGraph;
            scorers.Add(_scorer);
        }

        protected virtual void AssignTask(AiTask _task)
        {
            _task.AiGraph = AiGraph;
            tasks.Add(_task);
        }

        #endregion

#if UNITY_EDITOR
        private void Awake() => Reset();

        public void Reset()
        {
            if (string.IsNullOrEmpty(Name))
                Name = ObjectNames.NicifyVariableName(ToString());
            //if (guid == null)
//            guid = Guid.NewGuid().ToString();
        }
#endif
    }
}