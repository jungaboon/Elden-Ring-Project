// Created by Ronis Vision. All rights reserved
// 22.04.2021.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RVHonorAI.Animation;
using RVHonorAI.CharacterInspector;
using RVHonorAI.Combat;
using RVHonorAI.Systems;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content;
using RVModules.RVSmartAI.Content.Scanners;
using RVModules.RVUtilities.Editor;
using RVModules.RVUtilities.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNodeEditor;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using Object = UnityEngine.Object;

namespace RVHonorAI.Editor
{
    [UnityEditor.CustomEditor(typeof(Character), true)] [CanEditMultipleObjects]
    public class CharacterInspector : UnityEditor.Editor
    {
        #region Fields

        private static string selectedTab = "General";
        protected static bool runtimeDebugFold = true;

        //private static int tab;

        protected int bTab;

        protected Dictionary<string, SerializedProperty> generalProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> soundProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> combatProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> combatPropertiesManual = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> movementProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> eventsProperties = new Dictionary<string, SerializedProperty>();

        //protected Dictionary<string, SerializedProperty> animProperties = new Dictionary<string, SerializedProperty>();

        protected Dictionary<string, SerializedProperty> animPropertiesManual = new Dictionary<string, SerializedProperty>();
        protected ReorderableList rl;

        protected Character character;
        protected SerializedObject serCharAi;
        protected SerializedObject anim, serializedMovementScanner, serializedEnviroScanner, serializedMovement;

        protected bool drawWaypointsHandles;
        protected Ai aiComponent;
        protected UnityEditor.Editor aiEditor;

        protected UnityEditor.Editor modulesEditor;
        protected Dictionary<string, SerializedProperty> customProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> customAnimationProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> customSoundProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> customGeneralProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> customMovementProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> customCombatProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> customEventsProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> customSystemsProperties = new Dictionary<string, SerializedProperty>();
        protected Dictionary<string, SerializedProperty> customAiProperties = new Dictionary<string, SerializedProperty>();

        protected List<SerializedObject> serializedFieldExposing = new List<SerializedObject>();

        #endregion

        #region Public methods

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            serializedObject.Update();
            serCharAi.Update();
            serializedMovementScanner.Update();
            serializedEnviroScanner.Update();
            serializedMovement.Update();

            anim?.Update();
            foreach (var o in serializedFieldExposing) o.Update();
            drawWaypointsHandles = false;

            if (Application.isPlaying && character.gameObject.scene.IsValid())
                if (GUILayout.Button("Debug AI"))
                    NodeEditorWindow.OpenWithGraph(character.CharacterAi.Ai.MainAiGraph);

            GUILayoutUtility.GetRect(.00f, 2f);

            //tab = GUILayout.Toolbar(tab, new[] {"General", "Movement", "Combat", "Animations", "Sounds", "Events", "Ai"}, "ToolbarButton");
            ErrorsInfo();

            RuntimeDebugInfo();

            GUILayout.BeginHorizontal();
            Tab("General");
            Tab("Movement");
            Tab("Combat");
            Tab("Animations");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Tab("Sounds");
            Tab("Events");
            Tab("Systems");
            Tab("Ai");
            GUILayout.EndHorizontal();

            EditorHelpers.VerticalSpace(5);

            switch (selectedTab)
            {
                case "General":
                    GeneralTab();
                    break;
                case "Movement":
                    MovementTab();
                    break;
                case "Combat":
                    CombatTab();
                    break;
                case "Animations":
                    AnimationTab();
                    break;
                case "Sounds":
                    SoundsTab();
                    break;
                case "Events":
                    EventsTab();
                    break;
                case "Ai":
                    AiTab();
                    break;
                case "Systems":
                    SystemsTab();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            serCharAi.ApplyModifiedProperties();
            serializedMovementScanner.ApplyModifiedProperties();
            serializedEnviroScanner.ApplyModifiedProperties();
            serializedMovement.ApplyModifiedProperties();
            anim?.ApplyModifiedProperties();
            foreach (var o in serializedFieldExposing) o.ApplyModifiedProperties();
        }

        protected virtual bool DrawPropIfExist(SerializedProperty _prop)
        {
            if (_prop == null) return false;
            EditorGUILayout.PropertyField(_prop);
            return true;
        }

        protected virtual void RuntimeDebugInfo()
        {
            if (customProperties.Count <= 0 || !character.gameObject.scene.IsValid()) return;

            runtimeDebugFold = EditorGUILayout.Foldout(runtimeDebugFold, "Runtime info", true);
            if (!runtimeDebugFold) return;
            EditorHelpers.WrapInBox(() =>
            {
//                    EditorGUI.BeginChangeCheck();
                if (Application.isPlaying)
                {
                    EditorGUILayout.PropertyField(generalProperties["setTarget"]);
                    DrawPropIfExist(generalProperties["destination"]);

                    if (character.CurrentWeapon as Object != null)
                        EditorGUILayout.ObjectField(new GUIContent("Current weapon"), character.CurrentWeapon as Object, typeof(Component), true);

                    if (character.CurrentAttack as Object != null)
                    {
                        EditorGUILayout.ObjectField(new GUIContent("Current attack"), character.CurrentAttack as Object, typeof(Component), true);
                        if (character.CurrentAttack.Name != (character.CurrentAttack as Component).name)
                            EditorGUILayout.LabelField(character.CurrentAttack.Name);
                    }
                }
                // there is some serious unity bug, targets list isnt updated properly when adding target from unity inspector event callstack
//                    if (EditorGUI.EndChangeCheck())
//                    {
//                        var ai = character.GetComponent<CharacterAi>();
//                        if (ai != null)
//                        {
//                            var targetGo = generalProperties["setTarget"].objectReferenceValue as GameObject;
//                            if (targetGo != null)
//                            {
//                                var t = targetGo.GetComponent<ITarget>();
////                                Debug.Log(t, t.Object());
//                                if (t != null)
//                                {
////                                    ai.AddTarget(t, true);
////                                    Debug.Log(ai.TargetInfos.Count);
//                                }
//
//                                //ai.SetTarget(target);
//                            }
//                        }
//                    }
//                    generalProperties["setTarget"].objectReferenceValue = null;

                EditorHelpers.DrawProperties(customProperties);
            });
        }

        public static void ImportAnimations(ICharacterAnimationContainer _target, ICharacterAnimationContainer _source, bool mov = true, bool comMov = true,
            bool single = true, bool log = true)
        {
            if (_source == null || _target == null) throw new NullReferenceException();

//            var charAnim = _target.CharacterAi.CharacterAnimation as CharacterAnimation;
//            if (charAnim == null) throw new Exception("Importing animations is possible only on CharacterAnimation component or inheritants!");
            var targetObject = _target as Object;
            if (targetObject == null) throw new Exception("Target must be of Object type!");

            Undo.RecordObject(targetObject, $"{targetObject.name} animations import");
            if (mov) _target.MovementAnimations = _source.MovementAnimations.Copy();
            if (comMov) _target.CombatMovementAnimations = _source.CombatMovementAnimations.Copy();
            if (single) _target.SingleAnimations = _source.SingleAnimations.Copy();
            PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject);
            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssets();
            if (log) Debug.Log($"Imported animations from {_source} to {_target}", _target as Object);
        }

        public static void SetupAnimatorController(ICharacter character, ICharacterAnimation _characterAnimation)
        {
            if (_characterAnimation?.Animator == null)
                throw new Exception("Cannot setup animator controller, animator is null");
            if (_characterAnimation.Animator.runtimeAnimatorController == null)
                throw new Exception("Cannot setup animator controller, animator controller is null");

            var characterAnimation = _characterAnimation as CharacterAnimation;
            if (characterAnimation == null) throw new Exception("Setup animator aontroller is possible only on CharacterAnimation component or inheritants!");

            Undo.RecordObject(characterAnimation, "Setup animation controller");

            EditorAnimationHelpers.SetupAnimatorController(character, characterAnimation,
                character.Transform.GetComponentInChildren<Animator>().runtimeAnimatorController as AnimatorController);

            PrefabUtility.RecordPrefabInstancePropertyModifications(characterAnimation);
        }

        public static bool CreateNewAnimatorController(Character _character, out AnimatorController newController)
        {
            var animController = Resources.Load("animControllerBase");
            newController = null;

            var savePath = EditorUtility.SaveFilePanel("New animator controller", Application.dataPath, "New animator controller", "controller");
            if (string.IsNullOrEmpty(savePath)) return true;

            savePath = "Assets" + savePath.Substring(Application.dataPath.Length);
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(animController), savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            newController = AssetDatabase.LoadAssetAtPath<AnimatorController>(savePath);
            //Selection.objects = new[] {newController};
            if (_character != null)
            {
                var characterAnimation = _character.CharacterAi.CharacterAnimation as CharacterAnimation;
                if (characterAnimation != null)
                {
                    EditorAnimationHelpers.SetupAnimatorController(_character, characterAnimation, newController);
                    _character.CharacterAi.CharacterAnimation.Animator.runtimeAnimatorController = newController;
                }
            }

            return false;
        }

        #endregion

        #region Not public methods

        protected virtual void Tab(string tab)
        {
            if (GUILayout.Toggle(selectedTab == tab, tab, "ToolbarButton")) selectedTab = tab;
        }

        protected virtual void ErrorsInfo()
        {
            var errors = new List<ConfigIssue>();
            character = (Character) target;

            if (Application.isPlaying) return;
            
            ConfigurationErrorsHandling(errors);

            if (errors.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Configuration issues");

                foreach (var error in errors)
                {
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.HelpBox(error.errorMsg, MessageType.Error);
                    if (error.fixButtonAction != null)
                        if (GUILayout.Button(error.fixButtonText))
                            error.fixButtonAction();

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Separator();
                }

                EditorGUILayout.EndVertical();
            }
        }

        protected virtual void ConfigurationErrorsHandling(List<ConfigIssue> errors)
        {
            // no collider added to char
            if (character.GetComponent<Collider>() == null)
            {
                var noCollIssue = new ConfigIssue
                {
                    errorMsg = "There's no collider added to root game object of character. It won't be detectable by other agents",
                    fixButtonText = "Add capsule collider",
                    fixButtonAction = () => { CharacterWizard.AddCollider(character.gameObject); }
                };
                errors.Add(noCollIssue);
            }

            var shootingWeapon = character.CurrentWeapon as ShootingWeapon;
            // has shooting weapon assigned but didnt set projectile spawn parent
            if (shootingWeapon)
                if (shootingWeapon.ProjectileSpawnParent == null)
                    errors.Add("Assigned weapon is shooting type which requires projectile spawn parent to be selected. " +
                               "Typically it will be hand or dedicated weapon bone in hierarchy. Assign it in \"Combat\" tab.");

            // uses ragdoll but didnt assigned one
            if (character.UseRagdoll && character.ragdollCreator.ragdollPrefab == null)
                errors.Add("Ragdoll prefab is not assigned. Assign proper ragdoll prefab in \"General\" tab.");

            // no head transform
            if (character.HeadTransform == null) errors.Add("Assign head transform under \"General\" tab.");

            // no animation controller
            var animController = character.CharacterAi.CharacterAnimation?.Animator?.runtimeAnimatorController as AnimatorController;
            if (animController == null)
            {
                errors.Add("Animator controller is not assigned. Create and assign animator controller under \"Animations\" tab.");
            }
            // has anim controller and enabled look at but animator controller isnt configured properly 
            else if (generalProperties["lookAtPlayerAndTarget"].boolValue)
            {
                var ac = character.CharacterAi.CharacterAnimation.Animator.runtimeAnimatorController as AnimatorController;
                if (ac.layers.Count(_layer => _layer.iKPass) == 0)
                {
                    ConfigIssue configIssue = "Look at player won't work with IK pass disabled in animator controller." +
                                              " Enable IK pass in animator controller manually or use button below.";
                    configIssue.fixButtonAction = () =>
                    {
                        ac = character.CharacterAi.CharacterAnimation.Animator.runtimeAnimatorController as AnimatorController;
                        var layers = ac.layers;
                        layers[0].iKPass = true;
                        ac.layers = layers;
                    };
                    errors.Add(configIssue);
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (aiEditor != null) DestroyImmediate(aiEditor);
            if (modulesEditor != null) DestroyImmediate(modulesEditor);
        }

        protected virtual void OnEnable()
        {
            // todo handle multiselect (targets)
            character = (Character) target;

            character.FindReferences();
            character.GetAvailableWeaponsAndAttacks();
            serCharAi = new SerializedObject(character.CharacterAi as Object);

            CharacterAnimation ca = character.GetComponentInChildren<CharacterAnimation>(true);
            if (ca != null) ca.FindReferences();

            var charAi = character.CharacterAi as CharacterAi;
            if (charAi != null) charAi.FindReferences();
            
            serializedEnviroScanner = new SerializedObject(character.GetComponentInChildren<IEnvironmentScanner>(true) as Object);
            serializedMovementScanner = new SerializedObject(character.GetComponentInChildren<IMovementScanner>(true) as Object);
            serializedMovement = new SerializedObject(character.CharacterAi.Movement as Object);

            foreach (var fieldsInInspector in character.GetComponentsInChildren<IExposeCharInspectorFields>())
                serializedFieldExposing.Add(new SerializedObject(fieldsInInspector.Object()));

            //var characterAnimation = character.CharacterAi.CharacterAnimation as CharacterAnimation;
            var characterAiCharacterAnimation = character.CharacterAi.CharacterAnimation as Object;
            if (characterAiCharacterAnimation == null)
                Debug.LogError("Theres no ICharacterAnimation component! You need to provide ICharacterAnimation component for your character", target);
            else
                anim = new SerializedObject(characterAiCharacterAnimation);

            aiComponent = character.GetComponentInChildren<Ai>();
            aiEditor = CreateEditor(aiComponent);

            // general props
            EditorHelpers.AddProperties(generalProperties, serializedObject,
                "health",
                "healthRegeneration",
                "maxHealth",
                "healthRegenerationSpeed",
                "useRagdoll",
                "ragdollCreator",
                "showGuiInfo",
                "aimTransform",
                "removeDead",
                "removeDeadAfter");

            EditorHelpers.AddProperties(generalProperties, serCharAi, "lookAtPlayerAndTarget", "headTransform", "setTarget");
            EditorHelpers.AddProperties(generalProperties, serializedMovement, "destination");

            // sound props
            EditorHelpers.AddProperties(soundProperties, serializedObject,
                "characterSounds.footstepSounds",
                "characterSounds.gotHitSound",
                "characterSounds.chanceToPlayGotHitSound",
                "characterSounds.dieSound",
                "characterSounds.fightSound",
                "characterSounds.chanceToPlayFightSound",
                "characterSounds.attackSound",
                "characterSounds.chanceToPlayAttackSound"
                //"characterSounds.noWeaponAttackSound",
                //"characterSounds.noWeaponHitSound"
            );

            // combat props
            EditorHelpers.AddProperties(combatProperties, serCharAi, "aiGroup");
            EditorHelpers.AddProperties(combatPropertiesManual, serializedEnviroScanner, "scannerLayerMask");
            EditorHelpers.AddProperties(combatProperties, serializedObject, "weapon", "armor");
            EditorHelpers.AddProperties(combatProperties, serCharAi, "courage", "treatNeutralCharactersAsEnemies");
            EditorHelpers.AddProperties(combatPropertiesManual, serCharAi, "neverFlee", "fleeTime", "targetMemory");

            //EditorHelpers.AddProperties(combatPropertiesManual, serializedObject, "visibilityCheckTransform");
            EditorHelpers.AddProperties(combatPropertiesManual, serCharAi, "useFieldOfView", "fovAngle", "useRaycastsForFov", "detectionRange",
                "alwaysVisibleRange", "eyesTransform", "fovMask");

            // movement props
            EditorHelpers.AddProperties(movementProperties, serializedObject,
                "walkingSpeed", "runningSpeed", "reserveDestinationPosition");
            EditorHelpers.AddProperties(movementProperties, serializedMovementScanner, "groundLayerMask", "obstaclesLayerMask");
            EditorHelpers.AddProperties(movementProperties, serCharAi,
                "useLocalPositionForWaypoints", "waypoints", "waypointsLoop", "randomWaypoints", "moveTarget");


            rl = EditorHelpers.InitReorderableList(movementProperties["waypoints"], serCharAi);
            // not working as expected
//            rl.onAddCallback += _list =>
//            {
//                CharacterAi characterAi = (CharacterAi) ai.targetObject;
//                var waypoints = characterAi.Waypoints;
//
//                waypoints.Add(new Waypoint() {position = Vector3.zero, radius = 0});
//                if (waypoints.Count > 1)
//                    waypoints.Last().position = waypoints[waypoints.Count - 2].position + Vector3.forward;
//                else
//                    waypoints[0].position = characterAi.transform.position + Vector3.forward;
//            };

            // events props
            EditorHelpers.AddProperties(eventsProperties, serializedObject, "onGotHit", "onReceivedDamage", "onKilled", "onAttack");
            EditorHelpers.AddProperties(eventsProperties, serCharAi, "onEnteredCombat", "onFlee", "onNoMoreVisibleEnemies");

            // anim props
            //EditorHelpers.AddProperties(animProperties, anim, "");

//            EditorHelpers.AddProperties(animPropertiesManual, serCharAi, "animationEventBasedAttack");

            if (anim != null)
                EditorHelpers.AddProperties(animPropertiesManual, anim,
                    "movementAnimations",
                    "combatMovementAnimations",
                    "singleAnimations",
                    "useRootMotion",
                    "animationMovementHandler",
                    "autoUpdateAnimatorController", "chanceForRandomIdleAnimation");

            CollectCustomExposedFields();
        }

        protected virtual void CollectCustomExposedFields()
        {
            var children = character.GetComponentsInChildren<IExposeCharInspectorFields>();
            for (var i = 0; i < children.Length; i++)
            {
                var exposeFieldsInInspector = children[i];
//                var exposedFields = exposeFieldsInInspector.InspectorExposedFields;
                var exposedFields = new List<InspectorExposedFieldInfo>();

                foreach (var fieldInfo in exposeFieldsInInspector.GetType().GetAllFields())
                {
                    var charInspField = fieldInfo.GetCustomAttribute<CharacterInspectorFieldAttribute>();
                    if (charInspField == null) continue;
                    if (!charInspField.drawWhenPlaying && Application.isPlaying) continue;
                    if (!charInspField.drawWhenNotPlaying && !Application.isPlaying) continue;
                    exposedFields.Add(new InspectorExposedFieldInfo(charInspField.tab, fieldInfo.Name));
                }

                if (exposeFieldsInInspector.ExposeAllFieldsToCharInspector)
                {
                    foreach (var fieldInfo in exposeFieldsInInspector.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
//                    foreach (var fieldInfo in exposeFieldsInInspector.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | bi)
                    {
                        // avoid duplicates
                        if (exposedFields.Any(_info => _info.Name == fieldInfo.Name)) continue;
                        var charInspField = fieldInfo.GetCustomAttribute<CharacterInspectorFieldAttribute>();
                        if (charInspField != null)
                        {
                            if (!charInspField.drawWhenPlaying && Application.isPlaying) continue;
                            if (!charInspField.drawWhenNotPlaying && !Application.isPlaying) continue;
                        }

                        exposedFields.Add(new InspectorExposedFieldInfo(exposeFieldsInInspector.DefaultCharInspectorTab, fieldInfo.Name));
                    }
                }

                foreach (var info in exposedFields)
                {
                    if (info == null) continue;

                    Dictionary<string, SerializedProperty> dict = null;
                    switch (info.Group)
                    {
                        case CharacterInspectorTab.General:
                            dict = customGeneralProperties;
                            break;
                        case CharacterInspectorTab.Movement:
                            dict = customMovementProperties;
                            break;
                        case CharacterInspectorTab.Combat:
                            dict = customCombatProperties;
                            break;
                        case CharacterInspectorTab.Animations:
                            dict = customAnimationProperties;
                            break;
                        case CharacterInspectorTab.Sounds:
                            dict = customSoundProperties;
                            break;
                        case CharacterInspectorTab.Events:
                            dict = customEventsProperties;
                            break;
                        case CharacterInspectorTab.Systems:
                            dict = customSystemsProperties;
                            break;
                        case CharacterInspectorTab.Ai:
                            dict = customAiProperties;
                            break;
                        case CharacterInspectorTab.None:
                            dict = customProperties;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (dict == null)
                    {
                        Debug.LogError($"Wrong tab name in custom inspector exposed field: tab \"{info.Group}\" field name \"{info.Name}\"",
                            exposeFieldsInInspector.Object());
                        continue;
                    }

                    try
                    {
                        if (dict.ContainsKey(info.Name)) continue;
                        EditorHelpers.AddProperty(dict, info.Name, serializedFieldExposing[i]);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }

        protected virtual void GeneralTab()
        {
            EditorHelpers.WrapInBox(() =>
            {
                EditorGUILayout.PropertyField(generalProperties["health"]);
                EditorGUILayout.PropertyField(generalProperties["maxHealth"]);
                EditorGUILayout.PropertyField(generalProperties["healthRegeneration"]);
                if (character.HealthRegeneration)
                    //EditorGUILayout.HelpBox("HP regeneration per second", MessageType.Info);
                    EditorGUILayout.PropertyField(generalProperties["healthRegenerationSpeed"]);

                EditorGUILayout.PropertyField(generalProperties["useRagdoll"]);
                if (character.UseRagdoll) EditorGUILayout.PropertyField(generalProperties["ragdollCreator"].FindPropertyRelative("ragdollPrefab"), true);

                var removeDeadProp = generalProperties["removeDead"];
                EditorGUILayout.PropertyField(removeDeadProp);
                if (removeDeadProp.boolValue) EditorGUILayout.PropertyField(generalProperties["removeDeadAfter"]);

//                EditorGUILayout.PropertyField(generalProperties["lookAtPlayerAndTarget"]);

                EditorGUILayout.PropertyField(generalProperties["showGuiInfo"]);
                EditorGUILayout.PropertyField(generalProperties["headTransform"]);
                EditorGUILayout.PropertyField(generalProperties["aimTransform"]);
            }, 0);

            DrawCustomProps(customGeneralProperties);
        }

        protected virtual void DrawCustomProps(Dictionary<string, SerializedProperty> _dictionary, bool wrapInBox = true)
        {
            if (_dictionary.Count == 0) return;
            if (wrapInBox) EditorHelpers.WrapInBox(() => EditorHelpers.DrawProperties(_dictionary));
            else EditorHelpers.DrawProperties(_dictionary);
        }

        protected virtual void MovementTab()
        {
            EditorHelpers.WrapInBox(() =>
            {
                EditorGUI.BeginChangeCheck();

                if (character.CharacterAi.CharacterAnimation == null || !character.CharacterAi.CharacterAnimation.UseRootMotion)
                {
                    EditorGUILayout.PropertyField(movementProperties["walkingSpeed"]);
                    EditorGUILayout.PropertyField(movementProperties["runningSpeed"]);
                }
                else
                {
                    EditorGUILayout.HelpBox("Root motion is on, so character's animations will control moving speed", MessageType.Info);
                }

                serializedObject.ApplyModifiedProperties();
                // we check here if this field exist, because it can not exist if user use own animation component
                if (EditorGUI.EndChangeCheck() && animPropertiesManual.Count > 0 && animPropertiesManual["autoUpdateAnimatorController"] != null &&
                    !character.CharacterAi.CharacterAnimation.UseRootMotion)
                    if (animPropertiesManual["autoUpdateAnimatorController"].boolValue)
                        SetupAnimatorController(character, character.transform.GetComponentInChildren<ICharacterAnimation>());

                EditorGUILayout.PropertyField(movementProperties["groundLayerMask"]);
                EditorGUILayout.PropertyField(movementProperties["obstaclesLayerMask"]);
                EditorGUILayout.PropertyField(movementProperties["reserveDestinationPosition"]);
                EditorGUILayout.PropertyField(movementProperties["moveTarget"], new GUIContent("Follow target"));
            }, 0);

            EditorHelpers.WrapInBox(() =>
            {
                EditorGUILayout.LabelField("Waypoints");
                if (!Application.isPlaying)
                {
                    EditorGUILayout.PropertyField(movementProperties["randomWaypoints"]);
                    if (!movementProperties["randomWaypoints"].boolValue)
                        // opis
                        EditorGUILayout.PropertyField(movementProperties["waypointsLoop"]);

                    if (movementProperties["useLocalPositionForWaypoints"] != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(movementProperties["useLocalPositionForWaypoints"]);
                        if (EditorGUI.EndChangeCheck())
                        {
//                            var value = !character.CharacterAi.UseLocalPositionForWaypoints;
                            var value = !movementProperties["useLocalPositionForWaypoints"].boolValue;

                            for (var i = 0; i < character.CharacterAi.Waypoints.Count; i++)
                                if (value)
                                    movementProperties["waypoints"].GetArrayElementAtIndex(i).FindPropertyRelative("position").vector3Value -=
                                        character.transform.position;
                                else
                                    movementProperties["waypoints"].GetArrayElementAtIndex(i).FindPropertyRelative("position").vector3Value +=
                                        character.transform.position;
                        }
                    }
                }

                drawWaypointsHandles = true;
                rl.DoLayoutList();
                if (GUILayout.Button("Reverse waypoints"))
                {
                    Undo.RecordObject(character.CharacterAi as Object, "Waypoints reverse");
                    character.CharacterAi.Waypoints.Reverse();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(character.CharacterAi as Object);
                }

                if (GUILayout.Button("Clear waypoints"))
                {
                    Undo.RecordObject(character.CharacterAi as Object, "Waypoints clear");
                    character.CharacterAi.Waypoints.Clear();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(character.CharacterAi as Object);
                }
            }, 0);

            DrawCustomProps(customMovementProperties);
        }

        protected virtual void EventsTab()
        {
            EditorHelpers.DrawProperties(eventsProperties);
            DrawCustomProps(customEventsProperties);
        }

        protected virtual void CombatTab()
        {
            EditorHelpers.WrapInBox(() =>
            {
                EditorHelpers.DrawProperties(combatProperties);
                EditorGUILayout.PropertyField(combatPropertiesManual["neverFlee"]);
                if (!combatPropertiesManual["neverFlee"].boolValue) EditorGUILayout.PropertyField(combatPropertiesManual["fleeTime"]);
            });

            EditorHelpers.WrapInBox(() =>
            {
                EditorGUILayout.LabelField("Perception");
                EditorGUILayout.PropertyField(combatPropertiesManual["scannerLayerMask"]);
                EditorGUILayout.PropertyField(combatPropertiesManual["detectionRange"]);
                EditorGUILayout.PropertyField(combatPropertiesManual["targetMemory"]);
                EditorGUILayout.PropertyField(combatPropertiesManual["useFieldOfView"]);
                if (combatPropertiesManual["useFieldOfView"].boolValue)
                {
                    EditorGUILayout.PropertyField(combatPropertiesManual["fovAngle"]);
                    EditorGUILayout.PropertyField(combatPropertiesManual["alwaysVisibleRange"]);
                    EditorGUILayout.PropertyField(combatPropertiesManual["useRaycastsForFov"]);
                    if (combatPropertiesManual["useRaycastsForFov"].boolValue)
                        //EditorGUILayout.PropertyField(combatPropertiesManual["headTransform"]);
                        EditorGUILayout.PropertyField(combatPropertiesManual["fovMask"]);
                }

                //EditorGUILayout.PropertyField(combatPropertiesManual["visibilityCheckTransform"]);
            });
            if (character.AvailableWeapons.Count > 0)
                EditorHelpers.WrapInBox(() =>
                {
                    EditorGUILayout.LabelField("Weapons");
                    foreach (var characterAvailableWeapon in character.AvailableWeapons)
                    {
                        EditorGUILayout.ObjectField(characterAvailableWeapon as Object, typeof(Component), true);
                    }
                });
            DrawCustomProps(customCombatProperties);
        }

        protected virtual void AnimationTab()
        {
            EditorGUI.BeginChangeCheck();

            if (animPropertiesManual.Count == 0)
            {
                EditorGUI.EndChangeCheck();
                return;
            }

            if (animPropertiesManual["autoUpdateAnimatorController"] != null)
            {
                EditorHelpers.WrapInBox(() =>
                {
                    EditorGUILayout.PropertyField(animPropertiesManual["autoUpdateAnimatorController"]);
                    EditorGUILayout.PropertyField(animPropertiesManual["useRootMotion"]);
//                    EditorGUILayout.PropertyField(animPropertiesManual["animationEventBasedAttack"]);
                    EditorGUILayout.PropertyField(generalProperties["lookAtPlayerAndTarget"]);
                    EditorGUILayout.PropertyField(animPropertiesManual["chanceForRandomIdleAnimation"]);
                });
            }
//            if (animPropertiesManual["autoUpdateAnimatorController"] != null)
//            {
//                EditorHelpers.WrapInBox(() =>
//                {
//                    EditorGUILayout.PropertyField(animPropertiesManual["useRootMotion"]);
//                });
//            }

            EditorHelpers.WrapInBox(() =>
            {
                GUILayout.BeginHorizontal();
//            if (animProperties["animatorController"].objectReferenceValue != null)
                if (GUILayout.Button("Update animator controller")) SetupAnimatorController(character, character.GetComponentInChildren<ICharacterAnimation>());

                if (GUILayout.Button("Create new animator controller"))
                    if (CreateNewAnimatorController(character, out var c))
                        return;

                GUILayout.EndHorizontal();
            });

            if (character.CharacterAi.CharacterAnimation is ICharacterAnimationContainer characterAnimation)
            {
                EditorHelpers.WrapInBox(() =>
                {
                    if (GUILayout.Button("Export animations preset"))
                        EditorAnimationHelpers.ExportAnimationsPreset(characterAnimation.MovementAnimations, characterAnimation.CombatMovementAnimations,
                            characterAnimation.SingleAnimations);

                    if (GUILayout.Button("Import animations preset"))
                    {
                        var loadPath = EditorUtility.OpenFilePanelWithFilters("Import animations preset", "Assets", new[] {"Animation preset", "animset"});
                        if (!string.IsNullOrEmpty(loadPath))
                        {
                            loadPath = loadPath.Substring(loadPath.IndexOf("Assets"));
                            var animPreset = AssetDatabase.LoadAssetAtPath<AnimationsPreset>(loadPath);

                            ImportAnimations(characterAnimation, animPreset);
                        }
                    }
                });

                EditorHelpers.WrapInBox(() => { EditorGUILayout.PropertyField(animPropertiesManual["movementAnimations"], true); });

                EditorHelpers.WrapInBox(() =>
                {
                    EditorGUILayout.HelpBox("Combat animations are optional. If left empty equivalent from normal movement animations will be used.",
                        MessageType.Info);
                    EditorGUILayout.PropertyField(animPropertiesManual["combatMovementAnimations"], true);
                });

                EditorHelpers.WrapInBox(() => { EditorGUILayout.PropertyField(animPropertiesManual["singleAnimations"], true); });
            }

            anim.ApplyModifiedProperties();

            // do separately to make sure end change is always called
            if (EditorGUI.EndChangeCheck() && animPropertiesManual["autoUpdateAnimatorController"] != null)
                if (animPropertiesManual["autoUpdateAnimatorController"].boolValue)
                    SetupAnimatorController(character, character.GetComponentInChildren<ICharacterAnimation>());

            DrawCustomProps(customAnimationProperties);
        }

        protected virtual void SoundsTab()
        {
            EditorHelpers.WrapInBox(() =>
            {
                if (GUILayout.Button("Export sounds preset"))
                {
                    var savePath = EditorUtility.SaveFilePanelInProject("Export sounds preset", "New sounds preset", "asset", "Save");
                    if (!string.IsNullOrEmpty(savePath))
                    {
                        var soundsPreset = CreateInstance<SoundsPreset>();
                        soundsPreset.characterSounds = character.characterSounds;

                        AssetDatabase.CreateAsset(soundsPreset, savePath);
                        AssetDatabase.SaveAssets();
                    }
                }

                if (GUILayout.Button("Import sounds preset"))
                {
                    var loadPath = EditorUtility.OpenFilePanelWithFilters("Import soundset preset", "Assets", new[] {"Sounds preset", "asset"});
                    if (!string.IsNullOrEmpty(loadPath))
                    {
                        loadPath = loadPath.Substring(loadPath.IndexOf("Assets"));
                        var soundsPreset = AssetDatabase.LoadAssetAtPath<SoundsPreset>(loadPath);
                        character.characterSounds = soundsPreset.characterSounds;
                    }
                }
            });
            var property = serializedObject.FindProperty("useSoundPreset");
            EditorGUILayout.PropertyField(property);
            if (property.boolValue)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("soundsPreset"));
            else
                EditorHelpers.WrapInBox(() => EditorHelpers.DrawProperties(soundProperties));

            DrawCustomProps(customSoundProperties);
        }

        protected virtual void SystemsTab()
        {
            var systems = character.GetComponentInChildren<AiSystems>();
            if (modulesEditor == null) modulesEditor = CreateEditor(systems);
            modulesEditor.OnInspectorGUI();

            DrawCustomProps(customSystemsProperties);
        }

        protected virtual void AiTab()
        {
            EditorHelpers.WrapInBox(() => aiEditor.OnInspectorGUI());
            if (Application.isPlaying)
                EditorHelpers.WrapInBox(() =>
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("aiJobHandler").FindPropertyRelative("runningTasksInfo"),
                        new GUIContent("Ai jobs"),
                        true));

            DrawCustomProps(customAiProperties);
        }

        protected virtual void OnSceneGUI()
        {
            if (!drawWaypointsHandles) return;

            var refPoint = Vector3.zero;

            var charAiBase = character.CharacterAi as CharacterAi;
            if (charAiBase != null)
            {
                if (charAiBase.UseLocalPositionForWaypoints) refPoint = character.transform.position;
                HonorAiEditorHelpers.WaypointsHandles(character.CharacterAi.Waypoints, refPoint, charAiBase);
            }
        }

        protected virtual void BehaviourGUI() => bTab = GUILayout.Toolbar(bTab, new[]
        {
            "Movement", "Combat"
        });

        #endregion

        protected class ConfigIssue
        {
            #region Fields

            public string errorMsg;
            public string fixButtonText = "Press to fix";
            public Action fixButtonAction;

            #endregion

            #region Public methods

            public static implicit operator ConfigIssue(string _text) => new ConfigIssue {errorMsg = _text};

            #endregion
        }
    }
}