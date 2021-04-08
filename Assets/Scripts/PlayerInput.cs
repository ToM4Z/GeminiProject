using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    public float playerSpeed = 2.0f;
    public float jumpSpeed = 15.0f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;
    public float gravity = -9.81f;

    private CharacterController charController;
    private float _vertSpeed;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
        _vertSpeed = minFall;
    }

    private void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed);
        movement = transform.TransformDirection(movement);

        if (charController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
                _vertSpeed = jumpSpeed;
            else
                _vertSpeed = minFall;
        }
        else
        {
            _vertSpeed += gravity * 5 * Time.deltaTime;
            if (_vertSpeed < terminalVelocity)
                _vertSpeed = terminalVelocity;
        }
        movement.y = _vertSpeed;

        movement *= Time.deltaTime;
        charController.Move(movement);
    }

    //private Vector3 playerVelocity;
    //public bool groundedPlayer;
    /*void Update()
    {
        groundedPlayer = charController.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0){
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, playerSpeed);
        move = transform.TransformDirection(move);
        charController.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            Debug.Log("jump");
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        charController.Move(playerVelocity * Time.deltaTime);
    }*/
}