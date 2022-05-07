using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SensorToolkit;

public enum EnemyState
{
    Idle,
    Repositioning,
    MoveToTarget,
    Attack,
    Dodge,
    Block,
    Hurt,
    Die
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public EnemyState state;
    [HideInInspector] public RangeSensor sensor;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public GameObject target;
    [HideInInspector] public GameEventManager gameEventManager;
    [HideInInspector] public Collider mainCollider;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public EnemyAttack enemyAttack;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public bool heavyAttack;

    public bool lookingAtTarget;
    public bool attacking;
    public bool targetVisible;
    public float targetDistance;

    public Vector3 moveDirection;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        gameEventManager = GameEventManager.Instance;
        mainCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        sensor = GetComponent<RangeSensor>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();

        lookingAtTarget = false;
    }

    // USE FOR ROOT MOTION
    public void OnAnimatorMove()
    {
        switch (state)
        {
            case EnemyState.Attack:
            case EnemyState.Dodge:
            case EnemyState.Block:
                Vector3 position = animator.rootPosition;
                position.y = agent.nextPosition.y;
                animator.transform.position = position;
                agent.nextPosition = animator.transform.position;
                break;
        }
    }

    public virtual void Update()
    {
        MoveDirection();
        SetTargetDistance();
    }

    #region Finding Random Position
    public void MoveToPoint(Vector3 origin, float radius)
    {
        if (agent.isOnNavMesh) agent.SetDestination(FindNewRandomPoint(origin, radius));
    }
    public Vector3 FindPositionAroundTarget(Vector3 target, float radius)
    {
        float x = target.x + radius * Mathf.Cos(2 * Mathf.PI * Random.Range(0f, 1f));
        float y = target.y;
        float z = target.z + radius * Mathf.Sin(2 * Mathf.PI * Random.Range(0f, 1f));
        return new Vector3(x, y, z);
    }
    public void MoveToRadiusAroundTarget(Vector3 origin, float radius)
    {
        if (agent.isOnNavMesh) agent.SetDestination(FindPositionAroundTarget(origin, radius));
    }
    public Vector3 FindNewRandomPoint(Vector3 origin, float radius)
    {
        Vector3 newPos = RandomNavSphere(origin, radius, agent.areaMask);

        return newPos;
    }
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;


        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
    #endregion

    public void MoveDirection()
    {
        moveDirection = agent.velocity;

        float x = Vector3.Dot(transform.right, moveDirection);
        float y = Vector3.Dot(transform.forward, moveDirection);

        animator.SetFloat("x", x, 0.05f, Time.deltaTime);
        animator.SetFloat("y", y, 0.05f, Time.deltaTime);
        animator.SetFloat("velocity", /*moveDirection.normalized.sqrMagnitude*/ agent.velocity.magnitude, 0.2f, Time.deltaTime);
    }

    public void SetTargetDistance()
    {
        targetVisible = sensor.GetNearest();
        targetDistance = Vector3.Distance(target.transform.position, transform.position);

        animator.SetBool("targetVisible", targetVisible);
        animator.SetFloat("targetDistance", targetDistance);
    }

    public void LookAtTarget(float rotationSpeed = 0.1f)
    {
        if(lookingAtTarget)
        {
            Vector3 lookDirection = target.transform.position - transform.position;
            lookDirection.y = transform.position.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), rotationSpeed);
        }
    }
}
