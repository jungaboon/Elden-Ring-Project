// Created by Ronis Vision. All rights reserved
// 24.06.2020.

namespace RVModules.RVLoadBalancer.Tasks
{
    public abstract class LoadBalancedTaskMono : LoadBalancedTaskMonoBase, I<DummyMonoBehaviour>, I<LoadBalancedTaskWrapper>
    {
        private void Awake()
        {
        }

        protected virtual void OnDestroy()
        {
            Task.FinishTask();
        }
    }
}