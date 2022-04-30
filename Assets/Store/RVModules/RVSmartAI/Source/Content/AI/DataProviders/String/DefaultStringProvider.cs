// Created by Ronis Vision. All rights reserved
// 25.01.2021.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class DefaultStringProvider : StringProvider
    {
        [SerializeField]
        private string value;

        protected override string ProvideData() => value;
    }
}