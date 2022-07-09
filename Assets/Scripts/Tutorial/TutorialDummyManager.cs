using UnityEngine;

public class TutorialDummyManager : EnemyManager
{
    public bool dummy = false;
    public bool engagePlayer = false;

    [SerializeField]
    protected float chaseDistance = 13f;

    protected override void StateHandler()
    {
        if (isDead)
        {
            State = EnemyState.Idle;
            return;
        }

        agent.speed = Stats.MovementSpeed.Value;

        if (dummy)
        {
            State = EnemyState.Idle;
            PatrolState();
            return;
        }

        if (IsDetected && Combat.CanUseSkill)
        {
            //lookAt.m_TargetObject = PlayerManager.Instance.gameObject;
            State = EnemyState.Combat;
            CombatState();
        }
        else if (IsDetected && !Combat.CanUseSkill && !IsInteracting)
        {
            //lookAt.m_TargetObject = PlayerManager.Instance.gameObject;
            State = EnemyState.Pursue;
            PursueState();
        }
        else
        {
            State = EnemyState.Idle;
            PatrolState();
        }
        Combat.CombatHandler();
    }

    protected override void PursueState()
    {
        RotationHandler(agent.steeringTarget);
        float targetDis = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);

        if (targetDis > Combat.BasicAttackRange)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            agent.isStopped = false;
        }
        else if (targetDis <= Combat.BasicAttackRange && Combat.SkillCooldownIsReady == false)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
            agent.isStopped = true;
        }

        agent.SetDestination(PlayerManager.Instance.transform.position);
    }

    protected override void PatrolState()
    {
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Idle);
        agent.isStopped = true;
    }

    public override bool PlayerIsDetected(bool overrideDetection = false)
    {
        if (engagePlayer)
            return true;

        return false;
    }

    protected override void OnDeath_EnemyManager(EnemyManager enemy)
    {
        anim.SetBool(StringData.IsInteracting, true);
        anim.SetBool(StringData.IsDead, true);
        anim.Play(StringData.Dead);

        agent.ResetPath();
        agent.isStopped = true;
        transform.tag = StringData.Obstacle;
    }
}
