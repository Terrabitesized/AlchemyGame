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

    public float speed = 10f;
    public float dashSpeed = 17f;
    public bool isDashing = false;
    private bool canMove = true;

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
        Camera = Camera.main;
    }

    private void FixedUpdate()
    {

        HandleMovement();

        if (Input.GetKey(KeyCode.Tilde))
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.Locked;
    }      

    private void SetMovementDirection(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isDashing = true;

            if(canMove && movementDirection.magnitude >= .1f)
                StartCoroutine(DashFOV());
        }
        else if (context.canceled)
            isDashing = false;
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
            isDashing = false;
            return;
        }

        // Gravity
        if (character.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= jumpGravity * Time.deltaTime;
        }

        Vector3 movement = Vector3.up * verticalVelocity;


        // Always apply vertical movement
        //character.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        if (movementDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentAngleVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 rotatedMovement = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            if (isDashing)
                character.Move(rotatedMovement * dashSpeed * Time.deltaTime);
            else
                character.Move(rotatedMovement * speed * Time.deltaTime);
        }

        // One Move call for both horizontal and vertical movement
        character.Move(movement * Time.deltaTime);
    }

    private IEnumerator DashFOV()
    {
        OnSprintStarted?.Invoke();

        yield return new WaitUntil(() => !isDashing);

        OnSprintEnded?.Invoke();
    }

    public InputHandler GetInputHandler() { return inputHandler; }

    public void ToggleMovement(bool val) { canMove = val; }

    
}

