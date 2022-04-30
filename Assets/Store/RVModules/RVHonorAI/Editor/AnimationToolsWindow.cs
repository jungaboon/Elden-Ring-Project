// Created by Ronis Vision. All rights reserved
// 24.02.2021.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RVHonorAI.Animation;
using RVModules.RVUtilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVHonorAI.Editor
{
    public class AnimationToolsWindow : EditorWindow
    {
        #region Fields

        private List<AnimationInfo> animationInfos = new List<AnimationInfo>();
        private List<AnimationClip> animationClips = new List<AnimationClip>();

        [SerializeField]
        private MovementAnimations movementAnimations;

        [SerializeField]
        private SingleAnimations singleAnimations;

        [SerializeField]
        private string idleKeyword = "IDLE", walkKeyword = "WALK", runKeyword = "RUN", turnKeyword = "TURN";

        [SerializeField]
        private string[] attackingKeywords = {"ATTACK", "PUNCH", "SWING", "SWIPING", "SWIPE", "BITE", "BITING", "SHOOT", "THROW"};

        [SerializeField]
        private string[] customKeywords = {"TAUNT"};

        [SerializeField]
        private string[] dyingKeywords = {"DIE", "DYING"};

        [SerializeField]
        private string[] randomIdleKeywords = {"IDLE", "IDLING"};

        private bool logAssignments = true;

        private Vector2 scrollPos;

        private string searchedFolder;
        private string selectedFolder;

        #endregion

        #region Not public methods

        private void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorHelpers.WrapInWindow(AutoAssigner, "Auto assigner");

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorHelpers.WrapInWindow(() =>
                {
                    EditorGUILayout.HelpBox("Clips renamer\n" +
                                            "Allows you to bulk change clip names to match their model file name where model have only one clip",
                        MessageType.None);

                    EditorGUILayout.HelpBox("Select model files for which you want to rename clips and press rename", MessageType.Info);
                    var sel = Selection.objects;
                    if (sel.Length > 0)
                    {
                        if (GUILayout.Button("Rename clips to match their file name")) RenameClipsToMatchFileNames();
                        EditorGUILayout.HelpBox("Warning: this will brake references to all renamed clips, and cannot be undone!", MessageType.Warning);
                    }
                }, "Clips renamer"
            );

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorHelpers.WrapInWindow(AnimationImporter, "Animation importer");

            GUILayout.EndScrollView();
        }

        private Object animationImportSource;
        private Object animationImportTarget;

        private void AnimationImporter()
        {
//            GUILayout.BeginHorizontal();
            animationImportTarget = EditorGUILayout.ObjectField("Animations target", animationImportTarget, typeof(Object), true);
            animationImportSource = EditorGUILayout.ObjectField("Animations source", animationImportSource, typeof(Object), true);
//            GUILayout.EndHorizontal();

            var source = animationImportSource as ICharacterAnimationContainer;
            if (source == null)
            {
                var go = animationImportSource as GameObject;
                if (go != null) source = go.GetComponentInChildren<ICharacterAnimationContainer>();
            }

            var target = animationImportTarget as ICharacterAnimationContainer;
            if (target == null)
            {
                var go = animationImportTarget as GameObject;
                if (go != null) target = go.GetComponentInChildren<ICharacterAnimationContainer>();
            }

            if (source == null || target == null)
            {
                EditorGUILayout.HelpBox("Assign proper animation import target and source. They have to implement ICharacterAnimationContainer",
                    MessageType.Error);
            }
            else
            {
//                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Import all animations")) CharacterInspector.ImportAnimations(target, source);
                if (GUILayout.Button("Import movement animations")) CharacterInspector.ImportAnimations(target, source, true, false, false);
                if (GUILayout.Button("Import combat movement animations")) CharacterInspector.ImportAnimations(target, source, false, true, false);
                if (GUILayout.Button("Import single animations")) CharacterInspector.ImportAnimations(target, source, false, false, true);
//                GUILayout.EndHorizontal();
            }
        }

        private void AutoAssigner()
        {
            EditorGUILayout.HelpBox("Auto animation assigner\n" +
                                    "Allows for quick bulk assignment of animations from selected folder\n" +
                                    "based on clip names. If your clip animation names aren't relevant you can\n" +
                                    "use Clip renamer tool below to rename clips to their model file name", MessageType.None);

            EditorGUILayout.HelpBox("1.Select folder containing your animations\n" +
                                    "2.Press Find animations\n" +
                                    "   you can inspect detected animations in this window\n" +
                                    "3.Select Character or animation preset you want to assign animations\n" +
                                    "4.Press one of buttons that will show at bottom of Auto animation assigner\n" +
                                    "   This action can be undone", MessageType.Info);
            EditorGUILayout.Separator();

            if (Selection.activeObject != null) GetFolderFromSelection(out selectedFolder);
            else selectedFolder = "";

            var so = new SerializedObject(this);

            if (!string.IsNullOrEmpty(searchedFolder))
            {
                EditorGUILayout.HelpBox($"Searched folder: {searchedFolder}", MessageType.None);

                EditorGUILayout.Separator();
            }

            if (string.IsNullOrEmpty(selectedFolder)) EditorGUILayout.HelpBox("Select the folder you want to search", MessageType.Info);
            else if (GUILayout.Button("Find animations")) FindAnimations();

            so.Update();
            EditorGUILayout.PropertyField(so.FindProperty(nameof(idleKeyword)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(walkKeyword)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(runKeyword)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(turnKeyword)), true);

            EditorGUILayout.PropertyField(so.FindProperty(nameof(attackingKeywords)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(dyingKeywords)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(randomIdleKeywords)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(customKeywords)), true);
            so.ApplyModifiedProperties();

            if (animationInfos.Count > 0)
            {
                EditorGUILayout.Separator();
                var allAnims = movementAnimations.GetAllAnimations().ToList();
                allAnims.AddRange(singleAnimations.GetAllAnimations());
                EditorGUILayout.HelpBox(
                    $"Found {animationClips.Count} animations, detected {allAnims.Count(_config => _config != null && _config.clip != null)} matching",
                    MessageType.None);
                EditorGUILayout.PropertyField(so.FindProperty("movementAnimations"), true);
                EditorGUILayout.PropertyField(so.FindProperty("singleAnimations"), true);
            }

            if (animationInfos.Count > 0)
            {
                EditorGUILayout.Separator();
                if (GUILayout.Button("Create animation preset"))
                {
                    EditorAnimationHelpers.ExportAnimationsPreset(movementAnimations, new MovementAnimations(), singleAnimations);
                }
            }

            var selectedObj = Selection.activeObject;
            if (animationInfos.Count > 0 && selectedObj != null)
            {
                var animationContainer = selectedObj as ICharacterAnimationContainer;
                if (animationContainer == null)
                {
                    var go = selectedObj as GameObject;
                    if (go != null) animationContainer = go.GetComponentInChildren<ICharacterAnimationContainer>();
                }

                if (animationContainer == null) return;

                EditorGUILayout.Separator();

                EditorGUILayout.LabelField($"Assign animations to your {selectedObj}:");
                EditorGUILayout.BeginHorizontal();

//                if (GUILayout.Button("All"))
//                {
//                    Undo.RecordObject(charAnimations as Object, "Assign Character animations");
//                    charAnimations.MovementAnimations = movementAnimations.Copy();
//                    charAnimations.SingleAnimations = singleAnimations.Copy();
//                    Debug.Log($"Sucessfully assigned all animations to {charComp.name}", charComp);
//                }

// todo use import api to follow DRY
                if (GUILayout.Button("Movement"))
                {
                    Undo.RecordObject(animationContainer as Object, "Assign Character movement animations");
                    animationContainer.MovementAnimations = movementAnimations.Copy();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(animationContainer as Object);
                    EditorUtility.SetDirty(animationContainer as Object);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"Sucessfully assigned movement animations to {selectedObj.name}", animationContainer as Object);
                }

                if (GUILayout.Button("Combat movement"))
                {
                    Undo.RecordObject(animationContainer as Object, "Assign Character combat movement animations");
                    animationContainer.CombatMovementAnimations = movementAnimations.Copy();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(animationContainer as Object);
                    EditorUtility.SetDirty(animationContainer as Object);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"Sucessfully assigned combat movement animations to {selectedObj.name}", animationContainer as Object);
                }

                if (GUILayout.Button("Single"))
                {
                    Undo.RecordObject(animationContainer as Object, "Assign Character single animations");
                    animationContainer.SingleAnimations = singleAnimations.Copy();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(animationContainer as Object);
                    EditorUtility.SetDirty(animationContainer as Object);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"Sucessfully assigned single animations to {selectedObj.name}", animationContainer as Object);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void FindAnimations()
        {
            animationInfos.Clear();
            movementAnimations = new MovementAnimations();
            singleAnimations = new SingleAnimations();
            singleAnimations.attackingAnimations = new ClipConfig[0];
            singleAnimations.customAnimations = new ClipConfig[0];
            singleAnimations.dyingAnimations = new ClipConfig[0];
            singleAnimations.randomIdleAnimations = new ClipConfig[0];
            GetModelFiles();
            GetAnimationClips();

            foreach (var animationInfo in animationInfos) TryToAssignClip(animationInfo);

            if (logAssignments)
            {
                AssignmentLog(movementAnimations.idleAnimation, "idle");
                AssignmentLog(movementAnimations.walkingAnimation, "walking");
                AssignmentLog(movementAnimations.turningLeftAnimation, "turning left");
                AssignmentLog(movementAnimations.turningRightAnimation, "turning right");
                AssignmentLog(movementAnimations.walkingBackAnimation, "walking back");
                AssignmentLog(movementAnimations.walkingLeftAnimation, "walking left");

                foreach (var singleAnimationsAttackingAnimation in singleAnimations.attackingAnimations)
                    AssignmentLog(singleAnimationsAttackingAnimation, "attacking");
                foreach (var singleAnimationsAttackingAnimation in singleAnimations.customAnimations)
                    AssignmentLog(singleAnimationsAttackingAnimation, "custom");
                foreach (var singleAnimationsAttackingAnimation in singleAnimations.dyingAnimations)
                    AssignmentLog(singleAnimationsAttackingAnimation, "dying");
                foreach (var singleAnimationsAttackingAnimation in singleAnimations.randomIdleAnimations)
                    AssignmentLog(singleAnimationsAttackingAnimation, "random idle");
            }

            void AssignmentLog(ClipConfig _clipConfig, string animType)
            {
                if (_clipConfig == null || _clipConfig.clip == null) return;
                Debug.Log($"Assigned \"{_clipConfig.clip.name}\" as {animType} animation", _clipConfig.clip);
            }
        }

        private void RenameClipsToMatchFileNames()
        {
            var selection = Selection.objects;

            foreach (var o in selection)
            {
                var path = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrEmpty(path)) continue;

                var mi = AssetImporter.GetAtPath(path) as ModelImporter;

                // !!! FUCKING UNITY !!!and their retarded api never working as expected... thanks to that crap you lose references to animation clips
                // if you never 'edited' them after import
                var modelImporterClipAnimations = mi.clipAnimations;
                if (modelImporterClipAnimations.Length == 0) modelImporterClipAnimations = mi.defaultClipAnimations;

                if (modelImporterClipAnimations.Length != 1) continue;

                foreach (var modelImporterClipAnimation in modelImporterClipAnimations)
                {
                    var oldName = modelImporterClipAnimation.name;
                    var newName = o.name;
                    modelImporterClipAnimation.name = newName;
                    Debug.Log($"Renamed \"{oldName}\" to \"{newName}\"");
                }

                mi.clipAnimations = modelImporterClipAnimations;
                mi.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
        }

        private void GetAnimationClips()
        {
            animationClips.Clear();

            foreach (var animInfo in animationInfos)
            {
                // get animation Clip
                var objects = AssetDatabase.LoadAllAssetsAtPath(animInfo.filePath);

                foreach (var obj in objects)
                {
                    var clip = obj as AnimationClip;
                    if (clip == null) continue;
                    if (clip.name.Contains("__preview__")) continue;

                    animInfo.clips.Add(clip);
                    animationClips.Add(clip);
                }

                //mi.SaveAndReimport();
            }

            Debug.Log($"Found {animationClips.Count} animation clips");
        }

        private void TryToAssignClip(AnimationInfo _animationInfo)
        {
            if (_animationInfo.clips.Count == 0) return;

//            // first tyr to assign from fbx name
//            var assignedFromFileName = TryAssign(_animationInfo.modelFile.name, _animationInfo.clips[0]);
//            if (assignedFromFileName) return;

            // try to assign from clips
            foreach (var animationInfoClip in _animationInfo.clips)
            {
                var clip = animationInfoClip;
                if (TryAssign(clip.name, clip))
                {
                }
            }

            bool TryAssign(string n, AnimationClip clip)
            {
                n = n.ToUpper();
                //var assignedAnything = false;

                if (n.Contains(idleKeyword.ToUpper()))
                {
                    if (movementAnimations.idleAnimation != null)
                    {
                        if (LevenshteinDistance.GetClosestFit(idleKeyword.ToUpper(), new[] {movementAnimations.idleAnimation.clip.name.ToUpper(), n}) == n)
                        {
                            movementAnimations.idleAnimation = new ClipConfig(clip);
                            return true;
                        }
                    }
                    else
                    {
                        movementAnimations.idleAnimation = new ClipConfig(clip);
                        return true;
                    }
                }

                if (!n.Contains("RIGHT") && !n.Contains("LEFT") && !n.Contains("BACK"))
                {
                    if (n.Contains(walkKeyword.ToUpper()))
                    {
                        if (movementAnimations.walkingAnimation == null)
                        {
                            movementAnimations.walkingAnimation = new ClipConfig(clip);
                            return true;
                        }

                        if (LevenshteinDistance.GetClosestFit(walkKeyword.ToUpper(), new[] {movementAnimations.walkingAnimation.clip.name.ToUpper(), n}) == n)
                        {
                            movementAnimations.walkingAnimation = new ClipConfig(clip);
                            return true;
                        }
                    }

                    if (n.Contains(runKeyword.ToUpper()))
                    {
                        if (movementAnimations.runningAnimation == null)
                        {
                            movementAnimations.runningAnimation = new ClipConfig(clip);
                            return true;
                        }

                        if (LevenshteinDistance.GetClosestFit(runKeyword.ToUpper(), new[] {movementAnimations.runningAnimation.clip.name.ToUpper(), n}) == n)
                        {
                            movementAnimations.runningAnimation = new ClipConfig(clip);
                            return true;
                        }
                    }
                }

                if (n.Contains(walkKeyword.ToUpper()) && n.Contains("RIGHT"))
                    if (movementAnimations.walkingRightAnimation == null ||
                        LevenshteinDistance.GetClosestFit($"{walkKeyword.ToUpper()} RIGHT",
                            new[] {movementAnimations.walkingRightAnimation.clip.name.ToUpper(), n}) == n)
                    {
                        movementAnimations.walkingRightAnimation = new ClipConfig(clip);
                        return true;
                    }

                if (n.Contains("WALK") && n.Contains("LEFT"))
                    if (movementAnimations.walkingLeftAnimation == null ||
                        LevenshteinDistance.GetClosestFit($"{walkKeyword.ToUpper()} LEFT",
                            new[] {movementAnimations.walkingLeftAnimation.clip.name.ToUpper(), n}) == n)
                    {
                        movementAnimations.walkingLeftAnimation = new ClipConfig(clip);
                        return true;
                    }

                if (n.Contains("WALK") && n.Contains("BACK"))
                    if (movementAnimations.walkingBackAnimation == null ||
                        LevenshteinDistance.GetClosestFit($"{walkKeyword.ToUpper()} BACK",
                            new[] {movementAnimations.walkingBackAnimation.clip.name.ToUpper(), n}) == n)
                    {
                        movementAnimations.walkingBackAnimation = new ClipConfig(clip);
                        return true;
                    }

                if (n.Contains(turnKeyword.ToUpper()) && n.Contains("RIGHT"))
                    if (movementAnimations.turningRightAnimation == null ||
                        LevenshteinDistance.GetClosestFit($"{turnKeyword.ToUpper()} RIGHT",
                            new[] {movementAnimations.turningRightAnimation.clip.name.ToUpper(), n}) == n)
                    {
                        movementAnimations.turningRightAnimation = new ClipConfig(clip);
                        return true;
                    }

                if (n.Contains(turnKeyword.ToUpper()) && n.Contains("LEFT"))
                    if (movementAnimations.turningLeftAnimation == null ||
                        LevenshteinDistance.GetClosestFit($"{turnKeyword.ToUpper()} LEFT",
                            new[] {movementAnimations.turningLeftAnimation.clip.name.ToUpper(), n}) == n)
                    {
                        movementAnimations.turningLeftAnimation = new ClipConfig(clip);
                        return true;
                    }

                // single anims
                if (ContainsAndDont(n, attackingKeywords))
                {
                    var l = singleAnimations.attackingAnimations.ToList();
                    l.Add(new ClipConfig(clip));
                    singleAnimations.attackingAnimations = l.ToArray();
                    return true;
                }

                if (ContainsAndDont(n, dyingKeywords))
                {
                    var l = singleAnimations.dyingAnimations.ToList();
                    l.Add(new ClipConfig(clip));
                    singleAnimations.dyingAnimations = l.ToArray();
                    return true;
                }

                if (ContainsAndDont(n, customKeywords))
                {
                    var l = singleAnimations.customAnimations.ToList();
                    l.Add(new ClipConfig(clip));
                    singleAnimations.customAnimations = l.ToArray();
                    return true;
                }

                if (ContainsAndDont(n, randomIdleKeywords))
                {
                    var l = singleAnimations.randomIdleAnimations.ToList();
                    l.Add(new ClipConfig(clip));
                    singleAnimations.randomIdleAnimations = l.ToArray();
                    return true;
                }

//                return assignedAnything;
                return false;
            }
        }

        private bool ContainsAndDont(string text, string[] contains, params string[] dont)
        {
            foreach (var s in dont)
                if (text.ToUpper().Contains(s.ToUpper()))
                    return false;

            foreach (var contain in contains)
                if (text.ToUpper().Contains(contain.ToUpper()))
                    return true;

            return false;
        }

        [MenuItem("RVHonorAI/Open animation tools")]
        private static void ShowWindow()
        {
            var asw = CreateWindow<AnimationToolsWindow>("Animation tools");
        }

        private void GetModelFiles()
        {
            var sDataPath = GetFolderFromSelection(out var sFolderPath);
            searchedFolder = sFolderPath;

            Debug.Log($"Searching for animations: {sFolderPath}");

            // get the system file paths of all the files in the asset folder
            var aFilePaths = Directory.GetFiles(sFolderPath);

            // enumerate through the list of files loading the assets they represent and getting their type
            foreach (var sFilePath in aFilePaths)
            {
                if (new FileInfo(sFilePath).Extension.ToUpper() != ".FBX") continue;

                var sAssetPath = sFilePath.Substring(sDataPath.Length - 6);

                var objAsset = AssetDatabase.LoadAssetAtPath(sAssetPath, typeof(Object));
                if (objAsset == null) continue;

                var animationInfo = new AnimationInfo {filePath = sAssetPath, modelFile = objAsset};
                animationInfos.Add(animationInfo);
            }

//            Debug.Log($"Found {animationInfos.Count} model files");
        }

        private static string GetFolderFromSelection(out string sFolderPath)
        {
            var objSelected = Selection.activeObject;
            if (objSelected == null) throw new Exception("Select folder first");

            var sAssetFolderPath = AssetDatabase.GetAssetPath(objSelected);
            if (string.IsNullOrEmpty(sAssetFolderPath))
            {
                sFolderPath = "";
                return "";
            }

            // Construct the system path of the asset folder 
            var sDataPath = Application.dataPath;
            sFolderPath = sDataPath.Substring(0, sDataPath.Length - 6) + sAssetFolderPath;

            if (!new DirectoryInfo(sFolderPath).Exists) sFolderPath = new FileInfo(sFolderPath).DirectoryName;
            return sDataPath;
        }

        #endregion

        private class AnimationInfo
        {
            #region Fields

            public Object modelFile;
            public string filePath = "";
            public List<AnimationClip> clips = new List<AnimationClip>();

            #endregion
        }
    }
}