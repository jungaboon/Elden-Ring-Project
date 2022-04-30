// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    /// <summary>
    /// Returns IHealth.Health multiplied by score
    /// Required context: IHealth
    /// </summary>
    public class HealthScorer : AiScorer
    {
        #region Properties

        protected override string DefaultDescription => "Returns IHealth.Health multiplied by score.\nRequired context: IHealth";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => GetComponentFromContext<IHitPoints>().HitPoints * score;

        #endregion
    }
}