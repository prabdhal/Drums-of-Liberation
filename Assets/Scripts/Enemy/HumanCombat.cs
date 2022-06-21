using UnityEngine;

public class HumanCombat : MonoBehaviour, ICombat
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
    private string skillName01 = "Acid";
    [SerializeField]
    [TextArea(2, 3)]
    private string skillDescription01 = "A vicious left claw swipe, capable of causing the opponent to take bleed damage over time";
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
    private string skillName02 = "Acid Shot";
    [SerializeField]
    [TextArea(2, 3)]
    private string skillDescription02 = "A vicious right claw swipe that deals extra damage, capable of causing the opponent to take bleed damage over time";
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

        PlayerManager.Instance.Stats.CurrentHealth -= damage;
        Debug.Log("Player took " + damage + " from skill one");

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Physical);
        attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack01;
    }

    private void ApplyDamageAttack02()
    {
        float damage = manager.Stats.PhysicalPower.Value - PlayerManager.Instance.Stats.Armor.Value;

        PlayerManager.Instance.Stats.CurrentHealth -= damage;
        Debug.Log("Player took " + damage + " from skill two");

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

    #region Instantiate Attack Methods

    //private void InstantiateAttack01()
    //{
    //  GameObject skill = Instantiate(skill01Prefab, skill01Origin.position, skill01Origin.rotation);
    //  Projectile proj = skill.GetComponent<Projectile>();
    //  proj.OnApplyDamageEvent += ApplyDamageAttack01;
    //  proj.Init(skill01Speed, Utilities.GlobalMethods.GetDirection(skill01Direction, transform));
    //}

    //private void InstantiateAttack02()
    //{
    //  GameObject skill = Instantiate(skill02Prefab, skill02Origin.position, skill02Origin.rotation);
    //  Projectile proj = skill.GetComponent<Projectile>();
    //  proj.OnApplyDamageEvent += ApplyDamageAttack02;
    //  proj.Init(skill02Speed, Utilities.GlobalMethods.GetDirection(skill02Direction, transform));
    //}

    #endregion
}
