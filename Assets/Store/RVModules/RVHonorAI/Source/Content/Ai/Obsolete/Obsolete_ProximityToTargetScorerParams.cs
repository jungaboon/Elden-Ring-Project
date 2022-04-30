// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Obsolete
{
    /// <summary>
    /// 
    /// </summary>
    public class Obsolete_ProximityToTargetScorerParams : Obsolete_ProximityAiScorerParams
    {
        #region Fields

        private ITargetProvider targetProvider;

        #endregion

        #region Properties

        public override Vector3 PositionToMeasure => targetProvider.Target.Transform.position;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            targetProvider = Context as ITargetProvider;
        }

        #endregion
    }
}