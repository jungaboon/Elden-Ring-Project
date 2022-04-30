// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Scorers;

namespace RVHonorAI.Content.AI.Scorers
{
    /// <summary>
    /// Returns health percentage multiplied by score 
    /// </summary>
    public class HealthPercent : AiScorerCurve
    {
        #region Fields

        private IHitPoints health;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => GetScoreFromCurve(health.HitPoints / health.MaxHitPoints);

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            health = GetComponentFromContext<IHitPoints>();
        }

        #endregion
    }
}