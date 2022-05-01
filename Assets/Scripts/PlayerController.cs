using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : BasicCharacterController
{
    [HideInInspector] public PlayerState state;
    private HUDManager hudManager;
    private GameEventManager gameEventManager;

    private GameObject enemy;
    private Transform target;
    private Vector3 lockedMoveDir;
    private IEnumerator hitboxCoroutine;
    [SerializeField] private BoxCollider swordCollider;

    private float x, z;

    private bool lockedOn;

    public override void Start()
    {
        base.Start();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        hudManager = HUDManager.Instance;
        gameEventManager = GameEventManager.Instance;
    }

    public override void Update()
    {
        base.Update();
        DodgeControls();
    }
    public override void MoveControls()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        inputDirection = new Vector3(x, 0f, z);
        velocity = inputDirection.normalized.sqrMagnitude;

        if(lockedOn)
        {
            if (velocity >= 0.01f)
            {
                if(target != null)
                {
                    transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                }

                lockedMoveDir = transform.forward * z + transform.right * x;
                moveDirection = lockedMoveDir;
                if (!animator.applyRootMotion) controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            }

            if(target != null)
            {
                hudManager.MoveLockOnReticle(mainCam.WorldToScreenPoint(target.position + Vector3.up * 1.75f));
            }

        }
        else
        {
            if (velocity >= 0.01f)
            {
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothDampMultiplier);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                if (!animator.applyRootMotion) controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
        }

        animator.SetFloat("x", x);
        animator.SetFloat("z", z);
        animator.SetFloat("velocity", velocity, 0.05f, Time.deltaTime);
    }

    public override void CombatControls()
    {
        if(Input.GetButtonDown("Attack"))
        {
            if(lockedOn)
            {
                transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), 0.15f);
            }
            animator.SetTrigger("attack");
        }
        if (Input.GetButtonDown("HeavyAttack"))
        {
            if (lockedOn)
            {
                transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), 0.15f);
            }
            animator.SetTrigger("heavyAttack");
        }


        if(Input.GetButtonDown("LockOn"))
        {
            if(!lockedOn)
            {
                if (enemy != null) target = enemy.transform;
                lockedOn = true;
            }
            else
            {
                target = null;
                lockedOn = false;
            }
            gameEventManager.LockOn(lockedOn);
            animator.SetBool("lockedOn", lockedOn);
        }
    }

    private void DodgeControls()
    {
        if(!lockedOn)
        {
            if (Input.GetButtonDown("Dodge"))
            {
                animator.SetTrigger("dodge");
            }
        }
        else
        {
            if (Input.GetButtonDown("Dodge"))
            {
                float roundX = Mathf.Round(x);
                float roundZ = Mathf.Round(z);

                animator.SetFloat("dodgeX", roundX);
                animator.SetFloat("dodgeZ", roundZ);
                animator.SetTrigger("lockOnDodge");
            }

        }
    }

    public void StartActivateHitboxCoroutine(float duration = 0.15f)
    {
        if (hitboxCoroutine != null) StopCoroutine(hitboxCoroutine);
        hitboxCoroutine = ActivateHitbox(duration);
        StartCoroutine(hitboxCoroutine);
    }

    private IEnumerator ActivateHitbox(float duration = 0.15f)
    {
        swordCollider.enabled = true;
        while(duration > 0f)
        {
            duration -= Time.deltaTime;
            yield return null; 
        }
        swordCollider.enabled = false;
    }
}
