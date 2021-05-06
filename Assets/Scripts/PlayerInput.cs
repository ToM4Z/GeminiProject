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

    private bool attacking = false;
    private int attackIndex = 0;
    private string[] comboAttacks = { "Arms.Punch Left", "Arms.Punch Right", "Main.Air Attack"};
    private bool airAttackJustDone = false;

    private ControllerColliderHit _contact;

    protected float attackTimer = 0f;
    [SerializeField]
    private AttackTrigger[] hands = new AttackTrigger[2];

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

        checkAttack();

        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), 0.1f, Vector3.down, out hit))
        {
            float check = 0.02f;
            hitGround = hit.distance <= check;
        }

        switch (status)
        {
            case Status.IDLE:
                {
                    movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed);

                    if (!hitGround)
                    {
                        anim.Play("Falling");
                        status = Status.FALLING;
                        break;
                    }

                    if (Input.GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        //anim.SetTrigger("Jump");
                        anim.Play("Jump start");
                        status = Status.FALLING;
                        break;
                    }
                    else
                        _vertSpeed = minFall;

                    if (Input.GetButtonDown("Crouch"))
                    {
                        if (Mathf.Approximately(movement.magnitude, 0f))
                        {
                            status = Status.CROUCH;
                            anim.SetBool("Crouch", true);
                            break;
                        }
                        else
                        if(Input.GetAxis("Vertical") > 0){
                            directionSlide = movement.normalized;
                            status = Status.SLIDE;
                            //anim.SetTrigger("Slide");
                            anim.Play("Slide");

                            actualSlideSpeed = slideSpeed;
                            movement = directionSlide * actualSlideSpeed;
                        }
                    }

                    if (Input.GetButtonDown("Attack"))
                        attack();

                    break; 
                }
            case Status.CROUCH:
                {
                    movement = Vector3.ClampMagnitude(movement * playerCrouchedSpeed, playerCrouchedSpeed);

                    if (!hitGround)
                    {
                        anim.Play("Falling");
                        status = Status.FALLING;
                        anim.SetBool("Crouch", false);
                        break;
                    }

                    if (Input.GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        //anim.SetTrigger("Jump");
                        anim.Play("Jump start");
                        anim.SetBool("Crouch", false);

                        status = Status.FALLING;
                        break;
                    }
                    else
                        _vertSpeed = minFall;

                    if (Input.GetButtonDown("Crouch"))
                    {
                        status = Status.IDLE;
                        anim.SetBool("Crouch", false);
                        break;
                    }

                    if (Input.GetButtonDown("Attack"))
                    {
                        status = Status.IDLE;
                        anim.SetBool("Crouch", false);
                        attack();
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


                    if (hitGround)
                    {
                        // SE ATTERRO SU UN OGGETTO 'GOMMOSO', RIMBALZO PIU' IN ALTO
                        //anim.ResetTrigger("Jump");

                        status = Status.IDLE;
                        airAttackJustDone = false;
                        break;
                    }

                    if (Input.GetButtonDown("Attack"))
                        attack();

                    break;
                }
            case Status.SLIDE:
                {
                    // DA MIGLIORARE
                    actualSlideSpeed -= Mathf.Lerp(0f, slideSpeed, 0.08f);

                    if (actualSlideSpeed < 0)
                    {
                        //anim.ResetTrigger("Slide");
                        status = Status.IDLE;
                        break;
                    }

                    if (!charController.isGrounded)
                    {
                        anim.Play("Falling");
                        //anim.ResetTrigger("Slide");
                        status = Status.FALLING;
                        break;
                    }

                    if (Input.GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        //anim.SetTrigger("Jump");
                        anim.Play("Jump start");
                        //anim.SetTrigger("Jump");

                        status = Status.FALLING;
                        break;
                    }
                    else
                        _vertSpeed = minFall;

                    movement = directionSlide * actualSlideSpeed;

                    break;
                }
            case Status.DEATH:
                {
                    break;
                }

        }

        movement = transform.TransformDirection(movement);

        if (!hitGround && charController.isGrounded)
        {
            if (Vector3.Dot(movement, _contact.normal) < 0)
            {
                movement = _contact.normal * playerSpeed;
            }
            else
            {
                movement += _contact.normal * playerSpeed;
            }
        }

        movement.y = _vertSpeed;
        movement *= Time.deltaTime;
        charController.Move(movement);

        anim.SetFloat("MoveV", Input.GetAxis("Vertical"), 1f, Time.deltaTime * 10f);
        anim.SetFloat("MoveH", Input.GetAxis("Horizontal"), 1f, Time.deltaTime * 10f);
        anim.SetBool("IsOnGround", hitGround);
    }

    private void attack()
    {
        if (attacking)
            return;

        switch (status)
        {
            case Status.FALLING:
                {
                    if (!airAttackJustDone)
                    {
                        anim.Play(comboAttacks[attackIndex = comboAttacks.Length - 1]);
                        airAttackJustDone = true;
                        attacking = true;
                    }
                    break;
                }
            case Status.IDLE:
                {
                    attackIndex = (attackIndex + 1) % (comboAttacks.Length - 1); // -1 because the last attack is the air attack
                    anim.Play(comboAttacks[attackIndex]);
                    attacking = true;
                    break;
                }
        }
    }

    private void checkAttack()
    {
        if (attacking)
        {
            attackTimer -= Time.deltaTime;
            switch (status)
            {
                case Status.FALLING:
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(comboAttacks[attackIndex]))
                        {
                            attacking = false;
                        }
                        break;
                    }
                case Status.IDLE:
                    {



                        if (!anim.GetCurrentAnimatorStateInfo(1).IsName(comboAttacks[attackIndex]))
                        {
                            attacking = false;
                        }
                        break;
                    }
            }

        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }
}