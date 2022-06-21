using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour
{
    //components
    private NavMeshAgent agent;
    private CapsuleCollider col;
    private Rigidbody rb;
    private Animator anim;
    private PerfectLookAt lookAt;

    public Transform popupPos;

    // stats
    public EnemyStats Stats { get { return stats; } set { stats = value; } }
    [SerializeField]
    private EnemyStats stats;

    [Space]

    [SerializeField] bool isStrafer = false;
    [SerializeField] float rotSpeed = 10f;
    private float currSpeed;

    [Tooltip("Drag in head object only if head's z-axis is straight, else create an empty object on head with straight z-axis and drag that.")]
    [SerializeField]
    private Transform head;
    [SerializeField]
    private GameObject tempLookAtObj;
    [Tooltip("Raycast from head to player offset.")]
    [SerializeField]
    private Vector3 detectionOffset;
    [SerializeField]
    private string enemyTag = "Player";

    [SerializeField]
    private Patrol Patrol;
    [SerializeField]
    private Pursue Pursue;
    [SerializeField]
    private ICombat Combat;

    [Space]

    [SerializeField] Transform backStrafeObj, rightStrafeObj, leftStrafeObj;
    Transform curStrafeObj;
    [SerializeField] float startStrafeTimer = 3f;
    float curStrafeTimer;
    [SerializeField] StrafeDirections strafeDir;
    [SerializeField] int curStrafeDir;
    [SerializeField] float chaseDistance = 13f;
    [SerializeField] float strafeDistance = 10f;
    [SerializeField] float dangerDistance = 3f;
    private bool isStrafing = false;
    [SerializeField] float strafeSpeedMultiplier = 5f;

    [Space]

    [Tooltip("The XP reward upon killing this enemy.")]
    [SerializeField]
    private float xpReward = 150f;

    public EnemyState State = EnemyState.Patrol;


    // Important properties for state handler
    public Vector3 PlayerPos { get { return PlayerManager.Instance.transform.position; } }
    public float DistanceFromPlayer { get { return Vector3.Distance(PlayerPos, transform.position); } }
    public bool IsDetected { get { return PlayerIsDetected(); } }
    public bool IsSearching { get { return SearchingPlayer(); } }
    public Vector3? TargetPos { get { return targetPos; } }
    private Vector3? targetPos = null;
    public bool IsInteracting { get { return anim.GetBool(StringData.IsInteracting); } }
    public bool IsDead { get { return isDead; } }
    private bool isDead = false;

    private Vector3 _lastPosition;


    public event OnDeath OnDeathEvent;
    public delegate void OnDeath(EnemyManager enemy);


    private void Start()
    {
        Stats.Init();

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        Combat = GetComponent<ICombat>();
        //lookAt = GetComponent<PerfectLookAt>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode.Equals(CollisionDetectionMode.ContinuousDynamic);

        agent.updateRotation = false;
        agent.acceleration = 9999;
        agent.autoBraking = false;
        agent.stoppingDistance = 1f;

        col.center = new Vector3(0f, 1f, 0f);
        col.height = 2f;

        _lastPosition = gameObject.transform.position;
        curStrafeDir = UnityEngine.Random.Range(0, Enum.GetNames(typeof(StrafeDirections)).Length + 1);

        Patrol.currWP = UnityEngine.Random.Range(0, Patrol.waypoints.Length - 1);

        transform.tag = StringData.EnemyTag;

        OnDeathEvent += OnDeath_EnemyManager;
    }

    private void Update()
    {
        if (Stats.CurrentHealth <= 0)
            isDead = true;

        if (IsDead && agent.enabled)
        {
            OnDeathEvent(this);
            return;
        }

        Stats.Update();

        GetRelativeMoveDirection();
        StateHandler();
    }

    private void GetRelativeMoveDirection()
    {
        var currentPosition = gameObject.transform.position;
        var moveDirection = currentPosition - _lastPosition;

        Vector2 movement = UtilityMethods.AgentVelocityToVector2DInput(agent);

        float x = movement.x;
        float z = movement.y;

        anim.SetFloat(StringData.MoveSpeedX, x);
        anim.SetFloat(StringData.MoveSpeedY, z);

        _lastPosition = currentPosition;
    }
    
    private void RotationHandler(Vector3 rotateTo)
    {
        if (IsInteracting) return;

        if (isStrafing)
        {
            var lookPos = ((Vector3)TargetPos - transform.position).normalized;
            lookPos.y = 0;
            Quaternion lookRot = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRot, rotSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 targetDir = (rotateTo - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotSpeed * Time.deltaTime);
        }
    }

    private void StateHandler()
    {
        agent.speed = currSpeed;
        if (IsDetected && Combat.CanUseSkill)
        {
            isStrafing = false;
            State = EnemyState.Combat;
            //lookAt.m_TargetObject = PlayerManager.Instance.gameObject;
            CombatState();
        }
        else if (isStrafer && IsDetected && Combat.CanUseSkill == false && !IsInteracting ||
           isStrafer && IsSearching && DistanceFromPlayer > Combat.BasicAttackRange && !IsInteracting)
        {
            State = EnemyState.Pursue;
            //lookAt.m_TargetObject = PlayerManager.Instance.gameObject;
            PursueState();
        }
        else if (isStrafer == false && IsDetected && Combat.CanUseSkill == false && !IsInteracting ||
           isStrafer == false && IsSearching && DistanceFromPlayer > Combat.BasicAttackRange && !IsInteracting)
        {
            State = EnemyState.Pursue;
            //lookAt.m_TargetObject = PlayerManager.Instance.gameObject;
            NonStrafePursue();
        }
        else if (TargetPos == null && !IsInteracting)
        {
            isStrafing = false;
            State = EnemyState.Patrol;
            //lookAt.m_TargetObject = tempLookAtObj;
            PatrolState();
        }
        Combat.CombatHandler();
    }

    private void PatrolState()
    {
        //Debug.Log("Patrol State");
        RotationHandler(agent.steeringTarget);
        bool isInRange = Vector3.Distance(transform.position, Patrol.waypoints[Patrol.currWP].position) <= Patrol.stoppingDistance;
        currSpeed = Stats.MovementSpeed.Value;

        if (isInRange)
        {
            agent.SetDestination(Patrol.waypoints[Patrol.currWP].position);
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Idle);

            if (Patrol.currWaitTimer <= 0)
            {
                float waitTimer = UnityEngine.Random.Range(Patrol.waitTimerMin, Patrol.waitTimerMax);
                Patrol.currWP = UnityEngine.Random.Range(0, Patrol.waypoints.Length - 1);

                Patrol.currWaitTimer = waitTimer;
            }
            else
            {
                Patrol.currWaitTimer -= Time.deltaTime;
                agent.isStopped = true;
            }
        }
        else
        {
            agent.SetDestination(Patrol.waypoints[Patrol.currWP].position);
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Walking);
            agent.isStopped = false;
        }
    }

    private void NonStrafePursue()
    {
        RotationHandler(agent.steeringTarget);

        float targetDis = Vector3.Distance(transform.position, (Vector3)TargetPos);

        if (targetDis <= Combat.BasicAttackRange)
        {
            agent.isStopped = true;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
        }
        else
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination((Vector3)TargetPos);
        }
    }

    private void PursueState()
    {
        RotationHandler(agent.steeringTarget);

        float targetDis = Vector3.Distance(transform.position, (Vector3)TargetPos);

        
        if (targetDis > chaseDistance)
        {
            isStrafing = false;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination((Vector3)TargetPos);
            //Debug.Log("init chasing to get within st dis");
            curStrafeTimer = 0f;
            return;
        }
        else if (!isStrafing && targetDis > strafeDistance)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination((Vector3)TargetPos);
            //Debug.Log("Chasing to get within strafe distance");
            curStrafeTimer = 0f;
            return;
        }
        else if (targetDis <= chaseDistance && Combat.SkillCooldownIsReady == false)
        {
            isStrafing = true;

            if (targetDis <= dangerDistance)
            {
                curStrafeObj = backStrafeObj;
                strafeDir = (StrafeDirections)curStrafeDir;
                curStrafeTimer = startStrafeTimer;
                //Debug.Log("Strafing back since within danger:" + dangerDistance);
            }
            else if (curStrafeTimer <= 0)
            {
                curStrafeDir = UnityEngine.Random.Range(0, Enum.GetNames(typeof(StrafeDirections)).Length);
                if ((int)strafeDir != curStrafeDir)
                {
                    switch ((StrafeDirections)curStrafeDir)
                    {
                        case StrafeDirections.Right:
                            curStrafeObj = rightStrafeObj;
                            break;
                        case StrafeDirections.Left:
                            curStrafeObj = leftStrafeObj;
                            break;
                        default:
                            curStrafeObj = leftStrafeObj;
                            break;
                    }
                    strafeDir = (StrafeDirections)curStrafeDir;
                }

                curStrafeTimer = startStrafeTimer;
                //Debug.Log("Strafing right/left");
            }
            else
            {
                curStrafeTimer -= Time.deltaTime;
                //Debug.Log("Strafing timer");
            }

            if (curStrafeObj == null) curStrafeObj = rightStrafeObj;

            currSpeed = Stats.MovementSpeed.Value * strafeSpeedMultiplier;
            agent.isStopped = false;
            anim.SetInteger(StringData.EnemyMoveState, 6);
            agent.SetDestination(curStrafeObj.position);
            //Debug.Log("Strafing");
        }
        else
        {
            isStrafing = false;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination((Vector3)TargetPos);
            curStrafeTimer = 0f;
            //Debug.Log("PURSUE: Target outside of range - sprint");
        }
    }

    private void CombatState()
    {
        //Debug.Log("Combat State");
        RotationHandler(PlayerPos);
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
        agent.isStopped = true;

        Combat.AttackHandler(anim, DistanceFromPlayer);
    }

    public bool PlayerIsDetected(bool overrideDetection = false)
    {
        if (isDead) return false;

        if (overrideDetection)
            targetPos = PlayerManager.Instance.transform.position;

        if (DistanceFromPlayer <= Pursue.detectionRange)
        {
            Vector3 targetDir = (PlayerPos - head.position) + detectionOffset;
            float angle;
            if (TargetPos == null)
                angle = Vector3.SignedAngle(head.forward, targetDir, transform.up);
            else
                angle = 0;

            if (angle >= -Pursue.detectionRadius / 2 && angle <= Pursue.detectionRadius / 2)
            {
                RaycastHit hit;
                Vector3 origin = head.position;

                Debug.DrawRay(origin, targetDir * Pursue.detectionRange, Color.red);
                if (Physics.Raycast(origin, targetDir, out hit, Pursue.detectionRange, Pursue.targetLayer))
                {
                    if (!hit.transform.tag.Equals(StringData.PlayerTag)) return false;

                    Debug.Log("Player is in view of " + transform.name);
                    targetPos = PlayerManager.Instance.transform.position;
                    Pursue.currSearchingTimer = Pursue.startSearchingTimer;
                    return true;
                }
            }
        }
        return false;
    }

    private bool SearchingPlayer()
    {
        if (isDead) return false;

        if (TargetPos != null && !IsDetected)
        {
            if (Pursue.currSearchingTimer <= 0)
            {
                Pursue.currSearchingTimer = Pursue.startSearchingTimer;
                targetPos = null;
                return false;
            }
            else
                Pursue.currSearchingTimer -= Time.deltaTime;
            return true;
        }

        return false;
    }

    private void OnDeath_EnemyManager(EnemyManager enemy)
    {
        anim.SetBool(StringData.IsInteracting, true);
        anim.SetBool(StringData.IsDead, true);
        anim.SetTrigger(StringData.Dead);
        PlayerManager.Instance.Stats.AddPlayerExperience(xpReward);
        //taskChecker.CheckTaskCompletion(enemyTag);
        //QuestManager.Instance.UpdateKillQuestProgress(enemyTag);
        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;
        col.enabled = false;
        transform.tag = StringData.Untagged;
        Destroy(gameObject, GlobalValues.DestroyGOUponDeathTimer);
    }
}
