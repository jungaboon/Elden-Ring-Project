// Created by Ronis Vision. All rights reserved
// 18.03.2021.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using RVHonorAI;
using RVModules.RVSmartAI.Editor.SelectWindows;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.Nodes;
using RVModules.RVUtilities.Editor;
using RVModules.RVUtilities.Extensions;
using RVModules.RVUtilities.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.Editor.CustomInspectors
{
    public class SmartAiNodeInspector : UnityEditor.Editor
    {
        protected SerializedProperty nameProp;
        protected SerializedProperty descProp;

        private void OnEnable()
        {
            try
            {
                nameProp = serializedObject.FindProperty("_name");
                descProp = serializedObject.FindProperty("description");
            }
            catch
            {
            }
        }

        protected static void DrawScorers(AiGraph graph, IAiGraphElement parentGraphElement, List<AiScorer> scorers)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Scorers:");

            //if (!utility.AiGraph.isRuntimeDebugGraph)
            if (GUILayout.Button("+"))
            {
                var w = CreateInstance<SelectScorerWindow>();
                w.onSelectedItem = _type =>
                {
                    var ge = (IAiGraphElement) graph.CreateNewElement(_type, parentGraphElement);
                    Undo.RecordObject(parentGraphElement as Object, "inspector");
                    parentGraphElement.AssignSubSelement(ge);
                };
            }

            EditorGUILayout.EndHorizontal();

            for (var i = 0; i < scorers.Count; i++)
            {
                var scorer = scorers[i];
                if (scorer == null) continue;
                DrawScorer(graph, scorer, scorers, i);
            }
        }

        protected static void DrawScorer(AiGraph graph, AiScorer scorer, List<AiScorer> scorers = null, int i = 0)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            var scorerName = ObjectNames.NicifyVariableName(scorer.GetType().Name);
            if (graph.IsRuntimeDebugGraph) scorerName += $" ({scorer.lastScore})";

            EditorGUILayout.LabelField(scorerName);

            if (scorers != null)
            {
                GUIHelpers.UpDownRemove(
                    () => { scorers.Move(i, MoveDirection.Up); },
                    () => { scorers.Move(i, MoveDirection.Down); },
                    () =>
                    {
                        scorers.Remove(scorer);
                        scorer.Destroy();
                    }, !scorer.CanBeRemoved());
            }

            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(scorer.Description))
                //EditorGUILayout.LabelField("Description");
                //var sScorer = new SerializedObject(scorer);
                //EditorStyles.textField.wordWrap = true;
                // fucking unity never works as it sholud, ingores break lines... (!!!)
                EditorGUILayout.HelpBox(scorer.Description, MessageType.Info);
            //desc = EditorGUILayout.TextArea(descProp, GUILayout.MinHeight(50));

            GUIHelpers.GUIDrawFields(scorer);
            StageNodeInspector.ForceUpdateButton(scorer);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
        }

        protected static void DrawScorerParamsScores(TaskParamsDebugData _paramsDebugData, object _isolateScorer = null)
        {
            EditorHelpers.WrapInBox(() =>
            {
                var param = _paramsDebugData.param;
                float paramScore = _paramsDebugData.scoreSum;
                if (_isolateScorer != null)
                {
                    if (_paramsDebugData.scorers.ContainsKey(_isolateScorer)) paramScore = _paramsDebugData.scorers[_isolateScorer];
                }

                var unityComponent = (param as IUnityComponent)?.ToUnityComponent();

                EditorGUILayout.BeginHorizontal();

                if (unityComponent != null)
                {
                    EditorGUILayout.LabelField($"{unityComponent}");
                    EditorGUILayout.LabelField($" {paramScore}");

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

//                    if (GUILayout.Button("Highlight")) Highlighter.Highlight("Hierarchy", unityObject.ToString());
                    if (GUILayout.Button("Show"))
                    {
                        EditorGUIUtility.PingObject(unityComponent.gameObject);
                        if (SceneView.GetAllSceneCameras().Length > 0)
                            SceneView.GetAllSceneCameras()[0].transform.position =
                                unityComponent.transform.position - SceneView.GetAllSceneCameras()[0].transform.forward * 5;
                    }

                    if (GUILayout.Button("Select"))
                    {
                        var comp = unityComponent as Component;
                        Selection.objects = comp != null ? new Object[] {comp.gameObject} : new[] {unityComponent};
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField($"{param}");
                    EditorGUILayout.LabelField($" {paramScore}");
                    EditorGUILayout.EndHorizontal();
                }
            });
        }

        protected static void ForceUpdateButton(IAiGraphElement _graphElement)
        {
            if (_graphElement?.AiGraph == null) return;
            if (!_graphElement.AiGraph.IsRuntimeDebugGraph) return;
            if (!(_graphElement is IForceUpdate forceUpdate)) return;

            if (GUILayout.Button("Update")) forceUpdate.ForceUpdate();
        }

        protected void GUINameAndDesc(SmartAiNode sn)
        {
            EditorGUI.BeginChangeCheck();
            GUIHelpers.GUIDrawNameAndDescription(target, sn.GetType().Name, nameProp, descProp, out var desc);
            if (EditorGUI.EndChangeCheck()) sn.UpdateGameObjectName();
            sn.Description = desc;
        }

        protected void AiTaskInspector(AiTask _task)
        {
            GUIHelpers.GUIDrawNameAndDescription(_task, _task.ToString(), _task.Name, _task.Description, out var name, out var desc);
            _task.Name = name;
            _task.Description = desc;

            // custom action data
            GUIHelpers.GUIDrawFields(_task);

            ForceUpdateButton(_task);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (_task is IAiTaskParams)
                AiTaskParamsInspector(_task);
            else if (_task is ITaskWithScorers) DrawScorers(_task.AiGraph, _task, _task.taskScorers);
        }

        private static Type[] GetAssignableScorersParams(Type baseType)
        {
//            Debug.Log($"Getting derived for {baseType}, gen type {baseType.GetGenericArguments()[0]}");

            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            foreach (var type in assembly.GetTypes().Where(t =>
            {
                bool isGraphElement = typeof(IAiGraphElement).IsAssignableFrom(t);
                if (!isGraphElement) return false;
                bool isGeneric = t.BaseType != null && t.BaseType.IsGenericType;
                if (!isGeneric) return false;

                var genericBase = baseType.GetGenericArguments()[0];
                var genericArgument = t.BaseType.GetGenericArguments()[0];
                var assignable = (baseType.IsAssignableFrom(t) || genericBase.IsCastableTo(genericArgument)) && t.IsCastableTo(typeof(IAiScorer)); 
                                 //(TypeDescriptor.GetConverter(genericBase).CanConvertTo(genericArgument) &&
                                   //                               secondReqType.IsAssignableFrom(t.BaseType));

//                Debug.Log($"{genericBase.Name} assign to {t} gen {genericArgument.Name}: {assignable}");

                return assignable &&
                       !t.IsAbstract &&
                       t != baseType;
            }).ToArray())
                types.Add(type);
            return types.ToArray();
        }

        private void AiTaskParamsInspector(AiTask _task)
        {
            IAiTaskParams taskParams = _task as IAiTaskParams;
            var scorers = taskParams.GetScorers();

            EditorGUILayout.Separator();

            var lastParams = taskParams.LastParams.OrderByDescending(_data => _data.scoreSum).ToArray();

            if (_task.AiGraph.IsRuntimeDebugGraph)
            {
                var buttonTexture = GUIHelpers.DownButton();
                if (taskParams.DrawParamsDebug) buttonTexture = GUIHelpers.UpButton();

                if (GUILayout.Button(new GUIContent("Debug params", buttonTexture))) taskParams.DrawParamsDebug = !taskParams.DrawParamsDebug;

                if (taskParams.DrawParamsDebug)
                {
                    GUILayout.Label("Parameters score sums");
                    foreach (var taskParamsDebugData in lastParams)
                        DrawScorerParamsScores(taskParamsDebugData);
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            //scorers
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Scorers:");

            //if (!task.AiGraph.isRuntimeDebugGraph)
            if (GUILayout.Button("+", GUILayout.Width(100)))
            {
                // todo fix, properly detect AiScorerParams type in class hierarchy instead of assuming its one up
                var scorerType = typeof(AiScorerParams<>).MakeGenericType(_task.GetType().BaseType.GetGenericArguments()[0]);

                var okno = (SelectScorerWindow) CreateInstance(typeof(SelectScorerWindow));
//                okno.types = ReflectionHelper.GetDerivedTypes(scorerType);
                okno.types = GetAssignableScorersParams(scorerType);

////                 for reflection elements
//                var list = okno.types.ToList();
//                list.AddRange(ReflectionHelper.GetDerivedTypes(typeof(AiScorerParams<object>)));
//                okno.types = list.ToArray();

                okno.onSelectedItem = _type =>
                {
                    Undo.RecordObject(_task, "inspector");
                    scorers.Add(_task.AiGraph.CreateNewElement(_type, _task));
                    (_task as IAiTaskParams).SetScorers(scorers);
                };
            }

            EditorGUILayout.EndHorizontal();


            for (var i = 0; i < scorers.Count; i++)
            {
                var scorer = scorers[i];
                if (scorer == null) continue;
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(scorer.GetType().Name));

                GUIHelpers.UpDownRemove(
                    () =>
                    {
                        scorers.Move(i, MoveDirection.Up);
                        (_task as IAiTaskParams).SetScorers(scorers);
                    },
                    () =>
                    {
                        scorers.Move(i, MoveDirection.Down);
                        (_task as IAiTaskParams).SetScorers(scorers);
                    },
                    () =>
                    {
                        scorers.Remove(scorer);
                        (_task as IAiTaskParams).SetScorers(scorers);
                        (scorer as AiGraphElement).Destroy();
                    }, !(scorer as AiGraphElement).CanBeRemoved());

                EditorGUILayout.EndHorizontal();

                if (!string.IsNullOrEmpty((scorer as IAiGraphElement).Description))
                    //EditorGUILayout.LabelField("Description");
                    //var sScorer = new SerializedObject(scorer);
                    //EditorStyles.textField.wordWrap = true;
                    // fucking unity never works as it sholud, ingores break lines... (!!!)
                    EditorGUILayout.HelpBox((scorer as IAiGraphElement).Description, MessageType.Info);
                //desc = EditorGUILayout.TextArea(descProp, GUILayout.MinHeight(50));

                GUIHelpers.GUIDrawFields(scorer as AiGraphElement);
                ForceUpdateButton(scorer as AiGraphElement);
                if (!taskParams.DrawParamsDebug) EditorGUILayout.EndVertical();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                if (!taskParams.DrawParamsDebug) continue;

                lastParams = lastParams.OrderByDescending(_data => _data.scorers.ContainsKey(scorer) ? _data.scorers[scorer] : _data.scoreSum).ToArray();
                foreach (var taskParamsDebugData in lastParams) DrawScorerParamsScores(taskParamsDebugData, scorer);
                EditorGUILayout.EndVertical();
            }
        }
    }
}