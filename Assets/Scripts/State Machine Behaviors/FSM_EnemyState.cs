using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FSM_EnemyState : StateMachineBehaviour
{
    public EnemyState state;
    private Enemy enemy;

    [HideInInspector] public float moveSpeed = 3.5f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        enemy.state = state;
        enemy.agent.updateRotation = true;

        switch(state)
        {
            case EnemyState.Attack:
                enemy.agent.updateRotation = false;
                break;
            case EnemyState.Block:
                break;
            case EnemyState.Die:
                break;
            case EnemyState.Dodge:
                break;
            case EnemyState.Hurt:
                break;
            case EnemyState.Idle:
                break;
            case EnemyState.MoveToTarget:
                enemy.agent.updateRotation = false;
                break;
            case EnemyState.Repositioning:
                enemy.agent.updateRotation = false;
                break;
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (state)
        {
            case EnemyState.Attack:
                break;
            case EnemyState.Block:
                break;
            case EnemyState.Die:
                break;
            case EnemyState.Dodge:
                break;
            case EnemyState.Hurt:
                break;
            case EnemyState.Idle:
                break;
            case EnemyState.MoveToTarget:
                break;
            case EnemyState.Repositioning:
                break;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (state)
        {
            case EnemyState.Attack:
                break;
            case EnemyState.Block:
                break;
            case EnemyState.Die:
                break;
            case EnemyState.Dodge:
                break;
            case EnemyState.Hurt:
                break;
            case EnemyState.Idle:
                break;
            case EnemyState.MoveToTarget:
                break;
            case EnemyState.Repositioning:
                break;
        }
    }
}

[CustomEditor(typeof(FSM_EnemyState))]
public class FSM_EnemyStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FSM_EnemyState enemyState = (FSM_EnemyState)target;
        switch(enemyState.state)
        {
            case EnemyState.MoveToTarget:
            case EnemyState.Repositioning:
                enemyState.moveSpeed = EditorGUILayout.FloatField("Move Speed", enemyState.moveSpeed);
                break;
        }
    }
}
