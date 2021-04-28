using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    public enum Status
    {
        IDLE,
        CROUCH,
        FALLING,
        SLIDE,
        ATTACK,
        DEATH
    }

    private Status status;

    public float playerSpeed = 7f;
    public float playerCrouchedSpeed = 3f;
    public float jumpSpeed = 15f;
    public float slideSpeed = 28f;
    public float multiplierJumpSpeedOnTrampoline = 2f;
    public float terminalVelocity = -10f;
    public float minFall = -1.5f;
    public float gravity = -9.81f;

    private CharacterController charController;
    private Animator anim;
    private float _vertSpeed;
    private Vector3 directionSlide;
    private float actualSlideSpeed = 0f;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
        _vertSpeed = minFall;
        anim = GetComponentInChildren<Animator>();

        status = Status.IDLE;
    }

    private void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        switch (status)
        {
            case Status.IDLE:
                {
                    movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed);

                    if (!charController.isGrounded)
                    {
                        status = Status.FALLING;
                        break;
                    }

                    if (Input.GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        anim.SetTrigger("Jump");

                        status = Status.FALLING;
                        break;
                    }
                    else
                        _vertSpeed = minFall;

                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        if (Mathf.Approximately(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude, 0f))
                        {
                            status = Status.CROUCH;
                            anim.SetBool("Crouch", true);
                            break;
                        }
                        else
                        {
                            directionSlide = movement.normalized;
                            status = Status.SLIDE;
                            anim.SetTrigger("Slide");

                            actualSlideSpeed = slideSpeed;
                            movement = directionSlide * actualSlideSpeed;
                        }
                    }
                    break; 
                }
            case Status.CROUCH:
                {
                    movement = Vector3.ClampMagnitude(movement * playerCrouchedSpeed, playerCrouchedSpeed);

                    if (!charController.isGrounded)
                    {
                        status = Status.FALLING;
                        anim.SetBool("Crouch", false);
                        break;
                    }

                    if (Input.GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        anim.SetTrigger("Jump");
                        anim.SetBool("Crouch", false);

                        status = Status.FALLING;
                        break;
                    }
                    else
                        _vertSpeed = minFall;

                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        status = Status.IDLE;
                        anim.SetBool("Crouch", false);
                        break;
                    }

                    break;
                }
            case Status.FALLING:
                {
                    movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed);

                    _vertSpeed += gravity * 5 * Time.deltaTime;
                    if (_vertSpeed < terminalVelocity)
                        _vertSpeed = terminalVelocity;

                    if (charController.isGrounded)
                    {
                        // SE ATTERRO SU UN OGGETTO 'GOMMOSO', RIMBALZO PIU' IN ALTO
                        anim.ResetTrigger("Jump");

                        status = Status.IDLE;
                        break;
                    }


                    break;
                }
            case Status.SLIDE:
                {
                    actualSlideSpeed -= Mathf.Lerp(0f, slideSpeed, 0.015f);

                    if (actualSlideSpeed < 0)
                    {
                        anim.ResetTrigger("Slide");
                        status = Status.IDLE;
                        break;
                    }

                    if (!charController.isGrounded)
                    {
                        anim.ResetTrigger("Slide");
                        status = Status.FALLING;
                        break;
                    }

                    if (Input.GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        anim.ResetTrigger("Slide");
                        anim.SetTrigger("Jump");

                        status = Status.FALLING;
                        break;
                    }
                    else
                        _vertSpeed = minFall;

                    movement = directionSlide * actualSlideSpeed;

                    break;
                }
            case Status.ATTACK:
                {
                    break;
                }
            case Status.DEATH:
                {
                    break;
                }

        }

        movement = transform.TransformDirection(movement);
        movement.y = _vertSpeed;
        movement *= Time.deltaTime;
        charController.Move(movement);

        anim.SetFloat("MoveV", Input.GetAxis("Vertical"), 1f, Time.deltaTime * 10f);
        anim.SetFloat("MoveH", Input.GetAxis("Horizontal"), 1f, Time.deltaTime * 10f);
        anim.SetBool("IsOnGround", charController.isGrounded);
    }

}