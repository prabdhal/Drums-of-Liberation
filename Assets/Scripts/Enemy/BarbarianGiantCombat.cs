using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbarianGiantCombat : MonoBehaviour, ICombat
{
    private EnemyManager manager;
    public float BasicAttackRange { get { return basicAttackRange; } }
    [SerializeField]
    private float basicAttackRange = 7f;
    public bool SkillCooldownIsReady { get { return skillCooldownIsReady; } }
    private bool skillCooldownIsReady = true;
    public bool IsInRange { get { return isInRange; } }
    private bool isInRange = true;
    public bool CanUseSkill { get { return canUseSkill; } }
    private bool canUseSkill = true;

    [Space]

    [Header("Skill 01")]
    [SerializeField]
    private string skillName01 = "";
    [SerializeField]
    [TextArea(2, 3)]
    private string skillDescription01 = "";
    [SerializeField]
    private float skillRange01 = 3f;
    [SerializeField]
    private float startSkillCooldown01 = 5f;
    private float currSkillCooldown01 = 0;
    [SerializeField]
    [Tooltip("The attack collider gameobject of attack 01.")]
    private GameObject attackCollider01;
    private EnemyAttackCollider attackColliderScript01;

    [Space]

    [Header("Skill 02")]
    [SerializeField]
    private string skillName02 = "";
    [SerializeField]
    [TextArea(2, 3)]
    private string skillDescription02 = "";
    [SerializeField]
    private float skillRange02 = 3f;
    [SerializeField]
    private float startSkillCooldown02 = 10f;
    private float currSkillCooldown02 = 10f;
    [SerializeField]
    [Tooltip("The attack collider gameobject of attack 02.")]
    private GameObject attackCollider02;
    private EnemyAttackCollider attackColliderScript02;



    private void Start()
    {
        manager = GetComponent<EnemyManager>();
        basicAttackRange = Mathf.Min(skillRange01, skillRange02);
        attackColliderScript01 = attackCollider01.GetComponent<EnemyAttackCollider>();
        attackColliderScript02 = attackCollider02.GetComponent<EnemyAttackCollider>();
        attackCollider01.SetActive(false);
        attackCollider02.SetActive(false);
    }

    public void CombatHandler()
    {
        float playerDis = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

        if (currSkillCooldown01 <= 0 || currSkillCooldown02 <= 0)
            skillCooldownIsReady = true;
        else
            skillCooldownIsReady = false;

        if (playerDis <= skillRange01 || playerDis <= skillRange02)
            isInRange = true;
        else
            isInRange = false;

        if (currSkillCooldown01 <= 0 && playerDis <= skillRange01 ||
            currSkillCooldown02 <= 0 && playerDis <= skillRange02)
            canUseSkill = true;
        else
            canUseSkill = false;

        CooldownHandler();
    }

    private void CooldownHandler()
    {
        currSkillCooldown01 = Mathf.Clamp(currSkillCooldown01 -= Time.deltaTime, 0f, startSkillCooldown01);
        currSkillCooldown02 = Mathf.Clamp(currSkillCooldown02 -= Time.deltaTime, 0f, startSkillCooldown02);
    }

    public void AttackHandler(Animator anim, float playerDistance)
    {
        if (currSkillCooldown01 <= 0 && playerDistance <= skillRange01 && !anim.GetBool(StringData.IsInteracting))
        {
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack01);
            currSkillCooldown01 = startSkillCooldown01;
        }
        if (currSkillCooldown02 <= 0 && playerDistance <= skillRange02 && !anim.GetBool(StringData.IsInteracting))
        {
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack02);
            currSkillCooldown02 = startSkillCooldown02;
        }
    }

    #region Attack Damage & Effect Methods

    private void ApplyDamageAttack01()
    {
        float damage = manager.Stats.PhysicalPower.Value - PlayerManager.Instance.Stats.Armor.Value;

        PlayerManager.Instance.TakeDamage(damage, transform);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Physical);
        attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack01;
    }

    private void ApplyDamageAttack02()
    {
        float damage = manager.Stats.PhysicalPower.Value - PlayerManager.Instance.Stats.Armor.Value;

        PlayerManager.Instance.TakeDamage(damage, transform);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Physical);
        attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack02;
    }

    #endregion

    #region Attack Collider Methods

    public void AttackCollider01Enable()
    {
        attackCollider01.SetActive(true);
        attackColliderScript01.OnApplyDamageEvent += ApplyDamageAttack01;
    }

    public void AttackCollider01Disable()
    {
        attackCollider01.SetActive(false);
        attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack01;
    }
    public void AttackCollider02Enable()
    {
        attackCollider01.SetActive(true);
        attackColliderScript01.OnApplyDamageEvent += ApplyDamageAttack02;
    }

    public void AttackCollider02Disable()
    {
        attackCollider01.SetActive(false);
        attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack02;
    }

    #endregion
}
