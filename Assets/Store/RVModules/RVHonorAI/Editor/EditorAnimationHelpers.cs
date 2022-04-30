// Created by Ronis Vision. All rights reserved
// 23.01.2020.

using System;
using System.Collections.Generic;
using System.Linq;
using RVHonorAI.Animation;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVHonorAI.Editor
{
    internal static class EditorAnimationHelpers
    {
        #region Public methods

//        public static void SetupCharacterSpeedBasedOnAnimationsSpeed(ICharacter _character)
//        {
//        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_character"></param>
        /// <param name="_ca"></param>
        /// <param name="_animatorController"></param>
        public static void SetupAnimatorController(ICharacter _character, CharacterAnimation _ca, AnimatorController _animatorController)
        {
            try
            {
                var isHumanoid = false;
                if (_animatorController.animationClips.Length > 0) isHumanoid = _animatorController.animationClips[0].isHumanMotion;

                var baseLayerSm = _animatorController.layers[0].stateMachine;
                var upperLayerSm = _animatorController.layers[1].stateMachine;

                AnimatorState movementBlendState = null;
                AnimatorState combatMovementBlendState = null;

                var shit1 = baseLayerSm.states.FirstOrDefault(_s => _s.state.name == "movement");
                movementBlendState = shit1.state;

                var shit2 = baseLayerSm.states.FirstOrDefault(_s => _s.state.name == "combatMovement");
                combatMovementBlendState = shit2.state;

                AnimatorState upperEmpty = upperLayerSm.states.FirstOrDefault(_state => _state.state.name == "upper empty").state;

                var movementBlendTree = movementBlendState.motion as BlendTree;
                var combatMovementBlendTree = combatMovementBlendState.motion as BlendTree;

                var rotationState = baseLayerSm.states.FirstOrDefault(_s => _s.state.name == "rotation").state;
                var rotationBlendTree = rotationState.motion as BlendTree;
                var combatRotationState = baseLayerSm.states.FirstOrDefault(_s => _s.state.name == "combatRotation").state;
                var combatRotationBlendTree = combatRotationState.motion as BlendTree;

                ClipConfig[] movementClips = _ca.MovementAnimations.GetMovementAnimations();
                ClipConfig[] combatAnimationClips = _ca.CombatMovementAnimations.GetMovementAnimations();
                ClipConfig[] rotationClips = _ca.MovementAnimations.GetRotatingAnimations();
                ClipConfig[] combatRotationClips = _ca.CombatMovementAnimations.GetRotatingAnimations();

                // for empty combat clips assign them normal clips
                for (var i = 0; i < movementClips.Length; i++)
                {
                    var animationClip = movementClips[i];
                    if (combatAnimationClips[i].clip == null) combatAnimationClips[i] = animationClip;
                }

                for (var i = 0; i < rotationClips.Length; i++)
                {
                    var animationClip = rotationClips[i];
                    if (combatRotationClips[i].clip == null) combatRotationClips[i] = animationClip;
                }

                HandleMotionBlendTree(movementClips, movementBlendTree);
                HandleMotionBlendTree(combatAnimationClips, combatMovementBlendTree);
                HandleMotionBlendTree(rotationClips, rotationBlendTree);
                HandleMotionBlendTree(combatRotationClips, combatRotationBlendTree);

                // for not root motion well use agent target speed as our animation blending targets
                if (!_ca.UseRootMotion)
                {
                    BlendTreeMotionRangesNoRM(movementBlendTree);
                    BlendTreeMotionRangesNoRM(combatMovementBlendTree);
                }
                // for root motion we wanna use root animations velocities
                else
                {
                    var childMotions = movementBlendTree.children;
                    var runAnim = childMotions[5];
                    var walkAnim = childMotions[1];
                    SetCharacterSpeedFromAnimations(_character, walkAnim, runAnim);

                    BlendTreeMotionRangesRM(movementBlendTree);
                    BlendTreeMotionRangesRM(combatMovementBlendTree);
                }

                void BlendTreeMotionRangesNoRM(BlendTree _blendTree)
                {
                    var children = _blendTree.children;

                    // walk
                    if (children[1].motion != null) BlendTreeMotionRanges(children, 1, yChange: _character.WalkingSpeed);
                    // walk back
                    if (children[2].motion != null) BlendTreeMotionRanges(children, 2, yChange: -_character.WalkingSpeed);
                    // walk right
                    if (children[3].motion != null) BlendTreeMotionRanges(children, 3, xChange: _character.WalkingSpeed);
                    // walk left
                    if (children[4].motion != null) BlendTreeMotionRanges(children, 4, xChange: -_character.WalkingSpeed);
                    // run
                    if (children[5].motion != null) BlendTreeMotionRanges(children, 5, yChange: _character.RunningSpeed);

                    _blendTree.children = children;
                }

                void BlendTreeMotionRangesRM(BlendTree _blendTree)
                {
                    var childMotions = _blendTree.children;

                    var runAnim = childMotions[5];
                    var walkAnim = childMotions[1];
                    var charScale = _character.Transform.localScale;

//                    var walkSpeed = walkAnim.motion.averageSpeed.z * walkAnim.timeScale * charScale.z;
//                    var runSpeed = runAnim.motion.averageSpeed.z * runAnim.timeScale * charScale.z;

//                    // walk
//                    if (walkAnim.motion != null)
//                        BlendTreeMotionRanges(childMotions, 1, yChange: walkSpeed);
//
//                    // walk back
//                    if (childMotions[2].motion != null)
//                        BlendTreeMotionRanges(childMotions, 2, yChange: -walkSpeed,
//                            negValueIfZero: true);
//
//                    // walk right
//                    if (childMotions[3].motion != null)
//                    {
//                        var walkRightVelocity = walkSpeed;
//                        //if (childMotions[3].mirror) walkRightVelocity *= -1;
//                        BlendTreeMotionRanges(childMotions, 3, xChange: walkRightVelocity);
//                    }
//
//                    // walk left
//                    if (childMotions[4].motion != null)
//                    {
//                        var walkLeftVelocity = -walkSpeed;
//                        //if (childMotions[4].mirror) walkLeftVelocity *= -1;
//                        BlendTreeMotionRanges(childMotions, 4, xChange: walkLeftVelocity, negValueIfZero: true);
//                    }
//
//                    // run
//                    if (runAnim.motion != null)
//                        BlendTreeMotionRanges(childMotions, 5, yChange: runSpeed);

                    // old version, walking side and back had separate calculations for speed, which is pointless since navagent only uses one speed in all directions
                    // walk
                    if (walkAnim.motion != null)
                        BlendTreeMotionRanges(childMotions, 1, yChange: walkAnim.motion.averageSpeed.z * walkAnim.timeScale * charScale.z);

                    // walk back
                    if (childMotions[2].motion != null)
                        BlendTreeMotionRanges(childMotions, 2, yChange: childMotions[2].motion.averageSpeed.z * childMotions[2].timeScale * charScale.z,
                            negValueIfZero: true);

                    // walk right
                    if (childMotions[3].motion != null)
                    {
                        var walkRightVelocity = childMotions[3].motion.averageSpeed.x * childMotions[3].timeScale * charScale.x;
                        if (childMotions[3].mirror) walkRightVelocity *= -1;
                        BlendTreeMotionRanges(childMotions, 3, xChange: walkRightVelocity);
                    }

                    // walk left
                    if (childMotions[4].motion != null)
                    {
                        var walkLeftVelocity = childMotions[4].motion.averageSpeed.x * childMotions[4].timeScale * charScale.x;
                        if (childMotions[4].mirror) walkLeftVelocity *= -1;
                        BlendTreeMotionRanges(childMotions, 4, xChange: walkLeftVelocity, negValueIfZero: true);
                    }

                    // run
                    if (runAnim.motion != null)
                        BlendTreeMotionRanges(childMotions, 5, yChange: runAnim.motion.averageSpeed.z * runAnim.timeScale * charScale.z);

                    _blendTree.children = childMotions;
                }

                // setup upper layer
                SetupTriggerAnimations(_animatorController, upperLayerSm, new[] {upperEmpty}, "attacking",
                    _ca.SingleAnimations.attackingAnimations, true, false, false, false);

                // setup base layer
                var enterWhenNotMovingOnly = isHumanoid;
                var states = SetupTriggerAnimations(_animatorController, baseLayerSm, new[] {combatMovementBlendState}, "attacking",
                    _ca.SingleAnimations.attackingAnimations, true, false, enterWhenNotMovingOnly, false);

                foreach (var animatorState in states)
                {
                    animatorState.transitions = new AnimatorStateTransition[0];
                    if (isHumanoid)
                    {
                        CreateTransition(animatorState, combatRotationState, "rotation", .99f, AnimatorConditionMode.Greater, false, true);
                        CreateTransition(animatorState, combatRotationState, "rotation", -.99f, AnimatorConditionMode.Less, false, true);
                    }

                    CreateTransition(animatorState, combatRotationState, "rotating", 0, AnimatorConditionMode.If, true, true);
                    CreateTransition(animatorState, combatMovementBlendState, "", 0, AnimatorConditionMode.If, true, false);
                }

                // dying
                SetupTriggerAnimations(_animatorController, baseLayerSm, new[] {movementBlendState}, "dying",
                    _ca.SingleAnimations.dyingAnimations, false, false, false, false);

                // custom
                SetupTriggerAnimations(_animatorController, baseLayerSm, new[] {movementBlendState}, "custom",
                    _ca.SingleAnimations.customAnimations, true, false, false, false);

                // random idle
                SetupRandomIdleAnimations(_animatorController, baseLayerSm, new[] {movementBlendState}, _ca.SingleAnimations.randomIdleAnimations);
            }
            catch (Exception e)
            {
                Debug.LogError("Updating animator controller error: " + e, _animatorController);
            }

            EditorUtility.SetDirty(_animatorController);
        }

        private static void SetCharacterSpeedFromAnimations(ICharacter _character, ChildMotion walkAnim, ChildMotion runAnim)
        {
            if (walkAnim.motion == null && runAnim.motion == null) return;

            Undo.RecordObject(_character as Object, "");

            if (walkAnim.motion != null)
                _character.WalkingSpeed = walkAnim.motion.averageSpeed.z * walkAnim.timeScale * _character.Transform.localScale.z;

            if (runAnim.motion != null)
                _character.RunningSpeed = runAnim.motion.averageSpeed.z * runAnim.timeScale * _character.Transform.localScale.z;

            PrefabUtility.RecordPrefabInstancePropertyModifications(_character as Object);
        }

        /// <summary>
        /// to consider: make blend tress more generic, that is pass here model scale, and animation instead of specific x y changes
        /// </summary>
        /// <param name="children"></param>
        /// <param name="_index0"></param>
        /// <param name="xChange"></param>
        /// <param name="yChange"></param>
        private static void BlendTreeMotionRanges(ChildMotion[] children, int _index0, float xChange = -100, float yChange = -100, bool negValueIfZero = false)
        {
            var position = children[_index0].position;
            var minVel = .1f;

            if (xChange != -100)
            {
                if (xChange == 0)
                {
                    if (negValueIfZero) xChange = -minVel;
                    else xChange = minVel;
                }

                if (xChange > 0 && xChange < minVel) xChange = minVel;
                if (xChange < 0 && xChange > -minVel) xChange = -minVel;
                position.x = xChange;
            }

            if (yChange != -100)
            {
                if (yChange == 0)
                {
                    if (negValueIfZero) yChange = -minVel;
                    else yChange = minVel;
                }

                if (yChange > 0 && yChange < minVel) yChange = minVel;
                if (yChange < 0 && yChange > -minVel) yChange = -minVel;
                position.y = yChange;
            }

            children[_index0].position = position;
        }

        private static void HandleMotionBlendTree(ClipConfig[] _clips, BlendTree blendMotion)
        {
            var c = blendMotion.children;

            for (var i = 0; i < c.Length; i++)
            {
                c[i].motion = _clips[i];
                c[i].mirror = _clips[i].mirror;
                c[i].timeScale = _clips[i].animSpeed;
            }

            blendMotion.children = c;
        }

        #endregion

        #region Not public methods

        /// <summary>
        /// Creates new anim state with given name  
        /// </summary>
        /// <returns></returns>
        private static AnimatorState HandleAnimStateForBlendTrees(AnimatorStateMachine _stateMachine, string _stateName, List<AnimatorState> _states,
            string _animationSpeedMul = "")
        {
            bool exist = _stateMachine.states.Any(_s => _s.state.name == _stateName);
            var childAnimatorState = _stateMachine.states.FirstOrDefault(_s => _s.state.name == _stateName);
            var state = childAnimatorState.state;
            //_createdNewState = false;

            if (!exist)
            {
                state = _stateMachine.AddState(_stateName);
                //_createdNewState = true;
            }

            //state.motion = _animation;
            //state.speed = _animationSpeed;

            if (_animationSpeedMul != "")
            {
                state.speedParameterActive = true;
                state.speedParameter = _animationSpeedMul;
            }

            _states.Add(state);

            return state;
        }

        private static AnimatorState HandleAnimState(AnimatorStateMachine _stateMachine, string _stateName, List<AnimatorState> _states,
            out bool _createdNewState,
            Motion _animation, float _animationSpeed = 1, string _animationSpeedMul = "")
        {
            bool exist = _stateMachine.states.Any(_s => _s.state.name == _stateName);
            var childAnimatorState = _stateMachine.states.FirstOrDefault(_s => _s.state.name == _stateName);
            var state = childAnimatorState.state;
            _createdNewState = false;

            if (!exist)
            {
                state = _stateMachine.AddState(_stateName);
                _createdNewState = true;
            }

            state.motion = _animation;
            state.speed = _animationSpeed;

            if (_animationSpeedMul != "")
            {
                state.speedParameterActive = true;
                state.speedParameter = _animationSpeedMul;
            }

            _states.Add(state);

            return state;
        }

        private static AnimatorStateTransition CreateTransition(AnimatorState animatorState, AnimatorState _statesToMakeTransitionTo, string _parameter,
            float _threshold,
            AnimatorConditionMode _animatorConditionMode, bool _t1HasExitTime, bool condition)
        {
            // if we dont want to touch already existing transitions
//        if (dontCreateTransitionIfAlreadyExist && animatorState.transitions.Any(t => t.destinationState == _statesToMakeTransitionTo))
//            return null;

            // if we want to redo transitions every time
            //animatorState.transitions = new AnimatorStateTransition[0];

            var transition = animatorState.AddTransition(_statesToMakeTransitionTo);
            transition.hasExitTime = _t1HasExitTime;
            transition.duration = .25f;
            if (condition) transition.AddCondition(_animatorConditionMode, _threshold, _parameter);
            return transition;
        }

//    private static void CreateTransitions(AnimatorState[] _sources, AnimatorState _statesToMakeTransitionTo, string _parameter, int _threshold,
//        AnimatorConditionMode _animatorConditionMode, bool _t1HasExitTime)
//    {
//        foreach (var animatorState in _sources)
//        {
//            CreateTransition(animatorState, _statesToMakeTransitionTo, _parameter, _threshold, _animatorConditionMode, _t1HasExitTime);
//        }
//    }

        private static AnimatorState[] SetupTriggerAnimations(AnimatorController _animatorController, AnimatorStateMachine _stateMachine,
            AnimatorState[] statesToBackTo, string _animName, ClipConfig[] _animations,
            bool _createReturningTransitions, bool exitWhenTryingToMove, bool enterWhenNotMovingOnly, bool enterWhenNotRotating)
        {
            var h = 0;
            var animatorStates = new List<AnimatorState>();
            foreach (var motion in _animations)
            {
                //if (motion == null) continue;
                var paramName = _animName + h;
                AssureParamExist(_animatorController, paramName, AnimatorControllerParameterType.Trigger);

                var stateName = _animName + h;
                var attackState = _stateMachine.states.FirstOrDefault(_s => _s.state.name == stateName);
                var animState = attackState.state;

                var createdNewState = false;
                if (animState == null)
                {
                    animState = _stateMachine.AddState(stateName);
                    createdNewState = true;
                }

                animatorStates.Add(animState);

                if (_createReturningTransitions)
                {
                    if (!createdNewState)
                    {
                        animState.transitions = new AnimatorStateTransition[0];
                    }

                    if (exitWhenTryingToMove)
                    {
                        var t = CreateTransition(animState, statesToBackTo[0], "moving", 1, AnimatorConditionMode.If, false, true);
                        //t.AddCondition(AnimatorConditionMode.IfNot, 0, "rotating");
                    }
                    else
                    {
                        CreateTransition(animState, statesToBackTo[0], "State", 2, AnimatorConditionMode.NotEqual, true, false);
                    }
                    //CreateTransition(attackStateState, combatStates[0], "State", 2, AnimatorConditionMode.Equals, true);

                    //CreateTransition(attackStateState, normalStates[1], "MovingMode", 0, AnimatorConditionMode.Greater, false);
//                if (combatStates.Length > 1)
//                    CreateTransition(attackStateState, combatStates[1], "MovingMode", 0, AnimatorConditionMode.Greater, false);
                }

                //if (createdNewState)
                {
                    // remove all transitions from any state to our curr state
                    _stateMachine.anyStateTransitions =
                        _stateMachine.anyStateTransitions.Where(_transition => _transition.destinationState != animState).ToArray();
                    //var t = animatorState.AddTransition(attackStateState);
                    var t = _stateMachine.AddAnyStateTransition(animState);
                    t.AddCondition(AnimatorConditionMode.If, 1, paramName);
                    if (enterWhenNotMovingOnly) t.AddCondition(AnimatorConditionMode.IfNot, 0, "moving");
                    if (enterWhenNotRotating)
                    {
                        t.AddCondition(AnimatorConditionMode.Less, .2f, "rotation");
                        t.AddCondition(AnimatorConditionMode.Greater, -.2f, "rotation");
                    }
                }

                animState.motion = motion;
                animState.speed = motion.animSpeed;
                animState.mirror = motion.mirror;

                h++;
            }

            return animatorStates.ToArray();
        }

        private static AnimatorState[] SetupRandomIdleAnimations(AnimatorController _animatorController, AnimatorStateMachine _stateMachine,
                AnimatorState[] statesToBackTo, ClipConfig[] _animations)
            //bool _createReturningTransitions, bool exitWhenTryingToMove, bool enterWhenNotMovingOnly, bool enterWhenNotRotating)
        {
            AssureParamExist(_animatorController, "randomIdleId", AnimatorControllerParameterType.Int);

            var h = 1;
            var animatorStates = new List<AnimatorState>();
            foreach (var motion in _animations)
            {
                var _animName = $"idle";
                var stateName = _animName + h;
                var attackState = _stateMachine.states.FirstOrDefault(_s => _s.state.name == stateName);
                var animState = attackState.state;

                var createdNewState = false;
                if (animState == null)
                {
                    animState = _stateMachine.AddState(stateName);
                    createdNewState = true;
                }

                animatorStates.Add(animState);

//                if (_createReturningTransitions)
                {
                    animState.transitions = new AnimatorStateTransition[0];

//                    if (exitWhenTryingToMove)
                    {
                        CreateTransition(animState, statesToBackTo[0], "moving", 1, AnimatorConditionMode.If, false, true);
                        CreateTransition(animState, statesToBackTo[0], "State", 1, AnimatorConditionMode.Greater, false, true);
                        CreateTransition(animState, statesToBackTo[0], "", 1, AnimatorConditionMode.If, true, false);
                    }
//                    else
//                    {
//                        CreateTransition(animState, statesToBackTo[0], "State", 2, AnimatorConditionMode.NotEqual, true, false);
//                    }
                }

                statesToBackTo[0].transitions = statesToBackTo[0].transitions.Where(_transition => _transition.destinationState != animState).ToArray();

                var t = CreateTransition(statesToBackTo[0], animState, "", 0, AnimatorConditionMode.Equals, false, false);

                t.AddCondition(AnimatorConditionMode.Equals, h, "randomIdleId");
                //if (enterWhenNotMovingOnly) 
                t.AddCondition(AnimatorConditionMode.IfNot, 0, "moving");
                //if (enterWhenNotRotating)
                {
                    t.AddCondition(AnimatorConditionMode.Less, .2f, "rotation");
                    t.AddCondition(AnimatorConditionMode.Greater, -.2f, "rotation");
                }

                animState.motion = motion;
                animState.speed = motion.animSpeed;
                animState.mirror = motion.mirror;

                h++;
            }

            return animatorStates.ToArray();
        }

        private static void AssureParamExist(AnimatorController _animatorController, string _paramName, AnimatorControllerParameterType _paramType)
        {
            if (!_animatorController.parameters.Any(_p => _p.name == _paramName)) _animatorController.AddParameter(_paramName, _paramType);
        }

        #endregion

        public static void ExportAnimationsPreset(MovementAnimations _movementAnimations, MovementAnimations _combatMovementAnimations,
            SingleAnimations _singleAnimations)
        {
            var savePath = EditorUtility.SaveFilePanelInProject("Export animations preset", "New animations preset", "animset", "Save");
            if (!String.IsNullOrEmpty(savePath))
            {
                var animPreset = ScriptableObject.CreateInstance<AnimationsPreset>();


                animPreset.MovementAnimations = _movementAnimations.Copy();
                animPreset.CombatMovementAnimations = _combatMovementAnimations.Copy();
                animPreset.SingleAnimations = _singleAnimations.Copy();

                AssetDatabase.CreateAsset(animPreset, savePath);
                AssetDatabase.SaveAssets();
            }
        }
    }
}