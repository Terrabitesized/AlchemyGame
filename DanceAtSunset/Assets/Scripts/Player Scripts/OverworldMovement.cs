using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldMovement : MonoBehaviour
{
    public float speed = 10f;
    public float dashSpeed = 17f;
    public bool isDashing = false;

    public CharacterController character;

    public Camera Camera;

    [SerializeField] float rotationSmoothTime;
    float currentAngle;
    float currentAngleVelocity;

    void Start()
    {
        character = GetComponent<CharacterController>();
        Camera = Camera.main;
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        HandleMovement();

        if (Input.GetKey(KeyCode.Space))
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.Locked;

   


        if (Input.GetKey(KeyCode.LeftShift))
        {
            isDashing = true;
        }
        else
        {
            isDashing = false;
        }

        //Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void HandleMovement()
    {
        
        
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (movement.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentAngleVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 rotatedMovement = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            if (isDashing)
            {
                character.Move(rotatedMovement * dashSpeed * Time.deltaTime);
            }
            else
            {
                character.Move(rotatedMovement * speed * Time.deltaTime);
            }
        }


      //  Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
      //  Vector3 move = new Vector3(inputDir.x, 0, inputDir.z);

        /**
        if (isDashing)
        {
            character.Move(move * dashSpeed * Time.deltaTime);
        }  else
        {
            character.Move(move * speed * Time.deltaTime);
        }
        **/
    }
}
