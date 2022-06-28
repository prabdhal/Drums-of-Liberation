using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class ForestWitchManager : EnemyManager
{
    [Header("Forest Witch Exclusive")]
    public BossDetectionRange detectionRange;

    [SerializeField] float dangerRange = 5f;
    [SerializeField] float chaseRange = 30f;

    private ForestWitchCombat witchCombat;
    private Transform furthestLocationFromPlayer;
    private bool getLocation = false;

    protected override void Start()
    {
        base.Start();
        witchCombat = GetComponent<ForestWitchCombat>();
    }

    protected override void StateHandler()
    {
        currSpeed = Stats.MovementSpeed.Value;
        if (State.Equals(EnemyState.Patrol))
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;

        if (IsInteracting)
            agent.speed = 0f;
        else
            agent.speed = currSpeed;

        float targetDis = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);

        if (PlayerIsDetected() && Combat.CanUseSkill)
        {
            State = EnemyState.Combat;
            CombatState();
        }
        else if (PlayerIsDetected() && targetDis <= dangerRange)
        {
            GetFurthestLocation();

            float distanceFromFurthestLocation = Vector3.Distance(transform.position, furthestLocationFromPlayer.position);
            if (distanceFromFurthestLocation <= Patrol.stoppingDistance)
                getLocation = true;

            agent.isStopped = false;
            agent.SetDestination(furthestLocationFromPlayer.position);
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            RotationHandler(agent.steeringTarget);
        }
        else if (PlayerIsDetected() && targetDis >= chaseRange)
        {
            State = EnemyState.Pursue;
            PursueState();
        }
        else if (TargetPos == null && !IsInteracting)
        {
            State = EnemyState.Patrol;
            PatrolState();
        }
        Combat.CombatHandler();
    }

    private void  GetFurthestLocation()
    {
        if (!State.Equals(EnemyState.Patrol))
            getLocation = true;

        State = EnemyState.Patrol;
        float furthestDistance = 0;
        if (getLocation)
        {
            foreach (Transform loc in witchCombat.TeleportLocations)
            {
                float distanceFromPlayer = Vector3.Distance(loc.position, PlayerManager.Instance.transform.position);

                if (distanceFromPlayer > furthestDistance)
                {
                    furthestDistance = distanceFromPlayer;
                    furthestLocationFromPlayer = loc;
                }
            }
            getLocation = false;
        }
    }

    protected override void PatrolState()
    {
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Idle);
        agent.isStopped = true;
        RotationHandler(agent.steeringTarget);
    }

    protected override void PursueState()
    {
        agent.SetDestination(PlayerManager.Instance.transform.position);
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
        agent.isStopped = false;
        RotationHandler(agent.steeringTarget);
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
        if (isDead) return false;

        if (detectionRange.PlayerIsDetected && !PlayerManager.Instance.IsDead)
        {
            targetPos = PlayerManager.Instance.transform.position;
            return true;
        }

        return false;
    }
}
