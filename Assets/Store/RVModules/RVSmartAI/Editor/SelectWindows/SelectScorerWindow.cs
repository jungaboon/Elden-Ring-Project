// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using RVModules.RVSmartAI.GraphElements;

namespace RVModules.RVSmartAI.Editor.SelectWindows
{
    public class SelectScorerWindow : SelectWindowBase
    {
        #region Properties

        protected override Type GetTypes() => typeof(AiScorer);

        public override string Title => "Select AiScorer";

        #endregion

        //protected override Type GetWindowType() => GetType();
    }

//    public class SelectScorerParamsWindow : SelectWindowBase<AiScorerParams>
//    {
//        protected override string Title => "Select scorer";
//    }
}