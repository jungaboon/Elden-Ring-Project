// Created by Ronis Vision. All rights reserved
// 16.01.2020.

using System;
using System.Collections.Generic;
using System.Linq;
using RVHonorAI.Animation;
using RVHonorAI.Combat;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content;
using RVModules.RVSmartAI.Content.Scanners;
using RVModules.RVSmartAI.Editor.SelectWindows;
using RVModules.RVUtilities.Editor;
using RVModules.RVUtilities.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVHonorAI.Editor
{
    public class CharacterWizard : ScriptableWizard
    {
        #region Fields

        private Character charToCopySettingsFrom;
        private GameObject modelPrefab;

        private AnimationsPreset animationsPreset;
        private SoundsPreset soundsPreset;

        private Weapon weaponPrefab;
        private AiGroup aiGroup;

        private AnimatorController animatorController;

        private GameObject aiComponentsPrefab;

        private string selectedCharacterTypeName = nameof(Character);
        private Type selectedCharacterType = typeof(Character);

//        todo
//        private string selectedCharacterAiType = nameof(CharacterAi);
//        private string selectedAnimationType = nameof(CharacterAnimation);
//        private string selectedMovementType = nameof(UnityNavMeshMovement);

        #endregion

        #region Public methods

        [MenuItem("RVHonorAI/Open character creation wizard")]
        public static void CreateWizard() => DisplayWizard<CharacterWizard>("Character wizard", "Create");

        #endregion

        #region Not public methods

        protected override bool DrawWizardGUI()
        {
            //base.DrawWizardGUI();
            errorString = "";
            isValid = true;

            modelPrefab = (GameObject) EditorGUILayout.ObjectField("Character model", modelPrefab, typeof(GameObject), true);

            charToCopySettingsFrom = EditorGUILayout.ObjectField("Copy settings from other Character",
                charToCopySettingsFrom, typeof(Character), true) as Character;

            EditorGUILayout.Separator();

            if (modelPrefab != null)
            {
                Animator animator = modelPrefab.GetComponent<Animator>();
                if (animatorController == null && charToCopySettingsFrom == null)
                {
                    isValid = false;
                    errorString = "Create new animator controller or assign Character to copy settings from";
                }

                if (animator == null)
                {
                    errorString = "This is not proper object, it have to have Animator component on it.";
                    isValid = false;
                }

//                else
//                    errorString = "";
            }
            else
            {
//                errorString = "";
                isValid = false;
            }

            aiGroup = (AiGroup) EditorGUILayout.ObjectField("Ai group", aiGroup, typeof(AiGroup), false);
            weaponPrefab = (Weapon) EditorGUILayout.ObjectField("Weapon prefab", weaponPrefab, typeof(Weapon), false);

            EditorGUILayout.Separator();
            if (charToCopySettingsFrom != null && animatorController == null)
            {
                animationsPreset = null;
            }
            else
            {
                animationsPreset = (AnimationsPreset) EditorGUILayout.ObjectField("Animations preset",
                    animationsPreset, typeof(AnimationsPreset), true);
            }

            soundsPreset = (SoundsPreset) EditorGUILayout.ObjectField("Sounds preset", soundsPreset, typeof(SoundsPreset), true);

            EditorGUILayout.Separator();

            animatorController = (AnimatorController) EditorGUILayout.ObjectField("Animator controller", animatorController, typeof(AnimatorController), true);
            if (GUILayout.Button("Create animator controller"))
            {
                CharacterInspector.CreateNewAnimatorController(null, out animatorController);
            }


            GUILayout.BeginHorizontal();
            var btnTxt = $"Selected Character component\n{selectedCharacterTypeName ?? "None"}";
            if (GUILayout.Button(btnTxt))
            {
                var charSelWin = CreateInstance<SelectWindowBase>();
                charSelWin.types = ReflectionHelper.GetDerivedTypes(typeof(ICharacter));
                charSelWin.onSelectedItem += _type =>
                {
                    selectedCharacterTypeName = _type.Name;
                    selectedCharacterType = _type;
                    charSelWin.Close();
                    Focus();
                };
            }

            GUILayout.EndHorizontal();

            aiComponentsPrefab = EditorGUILayout.ObjectField("Ai components", aiComponentsPrefab, typeof(GameObject), false) as GameObject;
            if (aiComponentsPrefab == null) aiComponentsPrefab = Resources.Load<GameObject>("AiComponents");

            var movementScanner = aiComponentsPrefab.GetComponent<IMovementScanner>();
            var enviroScanner = aiComponentsPrefab.GetComponent<IEnvironmentScanner>();
            var characterAnimation = aiComponentsPrefab.GetComponent<ICharacterAnimation>();
            var ai = aiComponentsPrefab.GetComponent<Ai>();

            if (ai == null)
            {
                isValid = false;
                errorString += "Ai components prefab must have Ai component\n";
            }

            if (movementScanner as Object == null)
            {
                isValid = false;
                errorString += "Ai components prefab must have IMovementScanner component\n";
            }

            if (enviroScanner as Object == null)
            {
                isValid = false;
                errorString += "Ai components prefab must have IEnvironmentScanner component\n";
            }

            // animation moved to root go
//            if (characterAnimation as Object == null)
//            {
//                isValid = false;
//                errorString += "Ai components prefab must have ICharacterAnimation component";
//            }

            //createButtonName = "Create";

            return false;
        }

        private void OnWizardCreate()
        {
            GameObject charGo = EditorHelpers.InstantiatePrefab(modelPrefab);

            ICharacter character = charGo.AddComponent(selectedCharacterType) as ICharacter;
            var charBase = character as Character;

            //charGo.name = "New character";
            charGo.transform.localPosition = Vector3.zero;
            charGo.transform.localRotation = Quaternion.identity;

            // create character components
            GameObject charComponents = null;

            charComponents = (GameObject) PrefabUtility.InstantiatePrefab(aiComponentsPrefab);
            // apply characer components
            charComponents.name = aiComponentsPrefab.name;
            charComponents.transform.SetParent(charGo.transform);

            var charAi = charGo.AddComponent<CharacterAi>();
            var charAnim = charGo.AddComponent<CharacterAnimation>();
            var movement = charGo.AddComponent<UnityNavMeshMovement>();

            EditorHelpers.ChangeSerializedPropertyValue(charAnim as Object, "autoUpdateAnimatorController",
                _property => _property.boolValue = false);
            
            if (charToCopySettingsFrom != null)
            {
                // if you try to get prefab instance here you will get into terrible mess.. just not worth it, believe me...
                // it will spawn root of preafb(whole character) instead of aiComponents game object
                var sourceAiComps = charToCopySettingsFrom.Transform.GetComponentInChildren<Ai>().gameObject;

                ReflectionHelper.CopyFieldsOfUnityObject(charToCopySettingsFrom, character.Object());
                ReflectionHelper.CopyFieldsOfUnityObject(charToCopySettingsFrom.GetComponentInChildren<ICharacterAi>() as Object, charAi);
                ReflectionHelper.CopyFieldsOfUnityObject(charToCopySettingsFrom.GetComponentInChildren<ICharacterAnimation>() as Object, charAnim);
                ReflectionHelper.CopyGameObjectAllComponentsFields(sourceAiComps, charComponents);

                charGo.GetComponent<Animator>().runtimeAnimatorController = charToCopySettingsFrom.Transform.GetComponent<Animator>().runtimeAnimatorController;

                CharacterInspector.ImportAnimations(charGo.GetComponentInChildren<ICharacterAnimationContainer>(),
                    charToCopySettingsFrom.GetComponentInChildren<ICharacterAnimationContainer>(), log: false);

//                if (charBase != null)
//                {
////                    charBase.ragdollCreator = new RagdollCreator {ragdollPrefab = charToCopySettingsFrom.ragdollCreator.ragdollPrefab};
//                }
            }

            // its delayed... like unity development...
            //charBase.Invoke(nameof(Character.FindReferences), -1);
            if (charBase != null) charBase.FindReferences();
            charAi.FindReferences();

            CharacterAnimation ca = charGo.GetComponentInChildren<CharacterAnimation>();
            ICharacterAnimationContainer characterAnimationContainer = charGo.GetComponentInChildren<ICharacterAnimationContainer>();

            if (ca != null) ca.FindReferences();

            var serializedAi = new SerializedObject(charAi.Ai);
            serializedAi.FindProperty("contextProvider").objectReferenceValue = charAi;
            serializedAi.ApplyModifiedPropertiesWithoutUndo();

            // automatically assign head transform from any transform that has 'head' in its name
            charAi.HeadTransform = character.Transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name.ToUpper().Contains("HEAD"));
            if (charBase != null)
                charBase.AimTransform = character.Transform.GetComponentsInChildren<Transform>()
                    .FirstOrDefault(t => t.name.ToUpper().Contains("SPINE1") || t.name.ToUpper().Contains("SPINE"));

            // set anim and sounds presets
            if (animationsPreset != null && charBase != null) CharacterInspector.ImportAnimations(ca, animationsPreset);
            if (soundsPreset != null && charBase != null) charBase.characterSounds = soundsPreset.characterSounds;

            if (charToCopySettingsFrom == null) charAi.AiGroup = aiGroup;

            // set weapon
            if (weaponPrefab != null)
            {
                var weapon = EditorHelpers.InstantiatePrefab(weaponPrefab.gameObject);
                weapon.transform.SetParent(character.Transform);
                EditorHelpers.ChangeSerializedPropertyValue(character as Object, "weapon",
                    _property => _property.objectReferenceValue = weapon.GetComponent<IAttack>() as Object);
            }

            // animtor c
            if (animatorController != null)
            {
                character.Transform.GetComponent<Animator>().runtimeAnimatorController = animatorController;

                if (charToCopySettingsFrom == null && animationsPreset != null)
                {
                    CharacterInspector.SetupAnimatorController(character, character.Transform.GetComponentInChildren<ICharacterAnimation>());
                    EditorHelpers.ChangeSerializedPropertyValue(charAi.CharacterAnimation as Object, "autoUpdateAnimatorController",
                        _property => _property.boolValue = true);
                }
            }
            else
            {
//                var so = new SerializedObject(character.CharacterAi.CharacterAnimation);
//                so.Update();
//                so.FindProperty("autoUpdateAnimatorController").boolValue = false;
//                so.ApplyModifiedProperties();
            }

            // collider and layer setup
            AddCollider(charGo);

            // select newly created pos
            Selection.objects = new Object[] {charGo};
            var allSceneCameras = SceneView.GetAllSceneCameras();
            if (allSceneCameras.Length > 0)
                charGo.transform.position = allSceneCameras[0].transform.position + allSceneCameras[0].transform.forward * 5;
        }

        public static void AddCollider(GameObject charGo)
        {
            CapsuleCollider cc = Undo.AddComponent<CapsuleCollider>(charGo);
            cc.radius = .35f;
            cc.height = 2;
            cc.center = Vector3.up;
//            charGo.layer = 10;
        }

        //private void OnWizardOtherButton() => Close();

        #endregion
    }
}