using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // components
    private PlayerManager manager;
    private PlayerControls controls;
    private PlayerCombat combat;
    private CharacterController controller;
    private CapsuleCollider col;
    private Animator anim;
    private Transform cam;

    private float horizontal;
    private float vertical;
    private Vector3 moveDir;
    private Vector3 playerVelocity;
    private Vector3 diveMoveDir;

    // stats
    private float curSpeed;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float strafeSpeed = 3.5f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float rotSpeed = 5f;
    [SerializeField] float diveSpeed = 7f;
    [SerializeField] float diveStrafeSpeed = 5f;

    ////jump 
    //private float initialJumpVelocity;
    //[SerializeField]
    //private float maxJumpHeight = 4.0f;
    //[SerializeField]
    //private float maxJumpTime = 0.75f;

    private float diveX;
    private float diveZ;

    [SerializeField] float startRollTimer = 0.5f;
    [SerializeField] float startStrafeRollTimer = 0.5f;
    private float curRollTimer = 0f;

    [SerializeField] float startDiveTimeoutTimer = 0.5f;
    private float curDiveTimeoutTimer;

    //[SerializeField] float startJumpTimer = 1f;
    //private float curJumpTimer;

    private float colHeight;
    private float colY;
    private float controllerHeight;
    private float controllerY;

    [SerializeField] MovementState movementState;



    private void Start()
    {
        manager = PlayerManager.Instance;
        controls = PlayerControls.Instance;
        combat = GetComponentInChildren<PlayerCombat>();
        controller = GetComponent<CharacterController>();
        col = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<Animator>();
        cam = Camera.main.transform;

        colHeight = col.height;
        colY = col.center.y;
        controllerHeight = controller.height;
        controllerY = controller.center.y;

        controls.OnDiveEvent += OnDive;

        curRollTimer = startRollTimer;
        curDiveTimeoutTimer = startDiveTimeoutTimer;
        //curJumpTimer = startJumpTimer;

        //SetupJumpVariables();
    }

    private void Update()
    {
        if (manager.IsDead) return;

        HandleGravity();
        combat.CombatUpdate();

        if (PlayerManager.Instance.StatusEffectManager.IsStunned)
        {
            anim.SetBool(StringData.IsRolling, false);
            anim.Play(StringData.Idle);
            return;
        }
        AnimationHandler();
        if (manager.isDiving)
        {
            DiveHandler();
            return;
        }
        if (manager.IsInteracting == false)
        {
            anim.SetBool(StringData.IsRolling, false);
        }
        //JumpHandler();

        if (manager.TargetLock)
        {
            //Debug.Log("lock movement");
            LockMovementHandler();
            LockRotationHandler();
        }
        else
        {
            //Debug.Log("movement");
            MovementHandler();
            RotationHandler();
        }

        if (curDiveTimeoutTimer > 0)
            curDiveTimeoutTimer -= Time.deltaTime;
    }

    private void AnimationHandler()
    {
        var moveSpeed = moveDir;
        moveSpeed.y = 0;

        anim.SetFloat(StringData.MoveSpeed, moveSpeed.magnitude);
        anim.SetFloat(StringData.MoveSpeedX, controls.MovementDirection.x);
        anim.SetFloat(StringData.MoveSpeedY, controls.MovementDirection.y);
        anim.SetBool(StringData.IsSprinting, controls.IsSprinting);
        anim.SetBool(StringData.LockOn, manager.TargetLock);

        anim.SetBool(StringData.IsGrounded, controller.isGrounded);
        anim.SetBool(StringData.IsJumping, manager.IsJumping);

        manager.IsInteracting = anim.GetBool(StringData.IsInteracting);
    }

    private void MovementHandler()
    {
        if (PlayerManager.Instance.StatusEffectManager.IsRooted)
            curSpeed = 0f;
        else if (manager.IsInteracting)
            curSpeed = 0.1f;
        else if (controls.IsSprinting)
            curSpeed = sprintSpeed;
        else
            curSpeed = moveSpeed;

        horizontal = controls.MovementDirection.x;
        vertical = controls.MovementDirection.y;

        moveDir = new Vector3(horizontal, playerVelocity.y, vertical);
        moveDir = moveDir.x * cam.right + moveDir.z * cam.forward;
        moveDir.y = playerVelocity.y;
        moveDir.Normalize();

        controller.Move(moveDir * curSpeed * Time.deltaTime);
    }

    private void LockMovementHandler()
    {
        if (manager.IsInteracting)
            curSpeed = 0.1f;
        else
            curSpeed = strafeSpeed;

        horizontal = controls.MovementDirection.x;
        vertical = controls.MovementDirection.y;

        moveDir = new Vector3(horizontal, playerVelocity.y, vertical);
        moveDir = moveDir.x * cam.right + moveDir.z * cam.forward;
        moveDir.y = playerVelocity.y;
        moveDir.Normalize();

        controller.Move(moveDir * curSpeed * Time.deltaTime);
    }

    private void RotationHandler()
    {
        if (PlayerManager.Instance.IsInteracting) return;

        moveDir.y = 0;
        if (moveDir.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }

    private void LockRotationHandler()
    {
        Vector3 dir = manager.lockOnTarget.transform.position - transform.position;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
    }

    private void HandleGravity()
    {
        bool isFalling = playerVelocity.y <= 0.0f;
        float fallMultiplier = 2.0f;

        if (controller.isGrounded)
        {
            playerVelocity.y = GlobalValues.GroundedGravityForce;
        }
        else if (isFalling)
        {
            float prevYVel = playerVelocity.y;
            float newYVel = playerVelocity.y + (GlobalValues.GravityForce * fallMultiplier * Time.deltaTime);
            float nextYVel = Mathf.Max((prevYVel + newYVel) * 0.5f, -20.0f);
            playerVelocity.y = nextYVel;
        }
        else
        {
            float prevYVel = playerVelocity.y;
            float newYVel = playerVelocity.y + (GlobalValues.GravityForce * Time.deltaTime);
            float nextYVel = (prevYVel + newYVel) * 0.5f;
            playerVelocity.y = nextYVel;
        }
    }

    //private void SetupJumpVariables()
    //{
    //    float timeToApex = maxJumpTime / 2f;
    //    GlobalValues.GravityForce = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
    //    initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    //}

    //private void JumpHandler()
    //{
    //    if (!manager.IsJumping && controller.isGrounded && controls.IsJumpPressed && curJumpTimer <= 0)
    //    {
    //        manager.IsJumping = true;
    //        anim.Play("Jump_Start");
    //        playerVelocity.y = initialJumpVelocity;
    //        curJumpTimer = startJumpTimer;
    //    }
    //    else if (manager.IsJumping)
    //        manager.IsJumping = false;

    //    curJumpTimer -= Time.deltaTime;
    //}

    private void DiveHandler()
    {
        float speed = diveSpeed;
        if (manager.TargetLock)
            speed = diveStrafeSpeed;

        if (manager.TargetLock)
        {
            diveMoveDir = new Vector3(diveX, 0f, diveZ);
            diveMoveDir = diveMoveDir.x * new Vector3(transform.right.x, 0f, transform.right.z) + diveMoveDir.z * new Vector3(transform.forward.x, 0f, transform.forward.z);
        }
        else
            diveMoveDir = new Vector3(transform.forward.x, 0f, transform.forward.z);

        diveMoveDir.Normalize();
        controller.Move(diveMoveDir * speed * Time.deltaTime);

        if (curRollTimer <= 0)
        {
            manager.isDiving = false;
            controller.height = controllerHeight;
            controller.center = new Vector3(controller.center.x, controllerY, controller.center.z);
            col.height = colHeight;
            col.center = new Vector3(col.center.x, colY, col.center.z);
            anim.SetBool(StringData.IsRolling, false);
            anim.SetBool(StringData.IsInteracting, false);
        }
        else
            curRollTimer -= Time.deltaTime;
    }

    private void OnDive()
    {
        if (curDiveTimeoutTimer > 0) return;
        curDiveTimeoutTimer = startDiveTimeoutTimer;

        manager.isDiving = true;

        controller.height = controllerHeight / 2f;
        controller.center = new Vector3(controller.center.x, controllerY / 2f, controller.center.z);
        col.height = colHeight / 2f;
        col.center = new Vector3(col.center.x, colY / 2f, col.center.z);

        anim.SetBool(StringData.IsInteracting, true);

        if (manager.TargetLock)
        {
            curRollTimer = startStrafeRollTimer;
            anim.SetBool(StringData.IsRolling, true);
            anim.Play(StringData.StrafeRoll);
        }
        else
        {
            curRollTimer = startRollTimer;
            anim.Play(StringData.DiveRollForward);
        }

        diveX = controls.MovementDirection.x;
        diveZ = controls.MovementDirection.y;
        if (diveX == 0 && diveZ == 0)
            diveZ = 1;

        anim.SetFloat(StringData.RollX, diveX);
        anim.SetFloat(StringData.RollZ, diveZ);
    }
}