// Created by Ronis Vision. All rights reserved
// 04.07.2020.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Utilities;
using UnityEngine;
using XNode;

namespace RVModules.RVSmartAI.Nodes
{
    [CreateNodeMenu("Create graph node")] [Serializable]
    public class GraphNode : SmartAiNode
    {
        // on enable
        protected override void Init()
        {
        }

        public override IContext Context
        {
            get => graphReference?.Context;
            set => graphReference?.UpdateContext(value);
        }

        public override int OutputsCount => 0;

        public override object GetValue(NodePort port) => null;

        [Input]
        public AiUtility input;

        //[Output(connectionType: ConnectionType.Override)]
        //public AiUtility output1;

        public AiGraph graphReference;

        public override void AssignSubSelement(IAiGraphElement _aiGraphElement)
        {
        }

        public override void RemoveSubElement(IAiGraphElement _aiGraphElement, bool _destroyGameObject)
        {
        }

        public override void RemoveAllSubElements(bool _destroyGameObjects)
        {
        }

        public override IList ChildGraphElements => new List<IList>();

        public override IAiGraphElement[] GetAllGraphElements() => new IAiGraphElement[] {this};

        public override IAiGraphElement[] GetChildGraphElements() => new IAiGraphElement[0];

        public override Type[] GetAssignableSubElementTypes() => new Type[0];
    }
}