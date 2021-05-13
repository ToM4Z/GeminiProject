using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputModelController : MonoBehaviour
{
    public enum Status
    {
        IDLE,
        CROUCH,
        FALLING,
        SLIDE,
        DEATH,
        RESPAWN
    }

    private Status status;

    public float playerSpeed = 7f;
    public float playerCrouchedSpeed = 2f;
    public float jumpSpeed = 15f;
    public float slideSpeed = 28f;
    public float multiplierJumpSpeedOnTrampoline = 1.5f;
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
    private string trampolineMask = "Bouncy";
    private string enemyMask = "Enemy";

    private PlayerMaterialHandler materialHandler;
    private DeathEvent deathEvent;

    [SerializeField]
    private GameObject followTarget;
    [SerializeField]
    private AttackTrigger[] hands = new AttackTrigger[2];
    [SerializeField]
    private AttackTrigger[] foots = new AttackTrigger[2];

    private bool enableInput = true;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        materialHandler = GetComponent<PlayerMaterialHandler>();

        status = Status.IDLE;
        _vertSpeed = minFall;
    }

    private void Reset()
    {
        if (deathEvent == DeathEvent.FALLED_IN_VACUUM)
        {
            followTarget.transform.SetParent(this.transform);
            followTarget.transform.localPosition = new Vector3(0, .7f, -.3f);
            followTarget.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        deathEvent = 0;
        status = Status.RESPAWN;
        _vertSpeed = minFall;
        anim.Play("Idle - Run H");
        anim.speed = 1f;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForEndOfFrame();
        status = Status.IDLE;
    }

    private bool GetButtonDown(string button)
    {
        return enableInput && Input.GetButtonDown(button);
    }

    private float GetAxis(string button)
    {
        return enableInput ? Input.GetAxis(button) : 0;
    }

    private void Update()
    {
        if (status == Status.RESPAWN) return;

        Vector3 movement = new Vector3(GetAxis("Horizontal"), 0, GetAxis("Vertical"));

        anim.SetFloat("MoveV", movement.z, 1f, Time.deltaTime * 10f);
        anim.SetFloat("MoveH", movement.x, 1f, Time.deltaTime * 10f);
        anim.SetBool("IsOnGround", charController.isGrounded);

        checkAttack();

        switch (status)
        {
            case Status.IDLE:
                {
                    movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed);

                    _vertSpeed = minFall;

                    if (!charController.isGrounded)
                    {
                        anim.Play("Falling");
                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        //anim.SetTrigger("Jump");
                        anim.Play("Jump start");
                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Crouch"))
                    {
                        if (Mathf.Approximately(movement.magnitude, 0f))
                        {
                            status = Status.CROUCH;
                            anim.SetBool("Crouch", true);
                            break;
                        }
                        else
                        if(GetAxis("Vertical") > 0.5f){
                            directionSlide = movement.normalized;
                            status = Status.SLIDE;
                            //anim.SetTrigger("Slide");
                            anim.Play("Slide");
                            attacking = true;

                            actualSlideSpeed = slideSpeed;
                            movement = directionSlide * actualSlideSpeed;
                        }
                    }

                    if (GetButtonDown("Attack"))
                        attack();

                    break; 
                }
            case Status.CROUCH:
                {
                    movement = Vector3.ClampMagnitude(movement * playerCrouchedSpeed, playerCrouchedSpeed);

                    _vertSpeed = minFall;

                    if (!charController.isGrounded)
                    {
                        anim.Play("Falling");
                        status = Status.FALLING;
                        anim.SetBool("Crouch", false);
                        break;
                    }

                    if (GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        //anim.SetTrigger("Jump");
                        anim.Play("Jump start");
                        anim.SetBool("Crouch", false);

                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Crouch"))
                    {
                        status = Status.IDLE;
                        anim.SetBool("Crouch", false);
                        break;
                    }

                    if (GetButtonDown("Attack"))
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


                    if (charController.isGrounded)
                    {
                        //anim.ResetTrigger("Jump");
                        string layer = LayerMask.LayerToName(_contact.gameObject.layer);
                        if (layer.Equals(trampolineMask))
                        {
                            _vertSpeed = jumpSpeed * multiplierJumpSpeedOnTrampoline;
                            anim.Play("Jump start");
                            status = Status.FALLING;
                        }
                        else
                        if (layer.Equals(enemyMask))
                        {
                            _vertSpeed = jumpSpeed;
                            anim.Play("Jump start");
                            status = Status.FALLING;
                            _contact.collider.gameObject.GetComponent<AIEnemy>().hurt();
                        }
                        else
                        {
                            status = Status.IDLE;
                            airAttackJustDone = false;
                        }

                        break;
                    }

                    if (GetButtonDown("Attack"))
                        attack();

                    break;
                }
            case Status.SLIDE:
                {
                    actualSlideSpeed = Mathf.Lerp(actualSlideSpeed, 0f, 4f * Time.deltaTime);

                    _vertSpeed = minFall;

                    if (actualSlideSpeed < 3f)
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

                    if (GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        //anim.SetTrigger("Jump");
                        anim.Play("Jump start");
                        //anim.SetTrigger("Jump");

                        status = Status.FALLING;
                        break;
                    }

                    movement = directionSlide * actualSlideSpeed;

                    break;
                }
            case Status.DEATH:
                {
                    if(deathEvent != DeathEvent.FALLED_IN_VACUUM)
                        _vertSpeed = 0;
                    break;
                }
        }

        movement = transform.TransformDirection(movement);

        movement.y = _vertSpeed;
        movement *= Time.deltaTime;
        charController.Move(movement);
    }

    private void attack()
    {
        if (attacking)
            return;

        switch (status)
        {
            case Status.IDLE:
                {
                    attackIndex = (attackIndex + 1) % (comboAttacks.Length - 1); // -1 because the last attack is the air attack
                    anim.Play(comboAttacks[attackIndex]);
                    attacking = true;
                    break;
                }
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
        }
    }

    private void checkAttack()
    {
        if (attacking)
        {
            switch (status)
            {
                case Status.IDLE:
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(1).IsName(comboAttacks[attackIndex]))
                        {
                            attacking = false;
                        }
                        else
                        {
                            if (hands[attackIndex].EnteredTrigger)
                            {
                                hands[attackIndex].hitted.GetComponent<AIEnemy>().hurt();
                            }
                        }

                        break;
                    }
                case Status.FALLING:
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(comboAttacks[attackIndex]))
                        {
                            attacking = false;
                        }
                        else
                        {
                            if (foots[0].EnteredTrigger)
                            {
                                foots[0].hitted.GetComponent<AIEnemy>().hurt();
                            }
                        }
                        break;
                    }
                case Status.SLIDE:
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
                        {
                            attacking = false;
                        }
                        else
                        {
                            if (foots[0].EnteredTrigger)
                            {
                                foots[0].hitted.GetComponent<AIEnemy>().hurt();
                            }
                        }
                        break;
                    }
            }

        }
    }

    private void OnDeath(DeathEvent deathEvent)
    {
        status = Status.DEATH;
        this.deathEvent = deathEvent;
        attacking = false;

        switch (deathEvent)
        {
            case DeathEvent.FALLED_IN_VACUUM:
                {
                    anim.speed = 0f;
                    followTarget.transform.SetParent(null);
                    break;
                }
            case DeathEvent.HITTED:
                {
                    anim.Play("Death 1");
                    break;
                }
            case DeathEvent.BURNED:
                {
                    anim.speed = 0f;
                    materialHandler.burnMaterials();
                    break;
                }
            case DeathEvent.FROZEN:
                {
                    anim.speed = 0f;
                    materialHandler.frozenMaterials();
                    break;
                }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }

    private void Awake()
    {
        Messenger<bool>.AddListener(GameEvent.ENABLE_INPUT, EnableInput);
        Messenger<DeathEvent>.AddListener(GameEvent.DEATH, OnDeath);
        Messenger.AddListener(GameEvent.RESET, Reset);
    }

    private void OnDestroy()
    {
        Messenger<bool>.RemoveListener(GameEvent.ENABLE_INPUT, EnableInput);
        Messenger<DeathEvent>.RemoveListener(GameEvent.DEATH, OnDeath);
        Messenger.RemoveListener(GameEvent.RESET, Reset);
    }

    private void EnableInput(bool b)
    {
        enableInput = b;
    }
}