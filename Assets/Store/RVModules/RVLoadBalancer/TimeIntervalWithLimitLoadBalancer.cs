// Created by Ronis Vision. All rights reserved
// 26.02.2021.

using System;
using UnityEngine;

namespace RVModules.RVLoadBalancer
{
    public class TimeIntervalWithLimitLoadBalancer : TimeIntervalLoadBalancer
    {
        private int ticksPerPhaseLimit;

        public TimeIntervalWithLimitLoadBalancer(int _ticksPerInterval = 3, float _intervalTime = 5, int _ticksPerPhaseLimit = int.MaxValue,
            bool _calculateDt = false, bool _useUnscaledDeltaTime = false) : base(_ticksPerInterval, _intervalTime, _calculateDt, _useUnscaledDeltaTime)
        {
            ticksPerPhaseLimit = _ticksPerPhaseLimit;
        }

        protected override void CalculateTickablesToUpdatePerPhase()
        {
            tickablesToUpdatePerPhase = Actions.Count * ticksPerInterval;
            if (tickablesToUpdatePerPhase > ticksPerPhaseLimit)
            {
                // clamp ticks per phase to set limit
                tickablesToUpdatePerPhase = ticksPerPhaseLimit;
                
                // minimum guaranteed update frequency per action in interval
                // BUT also guarantee minimum calls per action
//                var minimumFrequencyPerActionInPhase = 1.0f;
//                var lowerLimit = Actions.Count * minimumFrequencyPerActionInPhase;
//                if (lowerLimit > ticksPerPhaseLimit)
//                {
//                    tickablesToUpdatePerPhase = lowerLimit;
//                }
            }
        }
    }
}