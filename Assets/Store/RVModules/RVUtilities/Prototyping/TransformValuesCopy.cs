// Created by Ronis Vision. All rights reserved
// 21.02.2017.

using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVUtilities.Prototyping
{
    public static class TransformValuesCopy
    {
        #region Public methods

        public static void Work(Transform objToCopyFrom, Transform objToPasteTo)
        {
            var sourceTransforms = objToCopyFrom.GetTransformsRecursive();
            var targetTransforms = objToPasteTo.GetTransformsRecursive();

            for (int i = 0; i < targetTransforms.Count; i++)
            {
                targetTransforms[i].localPosition = sourceTransforms[i].localPosition;
                targetTransforms[i].localRotation = sourceTransforms[i].localRotation;
            }
        }

        #endregion
    }
}