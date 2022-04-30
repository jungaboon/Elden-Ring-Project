// Created by Ronis Vision. All rights reserved
// 13.10.2020.

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class MoveTargetIsNullScorer : AiAgentScorer
    {
        #region Fields

        public bool not;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime)
        {
            if (not && MoveTarget != null) return score;
            if (!not && MoveTarget == null) return score;
            return 0;
        }

        #endregion
    }
}