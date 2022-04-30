// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;

namespace RVHonorAI.Content.AI.DataProviders
{
    public class DetectionRangeProvider : FloatProvider
    {
        #region Fields

        private IDetectionRangeProvider detectionRangeProvider;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => detectionRangeProvider = ContextAs<IDetectionRangeProvider>();

        protected override float ProvideData() => detectionRangeProvider.DetectionRange;

        #endregion
    }
}