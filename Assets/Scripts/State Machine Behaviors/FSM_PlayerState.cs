using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponent<PlayerController>();
        controller.state = state;

        controller.animator.applyRootMotion = false;
        controller.canRotate = true;

        switch(state)
        {
            case PlayerState.Attack:
                controller.animator.applyRootMotion = true;
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.Defend:
                break;
            case PlayerState.Dodge:
                controller.animator.applyRootMotion = true;
                break;
            case PlayerState.Heal:
                break;
            case PlayerState.Hurt:
                controller.animator.applyRootMotion = true;
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
}
