// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RVModules.RVSmartAI.Content;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.GraphElements
{
    /// <summary>
    /// It differs from basic task in that it have generic type, and operates on array of objects of that type. 
    /// It have GetBest method that evaluates array of parameters you have to provide using ScorerParams, which are also generic type.
    /// Those scorers must have matching type for AiTaskParam generic type, and that way you can use different scorers
    /// to differently score each element of mentioned array of parameters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AiTaskParams<T> : AiTask, IAiTaskParams
    {
        #region Fields

        [Tooltip("Set to true if any assigned scorers param will need to be converted using IConvertibleParam. Otherwise you'll get InvalidCastException")]
        [SerializeField]
        public bool convertibleParams;

        [SmartAiHideInInspector]
        public List<AiGraphElement> scorers = new List<AiGraphElement>();

//        [HideInInspector]
//        public bool showParamsDebug;

        public bool debugValues;

        // parameters for load balanced scoring
        private List<T> lbParameters = new List<T>();

        private T highestScoredParameter;
        private int currentlyCheckedParameterId;
        private float highestScoreDelayed = float.MinValue;

        public List<TaskParamsDebugData> lastParams = new List<TaskParamsDebugData>();

        [SerializeField]
        private bool drawParamsDebug;

        public List<TaskParamsDebugData> LastParams => lastParams;

        public bool DrawParamsDebug
        {
            get => drawParamsDebug;
            set => drawParamsDebug = value;
        }

        #endregion

        #region Properties

        public override IList ChildGraphElements => scorers;

        #endregion

        protected AiTaskParams() => scorers = scorers.Where(_s => _s != null).ToList();

        #region Public methods

        public override void RemoveNulls()
        {
            base.RemoveNulls();
            scorers = scorers.Where(_scorer => _scorer != null).ToList();
        }

        public override Type[] GetAssignableSubElementTypes() => new[] {typeof(IAiScorer), typeof(AiScorerParams<T>)};

        public override IAiGraphElement[] GetChildGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            list.AddRange(scorers);
            return list.ToArray();
        }

        public override IAiGraphElement[] GetAllGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            foreach (var aiGraphElement in scorers)
            {
                if (aiGraphElement == null) continue;
                list.AddRange(aiGraphElement.GetAllGraphElements());
            }

            return list.ToArray();
        }

        public override void AssignSubSelement(IAiGraphElement _aiGraphElement)
        {
            var s = _aiGraphElement as AiGraphElement;
            if (s == null)
            {
                Debug.LogError("Cannot assign null graph element!", this);
                base.AssignSubSelement(_aiGraphElement);
                return;
            }

            s.AiGraph = AiGraph;
            scorers.Add(s);
            base.AssignSubSelement(_aiGraphElement);
        }

        #endregion

        #region Not public methods

        List<object> IAiTaskParams.GetScorers() => scorers.Cast<object>().ToList();

        void IAiTaskParams.SetScorers(List<object> _scorers) => scorers = _scorers.Cast<AiGraphElement>().ToList();

        /// <summary>
        /// Returns best match using added scorers
        /// </summary>
        /// <param name="_parameters"></param>
        /// <returns></returns>
        protected virtual T GetBest(List<T> _parameters)
        {
            var highestScore = float.MinValue;
            T highestParam = default;

#if UNITY_EDITOR
            if (debugValues)
            {
                lastParams.Clear();
                foreach (var parameter in _parameters) lastParams.Add(new TaskParamsDebugData(parameter));
            }
#endif

            for (var i = 0; i < _parameters.Count; i++)
            {
                var parameter = _parameters[i];
                if (parameter == null) continue;
//                lastScores.Add(0);
                highestScore = HighestScore(parameter, highestScore, ref highestParam);
            }

            return highestParam;
        }

        /// <summary>
        /// For checking one parameter in one call, instead of all at once
        /// </summary>
        protected void SetParametersForExecuting(List<T> _parameters)
        {
            ResetRunningValues();
            lbParameters = _parameters;

#if UNITY_EDITOR
            if (debugValues)
            {
                lastParams.Clear();
                foreach (var parameter in _parameters) lastParams.Add(new TaskParamsDebugData(parameter));
            }
#endif
        }

        private void ResetRunningValues()
        {
            currentlyCheckedParameterId = 0;
            highestScoredParameter = default;
            highestScoreDelayed = float.MinValue;
        }

        /// <summary>
        /// Returns true when scored all parameters passed by SetParametersForExecuting, otherwise false,
        /// and <paramref name="bestParameter"/> will return default value
        /// </summary>
        /// <param name="bestParameter">When returned value was true this will be set to parameter with highest score</param>
        protected bool GetBestDelayed(out T bestParameter)
        {
            //bestParameter = default;
            if (lbParameters.Count == 0 || currentlyCheckedParameterId >= lbParameters.Count)
            {
                //ResetRunningValues();
                currentlyCheckedParameterId = 0;
                highestScoreDelayed = float.MinValue;
                bestParameter = highestScoredParameter;
                return true;
            }

            var parameter = lbParameters[currentlyCheckedParameterId];
            if (parameter == null)
            {
                currentlyCheckedParameterId++;
                return GetBestDelayed(out bestParameter);
            }

            highestScoreDelayed = HighestScore(parameter, highestScoreDelayed, ref highestScoredParameter);

            currentlyCheckedParameterId++;
            bestParameter = default;
            return false;
        }

        private float HighestScore(T parameter, float highestScore, ref T highestParam)
        {
            var unityObject = parameter as Object;
            if ((object) unityObject != null)
                if (unityObject == null)
                    return highestScore;

            float score = 0;
            for (var i = 0; i < scorers.Count; i++)
            {
                var scorer = scorers[i] as IAiScorer;
                if (scorer == null || !scorer.Enabled) continue;

                var s = convertibleParams ? scorer.ScoreConvertibleParams(parameter) : scorer.Score_(parameter);

#if UNITY_EDITOR
                if (debugValues) lastParams.First(_data => _data.param.Equals(parameter)).scorers.Add(scorer, s);
#endif

                switch (scorer.ScorerType)
                {
                    case ScorerType.Add:
                        score += s;
                        break;
                    case ScorerType.Subtract:
                        score -= s;
                        break;
                    case ScorerType.Multiply:
                        score *= s;
                        break;
                    case ScorerType.Divide:
                        score /= s;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
#if UNITY_EDITOR
            if (debugValues)
            {
                AiGraph.AddDebugValue(parameter, score);
                lastParams.First(_data => _data.param.Equals(parameter)).scoreSum = score;
            }
#endif

            if (!(score > highestScore)) return highestScore;
            highestParam = parameter;
            highestScore = score;
            return highestScore;
        }

        #endregion
    }

    public class TaskParamsDebugData
    {
        public object param;
        public Dictionary<object, float> scorers;
        public float scoreSum;

        public TaskParamsDebugData(object _param)
        {
            param = _param;
            scorers = new Dictionary<object, float>();
        }
    }
}