// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using RVModules.RVSmartAI.Content;

namespace RVModules.RVSmartAI.GraphElements
{
    /// <summary>
    /// for internal use only
    /// </summary>
    public interface IAiScorer : IAiGraphElement
    {
        #region Properties

        ScorerType ScorerType { get; }

        #endregion

        #region Public methods

        float Score_(object _parameter);
        float ScoreConvertibleParams(object _parameter);

        #endregion
    }
}