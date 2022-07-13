using UnityEngine;

public class OrcWarchiefCombat : MonoBehaviour, ICombat
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
    [SerializeField] string skillName01 = "";
    [TextArea(2, 3)]
    [SerializeField] string skillDescription01 = "";
    [SerializeField] float skillRange01 = 3f;
    [SerializeField] float startSkillCooldown01 = 5f;
    private float currSkillCooldown01 = 0;
    [Tooltip("The attack collider gameobject of attack 01.")]
    [SerializeField] GameObject attackCollider01;
    private EnemyAttackCollider attackColliderScript01;

    [Space]

    [Header("Skill 02")]
    [SerializeField] string skillName02 = "";
    [TextArea(2, 3)]
    [SerializeField] string skillDescription02 = "";
    [SerializeField] float skillRange02 = 3f;
    [SerializeField] float startSkillCooldown02 = 10f;
    private float currSkillCooldown02 = 10f;
    [Tooltip("The attack collider gameobject of attack 02.")]
    [SerializeField] GameObject attackCollider02;
    private EnemyAttackCollider attackColliderScript02;

    [Space]

    [Header("Skill 03")]
    [SerializeField] string skillName03 = "";
    [TextArea(2, 3)]
    [SerializeField] string skillDescription03 = "";
    [SerializeField] float skillRange03 = 3f;
    [SerializeField] float startSkillCooldown03 = 10f;
    private float currSkillCooldown03 = 10f;
    [Tooltip("The attack collider gameobject of attack 03.")]
    [SerializeField] GameObject attackCollider03;
    private EnemyAttackCollider attackColliderScript03;


    private void Start()
    {
        manager = GetComponent<EnemyManager>();
        basicAttackRange = Mathf.Min(skillRange01, skillRange02, skillRange03);
        attackColliderScript01 = attackCollider01.GetComponent<EnemyAttackCollider>();
        attackColliderScript02 = attackCollider02.GetComponent<EnemyAttackCollider>();
        attackColliderScript03 = attackCollider03.GetComponent<EnemyAttackCollider>();
        attackCollider01.SetActive(false);
        attackCollider02.SetActive(false);
        attackCollider03.SetActive(false);
    }

    public void CombatHandler()
    {
        float playerDis = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

        if (currSkillCooldown01 <= 0 || currSkillCooldown02 <= 0 || currSkillCooldown03 <= 0)
            skillCooldownIsReady = true;
        else
            skillCooldownIsReady = false;

        if (playerDis <= skillRange01 || playerDis <= skillRange02 || playerDis <= skillRange03)
            isInRange = true;
        else
            isInRange = false;

        if (currSkillCooldown01 <= 0 && playerDis <= skillRange01 ||
            currSkillCooldown02 <= 0 && playerDis <= skillRange02 ||
            currSkillCooldown03 <= 0 && playerDis <= skillRange03)
            canUseSkill = true;
        else
            canUseSkill = false;

        CooldownHandler();
    }

    private void CooldownHandler()
    {
        currSkillCooldown01 = Mathf.Clamp(currSkillCooldown01 -= Time.deltaTime, 0f, startSkillCooldown01);
        currSkillCooldown02 = Mathf.Clamp(currSkillCooldown02 -= Time.deltaTime, 0f, startSkillCooldown02);
        currSkillCooldown03 = Mathf.Clamp(currSkillCooldown03 -= Time.deltaTime, 0f, startSkillCooldown03);
    }

    public void AttackHandler(Animator anim, float playerDistance)
    {
        if (currSkillCooldown01 <= 0 && playerDistance <= skillRange01 && !anim.GetBool(StringData.IsInteracting))
        {
            manager.CanInterrupt = true;
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack01);
            currSkillCooldown01 = startSkillCooldown01;
        }
        if (currSkillCooldown02 <= 0 && playerDistance <= skillRange02 && !anim.GetBool(StringData.IsInteracting))
        {
            manager.CanInterrupt = true;
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack02);
            currSkillCooldown02 = startSkillCooldown02;
        }
        if (currSkillCooldown03 <= 0 && playerDistance <= skillRange03 && !anim.GetBool(StringData.IsInteracting))
        {
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack03);
            currSkillCooldown03 = startSkillCooldown03;
        }
    }

    #region Attack Damage & Effect Methods

    private void ApplyDamageAttack01()
    {
        float damage = manager.Stats.PhysicalPower.Value - PlayerManager.Instance.Stats.Armor.Value;
        damage = Mathf.Clamp(damage, 0, damage);

        PlayerManager.Instance.TakeDamage(damage, transform);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Physical);
        attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack01;
    }

    private void ApplyDamageAttack02()
    {
        float damage = manager.Stats.PhysicalPower.Value - PlayerManager.Instance.Stats.Armor.Value;
        damage = Mathf.Clamp(damage, 0, damage);

        PlayerManager.Instance.TakeDamage(damage, transform);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Physical);
        attackColliderScript02.OnApplyDamageEvent -= ApplyDamageAttack02;
    }

    private void ApplyDamageAttack03()
    {
        float damage = manager.Stats.PhysicalPower.Value - PlayerManager.Instance.Stats.Armor.Value;
        damage = Mathf.Clamp(damage, 0, damage);

        PlayerManager.Instance.TakeDamage(damage, transform);
        PlayerManager.Instance.ApplyWarChiefKickBack(transform);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Physical);
        attackColliderScript03.OnApplyDamageEvent -= ApplyDamageAttack03;
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
        attackCollider02.SetActive(true);
        attackColliderScript02.OnApplyDamageEvent += ApplyDamageAttack02;
    }

    public void AttackCollider02Disable()
    {
        attackCollider02.SetActive(false);
        attackColliderScript02.OnApplyDamageEvent -= ApplyDamageAttack02;
    }

    public void AttackCollider03Enable()
    {
        attackCollider03.SetActive(true);
        attackColliderScript03.OnApplyDamageEvent += ApplyDamageAttack03;
    }

    public void AttackCollider03Disable()
    {
        attackCollider03.SetActive(false);
        attackColliderScript03.OnApplyDamageEvent -= ApplyDamageAttack03;
    }

    #endregion
}
