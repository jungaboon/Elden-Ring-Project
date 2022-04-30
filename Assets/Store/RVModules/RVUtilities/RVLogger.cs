// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable] public class RvLogger
    {
        #region Fields

        public static bool globalDebugMode = true;

        /// <summary>
        /// Enables info, log and warning logs
        /// </summary>
        public bool debugMode = true;

        public RvLoggerLogLevel logLevel = RvLoggerLogLevel.Warning;

        public Dictionary<string, bool> logGroups = new Dictionary<string, bool>();

        [SerializeField]
        private string name = "RVLogger";

        private Dictionary<string, MeasureLog> measureLogs = new Dictionary<string, MeasureLog>();

        #endregion

        #region Properties

        public string Name => name;

        #endregion

        public RvLogger()
        {
        }

        public RvLogger(string _name = null, RvLoggerLogLevel _logLevel = RvLoggerLogLevel.Warning)
        {
            if (!string.IsNullOrEmpty(_name))
                name = _name;
            logLevel = _logLevel;
        }

        #region Public methods

        public void StartMeasure(string _message, Object _context = null, RvLoggerLogLevel _logLevel = RvLoggerLogLevel.Info, string _logGroup = null,
            string _measureGroup = "", bool _logAtStart = true)
        {
            if (measureLogs.TryGetValue(_measureGroup, out var ml))
                StopMeasure();

            if (_logAtStart) Log($"{_message}...", _context, _logLevel, _logGroup);

            ml = new MeasureLog(Stopwatch.StartNew(), _message, _context, _logLevel, _logGroup);
            measureLogs.Add(_measureGroup, ml);
        }

        public void StopMeasure(string _measureGroup = "")
        {
            if (!measureLogs.TryGetValue(_measureGroup, out var ml)) return;

            Log($"{ml.tsMsg} done: {ml.sw.ElapsedMilliseconds}ms", ml.tsContext, ml.tsLogLevel, ml.tslogGroup);
            ml.sw.Stop();
            measureLogs.Remove(_measureGroup);
        }

        public void Log(object _message, Object _context = null, RvLoggerLogLevel _logLevel = RvLoggerLogLevel.Info, string _logGroup = null)
        {
            if (!globalDebugMode) return;
            if (!debugMode || logLevel > _logLevel) return;
            if (IsLogGroupDisabled(_logGroup)) return;

            var msg = $"{name}: {_message}";
            switch (_logLevel)
            {
                case RvLoggerLogLevel.Info:
                    Debug.Log(msg, _context);
                    break;
                case RvLoggerLogLevel.Log:
                    Debug.Log(msg, _context);
                    break;
                case RvLoggerLogLevel.Warning:
                    Debug.LogWarning(msg, _context);
                    break;
                case RvLoggerLogLevel.Error:
                    Debug.LogError(msg, _context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_logLevel), _logLevel, null);
            }
        }

        public void LogInfo(object _message, Object _context = null, string _logGroup = null) => Log(_message, _context, RvLoggerLogLevel.Info, _logGroup);

        public void LogWarning(object _message, Object _context = null, string _logGroup = null) =>
            Log(_message, _context, RvLoggerLogLevel.Warning, _logGroup);

        public void LogError(object _message, Object _context = null, string _logGroup = null) => Log(_message, _context, RvLoggerLogLevel.Error, _logGroup);

        #endregion

        #region Not public methods

        protected void Log(object _message, Object _context = null, string _logGroup = null) => Log(_message, _context, RvLoggerLogLevel.Log, _logGroup);

        private bool IsLogGroupDisabled(string _logGroup)
        {
            if (_logGroup == null) return false;
            // if user provided group that isnt in logGroups dictionary, treat it as disabled group
            if (!logGroups.TryGetValue(_logGroup, out var logGroupEnabled)) return true;
            return !logGroupEnabled;
        }

        #endregion

        private class MeasureLog
        {
            #region Fields

            public Stopwatch sw;
            public string tsMsg;
            public Object tsContext;
            public RvLoggerLogLevel tsLogLevel;
            public string tslogGroup;

            #endregion

            public MeasureLog(Stopwatch _sw, string _tsMsg, Object _tsContext, RvLoggerLogLevel _tsLogLevel, string _tslogGroup)
            {
                sw = _sw;
                tsMsg = _tsMsg;
                tsContext = _tsContext;
                tsLogLevel = _tsLogLevel;
                tslogGroup = _tslogGroup;
            }
        }
    }

    public enum RvLoggerLogLevel
    {
        Info,
        Log,
        Warning,
        Error
    }
}