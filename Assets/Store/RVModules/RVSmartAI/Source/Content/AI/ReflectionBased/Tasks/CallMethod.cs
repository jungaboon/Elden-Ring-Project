using System;
using RVModules.RVSmartAI.GraphElements;

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Tasks
{
    public class CallMethod : AiTask, IForceUpdate
    {

        public string methodName;

        private Action<object> method;

        protected override void OnContextUpdated() => method = Helpers.BuildCallMethodAction(Context, methodName);

        protected override void Execute(float _deltaTime) => method.Invoke(Context);
        
        public void ForceUpdate() => method = Helpers.BuildCallMethodAction(Context, methodName);
    }
}