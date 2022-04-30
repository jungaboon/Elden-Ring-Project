// Created by Ronis Vision. All rights reserved
// 06.04.2021.

using System;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class OptionalDataProvider : Attribute
    {
        /// <summary>
        /// Won't create default DP automatically
        /// </summary>
        public OptionalDataProvider()
        {
        }
    }
}