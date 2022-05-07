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
    [HideInInspector] public float moveTimer = 1f;
    [HideInInspector] public bool moveToRadiusAroundTarget;
    [HideInInspector] public float radius = 1f;

    [HideInInspector] public bool heavy;
    [HideInInspector] public float attackDamage = 15f;

    private float moveTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        enemy.state = state;
        enemy.agent.updateRotation = true;
        enemy.agent.speed = moveSpeed;
        enemy.lookingAtTarget = false;
        enemy.attacking = false;

        switch(state)
        {
            case EnemyState.Attack:
                enemy.agent.updateRotation = false;
                enemy.agent.ResetPath();
                enemy.agent.velocity = Vector3.zero;
                enemy.lookingAtTarget = true;
                enemy.attacking = true;
                enemy.attackDamage = attackDamage;
                enemy.heavyAttack = heavy;

                if (Random.value < 0.5f)
                {
                    animator.SetInteger("attackType", Random.Range(0, 2));
                    animator.SetTrigger("attackAgain");
                }
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
                enemy.lookingAtTarget = true;
                break;
            case EnemyState.Repositioning:
                enemy.agent.updateRotation = false;
                enemy.lookingAtTarget = true;
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
                if (moveTime < moveTimer) moveTime += Time.deltaTime;
                else
                {
                    if (moveToRadiusAroundTarget) enemy.MoveToRadiusAroundTarget(enemy.target.transform.position, radius);
                    else enemy.MoveToPoint(enemy.target.transform.position, radius);
                    moveTime = 0f;
                }
                break;
            case EnemyState.Repositioning:
                if (moveTime < moveTimer) moveTime += Time.deltaTime;
                else
                {
                    enemy.MoveToPoint(enemy.target.transform.position, radius);
                    moveTime = 0f;
                }
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

#region Editor
#if UNITY_EDITOR
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
                enemyState.moveSpeed = EditorGUILayout.FloatField("Move Speed", enemyState.moveSpeed);
                enemyState.moveTimer = EditorGUILayout.FloatField("Move Timer", enemyState.moveTimer);
                enemyState.moveToRadiusAroundTarget = EditorGUILayout.Toggle("Move To Radius Around Target", enemyState.moveToRadiusAroundTarget);
                enemyState.radius = EditorGUILayout.FloatField("Radius", enemyState.radius);
                break;
            case EnemyState.Repositioning:
                enemyState.moveSpeed = EditorGUILayout.FloatField("Move Speed", enemyState.moveSpeed);
                enemyState.moveTimer = EditorGUILayout.FloatField("Move Timer", enemyState.moveTimer);
                enemyState.radius = EditorGUILayout.FloatField("Radius", enemyState.radius);
                break;
            case EnemyState.Attack:
                enemyState.heavy = EditorGUILayout.Toggle("Heavy", enemyState.heavy);
                enemyState.attackDamage = EditorGUILayout.FloatField("Attack Damage", enemyState.attackDamage);
                break;
        }
    }
}
#endif
#endregion
