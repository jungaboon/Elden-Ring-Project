// Created by Ronis Vision. All rights reserved
// 13.10.2020.

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Returns set score if AIAgent is at destination or if it isn't, depending on 'not' field
    /// </summary>
    public class AtDestination : AiAgentScorer
    {
        #region Fields

        public bool not;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime)
        {
            if (!not && movement.AtDestination) return score;
            if (not && !movement.AtDestination) return score;

            return 0;
        }

        #endregion
    }
}