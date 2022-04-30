// Created by Ronis Vision. All rights reserved
// 05.11.2019.

using UnityEngine;

namespace RVModules.RVLoadBalancer
{
    /// <summary>
    /// Updates setted number of Tickeables per tick
    /// </summary>
    public sealed class FixedNumberLoadBalancer : LoadBalancer
    {
        #region Fields

        private int quantityPerTick;

        #endregion

        #region Properties

        public int QuantityPerTick
        {
            get { return quantityPerTick; }
            set { quantityPerTick = Mathf.Clamp(value, 1, quantityPerTick + 1); }
        }

        #endregion

        public FixedNumberLoadBalancer(int _quantityPerTick, bool _calculateDeltaTime, bool _useUnscaledDeltaTime = false) : base(_calculateDeltaTime,
            _useUnscaledDeltaTime)
        {
            quantityPerTick = _quantityPerTick;
        }

        #region Public methods

        public override void Tick(float _deltaTime)
        {
            if (Actions.Count == 0) return;
            time += _deltaTime;

            for (var i = 0; i < quantityPerTick; i++)
            {
                if (indexToTick >= Actions.Count) indexToTick = 0;
                InvokeAction();
                indexToTick++;
            }
        }

        #endregion
    }
}