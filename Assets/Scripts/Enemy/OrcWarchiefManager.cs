using UnityEngine;

public class OrcWarchiefManager : EnemyManager
{
    [SerializeField]
    protected float chaseDistance = 15f;
    [SerializeField]
    protected float dangerDistance = 2f;
    private bool isChasing = false;


    protected override void Start()
    {
        base.Start();
    }

    protected override void StateHandler()
    {
        float distanceFromPlayer = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

        if (IsInteracting) agent.speed = 0.1f;
        else agent.speed = currSpeed;

        if (IsDetected && Combat.CanUseSkill)
        {
            State = EnemyState.Combat;
            CombatState();
        }
        else if (IsSearching && distanceFromPlayer > Combat.BasicAttackRange && !IsInteracting)
        {
            State = EnemyState.Searching;
            SearchingState();
        }
        else if (IsDetected && Combat.CanUseSkill == false && !IsInteracting)
        {
            State = EnemyState.Pursue;
            PursueState();
        }
        else if (!IsDetected && !IsSearching && !IsInteracting)
        {
            State = EnemyState.Patrol;
            PatrolState();
        }
        Combat.CombatHandler();
    }

    protected override void PatrolState()
    {
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

    protected override void PursueState()
    {
        RotationHandler(agent.steeringTarget);

        float targetDis = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);

        if (targetDis > chaseDistance)
        {
            isChasing = true;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination(PlayerManager.Instance.transform.position);
            return;
        }
        else if (targetDis <= dangerDistance)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
            isChasing = false;
            currSpeed = 0;
            agent.isStopped = true;
            return;
        }
        else if (targetDis > dangerDistance && targetDis <= chaseDistance && !isChasing)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Walking);
            currSpeed = Stats.MovementSpeed.Value;
            agent.isStopped = false;
            agent.SetDestination(PlayerManager.Instance.transform.position);
            return;
        }
        else if (!isChasing)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
            currSpeed = 0f;
            agent.isStopped = true;
            return;
        }
        else
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination(PlayerManager.Instance.transform.position);
            return;
        }
    }

    private void SearchingState()
    {
        if (targetPos == null)
            return;

        float distance = Vector3.Distance(transform.position, (Vector3)TargetPos);

        if (distance <= Patrol.stoppingDistance)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
            agent.isStopped = true;
        }
        else
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination((Vector3)TargetPos);
        }
    }

    protected override void CombatState()
    {
        float distanceFromPlayer = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);
        RotationHandler(PlayerManager.Instance.transform.position);
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
        agent.isStopped = true;

        Combat.AttackHandler(anim, distanceFromPlayer);
    }

    public override bool PlayerIsDetected(bool overrideDetection = false)
    {
        return base.PlayerIsDetected(overrideDetection);
    }

    protected override bool SearchingPlayer()
    {
        return base.SearchingPlayer();
    }

    protected override void RotationHandler(Vector3 rotateTo)
    {
        Vector3 targetDir = (rotateTo - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotSpeed * Time.deltaTime);
    }
}
