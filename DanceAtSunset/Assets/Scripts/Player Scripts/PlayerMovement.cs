using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.SceneView;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public InputHandler input;
    public float Speed = 10.0f;

    public GameObject character;

    public Transform Cam;


    void Start()
    {
       rb = GetComponent<Rigidbody>();
       input = GetComponent<InputHandler>();
       character = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        //Store user input as a movement vector
        Vector3 m_Input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        m_Input.Normalize();

        rb.MovePosition(transform.position + m_Input * Time.deltaTime * Speed);

        if (m_Input.magnitude != 0f)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Cam.GetComponent<ThirdPersonCamMovement>().sensivity * Time.deltaTime);


            Quaternion CamRotation = Cam.rotation;
            CamRotation.x = 0f;
            CamRotation.z = 0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, CamRotation, 0.1f);

        }

    }

    void Update()
    {
       // rb.linearVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Speed;
     //   var targetVector = new Vector3(input.InputVector.x, 0, input.InputVector.y);




    }

}
