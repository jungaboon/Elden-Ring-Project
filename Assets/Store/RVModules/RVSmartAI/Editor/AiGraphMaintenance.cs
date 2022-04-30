// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Collections.Generic;
using System.Linq;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Stages;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.Editor
{
    /// <summary>
    /// It will fix unassigned(ge has children but are not assigned to him as sub ges) or moved(reparented) graph elements.
    /// It won't fix node/stage relationship as it's special case
    /// </summary>
    public static class AiGraphMaintenance
    {
        #region Public methods

        [MenuItem("RVSmartAI/Analyse selected AiGraph")]
        public static void GraphAnalyzer() => AnalyseGraph(false);

        [MenuItem("RVSmartAI/Analyse and fix selected AiGraph")]
        public static void GraphAnalyzerAndFix() => AnalyseGraph(true);

        #endregion

        #region Not public methods

        private static void AnalyseGraph(bool _fix)
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                Debug.Log("AiGraph analysis is available only in prefab mode");
                return;
            }

            var selectedGo = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;

            var graph = selectedGo.GetComponent<AiGraph>();
            if (graph == null)
            {
                Debug.Log("Open AiGraph prefab! Proper AiGraph should have AiGraph component on root game object");
                return;
            }

            Debug.Log($"Analysing graph {graph.name}...");

            var allAiGraphElements = selectedGo.GetComponentsInChildren<IAiGraphElement>();
            var foundIssues = 0;
            var fixedIssues = 0;

            var fixDatas = new List<AiGraphElementFixData>();
            var allReparents = new List<IAiGraphElement>();

            //////////// OBJECTS WITH NO GRAPH ELEMENTS ANALYSE ////////////////

            foundIssues = AnalyseObjectsWithNoGraphElements(_fix, graph, foundIssues, ref fixedIssues);

            //////////// REPARENTED GRAPH ELEMENTS ANALYSE ///////////

            foundIssues = AnalyseReparentedGraphElements(allAiGraphElements, fixDatas, allReparents, foundIssues);

            ////////// REPARENTED GRAPH ELEMENTS FIX ///////////

            if (_fix) fixedIssues = FixReparentedGraphElements(fixDatas, fixedIssues);

            ////////// UNASSIGNED GRAPH ELEMENTS ANALYSE ///////////

            foundIssues = AnalyzeUnassignedGraphElements(_fix, allAiGraphElements, fixDatas, allReparents, foundIssues);

            ////////// UNASSIGNED GRAPH ELEMENTS FIX ///////////

            if (_fix) fixedIssues = FixUnassignedGraphElements(fixDatas, graph, fixedIssues);

            if (foundIssues == 0)
            {
                Debug.Log("No issues found");
            }
            else
            {
                Debug.Log($"Found {foundIssues} issues");
                if (_fix)
                    Debug.Log($"Fixed {fixedIssues} issues. Because some issues were found, " +
                              "its recommended to analyse again to make sure they were sucessfully fixed");
            }
        }

        private static int FixUnassignedGraphElements(List<AiGraphElementFixData> _fixDatas, AiGraph _graph, int _fixedIssues)
        {
            // its important to have separate loops for fixing reparenting and assignment
            foreach (var fixData in _fixDatas)
            {
                fixData.graphElement.RemoveAllSubElements(false);

                foreach (var aiGraphElement in fixData.allSubElements)
                {
                    var ret = AiGraphEditor.ValidateAndAddToGraphNewElement(_graph, aiGraphElement.gameObject, fixData.graphElement, false);

                    if (!fixData.elementsToAssign.Contains(aiGraphElement)) continue;
                    if (ret != null)
                    {
                        Debug.Log($"Fixed unnasigned child graph element! {aiGraphElement} ", ret.gameObject);
                        _fixedIssues++;
                    }
                    else
                    {
                        Debug.Log($"Failed to fix unnasigned child graph element! {aiGraphElement} ", aiGraphElement.gameObject);
                    }
                }
            }

            return _fixedIssues;
        }

        private static int AnalyzeUnassignedGraphElements(bool _fix, IAiGraphElement[] _allAiGraphElements, List<AiGraphElementFixData> _fixDatas,
            List<IAiGraphElement> _allReparents, int _foundIssues)
        {
            foreach (var graphElement in _allAiGraphElements)
            {
                if (graphElement is Stage) continue;
                // tryna fix, even if number is ok, there still can be some desync (lets say one assigned is not as childre and children not assigned) 
                var fixData = new AiGraphElementFixData(graphElement);
                _fixDatas.Add(fixData);

                var parentForSubElements = graphElement.gameObject.transform;

                for (var j = 0; j < parentForSubElements.childCount; j++)
                {
                    var child = parentForSubElements.GetChild(j).GetComponent<IAiGraphElement>();

                    if (child == null) continue;

                    fixData.elementsParentedUnder.Add(child);

                    if (fixData.assignedChildElements != null)
                    {
                        if (child is Stage) continue;

                        if (!fixData.allSubElements.Contains(child)) fixData.allSubElements.Add(child);

                        // if something was earlier diagnosed as reparente, dont diagnose it as unassigned !
                        if (!fixData.assignedChildElements.Contains(child) && !_allReparents.Contains(child))
                        {
                            _foundIssues++;

                            var expectedTypes = fixData.graphElement.GetAssignableSubElementTypes();
                            if (!expectedTypes.Any(t => t.IsInstanceOfType(child)))
                            {
                                Debug.Log("Found unnasigned child graph element with wrong type! (cant be sub graph element of graph element it is parented" +
                                          "under. This can't be fixed automatically to avoid potential loss of data. Investigate this object by yourself.",
                                    child.gameObject);
                                fixData.allSubElements.Remove(child);
                            }
                            else
                            {
                                fixData.elementsToAssign.Add(child);
                                Debug.Log($"Found unnasigned child graph element! {child} ", child.gameObject);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"{graphElement} game object have child, when it shouldn't have one", child.gameObject);
                        _foundIssues++;
                        if (_fix) Debug.Log("Can't fix this issue automatically, check referenced object", child.gameObject);
                    }
                }
            }

            return _foundIssues;
        }

        private static int FixReparentedGraphElements(List<AiGraphElementFixData> _fixDatas, int _fixedIssues)
        {
            foreach (var fixData in _fixDatas)
            foreach (var aiGraphElement in fixData.elementsToReparent)
                try
                {
                    aiGraphElement.gameObject.transform.SetParent(fixData.graphElement.gameObject.transform);
                    Debug.Log($"Fixed assigned sub element that isn't child of assigned graph element! {fixData.graphElement} ",
                        fixData.graphElement.gameObject);
                    _fixedIssues++;
                }
                catch (Exception e)
                {
                    Debug.Log($"Failed to fix assigned sub element that isn't child of assigned graph element! {fixData.graphElement} : " + e,
                        fixData.graphElement.gameObject);
                }

            return _fixedIssues;
        }

        private static int AnalyseReparentedGraphElements(IAiGraphElement[] _allAiGraphElements, List<AiGraphElementFixData> _fixDatas,
            List<IAiGraphElement> _allReparents, int _foundIssues)
        {
            foreach (var graphElement in _allAiGraphElements)
            {
                if (graphElement is Stage) continue;

                var fixData = new AiGraphElementFixData(graphElement);
                _fixDatas.Add(fixData);

                var parentForSubElements = graphElement.gameObject.transform;

                for (var j = 0; j < parentForSubElements.childCount; j++)
                {
                    var child = parentForSubElements.GetChild(j).GetComponent<IAiGraphElement>();
                    fixData.elementsParentedUnder.Add(child);
                }

                if (fixData.assignedChildElements == null) continue;

                foreach (var assignedChildElement in fixData.assignedChildElements)
                    if (!fixData.elementsParentedUnder.Contains(assignedChildElement))
                    {
                        _allReparents.Add(assignedChildElement);

                        Debug.Log($"Found assigned sub element that isn't child of assigned graph element! {assignedChildElement} ",
                            assignedChildElement as Object);
                        _foundIssues++;
                        fixData.elementsToReparent.Add(assignedChildElement);
                    }
            }

            return _foundIssues;
        }

        private static int AnalyseObjectsWithNoGraphElements(bool _fix, AiGraph _graph, int _foundIssues, ref int _fixedIssues)
        {
            // loop over all graph transform to find objects without IAiGraphElement component, there should be none
            foreach (var childTransform in _graph.GetComponentsInChildren<Transform>())
            {
                // ignore ai graph transform as its not of type IAiGraphElement
                if (childTransform.GetComponent<AiGraph>() != null) continue;

                var graphElement = childTransform.GetComponent<IAiGraphElement>();

                // game object with no IAiGraphElements component on it
                if (graphElement != null) continue;

                _foundIssues++;

                Debug.Log("There is game object that haven't AiGraphElement component", childTransform);
                if (!_fix) continue;

                Debug.Log("There is no automatic fix for this issue to avoid data loss." +
                          "This can be caused by renaming graph element's class name or if it has different class name than file it is contained in." +
                          "If that's not the case check for referenced game object by clicking last log. You should remove it after making sure you won't lose any needed data.");
            }

            return _foundIssues;
        }

        #endregion

        private class AiGraphElementFixData
        {
            #region Fields

            public IList<IAiGraphElement> assignedChildElements;
            public List<IAiGraphElement> elementsParentedUnder = new List<IAiGraphElement>();

            public List<IAiGraphElement> elementsToAssign = new List<IAiGraphElement>();
            public List<IAiGraphElement> allSubElements = new List<IAiGraphElement>();
            public List<IAiGraphElement> elementsToReparent = new List<IAiGraphElement>();

            public IAiGraphElement graphElement;

            #endregion

            public AiGraphElementFixData(IAiGraphElement _graphElement)
            {
                graphElement = _graphElement;
                assignedChildElements = graphElement.ChildGraphElements?.Cast<IAiGraphElement>().Where(_element => _element != null).ToList();
            }
        }
    }
}