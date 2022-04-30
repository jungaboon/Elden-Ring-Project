// Created by Ronis Vision. All rights reserved
// 27.06.2020.

using System;
using UnityEngine;
using RVModules.RVLoadBalancer;

namespace RVModules.RVLoadBalancer.Tasks
{
    /// <summary>
    /// Representation task that will take more than one method call to finish.
    /// Use TaskHandler to manage them, or use directly
    /// </summary>
    [Serializable] public class LoadBalancedTask : ILoadBalancedTask
    {
        #region Fields

        [SerializeField]
        protected string name;

        private Action onTaskStart;
        private Action onTaskFinish;

        [SerializeField]
        protected string layer;

        [SerializeField]
        protected int priority;

        protected Action<float> action;

        [SerializeField]
        private bool taskRunning;

        private LoadBalancerConfig loadBalancerConfig;

        #endregion

        #region Properties

        public bool IsRunning => taskRunning;

        public Action<float> Action => action;

        public string Name => name;

        public int Priority => priority;

        public Action OnTaskStart
        {
            get => onTaskStart;
            set => onTaskStart = value;
        }

        public Action OnTaskFinish
        {
            get => onTaskFinish;
            set => onTaskFinish = value;
        }

        public Action<ILoadBalancedTask> OnTaskFinishInternal { get; set; }

        public string Layer
        {
            get => layer;
            set => layer = value;
        }

        public LoadBalancerConfig BalancerConfig => loadBalancerConfig;

        #endregion

        public LoadBalancedTask(Action<float> _action, Action _onTaskStart = null, Action _onTaskFinish = null, int _priority = 0, string _name = "")
        {
            action = _action;
            OnTaskStart = _onTaskStart;
            OnTaskFinish = _onTaskFinish;
            priority = _priority;
            SetName(_name);
        }

        #region Public methods

        /// <summary>
        /// Task will run once per frame
        /// </summary>
        public void StartTask() => StartTaskInternal(new LoadBalancerConfig(LoadBalancerType.FixedNumberPerFrame, 1));

        public void StartTask(int frequency) => StartTaskInternal(new LoadBalancerConfig(LoadBalancerType.XtimesPerSecond, frequency));

        public void StartTask(LoadBalancerConfig balancerConfig) => StartTaskInternal(balancerConfig);

        private void StartTaskInternal(LoadBalancerConfig balancerConfig)
        {
            if (taskRunning) return;
            UpdateLoadBalancerConfig(balancerConfig);
            taskRunning = true;
            OnTaskStart?.Invoke();
        }

        public void UpdateLoadBalancerConfig(LoadBalancerConfig _balancerConfig)
        {
            loadBalancerConfig = _balancerConfig;
            LoadBalancerSingleton.Instance.Register(this, action, _balancerConfig);
        }

        public void FinishTask()
        {
            if (!taskRunning) return;
            LB.Unregister(this);
            taskRunning = false;
            OnTaskFinish?.Invoke();
            OnTaskFinishInternal?.Invoke(this);
        }

        #endregion

        #region Not public methods

        protected void SetName(string _name)
        {
            if (!string.IsNullOrEmpty(name)) return;
            name = string.IsNullOrEmpty(_name) ? Action?.ToString() : _name;
        }

        #endregion
    }
}