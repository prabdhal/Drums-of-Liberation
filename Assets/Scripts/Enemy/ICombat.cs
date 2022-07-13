using UnityEngine;


public interface ICombat
{
    public float BasicAttackRange { get; }
    public bool SkillCooldownIsReady { get; }
    public bool IsInRange { get; }
    public bool CanUseSkill { get; }
    public void CombatHandler();
    public void AttackHandler(Animator anim, float playerDistance);
}