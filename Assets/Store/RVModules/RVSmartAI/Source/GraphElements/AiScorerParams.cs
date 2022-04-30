// Created by Ronis Vision. All rights reserved
// 29.03.2021.

using System;
using RVHonorAI;
using RVModules.RVSmartAI.Content;
using UnityEngine;

namespace RVModules.RVSmartAI.GraphElements
{
    [Serializable] public abstract class AiScorerParams<T> : AiGraphElement, IAiScorer
    {
        #region Fields

        public ScorerType scorerType;

        public float score = 1;

        private Type myGenericType;

        #endregion

        #region Properties

        public ScorerType ScorerType => scorerType;

        #endregion

        #region Public methods

        public float ScoreConvertibleParams(object _parameter)
        {
            if (_parameter is IConvertibleParam<T> convertibleParam)
            {
                if (convertibleParam.MyType == myGenericType) return Score((T) _parameter);
                return Score(convertibleParam.Convert());
//                return Score((T) TypeDescriptor.GetConverter(_parameter).ConvertTo(_parameter, myGenericType));
            }

            return Score((T) _parameter);
        }

        public float Score_(object _parameter) => Score((T) _parameter);

        #endregion

        #region Not public methods

        private void Awake()
        {
            myGenericType = typeof(T);
        }

        protected abstract float Score(T _parameter);

        #endregion
    }
}