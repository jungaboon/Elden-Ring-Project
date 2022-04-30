// Created by Ronis Vision. All rights reserved
// 05.11.2019.

using UnityEngine;

namespace RVModules.RVLoadBalancer
{
    /// <summary>
    /// Updates setted percentage of Tickables per tick
    /// Does not guarantee any particular call frequency with low object numbers, but it's fastest type of lb
    /// </summary>
    public sealed class PercentageLoadBalancer : LoadBalancer
    {
        #region Fields

        private float percentPerTick;

        #endregion

        #region Properties

        public float PercentPerTick
        {
            get { return percentPerTick; }
            set { percentPerTick = Mathf.Clamp(value, 1f, 100f); }
        }

        #endregion

        public PercentageLoadBalancer(float _percentPerTick, bool _calculateDeltaTime, bool _useUnscaledDeltaTime = false) : base(_calculateDeltaTime,
            _useUnscaledDeltaTime)
        {
            percentPerTick = _percentPerTick;
        }

        #region Public methods

        public override void Tick(float _deltaTime)
        {
            if (Actions.Count == 0) return;
            time += _deltaTime;

            for (var i = 0; i < Actions.Count * PercentPerTick * 0.01f; i++)
            {
                if (indexToTick >= Actions.Count) indexToTick = 0;
                InvokeAction();
                indexToTick++;
            }
        }

        #endregion
    }
}