using UnityEngine;

public class BarbarianGiantCombat : MonoBehaviour, ICombat
{
    private EnemyManager manager;
    private Animator anim;
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
    [SerializeField] float startAttacking02Cooldown = 5f;
    private float curAttacking02Cooldown = 5f;
    private bool isAttacking02 = false;
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
    [SerializeField] GameObject skillPrefab03;



    private void Start()
    {
        manager = GetComponent<EnemyManager>();
        anim = GetComponent<Animator>();
        basicAttackRange = Mathf.Min(skillRange01, skillRange02);
        attackColliderScript01 = attackCollider01.GetComponent<EnemyAttackCollider>();
        attackColliderScript02 = attackCollider02.GetComponent<EnemyAttackCollider>();
        attackCollider01.SetActive(false);
        attackCollider02.SetActive(false);
    }

    public void CombatHandler()
    {
        float playerDis = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

        if (currSkillCooldown01 <= 0 || currSkillCooldown02 <= 0 || currSkillCooldown03 <= 0)
            skillCooldownIsReady = true;
        else
            skillCooldownIsReady = false;

        if (playerDis <= skillRange01 || playerDis <= skillRange02 || playerDis >= skillRange03)
            isInRange = true;
        else
            isInRange = false;

        if (currSkillCooldown01 <= 0 && playerDis <= skillRange01 ||
            currSkillCooldown02 <= 0 && playerDis <= skillRange02 ||
            currSkillCooldown03 <= 0 && playerDis <= skillRange03)
            canUseSkill = true;
        else
            canUseSkill = false;

        if (isAttacking02)
        {
            if (curAttacking02Cooldown <= 0)
            {
                curAttacking02Cooldown = startAttacking02Cooldown;
                anim.SetBool(StringData.IsAttacking02, false);
            }
            else curAttacking02Cooldown -= Time.deltaTime;
        }

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
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack01);
            currSkillCooldown01 = startSkillCooldown01;
        }
        if (currSkillCooldown02 <= 0 && playerDistance <= skillRange02 && !anim.GetBool(StringData.IsInteracting))
        {
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack02);
            curAttacking02Cooldown = startAttacking02Cooldown;
            anim.SetBool(StringData.IsAttacking02, true);
            isAttacking02 = true;

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

    private void ApplyDamageAttack03()
    {
        float physicalDamage = manager.Stats.PhysicalPower.Value - PlayerManager.Instance.Stats.Armor.Value;
        float magicalDamage = manager.Stats.MagicalPower.Value - PlayerManager.Instance.Stats.MagicResistance.Value;
        physicalDamage = Mathf.Clamp(physicalDamage, 0f, physicalDamage);
        magicalDamage = Mathf.Clamp(magicalDamage, 0f, magicalDamage);
        float damage = physicalDamage + magicalDamage;

        PlayerManager.Instance.TakeDamage(damage, transform);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(physicalDamage, DamageType.Physical);
        go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(magicalDamage, DamageType.Magical);
    }

    #endregion

    #region Instantiate Skills 

    public void InstantiateAttack03()
    {
        GameObject go = Instantiate(skillPrefab03, new Vector3(PlayerManager.Instance.transform.position.x, PlayerManager.Instance.transform.position.y + 10f, PlayerManager.Instance.transform.position.z), Quaternion.identity);
        var spell = go.GetComponent<AreaOfEffectSpell>();
        spell.OnHitPlayerEvent += ApplyDamageAttack03;
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
