// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content.AI.DataProviders;

namespace RVHonorAI.Content.AI.DataProviders
{
    /// <summary>
    /// Provides ITarget
    /// Required context: ITargetProvider
    /// </summary>
    public class TargetProvider : DataProvider<ITarget>
    {
        #region Not public methods

        protected override ITarget ProvideData() => ContextAs<ITargetProvider>().Target;

        #endregion
    }
}