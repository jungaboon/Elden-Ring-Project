// Created by Ronis Vision. All rights reserved
// 17.08.2019.

using System;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers
{
    [DefaultExecutionOrder(200)] public abstract class GetProperty : AiScorer, IForceUpdate
    {
        #region Fields


        public string property;

        private Func<object, object> propertyGetter;

        protected object GetPropertyValue => propertyGetter(Context);

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            if (propertyGetter != null) return;
            propertyGetter = Helpers.BuildPropertyGetter(Context, property);
        }

        #endregion

        public void ForceUpdate() => propertyGetter = Helpers.BuildPropertyGetter(Context, property);
    }
}