// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableVector3ListProvider : Vector3ListProvider
    {
        #region Fields

        [SerializeField]
        private string vector3ListName;

        #endregion

        #region Not public methods

        protected override List<Vector3> ProvideData() => aiGraph.GraphAiVariables.GetVector3List(vector3ListName);

        #endregion
    }
}