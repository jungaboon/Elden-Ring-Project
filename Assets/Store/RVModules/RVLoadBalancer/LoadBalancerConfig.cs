// Created by Ronis Vision. All rights reserved
// 16.12.2019.

using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace RVModules.RVLoadBalancer
{
    [Serializable] public struct LoadBalancerConfig
    {
        public string name;
        public LoadBalancerType loadBalancerType;

        [FormerlySerializedAs("value")]
        public int valueX;

        public int valueY;
        
        public bool calculateDeltaTime;
        public bool useUnscaledDeltaTime;
        public bool dontRemoveWhenEmpty;

        // custom equality comparer removes garbage creation in dictionaries operations on LBC
        private struct LoadBalancerConfigEqualityComparer : IEqualityComparer<LoadBalancerConfig>
        {
            public bool Equals(LoadBalancerConfig x, LoadBalancerConfig y)
            {
                return x.loadBalancerType == y.loadBalancerType && x.valueX == y.valueX && x.valueY == y.valueY &&
                       x.calculateDeltaTime == y.calculateDeltaTime &&
                       x.useUnscaledDeltaTime == y.useUnscaledDeltaTime && x.dontRemoveWhenEmpty == y.dontRemoveWhenEmpty && x.name == y.name;
            }

            public int GetHashCode(LoadBalancerConfig obj) => 0;
        }

        public static IEqualityComparer<LoadBalancerConfig> LoadBalancerConfigComparer { get; } = new LoadBalancerConfigEqualityComparer();

        /// <summary>
        /// todo pool load balancers
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal LoadBalancer GetLoadBalancer()
        {
            LoadBalancer lb = null;

            switch (loadBalancerType)
            {
                case LoadBalancerType.EveryXFrames:
                    lb = new EveryxFramesLoadBalancer(valueX, calculateDeltaTime, useUnscaledDeltaTime);
                    break;
                case LoadBalancerType.XtimesPerSecond:
                    lb = new TimeIntervalLoadBalancer(valueX, 1, calculateDeltaTime, useUnscaledDeltaTime);
                    break;
                case LoadBalancerType.PercentagePerFrame:
                    lb = new PercentageLoadBalancer(valueX, calculateDeltaTime, useUnscaledDeltaTime);
                    break;
                case LoadBalancerType.FixedNumberPerFrame:
                    lb = new FixedNumberLoadBalancer(valueX, calculateDeltaTime, useUnscaledDeltaTime);
                    break;
                case LoadBalancerType.XtimesPerSecondWithYLimit:
                    lb = new TimeIntervalWithLimitLoadBalancer(valueX, 1, valueY, calculateDeltaTime, useUnscaledDeltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return lb;
        }

        public LoadBalancerConfig(LoadBalancerType _loadBalancerType, int _valueX, bool _calculateDeltaTime = false, bool _useUnscaledDeltaTime = false,
            int _valueY = 0)
        {
            loadBalancerType = _loadBalancerType;
            valueX = _valueX;
            valueY = _valueY;
            calculateDeltaTime = _calculateDeltaTime;
            name = "";
            dontRemoveWhenEmpty = true;
            useUnscaledDeltaTime = _useUnscaledDeltaTime;
        }

        public LoadBalancerConfig(string _name, LoadBalancerType _loadBalancerType, int _valueX, bool _calculateDeltaTime = false,
            bool _useUnscaledDeltaTime = false, int _valueY = 0)
        {
            name = _name;
            loadBalancerType = _loadBalancerType;
            valueX = _valueX;
            valueY = _valueY;
            calculateDeltaTime = _calculateDeltaTime;
            dontRemoveWhenEmpty = true;
            useUnscaledDeltaTime = _useUnscaledDeltaTime;
        }

        public override string ToString()
        {
            var s = $"{name} -{loadBalancerType} (x{valueX}) (y{valueY})";
            if (calculateDeltaTime) s += " -dt";
            if (useUnscaledDeltaTime) s += " -unscaled";
            if (dontRemoveWhenEmpty) s += " -dontRemoveEmpty";
            return s;
        }
    }

    public enum LoadBalancerType
    {
        /// <summary>
        /// Updates all elements every X frames
        /// </summary>
        EveryXFrames,

        /// <summary>
        /// Updates X percent of elements every tick
        /// </summary>
        PercentagePerFrame,

        /// <summary>
        /// Updates all elements X times per second
        /// </summary>
        XtimesPerSecond,

        /// <summary>
        /// Update X elements every tick
        /// </summary>
        FixedNumberPerFrame,

        /// <summary>
        /// Updates all elements X times per second, with Y updates per second limit 
        /// </summary>
        XtimesPerSecondWithYLimit,
    }
}