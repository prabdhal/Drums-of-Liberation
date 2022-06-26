using System;
using UnityEngine;

public class HumanManager : EnemyManager
{
    [SerializeField]
    protected Transform backStrafeObj, rightStrafeObj, leftStrafeObj;
    [SerializeField]
    protected Transform curStrafeObj;
    [SerializeField]
    protected float startStrafeTimer = 3f;
    protected float curStrafeTimer;
    [SerializeField]
    protected StrafeDirections strafeDir;
    [SerializeField]
    protected int curStrafeDir;
    [SerializeField]
    protected float chaseDistance = 13f;
    [SerializeField]
    protected float strafeDistance = 10f;
    [SerializeField]
    protected float dangerDistance = 3f;
    [SerializeField]
    protected bool isStrafing = false;
    [SerializeField]
    protected float strafeSpeedMultiplier = 5f;



    protected override void Start()
    {
        base.Start();
        curStrafeDir = UnityEngine.Random.Range(0, Enum.GetNames(typeof(StrafeDirections)).Length + 1);
    }

    protected override void StateHandler()
    {
        agent.speed = currSpeed;
        if (IsDetected && Combat.CanUseSkill)
        {
            isStrafing = false;
            State = EnemyState.Combat;
            CombatState();
        }
        else if (IsDetected && Combat.CanUseSkill == false && !IsInteracting ||
           IsSearching && DistanceFromPlayer > Combat.BasicAttackRange && !IsInteracting)
        {
            State = EnemyState.Pursue;
            PursueState();
        }
        else if (TargetPos == null && !IsInteracting)
        {
            isStrafing = false;
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

        float targetDis = Vector3.Distance(transform.position, (Vector3)TargetPos);


        if (targetDis > chaseDistance)
        {
            isStrafing = false;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination((Vector3)TargetPos);
            curStrafeTimer = 0f;
            return;
        }
        else if (!isStrafing && targetDis > strafeDistance)
        {
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination((Vector3)TargetPos);
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
            }
            else
            {
                curStrafeTimer -= Time.deltaTime;
            }

            if (curStrafeObj == null) curStrafeObj = rightStrafeObj;

            currSpeed = Stats.MovementSpeed.Value * strafeSpeedMultiplier;
            agent.isStopped = false;
            anim.SetInteger(StringData.EnemyMoveState, 6);
            agent.SetDestination(curStrafeObj.position);
        }
        else
        {
            isStrafing = false;
            anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.Sprinting);
            currSpeed = Stats.MovementSpeed.Value * Pursue.speedRatioMultiplier;
            agent.isStopped = false;
            agent.SetDestination((Vector3)TargetPos);
            curStrafeTimer = 0f;
        }
    }

    protected override void CombatState()
    {
        RotationHandler(PlayerPos);
        anim.SetInteger(StringData.EnemyMoveState, (int)MovementState.CombatIdle);
        agent.isStopped = true;

        Combat.AttackHandler(anim, DistanceFromPlayer);
    }

    protected override void RotationHandler(Vector3 rotateTo)
    {
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
}
