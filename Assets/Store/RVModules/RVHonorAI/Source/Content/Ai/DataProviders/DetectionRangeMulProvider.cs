// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVHonorAI.Content.AI.DataProviders
{
    public class DetectionRangeMulProvider : FloatProvider
    {
        #region Fields

        [SerializeField]
        private FloatProvider rangeMultiplier;

        #endregion

        #region Not public methods

        protected override float ProvideData() => ContextAs<IDetectionRangeProvider>().DetectionRange * rangeMultiplier;

        #endregion
    }
}