using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour
{
    //components
    public NavMeshAgent agent;
    protected CapsuleCollider col;
    protected Rigidbody rb;
    protected Animator anim;
    protected PerfectLookAt lookAt;

    public Transform popupPos;

    // stats
    public EnemyStats Stats { get { return stats; } set { stats = value; } }
    [SerializeField] protected EnemyStats stats;

    [Space]

    [SerializeField]
    protected float rotSpeed = 10f;
    protected float currSpeed;

    [Tooltip("Drag in head object only if head's z-axis is straight, else create an empty object on head with straight z-axis and drag that.")]
    [SerializeField] protected Transform head;
    //protected GameObject tempLookAtObj;
    [Tooltip("Raycast from head to player offset.")]
    [SerializeField] protected Vector3 detectionOffset;
    [SerializeField] protected string enemyTag = StringData.PlayerTag;

    [SerializeField] protected Patrol Patrol;
    [SerializeField] protected Pursue Pursue;
    [SerializeField] protected ICombat Combat;

    [Space]

    [Tooltip("The XP reward upon killing this enemy.")]
    [SerializeField]
    protected float xpReward = 150f;
    public EnemyState State = EnemyState.Patrol;

    // Important properties for state handler
    public Vector3 PlayerPos { get { return PlayerManager.Instance.transform.position; } }
    public float DistanceFromPlayer { get { return Vector3.Distance(PlayerPos, transform.position); } }
    public bool IsDetected { get { return PlayerIsDetected(); } }
    public bool IsSearching { get { return SearchingPlayer(); } }
    public Vector3? TargetPos { get { return targetPos; } }
    protected Vector3? targetPos = null;
    public bool IsInteracting { get { return anim.GetBool(StringData.IsInteracting); } }
    public bool IsDead { get { return isDead; } }
    protected bool isDead = false;

    private Vector3 _lastPosition;

    public event OnDeath OnDeathEvent;
    public delegate void OnDeath(EnemyManager enemy);


    protected virtual void Start()
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
        agent.stoppingDistance = 2f;

        col.center = new Vector3(0f, 1f, 0f);
        col.height = 2f;

        _lastPosition = gameObject.transform.position;

        Patrol.currWP = UnityEngine.Random.Range(0, Patrol.waypoints.Length - 1);

        transform.tag = StringData.EnemyTag;

        OnDeathEvent += OnDeath_EnemyManager;
    }

    protected virtual void Update()
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

    protected virtual void GetRelativeMoveDirection()
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

    protected virtual void RotationHandler(Vector3 rotateTo)
    {
        if (IsInteracting) return;

        Vector3 targetDir = (rotateTo - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotSpeed * Time.deltaTime);
    }

    protected virtual void StateHandler()
    {
        agent.speed = currSpeed;

        if (IsDetected && Combat.CanUseSkill)
        {
            State = EnemyState.Combat;
            //lookAt.m_TargetObject = PlayerManager.Instance.gameObject;
            CombatState();
        }
        else if (IsDetected && Combat.CanUseSkill == false && !IsInteracting ||
           IsSearching && DistanceFromPlayer > Combat.BasicAttackRange && !IsInteracting)
        {
            State = EnemyState.Pursue;
            //lookAt.m_TargetObject = PlayerManager.Instance.gameObject;
            PursueState();
        }
        else if (IsDetected && Combat.CanUseSkill == false && !IsInteracting ||
           IsSearching && DistanceFromPlayer > Combat.BasicAttackRange && !IsInteracting)
        {
            State = EnemyState.Pursue;
            //lookAt.m_TargetObject = PlayerManager.Instance.gameObject;
            NonStrafePursue();
        }
        else if (TargetPos == null && !IsInteracting)
        {
            State = EnemyState.Patrol;
            //lookAt.m_TargetObject = tempLookAtObj;
            PatrolState();
        }
        Combat.CombatHandler();
    }

    protected virtual void PatrolState()
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

    protected virtual void NonStrafePursue()
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

    protected virtual void PursueState()
    {
        agent.SetDestination(PlayerManager.Instance.transform.position);
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
        agent.isStopped = false;

        RotationHandler(agent.steeringTarget);
    }

    protected virtual void CombatState()
    {
        //Debug.Log("Combat State");
        RotationHandler(PlayerPos);
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
        agent.isStopped = true;

        Combat.AttackHandler(anim, DistanceFromPlayer);
    }

    public virtual bool PlayerIsDetected(bool overrideDetection = false)
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

    protected bool SearchingPlayer()
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

    protected virtual void OnDeath_EnemyManager(EnemyManager enemy)
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

    public void GetHitDirection(Transform hitObj)
    {
        Vector3 incomingDir = transform.position - hitObj.position;

        float dir = Vector3.Dot(transform.forward, incomingDir);

        if (dir > 0.5f)
            anim.Play(StringData.HitB);
        else if (dir < 0.5f && dir > -0.5f)
        {
            dir = Vector3.Dot(transform.right, incomingDir);
            if (dir < 0)
                anim.Play(StringData.HitR);
            else
                anim.Play(StringData.HitL);
        }
        else if (dir < -0.5f)
            anim.Play(StringData.HitF);
    }
}
