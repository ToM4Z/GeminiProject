using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *  Class: PlayerInputModelController
 *  
 *  Description:
 *  This script allow player to move in the world and to perform different actions.
 *  
 *  Author: Thomas Voce
*/
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
        RESPAWN,
        VICTORY
    }

    public Status status { get; private set; }

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
    private AttackTrigger armActualAttack = null;

    private bool enableInput = true;

    private float heightCollider;
    [SerializeField] private float crouchedHeightCollider;
    private Vector3 centerCollider;
    [SerializeField] private Vector3 crouchedCenterCollider;

    [SerializeField] private List<TrailRenderer> trails;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        materialHandler = GetComponent<PlayerMaterialHandler>();

        heightCollider = charController.height;
        centerCollider = charController.center;

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

        SetNormalCollider();
        deathEvent = 0;
        _vertSpeed = minFall;
        anim.speed = 1f;
        anim.Play("Idle - Run H");
        status = Status.RESPAWN;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForEndOfFrame();
        status = Status.IDLE;
    }

    public bool GetButtonDown(string button)
    {
        return enableInput && Input.GetButtonDown(button);
    }

    public float GetAxis(string button)
    {
        return enableInput ? Input.GetAxis(button) : 0;
    }

    private void SetNormalCollider()
    {
        charController.height = heightCollider;
        charController.center = centerCollider;
    }

    private void SetCrouchedCollider()
    {
        charController.height = crouchedHeightCollider;
        charController.center = crouchedCenterCollider;
    }

    private void SetTrailOnOff(bool b)
    {
        foreach (TrailRenderer t in trails)
            t.enabled = b;
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
                        stopAttack();

                        SetTrailOnOff(true);
                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Jump"))
                    {
                        _vertSpeed = jumpSpeed;
                        anim.Play("Jump start");
                        stopAttack();

                        SetTrailOnOff(true);
                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Crouch"))
                    {
                        SetCrouchedCollider();

                        if (GetAxis("Vertical") <= 0.3f)
                        {
                            anim.SetBool("Crouch", true);

                            status = Status.CROUCH;
                            break;
                        }
                        else
                        {
                            directionSlide = movement.normalized;
                            actualSlideSpeed = slideSpeed;
                            movement = directionSlide * actualSlideSpeed;
                            anim.Play("Slide");
                            attack(Status.SLIDE);

                            SetTrailOnOff(true);
                            status = Status.SLIDE;
                            break;
                        }
                    }

                    if (GetButtonDown("Attack"))
                        attack(status);

                    break; 
                }
            case Status.CROUCH:
                {
                    movement = Vector3.ClampMagnitude(movement * playerCrouchedSpeed, playerCrouchedSpeed);

                    _vertSpeed = minFall;

                    if (!charController.isGrounded)
                    {
                        SetNormalCollider();
                        anim.Play("Falling");
                        anim.SetBool("Crouch", false);

                        SetTrailOnOff(true);
                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Jump"))
                    {
                        SetNormalCollider();
                        _vertSpeed = jumpSpeed;
                        anim.Play("Jump start");
                        anim.SetBool("Crouch", false);

                        SetTrailOnOff(true);
                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Crouch"))
                    {
                        SetNormalCollider();
                        anim.SetBool("Crouch", false);

                        status = Status.IDLE;
                        break;
                    }

                    if (GetButtonDown("Attack"))
                    {
                        SetNormalCollider();
                        anim.SetBool("Crouch", false);

                        attack(Status.IDLE);
                        status = Status.IDLE;
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
                        stopAttack();

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
                            _contact.collider.gameObject.GetComponent<AIEnemy>().hurt();

                            status = Status.FALLING;
                        }
                        else
                        {
                            SetTrailOnOff(false);
                            status = Status.IDLE;
                        }
                        break;
                    }

                    if (GetButtonDown("Attack"))
                        attack(status);

                    break;
                }
            case Status.SLIDE:
                {
                    actualSlideSpeed = Mathf.Lerp(actualSlideSpeed, 0f, 4f * Time.deltaTime);

                    _vertSpeed = minFall;

                    if (actualSlideSpeed < 3f)
                    {
                        stopAttack();
                        SetNormalCollider();

                        SetTrailOnOff(false);
                        status = Status.IDLE;
                        break;
                    }

                    if (!charController.isGrounded)
                    {
                        SetNormalCollider();
                        anim.Play("Falling");
                        stopAttack();

                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Jump"))
                    {
                        SetNormalCollider();
                        _vertSpeed = jumpSpeed;
                        anim.Play("Jump start");
                        stopAttack();

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

    private void attack(Status s)
    {
        if (attacking)
            return;

        switch (s)
        {
            case Status.IDLE:
                {
                    attackIndex = (attackIndex + 1) % (comboAttacks.Length - 1); // -1 because the last attack is the air attack
                    armActualAttack = hands[attackIndex];
                    anim.Play(comboAttacks[attackIndex]);
                    armActualAttack.EnableTrigger();
                    break;
                }
            case Status.FALLING:
                {
                    if (!airAttackJustDone)
                    {
                        anim.Play(comboAttacks[attackIndex = comboAttacks.Length - 1]);
                        armActualAttack = foots[0];
                        armActualAttack.EnableTrigger();
                        airAttackJustDone = true;
                    }
                    break;
                }
            case Status.SLIDE:
                {
                    armActualAttack = foots[0];
                    armActualAttack.EnableTrigger();
                    break;
                }
        }
        attacking = true;
    }

    private void stopAttack()
    {
        if (!attacking)
            return;
        armActualAttack.DisableTrigger();
        armActualAttack = null;
        airAttackJustDone = false;
        attacking = false;
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
                            stopAttack();
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
                            stopAttack();
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
                            stopAttack();
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
        stopAttack();

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

    public void OnVictory()
    {
        if (status == Status.VICTORY)
            return;

        anim.Play("Victory 1");
        enableInput = false;
        status = Status.VICTORY;
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