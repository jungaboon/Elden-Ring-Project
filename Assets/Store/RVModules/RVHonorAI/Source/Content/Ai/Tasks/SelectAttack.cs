// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVHonorAI.Content.AI.Tasks
{
    public class SelectAttack : AiTask
    {
        #region Fields

        [SerializeField]
        [Header("Time interval between AiTask execution, in seconds")]
        private FloatProvider callInterval;

        private float lastCallTime;

        private IAttacker attacker;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            attacker = GetComponentFromContext<IAttacker>();
        }

        protected override void Execute(float _deltaTime)
        {
            if (UnityTime.Time - lastCallTime < callInterval) return;
            lastCallTime = UnityTime.Time;

            attacker.SelectAttack();
        }

        #endregion
    }
}