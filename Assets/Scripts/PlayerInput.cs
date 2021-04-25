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
    public Animator anim;

    private CharacterController charController;
    private float _vertSpeed;
    private bool crouch = false;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
        _vertSpeed = minFall;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed);
        movement = transform.TransformDirection(movement);

        if (charController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _vertSpeed = jumpSpeed;
                anim.SetTrigger("Jump");
            }
            else
                _vertSpeed = minFall;

            if (Input.GetKeyDown(KeyCode.C))
                crouch = !crouch;
        }
        else
        {
            crouch = false;
            _vertSpeed += gravity * 5 * Time.deltaTime;
            if (_vertSpeed < terminalVelocity)
                _vertSpeed = terminalVelocity;
        }
        movement.y = _vertSpeed;

        movement *= Time.deltaTime;
        charController.Move(movement);

        anim.SetFloat("MoveV", Input.GetAxis("Vertical"), 1f, Time.deltaTime * 10f);
        anim.SetFloat("MoveH", Input.GetAxis("Horizontal"), 1f, Time.deltaTime * 10f);
        anim.SetBool("IsOnGround", charController.isGrounded);
        anim.SetBool("Crouch", crouch);
    }


}