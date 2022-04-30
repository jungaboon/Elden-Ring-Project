// Created by Ronis Vision. All rights reserved
// 29.06.2020.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVLoadBalancer.Tasks
{
    public partial class TaskHandler
    {
        /// <summary>
        /// for debugging in unity inspector only
        /// </summary>
        [SerializeField]
        private List<LoadBalancedTaskInfo> runningTasksInfo = new List<LoadBalancedTaskInfo>();

        /// <summary>
        /// for debugging in unity inspector only
        /// </summary>
        [SerializeField]
        private List<LoadBalancedTaskInfo> queueTasksInfo = new List<LoadBalancedTaskInfo>();

        private void UpdateDebugData()
        {
            runningTasksInfo.Clear();
            queueTasksInfo.Clear();

            foreach (var t in RunningTasks)
            {
                runningTasksInfo.Add(new LoadBalancedTaskInfo(t.Name, t.IsRunning, t.Priority, t.Layer));
            }

            foreach (var t in Queue)
            {
                queueTasksInfo.Add(new LoadBalancedTaskInfo(t.Name, t.IsRunning, t.Priority, t.Layer));
            }
        }

        [Serializable] private class LoadBalancedTaskInfo
        {
            public string name;
            public bool isRunning;
            public int priority;
            public string layer;

            public LoadBalancedTaskInfo(string name, bool isRunning, int priority, string layer)
            {
                this.name = name;
                this.isRunning = isRunning;
                this.priority = priority;
                this.layer = layer;
            }
        }
    }
}