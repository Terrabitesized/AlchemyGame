using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldMovement : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;

    public float speed = 10f;
    public float dashSpeed = 17f;
    public bool isDashing = false;

    public CharacterController character;
    public Camera Camera;

    [SerializeField] float rotationSmoothTime;
    float currentAngle;
    float currentAngleVelocity;

    [Header("Dash Feedback")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float dashFOVIncrease = 6f;
    [SerializeField] private float dashFOVInTime = 0.05f;
    [SerializeField] private float dashFOVOutTime = 0.15f;

    private float normalFOV;
    private Vector2 movementDirection;

    private void OnEnable()
    {
        inputHandler.PlayerInput.Overworld.Move.performed += SetMovementDirection;
        inputHandler.PlayerInput.Overworld.Move.canceled += SetMovementDirection;
        inputHandler.PlayerInput.Overworld.Sprint.performed += Sprint;
        inputHandler.PlayerInput.Overworld.Sprint.canceled += Sprint;
    }

    private void OnDisable()
    {
        inputHandler.PlayerInput.Overworld.Move.performed -= SetMovementDirection;
        inputHandler.PlayerInput.Overworld.Move.canceled -= SetMovementDirection;
        inputHandler.PlayerInput.Overworld.Sprint.performed -= Sprint;
        inputHandler.PlayerInput.Overworld.Sprint.canceled -= Sprint;
    }

    void Start()
    {
        character = GetComponent<CharacterController>();
        Camera = Camera.main;

        if (cinemachineCamera != null)
            normalFOV = cinemachineCamera.Lens.FieldOfView;
    }

    private void FixedUpdate()
    {
        HandleMovement();

        if (Input.GetKey(KeyCode.Space))
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

            if(movementDirection.magnitude >= .1f)
                StartCoroutine(DashFOV());
        }
        else if (context.canceled)
            isDashing = false;
    }

    private void HandleMovement()
    {
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
    }

    private IEnumerator DashFOV()
    {
        if (cinemachineCamera == null)
            yield break;

        float dashFOV = normalFOV + dashFOVIncrease;

        float elapsed = 0f;

        // FOV expands
        while (elapsed < dashFOVInTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / dashFOVInTime;

            cinemachineCamera.Lens.FieldOfView =
                Mathf.Lerp(normalFOV, dashFOV, t);

            yield return null;
        }

        // Stay zoomed while sprinting
        while (isDashing)
        {
            yield return null;
        }

        // FOV returns to normal
        elapsed = 0f;

        while (elapsed < dashFOVOutTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / dashFOVOutTime;

            cinemachineCamera.Lens.FieldOfView =
                Mathf.Lerp(dashFOV, normalFOV, t);

            yield return null;
        }

        cinemachineCamera.Lens.FieldOfView = normalFOV;
    }


}

