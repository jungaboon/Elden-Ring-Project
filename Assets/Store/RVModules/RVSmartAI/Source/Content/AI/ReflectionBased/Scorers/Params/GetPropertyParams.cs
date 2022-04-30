// Created by Ronis Vision. All rights reserved
// 02.01.2020.

using System;
using System.Threading.Tasks;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers.Params
{
    /// <summary>
    /// Use threading to build property getter to avoid performance loss as for scorer params there is no way to know type T even at start of scene,
    /// it has to be figured out at runtime, at first use. For this reason first usage of GetPropertyValue will always return null.
    /// If you prefer immediate property getter build check 'immediatePropertyGetterCreation' to true
    /// </summary>
    [DefaultExecutionOrder(200)] public abstract class GetPropertyParams<T> : AiScorerParams<T>, IForceUpdate
    {
        #region Fields

        [Header("Check GetPropertyParams class to see explanation")]
        public bool immediatePropertyGetterCreation;

        public string property;

        private Func<object, object> propertyGetter;

        private bool createdGetter;
        private bool threadRun;

        #endregion

        #region Not public methods

        /// <summary>
        /// Can return null!
        /// </summary>
        protected object GetPropertyValue(T _param)
        {
            if (createdGetter) return propertyGetter(_param);
            if (threadRun) return null;

            void BuildPropertyGetter()
            {
                //var sw = Stopwatch.StartNew();
                propertyGetter = Helpers.BuildPropertyGetter(_param, property);
                createdGetter = true;
                //Debug.Log($"Built prop getter in {sw.ElapsedMilliseconds}ms");
            }

            if (immediatePropertyGetterCreation)
            {
                BuildPropertyGetter();
                return propertyGetter(_param);
            }

            Task.Run(BuildPropertyGetter);
            threadRun = true;
            return null;
        }

        /// <summary>
        /// Will force to build property getter with new property name on nest GetPropertyValue / refresher
        /// </summary>
        public void ForceUpdate()
        {
            createdGetter = false;
            threadRun = false;
        }

        #endregion
    }
}