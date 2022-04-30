// Created by Ronis Vision. All rights reserved
// 27.03.2021.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableObjectProvider : ObjectDataProvider
    {
        #region Fields

        [SerializeField]
        private string objectName;

        #endregion

        #region Not public methods

        protected override Object ProvideData() => AiGraph.GraphAiVariables.GetUnityObject(objectName);

        #endregion
    }
}