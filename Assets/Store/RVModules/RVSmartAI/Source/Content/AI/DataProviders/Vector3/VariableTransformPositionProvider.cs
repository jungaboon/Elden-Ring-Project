// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableTransformPositionProvider : Vector3Provider
    {
        #region Fields

        [SerializeField]
        private string transformName;

        #endregion

        #region Not public methods

        protected override Vector3 ProvideData()
        {
            var t = AiGraph.GraphAiVariables.GetUnityObjectAs<Transform>(transformName);
            return t.position;
        }

        #endregion
    }
}