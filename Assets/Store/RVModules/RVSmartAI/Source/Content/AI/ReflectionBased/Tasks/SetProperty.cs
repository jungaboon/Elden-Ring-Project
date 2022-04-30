// Created by Ronis Vision. All rights reserved
// 31.12.2019.

using System;
using RVModules.RVSmartAI.GraphElements;

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Tasks
{
    public abstract class SetProperty : AiTask, IForceUpdate
    {
        #region Fields


        public string property;

        private Action<object, object> propertySetter;

        #endregion

        #region Properties

        protected abstract object Value { get; }

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            propertySetter = Helpers.BuildPropertySetter(Context, property);
        }

        protected override void Execute(float _deltaTime) => propertySetter.Invoke(Context, Value);

        #endregion

        public void ForceUpdate() => propertySetter = Helpers.BuildPropertySetter(Context, property);
    }
}