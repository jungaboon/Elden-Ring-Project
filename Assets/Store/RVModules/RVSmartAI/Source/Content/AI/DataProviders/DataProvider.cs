// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    /// <summary>
    /// Provides data for any AiGraphElement or other DataProvider.
    /// Context for data providers is updated automatically
    /// </summary>
    /// <typeparam name="T">Type of provided data</typeparam>
    public abstract class DataProvider<T> : DataProviderBase
    {
        #region Fields

        /// <summary>
        /// For debug purposes. It will be showed in inspector at runtime only
        /// </summary>
        [SerializeField]
        protected T lastValue;

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public T GetData()
        {
            lastValue = ProvideData();
            return lastValue;
        }

        #endregion

        #region Not public methods

        protected abstract T ProvideData();

        /// <summary>
        /// Helper method to use instead of casting to your context type 
        /// </summary>
        protected TExpectedContext ContextAs<TExpectedContext>() where TExpectedContext : class => Context as TExpectedContext;

        #endregion
    }
}