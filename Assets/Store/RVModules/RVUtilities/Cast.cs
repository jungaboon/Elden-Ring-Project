// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// Helper class for casting data
    /// </summary>
    public static class Cast
    {
        #region Public methods

        /// <summary> 
        /// Use if you dont want to use if null pattern after casting
        /// </summary>
        /// <returns></returns>
        public static T Try<T>(object _dataToConvert, Action<T> onSuccess = null) where T : class
        {
            var convertedData = _dataToConvert as T;
            if (convertedData != null) onSuccess?.Invoke(convertedData);
            return convertedData;
        }

        #endregion
    }
}