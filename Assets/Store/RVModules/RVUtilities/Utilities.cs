// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RVModules.RVUtilities
{
    public static class Utilities
    {
        #region Fields

        public static Color RonisVisionOrange = new Color(0.984f, 0.690f, 0.250f);

        #endregion

        #region Public methods

        /// <summary>
        /// Returns time it took for _action, in miliseconds
        /// </summary>
        /// <param name="_action"></param>
        /// <returns></returns>
        public static int MeasureMs(Action _action)
        {
            var sw = Stopwatch.StartNew();
            _action.Invoke();
            return (int) sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void LogMs(Action _action, string _msgBeforeMs = "") => Debug.Log(_msgBeforeMs + MeasureMs(_action) + "ms");


        public static string GetAfter(this string input, string _after)
        {
            var index = input.IndexOf(_after);
            var output = "";
            if (index > 0)
                output = input.Substring(0, index);
            return output;
        }

        #endregion

        // 251, 176, 64
    }
}