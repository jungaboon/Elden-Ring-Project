// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVModules.RVSmartAI.Editor.SelectWindows
{
    public class DataProvidersWindow : SelectWindowBase
    {
        #region Properties

        protected override Type GetTypes() => typeof(DataProviderBase);

        protected override string NameToDisplay(Type type) => base.NameToDisplay(type).Replace("Provider", "");

        #endregion
    }
}