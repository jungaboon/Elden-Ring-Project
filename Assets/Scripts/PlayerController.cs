using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : BasicCharacterController
{
    [HideInInspector] public PlayerState state;
    private HUDManager hudManager;
    private GameEventManager gameEventManager;
    private PlayerHealth health;

    private GameObject enemy;
    private Transform target;
    private Vector3 lockedMoveDir;
    private IEnumerator hitboxCoroutine;
    private IEnumerator attackDodgeCoroutine;
    [SerializeField] private BoxCollider swordCollider;

    private float x, z;
    private float defControllerHeight;
    private Vector3 defControllerPos;
    private WaitForSeconds dodgeCooldown = new WaitForSeconds(0.2f);
    private WaitForSeconds invincibilityDelay = new WaitForSeconds(0.25f);

    private bool lockedOn;
    private bool canDodge;
    [HideInInspector] public bool canPressDodge;
    [HideInInspector] public bool canRotate;

    public override void Start()
    {
        base.Start();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        hudManager = HUDManager.Instance;
        gameEventManager = GameEventManager.Instance;
        health = GetComponent<PlayerHealth>();

        defControllerHeight = controller.height;
        defControllerPos = controller.center;

        gameEventManager.LockOn(false);
        canDodge = true;
        canRotate = true;
    }

    public override void Update()
    {
        base.Update();
        DodgeControls();
        MiscControls();
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
                if(canRotate) transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                if (!animator.applyRootMotion) controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
        }

        animator.SetFloat("x", x, 0.1f, Time.deltaTime);
        animator.SetFloat("z", z, 0.1f, Time.deltaTime);
        animator.SetFloat("velocity", velocity, 0.1f, Time.deltaTime);
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

    private void MiscControls()
    {
        // TO DO: Make this just based on the directional pad
        // Maybe add some sort of icon that appears?
        if(Input.GetButtonDown("Taunt"))
        {
            int tauntType = Random.Range(0, 6);
            animator.SetFloat("tauntType", (float)tauntType);
            animator.SetTrigger("taunt");
        }
    }

    private void DodgeControls()
    {
        if (!canDodge || !canPressDodge) return;
        if(!lockedOn)
        {
            if (Input.GetButtonDown("Dodge"))
            {
                animator.Play("Dodge");
                StartCoroutine(DodgeCooldown());
            }
        }
        else
        {
            if (Input.GetButtonDown("Dodge"))
            {
                animator.SetFloat("dodgeX", Mathf.Round(x));
                animator.SetFloat("dodgeZ", Mathf.Round(z));
                animator.Play("Lock On Dodge");
                StartCoroutine(DodgeCooldown());
            }

        }
    }
    private IEnumerator DodgeCooldown()
    {
        canDodge = false;
        controller.height = 1f;
        controller.center = new Vector3(0f, 0.5f, 0f);
        health.canTakeDamage = false;
        yield return invincibilityDelay;
        health.canTakeDamage = true;
        yield return dodgeCooldown;
        DOTween.To(() => controller.height, x => controller.height = x, defControllerHeight, 0.15f);
        DOTween.To(() => controller.center, x => controller.center = x, defControllerPos, 0.15f);
        canDodge = true;
    }
    public void StartAttackDodgeCooldown(float duration)
    {
        if (attackDodgeCoroutine != null) StopCoroutine(attackDodgeCoroutine);
        attackDodgeCoroutine = AttackDodgeCooldown(duration);
        StartCoroutine(attackDodgeCoroutine);
    }
    private IEnumerator AttackDodgeCooldown(float duration)
    {
        canPressDodge = false;
        yield return new WaitForSeconds(duration);
        canPressDodge = true;
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
