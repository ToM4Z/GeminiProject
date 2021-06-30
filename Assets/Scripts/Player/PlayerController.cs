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
        IDLE,       // still or in movement
        CROUCH,     // still or in movement crouched
        FALLING,
        SLIDE,
        DEATH,
        RESPAWN,    // is used as middle status between DEATH and IDLE, so, in that status, the update method will not perform and position will not be modify during that phase
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

    private float _vertSpeed;

    // this variable disable user input 
    private bool enableInput = true;

    // this variables define invulnerability period of time
    public bool Invulnerability { get; private set; } = false;
    [SerializeField] private float InvulnerabilityTime = 3f;
    private float invulnerabilityTimer = 0f;

    private CharacterController charController;
    private Animator anim;

    // direction where i performed slide and his actual speed
    private Vector3 directionSlide;
    private float actualSlideSpeed = 0f;

    // is player performing an attack
    private bool attacking = false;

    // index of which attack I performed
    private int attackIndex = 0;
    private string[] comboAttacks = { "Arms.Punch Left", "Arms.Punch Right", "Main.Air Attack"};

    // the air attack have to be performed one time for jump
    private bool airAttackJustDone = false;

    private ControllerColliderHit _contact;
    private readonly string trampolineMask = "Bouncy";
    private readonly string enemyMask = "Enemy";

    private PlayerMaterialHandler materialHandler;
    private DeathEvent deathEvent;

    // follow target has to be de-parented by player when he fall in the vacuum
    [SerializeField] private GameObject followTarget;
    // when respawn I have to reparent it to this go and replace it to the original position
    private Vector3 originFollowTargetPos;

    // attack trigger that is actual activated
    private AttackTrigger armActualAttack = null;
    [SerializeField] private AttackTrigger[] hands, foots;

    // trail renderer to be activated when quick movement are performed
    [SerializeField] private List<TrailRenderer> trails;

    // when player will crouch I change character controller height
    private float heightCollider;
    private Vector3 centerCollider;
    [SerializeField] private float crouchedHeightCollider;
    [SerializeField] private Vector3 crouchedCenterCollider;

    // particle of varius damage type
    [SerializeField] private GameObject burnFX, freezeFX;
    [SerializeField] private ParticleSystem hitFX;

    // audioclips
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] footStepSFX, missingHitSFX, boingSFX, hitSFX;
    [SerializeField] private AudioClip slideSFX, jumpSFX, landedSFX, mashedSFX, fall_vacuumSFX, burnSFX, freezeSFX;
    private bool[] footStepSoundJustPlayed = new bool[2];

    // timer used to play different idle animation every 5 seconds 
    private readonly float idleTime = 5f;
    private float idleTimer = 0f;
    private int idleAnimIndex = 0;
    // i play different idle animation sequentially
    private int lastIdleAnimPlayed = 0;

    // I have different animation for the same actions and are choosen randomly
    private readonly int maxIdleAnim = 2, maxDeathAnim = 2, maxVictoryAnim = 3;

    private readonly int AnimLayerBase = 0, AnimLayerArms = 2, AnimLayerAllDirection = 1;

    // When I in a cave, I can rotate player to all direction
    [HideInInspector]
    public bool rotateToDirection = false;

    // in level 2, rotateToDirection will set on for all time
    [SerializeField] private bool startWithRotateDirection = false;

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

        if (startWithRotateDirection)
            ActivateRotateToDirection(true);
        hitFX.Stop();
        idleTimer = idleTime;
        status = Status.IDLE;
        _vertSpeed = minFall;
    }

    // when player must be reset
    private void Reset()
    {
        // If I was dead by falling in vacuum, I re-parent follow target to me
        if (deathEvent == DeathEvent.FALLED_IN_VACUUM)
        {
            followTarget.transform.SetParent(this.transform);
            followTarget.transform.localPosition = originFollowTargetPos;
            followTarget.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        // and I reset variables
        if (startWithRotateDirection)
            ActivateRotateToDirection(true);
        else
            ActivateRotateToDirection(false);

        materialHandler.resetMaterials();
        SetNormalCollider();
        deathEvent = 0;
        _vertSpeed = minFall;
        anim.speed = 1f;
        idleAnimIndex = lastIdleAnimPlayed = 0;
        anim.Play("Idle - Run H");
        idleTimer = idleTime;

        // finally, I wait one second before pass to IDLE state,
        // in this way, RespawnManager can reset player position without this update method change position too
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

    // reset normal character controller collider size
    private void SetNormalCollider()
    {
        charController.height = heightCollider;
        charController.center = centerCollider;
    }

    // set character controller collider size for crouched status
    private void SetCrouchedCollider()
    {
        charController.height = crouchedHeightCollider;
        charController.center = crouchedCenterCollider;
    }

    // this method is called by ActivateVirtualCamera (when player enter in a cave), player rotate towards direction
    // and I change layer weight in animator because animation have to be ever use 'running' animation in all directions
    public void ActivateRotateToDirection(bool _activate)
    {
        rotateToDirection = _activate;
        anim.SetLayerWeight(AnimLayerAllDirection, _activate ? 1 : 0);
    }

    // dis/activate trail renderers 
    private void SetTrailOnOff(bool b)
    {
        foreach (TrailRenderer t in trails)
            t.enabled = b;
    }

    // this method had to do ground detection because when i open pause, the charController
    // not detect ground more. Unfortunately, this method lock player jump on platform movement, so I removed this check
    private bool checkIsGrounded()
    {
        return charController.isGrounded;
        //if (charController.isGrounded)
        //    return true;
        //return Physics.Raycast(transform.position, Vector3.down, out _, 0.0175f, LayerMask.GetMask("Static", "Default"));
    }

    private void Update()
    {
        // if I'm in other status or the game is in pause or I'm in particular type of death, I don't execute update
        if (
            status == Status.RESPAWN || 
            status == Status.VICTORY || 
            GlobalVariables.isPaused ||
                (status == Status.DEATH && (deathEvent == DeathEvent.FROZEN || deathEvent == DeathEvent.BURNED))
            ) return;

        // if I'm Invulnerable, I make transparency animation
        if (Invulnerability && invulnerabilityTimer > 0)
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

        // if InvertDirectionRespectToCamera is true, I invert input on horizontal axis
        Vector3 input = new Vector3(GetAxis("Horizontal"), 0, GetAxis("Vertical"));
        Vector3 movement = input;

        bool isGrounded = checkIsGrounded();

        // update animation variable 
        anim.SetFloat("MoveV", movement.z, 1f, Time.deltaTime * 10f);
        anim.SetFloat("MoveH", movement.x, 1f, Time.deltaTime * 10f);
        anim.SetBool("IsOnGround", isGrounded);
        PlayFootStepSound();

        checkAttack();

        switch (status)
        {
            case Status.IDLE:
                {
                    movement = Vector3.ClampMagnitude(movement * playerSpeed, playerSpeed);

                    _vertSpeed = minFall;

                    // here I manage idles animation, If I do nothing, after 5 seconds, I perform one idle animation
                    if (anim.GetCurrentAnimatorStateInfo(AnimLayerBase).IsName("Idle - Run H") && 
                        anim.GetCurrentAnimatorStateInfo(AnimLayerArms).IsName("Empty") && 
                        input == Vector3.zero)
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

                    // if I move or attack, I stop animation
                    if (idleAnimIndex != 0 && anim.GetCurrentAnimatorStateInfo(AnimLayerBase).IsName("Idle "+idleAnimIndex) && 
                        (input != Vector3.zero || !anim.GetCurrentAnimatorStateInfo(AnimLayerArms).IsName("Empty")))
                    {
                        idleAnimIndex = 0;
                        anim.Play("Idle - Run H");
                    }

                    // if I'm not touching ground, I'm falling
                    if (!isGrounded)
                    {
                        anim.Play("Falling");
                        stopAttack();

                        SetTrailOnOff(true);
                        status = Status.FALLING;
                        break;
                    }

                    // If press Jump, player jump
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

                    // if press crouch while move towards front, I perform a slide, otherwise I crouch
                    if (GetButtonDown("Crouch"))
                    {
                        SetCrouchedCollider();

                        // if rotateDirection is enable, I can perform a slide to back 
                        if( ! (GetAxis("Vertical") > 0.2f || (rotateToDirection && GetAxis("Vertical") < -0.2f)) )
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

                    // if I'm not touching ground, I'm falling
                    if (!isGrounded)
                    {
                        SetNormalCollider();
                        anim.Play("Falling");
                        anim.SetBool("Crouch", false);

                        SetTrailOnOff(true);
                        status = Status.FALLING;
                        break;
                    }

                    // If press Jump, player jump
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

                    // If press crouch, player stand up
                    if (GetButtonDown("Crouch"))
                    {
                        SetNormalCollider();
                        anim.SetBool("Crouch", false);

                        status = Status.IDLE;
                        break;
                    }

                    // If press attack, player stand up and attack
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

                    // if I touch ground
                    if (isGrounded)
                    {
                        // stop air attack (if he was doing it)
                        stopAttack();

                        // If I landed on an trampoline, I rejump higher
                        // If I landed on an enemy, I hit him and rejump
                        // otherwise I landed
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

                    // if I attack, I start air attack
                    if (GetButtonDown("Attack"))
                        attack(status);

                    break;
                }
            case Status.SLIDE:
                {
                    // decrease slide speed
                    actualSlideSpeed = Mathf.Lerp(actualSlideSpeed, 0f, 4f * Time.deltaTime);

                    _vertSpeed = minFall;

                    // I will change status normally (when animation finish) by StopSlide method


                    // if I'm not touching ground, I'm falling
                    if (!isGrounded)
                    {
                        StopSlide();

                        SetTrailOnOff(true);
                        anim.Play("Falling");
                        status = Status.FALLING;
                        break;
                    }

                    // If press Jump, player jump
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

                    // player move towards direction where slide began
                    movement = directionSlide * actualSlideSpeed;

                    break;
                }
            case Status.DEATH:
                {
                    // In death status, if I wasn't falling in vacuum, I set vertSpeed to 0
                    if(deathEvent != DeathEvent.FALLED_IN_VACUUM)
                        _vertSpeed = 0;
                    break;
                }
        }

        movement.y = _vertSpeed;
        
        // If I move player, move player in base of camera look's direction 
        if(input.magnitude > 0)
        {
            Vector3 projectCameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
            Quaternion rotationToCamera = Quaternion.LookRotation(projectCameraForward, Vector3.up);

            movement = rotationToCamera * movement;

            // if rotate direction is true, I rotate player towards input direction
            // otherwise I rotate player towards camera look's direction
            if (rotateToDirection)
            {
                Quaternion rotateToMoveDirection = Quaternion.LookRotation(new Vector3(movement.x, 0, movement.z), Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateToMoveDirection, 310f * Time.deltaTime);
            }
            else
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToCamera, 310f * Time.deltaTime);
        }
        // move player
        movement *= Time.deltaTime;
        charController.Move(movement);
    }

    // called by update method, when I press attack button
    private void attack(Status s)
    {
        // If I'm already attacking, don't begin again
        if (attacking)
            return;

        SetTrailOnOff(true);

        switch (s)
        {
            // if I'm in idle status, I attack with one hand alternating 
            case Status.IDLE:
                {
                    attackIndex = (attackIndex + 1) % (comboAttacks.Length - 1); // -1 because the last attack is the air attack
                    armActualAttack = hands[attackIndex];
                    anim.Play(comboAttacks[attackIndex]);
                    armActualAttack.EnableTrigger();
                    PlayClip(ref missingHitSFX);
                    break;
                }
                // if I'm falling, I can start only air attack
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
                // Start slide attack
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

    // stop attack, called in update method or checkAttack
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

    // check attack is called every frame in update method
    private void checkAttack()
    {
        if (attacking)  // If I'm attacking
        {
            switch (status)
            {
                case Status.IDLE:   // if I'm in IDLE status
                    {
                        // If animation end, I stop attack
                        if (!anim.GetCurrentAnimatorStateInfo(AnimLayerArms).IsName(comboAttacks[attackIndex]))
                        {
                            stopAttack();
                        }
                        else
                        {   // I hand's AttackTrigger hit enemy, I hit it
                            if (hands[attackIndex].EnteredTrigger)
                            {
                                if(hands[attackIndex].hitted.GetComponent<IHittable>().hit())
                                    PlayClip(ref hitSFX);
                            }
                        }

                        break;
                    }
                case Status.FALLING:   // same logic of before
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(AnimLayerBase).IsName(comboAttacks[attackIndex]))
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
                        if (!anim.GetCurrentAnimatorStateInfo(AnimLayerBase).IsName("Slide"))
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

    // when player die, I start the right animation, depending by deathEvent value
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

    // when player win, he start a random win animation 
    public void OnVictory()
    {
        if (status == Status.VICTORY)
            return;

        anim.Play("Victory " + (Random.Range(0, maxVictoryAnim) + 1));
        enableInput = false;
        status = Status.VICTORY;
    }

    // this method allow to check the y position of the player foot and, If they just lean foot on the ground, I play footstep clip
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

    // return the object where I stay on
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }

    // If player get hurt, start invulnerability timer and play hurt animation depending by deathEvent value
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

    // when receive enable input message, I enable or disable input
    private void EnableInput(bool b)
    {
        enableInput = b;
    }

    // method called by animation event in slide animation
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