using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using Alchemy.Inspector;

public class CombatMovement : MonoBehaviour, IInvulnerable
{
    public static CombatMovement Instance;
    [SerializeField] private GameObject playerModel;

    [Header("Casting Parameters")]
    [SerializeField] private bool freezeOnSpellCast = true;
    [ShowIf(nameof(freezeOnSpellCast))][SerializeField] private float castingFreezeDuration = .5f;
    [ShowIf(nameof(freezeOnSpellCast))][SerializeField] private AnimationCurve castRotationCurve;
    private bool isRotatingFromCast = false;

    public event Action OnInvulnerabilityStarted;
    public event Action OnInvulnerabilityEnded;

    [SerializeField] private InputHandler inputHandler;

    [SerializeField] private float speed = 30.0f;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachineInputAxisController cameraInput;
    [SerializeField] private CinemachineOrbitalFollow follow;

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

    [Header("Acceleration")]
    // How quick to get to full speed:
    // Normal speed is 30, accell is 90 - it takes 30/90 of a second to reach full speed
    [SerializeField] private float acceleration = 90f;
    // Comes to a stop in 30/120 of a second
    [SerializeField] private float deceleration = 120f;
    // How quickly to change direction when already moving
    [SerializeField] private float directionChangeSpeed = 150f;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepClips;

    [SerializeField] private float slowStepInterval = 0.55f;
    [SerializeField] private float fastStepInterval = 0.25f;
    [SerializeField] private float minimumFootstepSpeed = 1f;
    [SerializeField] private float footstepVolume = 0.7f;

    private float footstepTimer;

    private Vector3 currentMoveVelocity;

    private bool isDashing = false;
    private bool canDash = true;

    [Header("Dash Feedback")]
    [SerializeField] private float dashFOVIncrease = 6f;
    [SerializeField] private float dashFOVInTime = 0.05f;
    [SerializeField] private float dashFOVOutTime = 0.15f;

    private float normalFOV;
    private Vector2 movementDirection;

    // Hop variables
    [Header("Combat Hop")]
    [SerializeField] private bool enableHopping = true;
    [SerializeField] private float hopHeight = 0.75f;
    [SerializeField] private float hopGravity = 30f;
    [SerializeField] private float hopDistance = 1.25f;
    [SerializeField] private float hopDuration = 0.3f;
    [SerializeField]
    private AnimationCurve hopCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Spell Cast Hop")]
    [SerializeField] private AnimationCurve castHopHeightCurve;
    [SerializeField] private float castMovementSpeedReduction = .8f;
    private bool isCasting = false;
    private bool hasLandedAfterCast = false;

    [Header("Combat Jump")]
    [SerializeField] private bool enableJump = true;
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float jumpGravity = 30f;


    // How big is the spin
    [Header("Hop Spin")]
    [SerializeField] private float hopSpinAmount = 360f;

    private float verticalVelocity;
    private bool isHopping;
    private Coroutine hopCoroutine;

    [Header("Player Facing")]
    [SerializeField] private bool faceCameraDirection = false;
    [SerializeField] private Transform playerVisual;

    private void OnEnable()
    {
        CombatManager.OnSpellCast += RotateOnCast;

        CombatManager.OnSpellCast += CastHop; // swap these
        //IngredientScript.OnCrystalCollected += IngredientHop;


        inputHandler.PlayerInput.Combat.Move.performed += SetMovementDirection;
        inputHandler.PlayerInput.Combat.Move.canceled += SetMovementDirection;
        inputHandler.PlayerInput.Combat.Dash.performed += Dash;
        inputHandler.PlayerInput.Combat.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        CombatManager.OnSpellCast -= RotateOnCast;

        CombatManager.OnSpellCast -= CastHop;
        //IngredientScript.OnCrystalCollected -= IngredientHop;


        inputHandler.PlayerInput.Combat.Move.performed -= SetMovementDirection;
        inputHandler.PlayerInput.Combat.Move.canceled -= SetMovementDirection;
        inputHandler.PlayerInput.Combat.Dash.performed -= Dash;
        inputHandler.PlayerInput.Combat.Jump.performed -= Jump;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        character = GetComponent<CharacterController>();
        Camera = Camera.main;

        currentAngle = playerModel.transform.eulerAngles.y;

        if (cinemachineCamera != null)
            normalFOV = cinemachineCamera.Lens.FieldOfView;
    }

    private void Update()
    {
        // REMOVE THIS IS FOR REBUG
        if(Input.GetKeyDown(KeyCode.H))
            follow.HorizontalAxis.Value = 0f;

        UpdateFootsteps();
    }

    private void FixedUpdate()
    {
        if (!CombatManager.Instance.CombatFlow)
            return;

        if (canMove)
        {
            HandleMovement();

            if (Input.GetKey(KeyCode.Tilde))
                Cursor.lockState = CursorLockMode.None;
            if (Input.GetKey(KeyCode.Escape))
                Cursor.lockState = CursorLockMode.Locked;
            if (Input.GetKey(KeyCode.LeftControl))
                faceCameraDirection = !faceCameraDirection;
        }
    }

    private void SetMovementDirection(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        if (!canMove)
            return;

        if(!hasLandedAfterCast)
        {
            verticalVelocity -= jumpGravity * .3f * Time.deltaTime;

            if(character.isGrounded)
                hasLandedAfterCast = true;
        }
        else
        {
            // Gravity
            if (character.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity -= jumpGravity * Time.deltaTime;
            }
        }

        if (isDashing)
            return;

        Vector3 targetMoveVelocity = Vector3.zero;
        bool hasMovementInput = movementDirection.magnitude >= 0.1f;

        if (hasMovementInput)
        {
            float moveAngle = Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;

            Vector3 moveDirection = Quaternion.Euler(0f, moveAngle, 0f) * Vector3.forward;

            targetMoveVelocity = moveDirection * speed;

            float facingAngle;

            if (faceCameraDirection)
            {
                Vector3 cameraForward = Camera.transform.forward;
                cameraForward.y = 0f;
                cameraForward.Normalize();

                facingAngle = cameraForward.sqrMagnitude > 0.001f
                    ? Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg
                    : transform.eulerAngles.y;
            }
            else
            {
                facingAngle = moveAngle;
            }

            currentAngle = Mathf.SmoothDampAngle(currentAngle, facingAngle, ref currentAngleVelocity, rotationSmoothTime);

            if (!isRotatingFromCast)
                playerModel.transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
            else
                currentAngleVelocity = 0f;
        }

        // Smoothly build speed, redirect momentum, or come to a stop
        float changeRate = hasMovementInput ? acceleration : deceleration;

        // Allow direction changes to have their own tuning
        // Compares directions. If the change is large enough, use the directionChangeSpeed instead of acceleration
        if (hasMovementInput &&
            currentMoveVelocity.sqrMagnitude > 0.01f &&
            Vector3.Dot(currentMoveVelocity.normalized,
                         targetMoveVelocity.normalized) < 0.95f)
        {
            changeRate = directionChangeSpeed;
        }

        // Movetowards moves us by a limited amount towards another vector
        currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, targetMoveVelocity, changeRate * Time.deltaTime);

        Vector3 movement = currentMoveVelocity + Vector3.up * verticalVelocity;

        character.Move(movement * Time.deltaTime);
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!canMove || isCasting)
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

        CombatSFXManager.Instance.PlayDashSfx();
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

        yield return GameFlowUtility.WaitForGameplaySeconds(dashCooldown);

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

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(
            $"JUMP | phase: {context.phase} | grounded: {character.isGrounded} | " +
            $"canMove: {canMove} | isDashing: {isDashing} | " +
            $"verticalVelocity: {verticalVelocity}"
        );

        if (!enableJump || !canMove || isDashing)
            return;

        if (!character.isGrounded)
            return;

        verticalVelocity = Mathf.Sqrt(2f * jumpGravity * jumpHeight);

        Debug.Log($"JUMP ACCEPTED | velocity: {verticalVelocity}");
    }

    private void IngredientHop()
    {
        IngredientHop(new CombatCrystal());
    }

    private void IngredientHop(CombatCrystal ingredient)
    {
        if (!enableHopping)
            return;

        if (!canMove || isDashing)
            return;

        // Calculate the upward velocity needed to reach hopHeight
        verticalVelocity = Mathf.Sqrt(2f * hopGravity * hopHeight);

        // Restart the horizontal hop
        if (hopCoroutine != null)
        {
            StopCoroutine(hopCoroutine);
        }

        hopCoroutine = StartCoroutine(Hop());
    }

    private IEnumerator Hop()
    {
        isHopping = true;

        Vector3 hopDirection = Camera.transform.forward;
        hopDirection.y = 0f;
        hopDirection.Normalize();

        float elapsed = 0f;
        float previousT = 0f;

        // Spin direction for player visual
        float spinDirection =
            UnityEngine.Random.value < 0.5f ? -1f : 1f;

        while (elapsed < hopDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / hopDuration);

            // Use animation curve to determine horizontal movement
            float currentHorizontalT = hopCurve.Evaluate(t);
            float previousHorizontalT = hopCurve.Evaluate(previousT);

            float horizontalDelta =
                currentHorizontalT - previousHorizontalT;

            // actually move player
            character.Move(hopDirection * hopDistance * horizontalDelta);

            // Visual spin
            if (playerVisual != null)
            {
                float spinAngle =
                    hopSpinAmount * spinDirection * t;

                playerVisual.localRotation =
                    Quaternion.Euler(0f, spinAngle, 0f);
            }

            previousT = t;

            yield return null;
        }

        if (playerVisual != null)
        {
            playerVisual.localRotation = Quaternion.identity;
        }

        isHopping = false;
        hopCoroutine = null;
    }

    private void CastHop()
    {
        StartCoroutine(CastHopCoroutine());
    }

    private IEnumerator CastHopCoroutine()
    {
        isCasting = true;
        CombatManager.Instance.CanCollectCrystals = false;

        float progress = 0f;
        float startY = character.transform.position.y;

        while (progress < 1f)
        {
            // If combat is paused, don't move
            if (!CombatManager.Instance.CombatFlow)
            {
                yield return null;
                continue;
            }

            progress += Time.deltaTime / castingFreezeDuration;

            // Curve value represents the vertical offset from the start.
            float height = castHopHeightCurve.Evaluate(progress);

            // Calculate the desired world-space Y position.
            float targetY = startY + height;

            // Move only by the difference between current and target height.
            float deltaY = targetY - character.transform.position.y;

            character.Move(Vector3.up * deltaY);

            yield return null;
        }

        hasLandedAfterCast = false;
        verticalVelocity = 0f;

        isCasting = false;
        CombatManager.Instance.CanCollectCrystals = true;
    }

    public float getSpeed()
    {
        return speed;
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void TogglePlayerCameraRotatable(bool val)
    {
        cameraInput.enabled = val;
    }

    public void RotateOnCast()
    {
        if (!freezeOnSpellCast)
            return;

        StartCoroutine(RotateOnCastCoroutine());
    }

    public IEnumerator RotateOnCastCoroutine()
    {
        isRotatingFromCast = true;

        float pSpeed = getSpeed();
        setSpeed(0f);

        // Disable camera input while the player turns.
        if (cameraInput != null)
            cameraInput.enabled = false;

        float progress = 0f;

        Quaternion startRotation = playerModel.transform.rotation;
        Vector3 targetPosition = new Vector3(0f, 1f, 0f); // TODO CYRENE: REPLACE WITH ENEMY POSITION?
        Vector3 direction = targetPosition - playerModel.transform.position;

        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float startAxisValue = follow.HorizontalAxis.Value;

        // Desired camera orbit angle in world space.
        float targetAxisValue = targetRotation.eulerAngles.y;

        while (progress < 1f)
        {
            // If combat is paused, don't move
            if (!CombatManager.Instance.CombatFlow)
            {
                yield return null;
                continue;
            }

            progress += Time.deltaTime / castingFreezeDuration;

            playerModel.transform.rotation = Quaternion.Slerp(startRotation, targetRotation,
                castRotationCurve.Evaluate(progress));

            follow.HorizontalAxis.Value = Mathf.LerpAngle(startAxisValue,
                targetAxisValue, castRotationCurve.Evaluate(progress));

            yield return null;
        }

        // Restore camera input after the turn.
        if (cameraInput != null)
            cameraInput.enabled = true;

        currentAngle = playerModel.transform.eulerAngles.y;
        currentAngleVelocity = 0f;

        // Return speed
        setSpeed(pSpeed);

        isRotatingFromCast = false;
    }

    private void UpdateFootsteps()
    {
        // Don't play footsteps outside active combat
        if (!CombatManager.Instance.CombatFlow ||
            !canMove ||
            isDashing ||
            isCasting ||
            isHopping ||
            !character.isGrounded ||
            currentMoveVelocity.magnitude < minimumFootstepSpeed)
        {
            footstepTimer = 0f;
            return;
        }

        // Calculate movement speed relative to maximum speed
        float normalizedSpeed = Mathf.Clamp01(currentMoveVelocity.magnitude / Mathf.Max(speed, 0.01f));

        // Faster movement = shorter interval between steps
        float stepInterval = Mathf.Lerp(slowStepInterval, fastStepInterval, normalizedSpeed);

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= stepInterval)
        {
            PlayFootstep();
            footstepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (footstepAudioSource == null ||
            footstepClips == null ||
            footstepClips.Length == 0)
        {
            return;
        }

        AudioClip clip = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];
        footstepAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

        footstepAudioSource.PlayOneShot(clip, footstepVolume);
    }

}
