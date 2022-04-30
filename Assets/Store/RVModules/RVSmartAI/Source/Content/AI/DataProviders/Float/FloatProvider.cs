// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public abstract class FloatProvider : DataProvider<float>
    {
        #region Public methods

        private Func<float> getData;

        public static implicit operator float(FloatProvider _floatProvider) => _floatProvider.getData();

        [ConditionalHide("float", hideInInspector = true)]
        [SerializeField]
        [Tooltip("Applies only if you add child float provider. Cached at initialization, so runtime changes will be ignored")]
        protected ScorerType @operator;

        [SerializeField]
        [OptionalDataProvider]
        [Tooltip(
            "Additional float operation, allows for more advanced calculations.\nEquation: first is this(parent) value then (parent)operator then child float value.\n" +
            "More than one level of depth is not recommended" +
            " as it work in recursive way so children are calculated first which can be confusing and produce unexpected results in order-" +
            "dependent equation")]
        protected FloatProvider @float;

        protected virtual void Awake()
        {
            if (@float == null)
            {
                getData = GetData;
            }
            else
            {
                switch (@operator)
                {
                    case ScorerType.Add:
                        getData = () =>
                        {
                            lastValue = GetData() + @float;
                            return lastValue;
                        };
                        break;
                    case ScorerType.Subtract:
                        getData = () =>
                        {
                            lastValue = GetData() - @float;
                            return lastValue;
                        };
                        break;
                    case ScorerType.Multiply:
                        getData = () =>
                        {
                            lastValue = GetData() * @float;
                            return lastValue;
                        };
                        break;
                    case ScorerType.Divide:
                        getData = () =>
                        {
                            lastValue = GetData() / @float;
                            return lastValue;
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #endregion
    }
}