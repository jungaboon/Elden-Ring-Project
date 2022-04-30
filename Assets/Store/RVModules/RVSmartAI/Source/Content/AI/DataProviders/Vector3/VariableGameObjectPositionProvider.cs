// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableGameObjectPositionProvider : Vector3Provider
    {
        #region Fields

        [SerializeField]
        private string gameObjectName;

        #endregion

        #region Not public methods

        protected override Vector3 ProvideData()
        {
            var go = AiGraph.GraphAiVariables.GetUnityObjectAs<GameObject>(gameObjectName);
            return go.transform.position;
        }

        #endregion
    }
}