// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RVModules.RVSmartAI.GraphElements.Utilities;

namespace RVModules.RVSmartAI.GraphElements.Stages
{
    /// <summary>
    /// Stages are main part of AIGraph. They represent stage/state/situation in consideration. Their job is to select utility based on stage implementation.
    /// E.g HighestWinsStage(most common), selects highest scoring utility. If selected utility has connection to other stage, AI goes into this
    /// stage and this process loops until some stage select utility which has no connection to other stage. Then it goes to the root stage, and
    /// whole cycle starts from beginning.
    /// </summary>
    [Serializable] public abstract class Stage : AiGraphElement
    {
        #region Fields

        public List<AiUtility> utilities = new List<AiUtility>();

        #endregion

        #region Properties

        public override IList ChildGraphElements => utilities;

        #endregion

        #region Public methods

        public override string ToString() => GetType().Name;

        public override void AssignSubSelement(IAiGraphElement _aiGraphElement)
        {
            var aiUtility = _aiGraphElement as AiUtility;
            if (aiUtility == null) return;
            aiUtility.AiGraph = AiGraph;
//            aiUtility.guid = Guid.NewGuid().ToString();
            utilities.Add(aiUtility);
            base.AssignSubSelement(_aiGraphElement);
        }

        public override Type[] GetAssignableSubElementTypes() => new[] {typeof(AiUtility)};

        public override IAiGraphElement[] GetAllGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            utilities = utilities.Where(_u => _u != null).ToList();

            foreach (var aiUtility in utilities)
                list.AddRange(aiUtility.GetAllGraphElements());

            return list.ToArray();
        }

        public override IAiGraphElement[] GetChildGraphElements()
        {
            var list = new List<IAiGraphElement> {this};
            list.AddRange(utilities);
            return list.ToArray();
        }

        public abstract AiUtility Select(float _deltaTime);

        #endregion

        #region Not public methods

//        private void OnDestroy()
//        {
//            foreach (var childGraphElement in GetChildGraphElements()) Destroy(childGraphElement.gameObject);
//        }

        #endregion
    }
}