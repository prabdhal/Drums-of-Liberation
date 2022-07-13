using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class BarbarianGiantManager : EnemyManager
{
    [Header("Barbarian Giant Exclusive")]
    public BossDetectionRange detectionRange;

    [SerializeField] float chaseRange = 15f;

    private BarbarianGiantCombat barbarianGiantCombat;


    protected override void Start()
    {
        base.Start();
        barbarianGiantCombat = GetComponent<BarbarianGiantCombat>();
    }

    protected override void StateHandler()
    {
        if (IsInteracting && !barbarianGiantCombat.isAttacking02)
            agent.speed = 0f;
        else
            agent.speed = currSpeed;

        if (IsDetected && Combat.CanUseSkill)
        {
            State = EnemyState.Combat;
            CombatState();
        }
        else if (IsDetected && Combat.CanUseSkill == false && !IsInteracting)
        {
            State = EnemyState.Pursue;
            PursueState();
        }
        else if (!IsDetected)
        {
            State = EnemyState.Patrol;
            PatrolState();
        }
        Combat.CombatHandler();
    }

    protected override void PursueState()
    {
        RotationHandler(agent.steeringTarget);
        float targetDis = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);
        
        if (targetDis >= chaseRange)
        {
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.SetDestination(PlayerManager.Instance.transform.position);
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            agent.SetDestination(PlayerManager.Instance.transform.position); 
            agent.isStopped = false;
        }
        else if (targetDis > barbarianGiantCombat.BasicAttackRange)
        {
            currSpeed = Stats.MovementSpeed.Value;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Walking);
            agent.SetDestination(PlayerManager.Instance.transform.position);
            agent.isStopped = false;
        }
        else
        {
            currSpeed = 0f;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
            agent.isStopped = true;
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
        if (isDead) return false;

        if (detectionRange.PlayerIsDetected && !PlayerManager.Instance.IsDead)
        {
            targetPos = PlayerManager.Instance.transform.position;
            return true;
        }

        return false;
    }
}
