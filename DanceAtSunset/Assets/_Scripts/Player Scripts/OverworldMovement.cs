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
    private bool canMove = true;

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
        // Restore player input after combat
        inputHandler.PlayerInput.Overworld.Enable();

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

        inputHandler.PlayerInput.Overworld.Disable();
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

            if(canMove && movementDirection.magnitude >= .1f)
                StartCoroutine(DashFOV());
        }
        else if (context.canceled)
            isDashing = false;
    }

    private void HandleMovement()
    {
        if (!canMove)
        {
            isDashing = false;
            return;
        }

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
        Coroutine fovCoroutine = StartCoroutine(AnimateCameraFOVCoroutine(dashFOV, dashFOVInTime));

        yield return new WaitUntil(() => !isDashing);

        StopCoroutine(fovCoroutine);
        StartCoroutine(ResetCameraFOVCoroutine(dashFOVOutTime));
    }

    public InputHandler GetInputHandler() { return inputHandler; }

    public void ToggleMovement(bool val) { canMove = val; }

    public void AnimateCameraFOV(float fovChange, float animationDuration)
    { StartCoroutine(AnimateCameraFOVCoroutine(fovChange, animationDuration)); }

    private IEnumerator AnimateCameraFOVCoroutine(float fovChange, float animationDuration)
    {
        float elapsed = 0f;

        while (elapsed < dashFOVInTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / animationDuration;

            cinemachineCamera.Lens.FieldOfView =
                Mathf.Lerp(normalFOV, fovChange, t);

            yield return null;
        }
    }

    public void ResetCameraFOV(float animationDuration)
    { StartCoroutine(ResetCameraFOVCoroutine(animationDuration)); }

    private IEnumerator ResetCameraFOVCoroutine(float animationDuration)
    {
        float elapsed = 0f;
        float currentFOV = cinemachineCamera.Lens.FieldOfView;

        while (elapsed < dashFOVInTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / animationDuration;

            cinemachineCamera.Lens.FieldOfView =
                Mathf.Lerp(currentFOV, normalFOV, t);

            yield return null;
        }
    }
}

