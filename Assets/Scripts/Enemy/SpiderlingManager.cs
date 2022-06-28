using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class SpiderlingManager : EnemyManager, ISummon
{
    public Transform Summoner { get; set; }
    [SerializeField] float attackDistanceFromSummoner;

    [Header("Spiderling Exclusive")]
    [SerializeField] float chaseRange = 2f;

    private bool isSpawned = true;
    private float spawnTimer = 0;


    protected override void Start()
    {
        base.Start();
        isSpawned = true;
    }

    protected override void Update()
    {
        base.Update();
        
        if (isSpawned && !isDead)
        {
            agent.isStopped = false;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Idle);

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= 1f)
                isSpawned = false;
        }
    }

    protected override void StateHandler()
    {
        if (isDead || isSpawned) return;

        currSpeed = Stats.MovementSpeed.Value;
        agent.speed = currSpeed;

        float targetDis = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);

        if (PlayerIsDetected() && Combat.CanUseSkill)
        {
            State = EnemyState.Combat;
            CombatState();
        }
        else if (PlayerIsDetected() && !IsInteracting)
        {
            State = EnemyState.Pursue;
            PursueState();
        }
        else if (!PlayerIsDetected() && !IsInteracting)
        {
            State = EnemyState.Patrol;
            PatrolState();
        }
        Combat.CombatHandler();
    }

    protected override void PatrolState()
    {
        RotationHandler(agent.steeringTarget);

        float dis = Vector3.Distance(transform.position, Summoner.transform.position);

        if (dis <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Idle);
            return;
        }

        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Walking);
        agent.SetDestination(Summoner.position);
        agent.isStopped = false;
    }

    protected override void PursueState()
    {
        float targetDis = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

        if (targetDis > chaseRange)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            if (agent.enabled) agent.isStopped = false;
        }
        else
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Idle);
            agent.isStopped = true;
        }
        if (agent.enabled) agent.SetDestination(PlayerManager.Instance.transform.position);
        RotationHandler(PlayerManager.Instance.transform.position);
    }

    protected override void CombatState()
    {
        float distanceFromPlayer = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);
        RotationHandler(PlayerManager.Instance.transform.position);
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Idle);
        if (agent.enabled) agent.isStopped = true;

        Combat.AttackHandler(anim, distanceFromPlayer);
    }

    public override bool PlayerIsDetected(bool overrideDetection = false)
    {
        if (isDead || PlayerManager.Instance.IsDead) return false;

        return true;
    }


    protected override void OnDeath_EnemyManager(EnemyManager enemy)
    {
        anim.SetBool(StringData.IsInteracting, true);
        anim.SetBool(StringData.IsDead, true);
        anim.SetTrigger(StringData.Dead);

        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;
        col.enabled = false;
        transform.tag = StringData.Untagged;
        Destroy(gameObject, GlobalValues.DestroyGOUponDeathTimer);
    }
}
