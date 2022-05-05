using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PlayerState
{
    Normal,
    Defend,
    Parry,
    Attack,
    Heal,
    Dodge,
    Hurt,
    Dead,
    Taunt
}

public class FSM_PlayerState : StateMachineBehaviour
{
    [SerializeField] private PlayerState state;
    private PlayerController controller;

    [HideInInspector] public bool isActualAttackState = true;
    [HideInInspector] public float timeBeforeAbleToDodge = 0.3f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponent<PlayerController>();
        controller.state = state;

        controller.animator.applyRootMotion = false;
        controller.canRotate = true;
        controller.canPressDodge = true;

        switch(state)
        {
            case PlayerState.Attack:
                controller.animator.applyRootMotion = true;
                if(isActualAttackState) controller.StartAttackDodgeCooldown(timeBeforeAbleToDodge);
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.Defend:
                break;
            case PlayerState.Dodge:
                controller.animator.applyRootMotion = true;
                break;
            case PlayerState.Heal:
                controller.canPressDodge = false;
                break;
            case PlayerState.Hurt:
                controller.canRotate = false;
                controller.animator.applyRootMotion = true;
                controller.canPressDodge = false;
                break;
            case PlayerState.Normal:
                animator.ResetTrigger("attack");
                animator.ResetTrigger("heavyAttack");
                animator.ResetTrigger("dodge");
                animator.ResetTrigger("lockOnDodge");
                break;
            case PlayerState.Parry:
                controller.canRotate = false;
                controller.animator.applyRootMotion = true;
                controller.canPressDodge = false;
                break;
            case PlayerState.Taunt:
                controller.canRotate = false;
                controller.animator.applyRootMotion = true;
                break;
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (state)
        {
            case PlayerState.Attack:
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.Defend:
                break;
            case PlayerState.Dodge:
                break;
            case PlayerState.Heal:
                break;
            case PlayerState.Hurt:
                break;
            case PlayerState.Normal:
                break;
            case PlayerState.Parry:
                break;
            case PlayerState.Taunt:
                break;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (state)
        {
            case PlayerState.Attack:
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.Defend:
                break;
            case PlayerState.Dodge:
                break;
            case PlayerState.Heal:
                break;
            case PlayerState.Hurt:
                break;
            case PlayerState.Normal:
                break;
            case PlayerState.Parry:
                break;
            case PlayerState.Taunt:
                break;
        }
    }


    #region Editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(FSM_PlayerState))]
    public class FSM_PlayerStateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            FSM_PlayerState playerState = (FSM_PlayerState)target;
            switch(playerState.state)
            {
                case PlayerState.Attack:
                    playerState.isActualAttackState = EditorGUILayout.Toggle("Is Actual Attack State", playerState.isActualAttackState);
                    if(playerState.isActualAttackState)
                    {
                        playerState.timeBeforeAbleToDodge = EditorGUILayout.FloatField("Time Before Able To Dodge", playerState.timeBeforeAbleToDodge);
                    }
                    break;
            }
        }
    }
    #endif
    #endregion
}
