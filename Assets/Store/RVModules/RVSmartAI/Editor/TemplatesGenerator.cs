// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace RVModules.RVSmartAI.Editor
{
    /// <summary>
    /// Generate template scripts
    /// </summary>
    public class TemplatesGenerator
    {
        #region Fields

        private static Texture2D scriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

        private static Dictionary<string, string> replaces = new Dictionary<string, string>();

        #endregion

        #region Public methods

        public static void CreateFromTemplate(string initialName, string templatePath) =>
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateCodeFile>(),
                initialName,
                scriptIcon,
                templatePath
            );

        #endregion

        #region Not public methods

        private static void CreateClass(string _name, string _filename)
        {
            var guids = AssetDatabase.FindAssets(_name);
            if (guids.Length == 0)
                return;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            CreateFromTemplate(_filename, path);
        }

        /// <summary>Creates Script from Template's path.</summary>
        internal static Object CreateScript(string pathName, string templatePath)
        {
            var className = Path.GetFileNameWithoutExtension(pathName).Replace(" ", string.Empty);
            var templateText = string.Empty;

            var encoding = new UTF8Encoding(true, false);

            if (File.Exists(templatePath))
            {
                /// Read procedures.
                var reader = new StreamReader(templatePath);
                templateText = reader.ReadToEnd();
                reader.Close();

                templateText = templateText.Replace("#SCRIPTNAME#", className);
                foreach (var replace in replaces)
                    templateText = templateText.Replace(replace.Key, replace.Value);

                /// Write procedures.
                var writer = new StreamWriter(Path.GetFullPath(pathName), false, encoding);
                writer.Write(templateText);
                writer.Close();

                AssetDatabase.ImportAsset(pathName);
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            }

            Debug.LogError(string.Format("The template file was not found: {0}", templatePath));
            return null;
        }

        #endregion

        /// Inherits from EndNameAction, must override EndNameAction.Action
        public class DoCreateCodeFile : EndNameEditAction
        {
            #region Public methods

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var o = CreateScript(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }

            #endregion
        }

        #region scorer templates

        [MenuItem("Assets/Create/RVSmartAI/AiScorer C# Script", false)]
        private static void CreateAiScorer()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiScorer");
            CreateClass("AiScorerTemplate.cs", "NewScorer.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/AiScorerCurve C# Script", false)]
        private static void CreateAiScorerCurve()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiScorerCurve");
            replaces.Add("0", "GetScoreFromCurve(0)");
            CreateClass("AiScorerTemplate.cs", "NewScorer.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/AiScorerParams C# Script", false)]
        private static void CreateAiScorerParams()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiScorerParams<Vector3>");
            CreateClass("AiScorerParamsTemplate.cs", "NewScorer.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/AiScorerCurveParams C# Script", false)]
        private static void CreateAiScorerCurveParams()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiScorerCurveParams<Vector3>");
            replaces.Add("0", "GetScoreFromCurve(0)");
            CreateClass("AiScorerParamsTemplate.cs", "NewScorer.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/AiScorer C# Script", false)]
        private static void CreateAiScorerGenericContext()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiAgentBaseScorer");
            CreateClass("AiScorerTemplate.cs", "NewScorer.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/AiScorerCurve C# Script", false)]
        private static void CreateAiScorerCurveGenericContext()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiAgentBaseScorerCurve");
            replaces.Add("0", "GetScoreFromCurve(0)");
            CreateClass("AiScorerTemplate.cs", "NewScorer.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/AiScorerParams C# Script", false)]
        private static void CreateAiScorerParamsGenericContext()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiAgentScorerParams<Vector3>");
            CreateClass("AiScorerParamsTemplate.cs", "NewScorer.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/AiScorerCurveParams C# Script", false)]
        private static void CreateAiScorerCurveParamsGenericContext()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiAgentBaseScorerCurveParams<Vector3>");
            replaces.Add("0", "GetScoreFromCurve(0)");
            CreateClass("AiScorerParamsTemplate.cs", "NewScorer.cs");
        }

        #endregion

        #region task templates

        [MenuItem("Assets/Create/RVSmartAI/AiTask C# Script", false)]
        private static void Createtask()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiTask");
            CreateClass("AiTaskTemplate.cs", "NewAiTask.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/AiTaskParams C# Script", false)]
        private static void CreateAiTaskParams()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiTaskParams<Vector3>");
            CreateClass("AiTaskParamsTemplate.cs", "NewAiTask.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/AiTask C# Script", false)]
        private static void CreateTaskGenericContext()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiAgentBaseTask");
            CreateClass("AiTaskTemplate.cs", "NewAiTask.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/AiTaskParams C# Script", false)]
        private static void CreateAiTaskParamsGenericContext()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiAgentBaseTaskParams<Vector3>");
            CreateClass("AiTaskParamsTemplate.cs", "NewAiTask.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/IntervalAiTask C# Script", false)]
        private static void CreateIntervalTask()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiTask");
            CreateClass("IntervalAiTaskTemplate.cs", "NewAiTask.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/IntervalAiTask C# Script", false)]
        private static void CreateIntervalTaskParams()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiTaskParams<Vector3>");
            CreateClass("IntervalAiTaskTemplate.cs", "NewAiTask.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/IntervalAiTask C# Script", false)]
        private static void CreateIntervalTaskGenericContext()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiAgentBaseTask");
            CreateClass("IntervalAiTaskTemplate.cs", "NewAiTask.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/IntervalAiTask C# Script", false)]
        private static void CreateIntervalTaskParamsGenericContext()
        {
            replaces.Clear();
            replaces.Add("BASECLASS", "AiAgentBaseTaskParams<Vector3>");
            CreateClass("IntervalAiTaskTemplate.cs", "NewAiTask.cs");
        }

        [MenuItem("Assets/Create/RVSmartAI/GenericContext/ScannerAiTask C# Script", false)]
        private static void CreateScannerTask()
        {
            replaces.Clear();
            CreateClass("ScannerAiTaskTemplate.cs", "NewAiTask.cs");
        }

        #endregion
    }
}