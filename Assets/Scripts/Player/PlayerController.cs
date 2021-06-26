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
public class PlayerController : MonoBehaviour
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

    private bool enableInput = true;

    public bool Invulnerability { get; private set; } = false;
    [SerializeField] private float InvulnerabilityTime = 3f;
    private float invulnerabilityTimer = 0f;

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
    private readonly string trampolineMask = "Bouncy";
    private readonly string enemyMask = "Enemy";

    private PlayerMaterialHandler materialHandler;
    private DeathEvent deathEvent;

    [SerializeField] private GameObject followTarget;
    private Vector3 originFollowTargetPos;

    private AttackTrigger armActualAttack = null;
    [SerializeField] private AttackTrigger[] hands, foots;

    [SerializeField] private List<TrailRenderer> trails;

    private float heightCollider;
    private Vector3 centerCollider;
    [SerializeField] private float crouchedHeightCollider;
    [SerializeField] private Vector3 crouchedCenterCollider;

    public bool InvertDirectionRespectToCamera = false;

    [SerializeField] private GameObject burnFX, freezeFX;
    [SerializeField] private ParticleSystem hitFX;

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] footStepSFX, missingHitSFX, boingSFX, hitSFX;
    [SerializeField] private AudioClip slideSFX, jumpSFX, landedSFX, mashedSFX, fall_vacuumSFX, burnSFX, freezeSFX;
    private bool[] footStepSoundJustPlayed = new bool[2];

    private readonly float idleTime = 5f;
    private float idleTimer = 0f;
    private int idleAnimIndex = 0;
    private readonly int maxIdleAnim = 2, maxDeathAnim = 2, maxVictoryAnim = 3;
    private int lastIdleAnimPlayed = 0;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        materialHandler = GetComponent<PlayerMaterialHandler>();
        audioSource = GetComponentInChildren<AudioSource>();

        originFollowTargetPos = followTarget.transform.localPosition;

        heightCollider = charController.height;
        centerCollider = charController.center;

        footStepSoundJustPlayed[0] = false;
        footStepSoundJustPlayed[1] = false;

        hitFX.Stop();
        idleTimer = idleTime;
        status = Status.IDLE;
        _vertSpeed = minFall;
    }

    private void Reset()
    {
        if (deathEvent == DeathEvent.FALLED_IN_VACUUM)
        {
            followTarget.transform.SetParent(this.transform);
            followTarget.transform.localPosition = originFollowTargetPos;
            followTarget.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        materialHandler.resetMaterials();
        SetNormalCollider();
        deathEvent = 0;
        _vertSpeed = minFall;
        anim.speed = 1f;
        idleAnimIndex = lastIdleAnimPlayed = 0;
        anim.Play("Idle - Run H");
        idleTimer = idleTime;
        status = Status.RESPAWN;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        status = Status.IDLE;
        Invulnerability = false;
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
        if (status == Status.RESPAWN || status == Status.VICTORY || GlobalVariables.isPaused) return;

        if (status == Status.DEATH &&
            (deathEvent == DeathEvent.FROZEN || deathEvent == DeathEvent.BURNED))
            return;

        if (Invulnerability)
        {
            invulnerabilityTimer -= Time.deltaTime;
            materialHandler.setTransparencyAlpha(Mathf.PingPong(Time.time, 0.5f) / 0.5f);

            if (invulnerabilityTimer < 0f)
            {
                Invulnerability = false;
                invulnerabilityTimer = 0f;
                materialHandler.resetMaterials();
                hitFX.Stop();
            }
        }

        Vector3 input = new Vector3(GetAxis("Horizontal"), 0, GetAxis("Vertical"));
        if (InvertDirectionRespectToCamera)
            input.x *= -1;
        Vector3 movement = input;

        anim.SetFloat("MoveV", movement.z, 1f, Time.deltaTime * 10f);
        anim.SetFloat("MoveH", movement.x, 1f, Time.deltaTime * 10f);
        anim.SetBool("IsOnGround", charController.isGrounded);
        PlayFootStepSound();

        checkAttack();

        switch (status)
        {
            case Status.IDLE:
                {
                    movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed);

                    _vertSpeed = minFall;

                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle - Run H") && anim.GetCurrentAnimatorStateInfo(1).IsName("Empty") && input == Vector3.zero)
                    {
                        idleTimer -= Time.deltaTime;
                        if (idleTimer <= 0)
                        {
                            lastIdleAnimPlayed = ++lastIdleAnimPlayed % (maxIdleAnim + 1);
                            lastIdleAnimPlayed = lastIdleAnimPlayed == 0 ? 1 : lastIdleAnimPlayed;
                            anim.Play("Idle "+ (idleAnimIndex = lastIdleAnimPlayed));
                            idleTimer = idleTime;
                        }
                    }
                    else
                        idleTimer = idleTime;

                    if (idleAnimIndex != 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle "+idleAnimIndex) && 
                        (input != Vector3.zero || !anim.GetCurrentAnimatorStateInfo(1).IsName("Empty")))
                    {
                        idleAnimIndex = 0;
                        anim.Play("Idle - Run H");
                    }

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
                        PlayClip(ref jumpSFX);
                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Crouch"))
                    {
                        SetCrouchedCollider();

                        if (GetAxis("Vertical") <= 0.3f)
                        {
                            anim.Play("Idle - Run H");
                            anim.SetBool("Crouch", true);

                            status = Status.CROUCH;
                            break;
                        }
                        else
                        {
                            anim.Play("Idle - Run H");
                            directionSlide = movement.normalized;
                            actualSlideSpeed = slideSpeed;
                            movement = directionSlide * actualSlideSpeed;
                            attack(Status.SLIDE);

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
                        PlayClip(ref jumpSFX);
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
                    movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed); /// 1.5f;

                    _vertSpeed += gravity * 5 * Time.deltaTime;
                    if (_vertSpeed < terminalVelocity)
                        _vertSpeed = terminalVelocity;


                    if (charController.isGrounded)
                    {
                        stopAttack();

                        string layer = "";
                        if(_contact != null)
                            layer = LayerMask.LayerToName(_contact.gameObject.layer);
                        if (layer.Equals(trampolineMask))
                        {
                            _vertSpeed = jumpSpeed * multiplierJumpSpeedOnTrampoline;
                            anim.Play("Jump start");

                            PlayClip(ref boingSFX);
                            status = Status.FALLING;
                        }
                        else
                        if (layer.Equals(enemyMask))
                        {
                            _vertSpeed = jumpSpeed;
                            anim.Play("Jump start");

                            if(_contact.collider.gameObject.GetComponent<IHittable>().hit())
                                PlayClip(ref hitSFX);

                            status = Status.FALLING;
                        }
                        else
                        {
                            SetTrailOnOff(false);
                            PlayClip(ref landedSFX);
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

                    // I will change status from StopSlide method

                    if (!charController.isGrounded)
                    {
                        StopSlide();

                        SetTrailOnOff(true);
                        anim.Play("Falling");
                        status = Status.FALLING;
                        break;
                    }

                    if (GetButtonDown("Jump"))
                    {
                        StopSlide();

                        SetTrailOnOff(true);
                        _vertSpeed = jumpSpeed;
                        anim.Play("Jump start");
                        PlayClip(ref jumpSFX);
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

        movement.y = _vertSpeed;
        
        if(input.magnitude > 0)
        {
            Vector3 projectCameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward * (InvertDirectionRespectToCamera ? -1 : 1), Vector3.up);
            Quaternion rotationToCamera = Quaternion.LookRotation(projectCameraForward, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToCamera, 310f * Time.deltaTime);

            movement = rotationToCamera * movement;
        }
        movement *= Time.deltaTime;
        charController.Move(movement);
    }

    private void attack(Status s)
    {
        if (attacking)
            return;

        SetTrailOnOff(true);

        switch (s)
        {
            case Status.IDLE:
                {
                    attackIndex = (attackIndex + 1) % (comboAttacks.Length - 1); // -1 because the last attack is the air attack
                    armActualAttack = hands[attackIndex];
                    anim.Play(comboAttacks[attackIndex]);
                    armActualAttack.EnableTrigger();
                    PlayClip(ref missingHitSFX);
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
                    anim.Play("Slide");

                    armActualAttack = foots[0];
                    armActualAttack.EnableTrigger();
                    PlayClip(ref slideSFX);
                    break;
                }
        }
        attacking = true;
    }

    private void stopAttack()
    {
        if (!attacking)
            return;

        SetTrailOnOff(false);
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
                                if(hands[attackIndex].hitted.GetComponent<IHittable>().hit())
                                    PlayClip(ref hitSFX);
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
                                if (foots[0].hitted.GetComponent<IHittable>().hit())
                                    PlayClip(ref hitSFX);
                            }
                        }
                        break;
                    }
                case Status.SLIDE:
                    {
                        // the slide will be stopped or here or by animation event
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
                        {
                            StopSlide();
                        }
                        else
                        {
                            if (foots[0].EnteredTrigger)
                            {
                                if (foots[0].hitted.GetComponent<IHittable>().hit())
                                    PlayClip(ref hitSFX);
                            }
                        }
                        break;
                    }
            }

        }
    }

    private void OnDeath(DeathEvent deathEvent)
    {
        Invulnerability = true;
        status = Status.DEATH;
        this.deathEvent = deathEvent;
        stopAttack();

        switch (deathEvent)
        {
            case DeathEvent.FALLED_IN_VACUUM:
                {
                    anim.speed = 0f;
                    followTarget.transform.SetParent(null);
                    PlayClip(ref fall_vacuumSFX);
                    break;
                }
            case DeathEvent.HITTED:
                {
                    anim.Play("Death "+ (Random.Range(0, maxDeathAnim)+1));
                    PlayClip(ref hitSFX);
                    break;
                }
            case DeathEvent.MASHED:
                {
                    anim.Play("Mashed Death");
                    PlayClip(ref mashedSFX);
                    break;
                }
            case DeathEvent.BURNED:
                {
                    anim.speed = 0f;
                    materialHandler.burnMaterials();
                    PlayClip(ref burnSFX);
                    Instantiate(burnFX, transform);
                    break;
                }
            case DeathEvent.FROZEN:
                {
                    anim.speed = 0f;
                    materialHandler.frozenMaterials();
                    PlayClip(ref freezeSFX);
                    Instantiate(freezeFX, transform);
                    break;
                }
        }
    }

    public void OnVictory()
    {
        if (status == Status.VICTORY)
            return;

        anim.Play("Victory " + (Random.Range(0, maxVictoryAnim) + 1));
        enableInput = false;
        status = Status.VICTORY;
    }

    private void PlayFootStepSound()
    {
        if(status == Status.IDLE || status == Status.CROUCH)
            for( int i =0; i < 2; ++i) 
                if(foots[i].transform.position.y <= transform.position.y+.01f && !footStepSoundJustPlayed[i])
                {
                    PlayClip(ref footStepSFX);
                    footStepSoundJustPlayed[i] = true;
                    break;
                }
                else
                    if (foots[i].transform.position.y > transform.position.y + .01f)
                        footStepSoundJustPlayed[i] = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }

    public void Hurt(DeathEvent deathEvent)
    {
        Invulnerability = true;
        invulnerabilityTimer = InvulnerabilityTime;
        anim.Play("Get hit");

        switch (deathEvent)
        {
            case DeathEvent.HITTED:
            {
                PlayClip(ref hitSFX);
                hitFX.Play();
                break;
            }
            case DeathEvent.BURNED:
            {
                PlayClip(ref burnSFX);
                materialHandler.burnMaterials();
                Instantiate(burnFX, transform);
                break;
            }
            case DeathEvent.FROZEN:
            {
                PlayClip(ref freezeSFX);
                materialHandler.frozenMaterials();
                Instantiate(freezeFX, transform);
                break;
            }
        }
        materialHandler.ToFadeMode();
    }

    private void Awake()
    {
        Messenger<bool>.AddListener(GlobalVariables.ENABLE_INPUT, EnableInput);
        Messenger<DeathEvent>.AddListener(GlobalVariables.DEATH, OnDeath);
        Messenger.AddListener(GlobalVariables.RESET, Reset);
    }

    private void OnDestroy()
    {
        Messenger<bool>.RemoveListener(GlobalVariables.ENABLE_INPUT, EnableInput);
        Messenger<DeathEvent>.RemoveListener(GlobalVariables.DEATH, OnDeath);
        Messenger.RemoveListener(GlobalVariables.RESET, Reset);
    }

    private void EnableInput(bool b)
    {
        enableInput = b;
    }

    public void StopSlide()
    {
        stopAttack();

        SetNormalCollider();
        status = Status.IDLE;
    }

    private void PlayClip(ref AudioClip a)
    {
        audioSource.PlayOneShot(a);
    }

    private void PlayClip(ref AudioClip[] a)
    {
        PlayClip(ref a[Random.Range(0, a.Length)]);
    }
}