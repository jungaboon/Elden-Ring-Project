using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class BasicCharacterController : MonoBehaviour
{
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Camera mainCam;
    [HideInInspector] public Transform cam;
    [HideInInspector] public Animator animator;
    public LayerMask groundMask;
    public LayerMask enemyMask;

    [HideInInspector] public Vector3 inputDirection;
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public Vector3 playerVelocity;
    public Vector3 drag;

    public bool faceCameraDirection;
    public float moveSpeed = 3f;
    public float turnSpeed = 0.2f;
    public float smoothDampMultiplier = 0.2f;
    public float defaultStepOffset = 0.3f;
    public float gravity = -9.8f;
    public float jumpHeight = 5f;
    public float dashDistance = 5f;

    [HideInInspector] public float turnSmoothVelocity;
    [HideInInspector] public float velocity;
    [HideInInspector] public bool grounded, previouslyGrounded;

    public virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        mainCam = Camera.main;
        cam = mainCam.transform;
        controller.stepOffset = defaultStepOffset;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public virtual void Update()
    {
        GroundCheck();
        MoveControls();
        CombatControls();
    }

    public virtual void MoveControls()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        velocity = inputDirection.normalized.sqrMagnitude;

        if (velocity >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothDampMultiplier);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (!animator.applyRootMotion) controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        animator.SetFloat("velocity", velocity, 0.05f, Time.deltaTime);
    }
    public virtual void CombatControls()
    {

    }

    public virtual void GroundCheck()
    {
        grounded = Physics.CheckSphere(transform.position, 0.2f, groundMask);

        if(grounded)
        {
            playerVelocity.y += -2f * Time.deltaTime;
            controller.stepOffset = defaultStepOffset;
        }
        else
        {
            controller.stepOffset = 0f;
        }

        playerVelocity.y += gravity * Time.deltaTime;

        playerVelocity.x /= 1 + drag.x * Time.deltaTime;
        playerVelocity.y /= 1 + drag.y * Time.deltaTime;
        playerVelocity.z /= 1 + drag.z * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
        animator.SetBool("grounded", grounded);
        if (grounded && !previouslyGrounded) animator.Play("Jump Land");
        previouslyGrounded = grounded;
    }
    public virtual void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.Play("Start Jump");
        }
    }
    public virtual void ApplyDirectionForce(Vector3 direction = default(Vector3))
    {
        playerVelocity += Vector3.Scale(direction, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * drag.x + 1)) / -Time.deltaTime), 0f, (Mathf.Log(1f / (Time.deltaTime * drag.z + 1)) / -Time.deltaTime)));
    }
}
