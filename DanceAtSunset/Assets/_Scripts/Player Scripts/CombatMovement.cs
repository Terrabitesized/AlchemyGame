using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CombatMovement : MonoBehaviour, IInvulnerable
{

    public event Action OnInvulnerabilityStarted;
    public event Action OnInvulnerabilityEnded;

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
    public bool IsInvulnerable { get; private set; }

    private bool isDashing = false;
    private bool canDash = true;

    [Header("Dash Feedback")]
    [SerializeField] private float dashFOVIncrease = 6f;
    [SerializeField] private float dashFOVInTime = 0.05f;
    [SerializeField] private float dashFOVOutTime = 0.15f;

    private float normalFOV;
    private Vector2 movementDirection;

    [Header("Player Facing")]
    [SerializeField] private bool faceCameraDirection = false;
    private void OnEnable()
    {
        //PotionManager.OnSpellCast += DisableMovementOnCast;

        inputHandler.PlayerInput.Combat.Move.performed += SetMovementDirection;
        inputHandler.PlayerInput.Combat.Move.canceled += SetMovementDirection;
        inputHandler.PlayerInput.Combat.Dash.performed += Dash;
    }

    private void OnDisable()
    {
        //PotionManager.OnSpellCast -= DisableMovementOnCast;

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
            // Calculate movement direction relative to the camera
            float moveAngle =
                Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg
                + Camera.transform.eulerAngles.y;

            Vector3 moveDirection =
                Quaternion.Euler(0, moveAngle, 0) * Vector3.forward;

            // Decide which direction the player should face
            float facingAngle;

            if (faceCameraDirection)
            {
                // Face the direction the camera is looking
                Vector3 cameraForward = Camera.transform.forward;
                cameraForward.y = 0f;
                cameraForward.Normalize();

                if (cameraForward.sqrMagnitude > 0.001f)
                {
                    facingAngle = Mathf.Atan2(
                        cameraForward.x,
                        cameraForward.z
                    ) * Mathf.Rad2Deg;
                }
                else
                {
                    facingAngle = transform.eulerAngles.y;
                }
            }
            else
            {
                // Face movement direction
                facingAngle = moveAngle;
            }

            // Smoothly rotate player
            currentAngle = Mathf.SmoothDampAngle(
                currentAngle,
                facingAngle,
                ref currentAngleVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0, currentAngle, 0);

            // Movement remains independent of player facing
            character.Move(
                moveDirection * speed * Time.deltaTime
            );
        }
    
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!canMove)
            return;

        if (isDashing || !canDash)
            return;

        Vector3 dashDirection;

        if (faceCameraDirection)
        {
            // Dash in the direction the player is facing
            dashDirection = transform.forward;
        }
        else if (movementDirection.normalized.magnitude >= 0.1f)
        {
            // Dash in the movement direction
            float targetAngle =
                Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg
                + Camera.transform.eulerAngles.y;

            dashDirection =
                Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
        else
        {
            // No movement input, so dash forward
            dashDirection = transform.forward;
        }

        StartCoroutine(Dash(dashDirection));
    }

    private IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;
        canDash = false;

        OnInvulnerabilityStarted?.Invoke();
        IsInvulnerable = true;

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

        IsInvulnerable = false;
        OnInvulnerabilityEnded?.Invoke();

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
