using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 30.0f;

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

    private void Awake()
    {
        PotionManager.OnSpellCast += DisableMovementOnCast;
    }

    private void OnDestroy()
    {
        PotionManager.OnSpellCast -= DisableMovementOnCast;
    }

    void Start()
    {
        character = GetComponent<CharacterController>();
        Camera = Camera.main;
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

    private void Update()
    {
        if (canMove)
        {
            HandleDash();
        }
    }

    private void HandleDash()
    {
        if (!Input.GetKeyDown(KeyCode.LeftShift))
            return;

        if (isDashing || !canDash)
            return;

        Vector3 inputDir = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg
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

    private void HandleMovement()
    {
        if (isDashing) return;

        Vector3 movement = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (movement.magnitude >= 0.1f)
        {
            // Calculate move direction
            float targetAngle =
                Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg
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

    private IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;
        canDash = false;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / dashDuration;

            float dashMultiplier = dashCurve.Evaluate(t);

            character.Move(
                direction * dashSpeed * dashMultiplier * Time.deltaTime
            );

            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
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
