// Created by Ronis Vision. All rights reserved
// 28.06.2020.

using System;

namespace RVModules.RVLoadBalancer.Tasks
{
    public abstract class LoadBalancedTaskWrapper
    {
        private ILoadBalancedTask task;
        private Action onTaskStart;
        private Action onTaskFinish;

        public ILoadBalancedTask Task => task;

        protected abstract void TaskUpdate(float _dt);
        protected abstract void OnTaskStart();
        protected abstract void OnTaskFinish();


        public LoadBalancedTaskWrapper(int _priority = 0, string _name = "")
        {
            task = new LoadBalancedTask(TaskUpdate, OnTaskStart, OnTaskFinish, _priority, _name);
        }

        public static ILoadBalancedTask ToILoadBalancedTask(LoadBalancedTaskWrapper task) => task.Task;
    }
}