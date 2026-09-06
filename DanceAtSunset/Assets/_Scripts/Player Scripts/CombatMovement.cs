using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CombatMovement : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;

    [SerializeField] private float speed = 30.0f;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    public CharacterController character;

    public Camera Camera;

    public bool canMove = true;

    [SerializeField] private float rotationSmoothTime = 0.1f;
    float currentAngle;
    float currentAngleVelocity;

    //dash 
    public float dashSpeed = 80f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 0.1f;
    [SerializeField] private AnimationCurve dashCurve;

    private bool isDashing = false;
    private bool canDash = true;

    [Header("Dash Feedback")]
    [SerializeField] private float dashFOVIncrease = 6f;
    [SerializeField] private float dashFOVInTime = 0.05f;
    [SerializeField] private float dashFOVOutTime = 0.15f;

    private float normalFOV;
    private Vector2 movementDirection;

    private void OnEnable()
    {
        PotionManager.OnSpellCast += DisableMovementOnCast;

        inputHandler.PlayerInput.Combat.Move.performed += SetMovementDirection;
        inputHandler.PlayerInput.Combat.Move.canceled += SetMovementDirection;
        inputHandler.PlayerInput.Combat.Dash.performed += Dash;
    }

    private void OnDisable()
    {
        PotionManager.OnSpellCast -= DisableMovementOnCast;

        inputHandler.PlayerInput.Combat.Move.performed -= SetMovementDirection;
        inputHandler.PlayerInput.Combat.Move.canceled -= SetMovementDirection;
        inputHandler.PlayerInput.Combat.Dash.performed -= Dash;
    }

    void Start()
    {
        character = GetComponent<CharacterController>();
        Camera = Camera.main;

        currentAngle = transform.eulerAngles.y;

        if (cinemachineCamera != null)
            normalFOV = cinemachineCamera.Lens.FieldOfView;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            HandleMovement();

            if (Input.GetKey(KeyCode.Space))
                Cursor.lockState = CursorLockMode.None;
            if (Input.GetKey(KeyCode.Escape))
                Cursor.lockState = CursorLockMode.Locked;

           
        }
    }

    private void SetMovementDirection(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        if (isDashing) return;

        if (movementDirection.normalized.magnitude >= 0.1f)
        {
            // Calculate move direction
            float targetAngle =
                Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg
                + Camera.transform.eulerAngles.y;

            // Smoothly rotate player toward movement direction
            currentAngle = Mathf.SmoothDampAngle(
                currentAngle,
                targetAngle,
                ref currentAngleVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0, currentAngle, 0);

            // Movement uses the TARGET direction, not the smoothed rotation
            Vector3 moveDirection =
                Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            character.Move(moveDirection * speed * Time.deltaTime);
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!canMove)
            return;

        if (isDashing || !canDash)
            return;

        if (movementDirection.normalized.magnitude >= 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg
                + Camera.transform.eulerAngles.y;

            Vector3 dashDirection =
                Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            StartCoroutine(Dash(dashDirection));
        }
        else
        {
            StartCoroutine(Dash(transform.forward));
        }
    }

    private IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;
        canDash = false;

        MusicManager.Instance.PlayDashSfx();
        StartCoroutine(DashFOV());

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / dashDuration;

            float dashMultiplier = dashCurve.Evaluate(t);

            character.Move(direction * dashSpeed * dashMultiplier * Time.deltaTime);

            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private IEnumerator DashFOV()
    {
        if (cinemachineCamera == null)
            yield break;

        float dashFOV = normalFOV + dashFOVIncrease;

        float elapsed = 0f;

        // Increase FOV when dashing
        while (elapsed < dashFOVInTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / dashFOVInTime;

            cinemachineCamera.Lens.FieldOfView =
                Mathf.Lerp(normalFOV, dashFOV, t);

            yield return null;
        }

        elapsed = 0f;

        // Return to normal
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

    public float getSpeed()
    {
        return speed;
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void DisableMovementOnCast(Spell spell)
    {
        StartCoroutine(DisableMovementOnCastCoroutine(spell.spellAbility.castDuration));
    }

    public IEnumerator DisableMovementOnCastCoroutine(float duration)
    {
        float pSpeed = getSpeed();
        setSpeed(0f);

        // Wait until the cast duartion is up
        yield return new WaitForSeconds(duration);

        // Return speed
        setSpeed(pSpeed);
    }
}
