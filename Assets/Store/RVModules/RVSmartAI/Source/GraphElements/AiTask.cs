// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI.Content;
using UnityEngine;

namespace RVModules.RVSmartAI.GraphElements
{
    /// <summary>
    /// Base class for AiTask
    /// </summary>
    public abstract class AiTask : AiGraphElement
    {
        #region Fields

        /// <summary>
        /// for debugging only
        /// </summary>
        [HideInInspector]
        public float lastScore;

        [SmartAiHideInInspector]
        public List<AiScorer> taskScorers = new List<AiScorer>();

        #endregion

        #region Properties

        public override IList ChildGraphElements => taskScorers;

        #endregion

        #region Public methods

        public override Type[] GetAssignableSubElementTypes() => new[] {typeof(AiScorer)};

        public override IAiGraphElement[] GetChildGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            list.AddRange(taskScorers);
            return list.ToArray();
        }

        public override IAiGraphElement[] GetAllGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            foreach (var aiGraphElement in taskScorers) list.AddRange(aiGraphElement.GetAllGraphElements());

            return list.ToArray();
        }

        public override void RemoveNulls()
        {
            base.RemoveNulls();
            taskScorers = taskScorers.Where(_scorer => _scorer != null).ToList();
        }

        public override void AssignSubSelement(IAiGraphElement _aiGraphElement)
        {
            var s = _aiGraphElement as AiScorer;
            if (s == null)
            {
                Debug.LogError("Cannot assign null graph element!", this);
                base.AssignSubSelement(_aiGraphElement);
                return;
            }

            s.AiGraph = AiGraph;
            taskScorers.Add(s);
            base.AssignSubSelement(_aiGraphElement);
        }

        #endregion

        #region Not public methods

        /// <summary>
        /// Logic implementation of task
        /// </summary>
        protected abstract void Execute(float _deltaTime);

        internal void Exec(float _deltaTime) => Execute(_deltaTime);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Total score calculated from scorers assigned to this task</returns>
        protected float Score(float _deltaTime)
        {
            float score = 0;
            for (var i = 0; i < taskScorers.Count; i++)
            {
                var scorer = taskScorers[i];
                if (scorer == null || !scorer.Enabled) continue;
                var s = scorer.lastScore = scorer.Score(_deltaTime);
                switch (scorer.scorerType)
                {
                    case ScorerType.Add:
                        score += s;
                        break;
                    case ScorerType.Subtract:
                        score -= s;
                        break;
                    case ScorerType.Multiply:
                        score *= s;
                        break;
                    case ScorerType.Divide:
                        score /= s;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            lastScore = score;
            return score;
        }

        #endregion

        #region Experimental runningTasks feature

        /// <summary>
        /// Override if you want this task to be 'running' task - one that doens't ends immediately and can take more than one frame to execute
        /// </summary>
        public virtual bool IsRunningTask => false;

//        /// <summary>
//        /// If task will be running for longer than this value, error will be thrown
//        /// </summary>
//        protected virtual float MaxAllowedRunningTime => .1f;

        private Action<float> executeLb;

        /// <summary>
        /// Load balancer config for running task
        /// Default implementation returns default LoadBalancerConfig constructor (calls method every frame)
        /// </summary>
        protected virtual LoadBalancerConfig RunningTaskLbc => defaultLbc;

        private LoadBalancerConfig defaultLbc = new LoadBalancerConfig();

        /// <summary>
        /// Logic implementation of task that will take some time to execute and block further processing of graph until it's finished
        /// </summary>
        /// <param name="_deltaTime">Delta time. Note that you have to use LoadBalancerConfig with calculate delta time set to true to have this value,
        /// otherwise it will always return -1</param>
        /// <exception cref="NotImplementedException">Thrown if you return true in IsRunningTask, but didn't implemented this method</exception>
        protected virtual void Executing(float _deltaTime) => throw new NotImplementedException();

        /// <summary>
        /// Called every time task starts to execute. Relevant only for running tasks.
        /// Once it's called, graph execution will be stopped until you finish this task by calling StopExecuting()
        /// </summary>
        /// <returns>Return true if you want to start executing this task, if you return false, task won't start</returns>
        protected virtual bool StartExecuting() => true;

        /// <summary>
        /// Called in StopExecuting
        /// Event is nulled after every invokation
        /// Needed for graph executing getting back to specific task
        /// </summary>
        internal Action onStoppedExecuting;

        public bool IsExecuting { get; private set; }

        internal bool StartExecutingInternal()
        {
            if (executeLb == null) executeLb = Executing;
            if (!StartExecuting()) return false;
            // todo maybe register this as owner object !? that would be much nicer in load balancer debugger,
            // but also would create risk of user unregistering all actions of his owner object - that would also unregister all his graphs logic...
            // hmm solution could be to register to context maybe !?
            LB.Register(this, executeLb, RunningTaskLbc);
            IsExecuting = true;
            return true;
        }

        /// <summary>
        /// Immediately stops executing this running task and allows graph to move on
        /// </summary>
        protected void StopExecuting()
        {
            IsExecuting = false;
            LB.Unregister(this, executeLb);
            onStoppedExecuting?.Invoke();
//            onStoppedExecuting = null;
        }

        protected virtual void OnDestroy()
        {
            if (IsExecuting) StopExecuting();
        }

        #endregion
    }
}