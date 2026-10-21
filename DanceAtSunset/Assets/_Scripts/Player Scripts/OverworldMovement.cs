using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldMovement : MonoBehaviour
{
    public static Action OnSprintStarted;
    public static Action OnSprintEnded;
    [SerializeField] private InputHandler inputHandler;

    [Header("Overworld Movement")]
    public float speed = 10f;
    public float dashSpeed = 17f;
    public bool isDashing = false;
    private bool canMove = true;

    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float deceleration = 45f;
    [SerializeField] private float directionChangeSpeed = 60f;

    private Vector3 currentMoveVelocity;

    [Header("Overworld Jump")]
    [SerializeField] private bool enableJump = true;
    [SerializeField] private float jumpHeight = 0.2f;
    [SerializeField] private float jumpGravity = 20f;
    

    private float verticalVelocity;

    public CharacterController character;
    public Camera Camera;

    [SerializeField] float rotationSmoothTime;
    float currentAngle;
    float currentAngleVelocity;
    private Vector2 movementDirection;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float walkingStepInterval = 0.5f;
    [SerializeField] private float sprintingStepInterval = 0.3f;
    [SerializeField] private float minimumFootstepSpeed = 1f;
    [SerializeField] private float footstepVolume = 0.7f;

    private float footstepTimer;

    public Vector2 CurrentMovementDirection => movementDirection;

    private void OnEnable()
    {
        // Restore player input after combat
        inputHandler.PlayerInput.Overworld.Enable();

        inputHandler.PlayerInput.Overworld.Move.performed += SetMovementDirection;
        inputHandler.PlayerInput.Overworld.Move.canceled += SetMovementDirection;
        inputHandler.PlayerInput.Overworld.Sprint.performed += Sprint;
        inputHandler.PlayerInput.Overworld.Sprint.canceled += Sprint;
        inputHandler.PlayerInput.Overworld.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        inputHandler.PlayerInput.Overworld.Move.performed -= SetMovementDirection;
        inputHandler.PlayerInput.Overworld.Move.canceled -= SetMovementDirection;
        inputHandler.PlayerInput.Overworld.Sprint.performed -= Sprint;
        inputHandler.PlayerInput.Overworld.Sprint.canceled -= Sprint;
        inputHandler.PlayerInput.Overworld.Jump.performed -= Jump;

        inputHandler.PlayerInput.Overworld.Disable();
    }

    void Start()
    {
        character = GetComponent<CharacterController>();
        currentAngle = transform.eulerAngles.y;
        Camera = Camera.main;
    }

    private void Update()
    {
        UpdateFootsteps();

        if (Input.GetKeyDown(KeyCode.Tilde))
            Cursor.lockState = CursorLockMode.None;

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }      

    private void SetMovementDirection(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isDashing)
                return;

            isDashing = true;
            OnSprintStarted?.Invoke();
        }
        else if (context.canceled)
        {
            if (!isDashing)
                return;

            isDashing = false;
            OnSprintEnded?.Invoke();
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!enableJump || !canMove || !character.isGrounded)
            return;

        verticalVelocity = Mathf.Sqrt(2f * jumpGravity * jumpHeight);
    }

        private void HandleMovement()
    {
        if (!canMove)
        {
            currentMoveVelocity = Vector3.zero;
            isDashing = false;
            verticalVelocity = 0f;
            footstepTimer = 0f;
            return;
        }

        float deltaTime = Time.fixedDeltaTime;

        // Gravity and grounded handling
        if (character.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= jumpGravity * deltaTime;
        }

        // Calculate desired horizontal movement
        Vector3 targetMoveVelocity = Vector3.zero;
        bool hasMovementInput = movementDirection.magnitude >= 0.1f;

        if (hasMovementInput)
        {
            float targetAngle =
                Mathf.Atan2(movementDirection.x, movementDirection.y)
                * Mathf.Rad2Deg
                + Camera.transform.eulerAngles.y;

            Vector3 moveDirection =
                Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            currentAngle = Mathf.SmoothDampAngle(
                currentAngle,
                targetAngle,
                ref currentAngleVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);

            float targetSpeed = isDashing ? dashSpeed : speed;
            targetMoveVelocity = moveDirection * targetSpeed;
        }

        // Choose acceleration, stopping, or direction change rate
        float changeRate = hasMovementInput ? acceleration : deceleration;

        if (hasMovementInput &&
            currentMoveVelocity.sqrMagnitude > 0.01f &&
            Vector3.Dot(
                currentMoveVelocity.normalized,
                targetMoveVelocity.normalized
            ) < 0.95f)
        {
            changeRate = directionChangeSpeed;
        }

        // Smoothly approach the target horizontal velocity
        currentMoveVelocity = Vector3.MoveTowards(
            currentMoveVelocity,
            targetMoveVelocity,
            changeRate * deltaTime
        );

        // One call for horizontal and vertical movement
        Vector3 movement =
            currentMoveVelocity + Vector3.up * verticalVelocity;

        character.Move(movement * deltaTime);
    }

    // For area transitioning
    public void ContinueMovement(Vector2 direction)
    {
        movementDirection = direction;
    }

    public void SetTransitionMovement(bool active, Vector2 direction)
    {
        if (active)
            movementDirection = direction;
        else
            movementDirection = Vector2.zero;
    }

    private void UpdateFootsteps()
    {
        if (!canMove ||
            isDashing && currentMoveVelocity.magnitude < minimumFootstepSpeed ||
            character == null ||
            !character.isGrounded ||
            verticalVelocity > 0f ||
            currentMoveVelocity.magnitude < minimumFootstepSpeed)
        {
            footstepTimer = 0f;
            return;
        }

        if (footstepAudioSource == null ||
            footstepClips == null ||
            footstepClips.Length == 0)
        {
            return;
        }

        // Map walking speed to 0 and sprint speed to 1
        float sprintProgress = Mathf.InverseLerp(
            speed,
            Mathf.Max(speed, dashSpeed),
            currentMoveVelocity.magnitude
        );

        float stepInterval = Mathf.Lerp(
            walkingStepInterval,
            sprintingStepInterval,
            sprintProgress
        );

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= stepInterval)
        {
            PlayFootstep();
            footstepTimer -= stepInterval;
        }
    }

    private void PlayFootstep()
    {

        AudioClip clip = footstepClips[
            UnityEngine.Random.Range(0, footstepClips.Length)
        ];

        footstepAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

        footstepAudioSource.PlayOneShot(clip, footstepVolume);
    }

    public InputHandler GetInputHandler() { return inputHandler; }

    public void ToggleMovement(bool val) { canMove = val; }

    
}

