using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerInput : MonoBehaviour
{
    private CharacterController _charController;
    public float speed = 6f;
    public float gravity = -9.8f;
    public float jumpHeight = 1f;
    
    void Start()
    {
        _charController = GetComponent<CharacterController>();

    }

    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        movement.y = gravity;
        
        movement *= Time.deltaTime;

        if (Input.GetButtonDown("Jump") && _charController.isGrounded)
            movement.y += Mathf.Sqrt(jumpHeight * -2f * gravity);

        movement = transform.TransformDirection(movement);

        

        _charController.Move(movement);

        

    }
}
