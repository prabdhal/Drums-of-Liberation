using UnityEngine;

public class HumanArcherCombat : MonoBehaviour, ICombat
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
    [SerializeField] GameObject attackPrefab01;
    [SerializeField] Transform attackOrigin01;
    [SerializeField] float skillSpeed01;

    [Space]

    [Header("Skill 02")]
    [SerializeField] string skillName02 = "Acid Shot";
    [TextArea(2, 3)]
    [SerializeField] string skillDescription02 = "A vicious right claw swipe that deals extra damage, capable of causing the opponent to take bleed damage over time";
    [SerializeField] float skillRange02 = 3f;
    [SerializeField] float startSkillCooldown02 = 10f;
    private float currSkillCooldown02 = 10f;
    [Tooltip("The attack collider gameobject of attack 02.")]
    [SerializeField] GameObject attackPrefab02;
    [SerializeField] Transform attackOrigin02;
    [SerializeField] float skillSpeed02;
    [SerializeField] float ignoreArmorAmount = 5f;

    [Header("Bleed Effect")]
    [SerializeField] float damageOverTime = 5f;
    [SerializeField] float damageDuration = 5f;
    [SerializeField] float damageInterval = 1f;
    [SerializeField] DamageOverTimeEffectType damageOverTimeType = DamageOverTimeEffectType.Bleed;




    private void Start()
    {
        manager = GetComponent<EnemyManager>();
        basicAttackRange = Mathf.Min(skillRange01, skillRange02);
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
            manager.CanInterrupt = true;
            anim.Play(StringData.Attack01);
            currSkillCooldown01 = startSkillCooldown01;
        }
        if (currSkillCooldown02 <= 0 && playerDistance <= skillRange02 && !anim.GetBool(StringData.IsInteracting))
        {
            anim.SetBool(StringData.IsInteracting, true);
            manager.CanInterrupt = true;
            anim.Play(StringData.Attack02);
            currSkillCooldown02 = startSkillCooldown02;
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
    }

    private void ApplyDamageAttack02()
    {
        float armor = PlayerManager.Instance.Stats.Armor.Value - ignoreArmorAmount;
        armor = Mathf.Clamp(armor, 0, armor);

        float damage = manager.Stats.PhysicalPower.Value - armor;
        damage = Mathf.Clamp(damage, 0, damage);

        PlayerManager.Instance.TakeDamage(damage, transform);

        DamageOverTimeEffect effect = new DamageOverTimeEffect(damageOverTime, damageDuration, damageInterval, damageOverTimeType);
        PlayerManager.Instance.StatusEffectManager.ApplyStatusEffects(effect);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Physical);
    }

    #endregion

    #region Instantiate Attack Methods

    public void InstantiateAttack01()
    {
        GameObject go = Instantiate(attackPrefab01, attackOrigin01.position, attackOrigin01.rotation);
        var proj = go.GetComponent<ArrowProjectile>();
        proj.Init(skillSpeed01, skillRange01);
        proj.OnHitPlayerEvent += ApplyDamageAttack01;
    }

    public void InstantiateAttack02()
    {
        GameObject go = Instantiate(attackPrefab02, attackOrigin02.position, attackOrigin02.rotation);
        var proj = go.GetComponent<ArrowProjectile>();
        proj.Init(skillSpeed02, skillRange01);
        proj.OnHitPlayerEvent += ApplyDamageAttack02;
    }

    #endregion
}
