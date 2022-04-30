// Created by Ronis Vision. All rights reserved
// 25.06.2020.

using System;

namespace RVModules.RVLoadBalancer.Tasks
{
    [Serializable] public abstract class LoadBalancedTaskBase : LoadBalancedTask
    {
        protected abstract void Task(float _dt);

        private void Initialization()
        {
            action = Task;
        }

        protected LoadBalancedTaskBase(Action _onTaskStart = null, Action _onTaskFinish = null, int _priority = 0, string _name = "") :
            base(null, _onTaskStart, _onTaskFinish, _priority, _name)
        {
            Initialization();
            SetName(_name);
        }
    }
}