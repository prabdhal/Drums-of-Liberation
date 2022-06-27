using UnityEngine;

public class SpiderlingCombat : MonoBehaviour, ICombat
{
    private EnemyManager manager;
    public float BasicAttackRange { get { return basicAttackRange; } }
    [SerializeField] float basicAttackRange = 7f;
    public bool SkillCooldownIsReady { get { return skillCooldownIsReady; } }
    private bool skillCooldownIsReady = true;
    public bool IsInRange { get { return isInRange; } }
    private bool isInRange = true;
    public bool CanUseSkill { get { return canUseSkill; } }
    private bool canUseSkill = true;

    [Space]

    [Header("Skill 01")]
    [SerializeField] string skillName01;
    [TextArea(2, 3)]
    [SerializeField] string skillDescription01;
    [SerializeField] float skillRange01 = 3f;
    [SerializeField] float startSkillCooldown01 = 5f;
    private float currSkillCooldown01 = 0;
    [Tooltip("The attack collider gameobject of attack 01.")]
    [SerializeField] GameObject clawAttackCollider;
    private EnemyAttackCollider attackColliderScript01;

    [Space]

    [Header("Skill 02")]
    [SerializeField] string skillName02;
    [TextArea(2, 3)]
    [SerializeField] private string skillDescription02;
    [SerializeField] float skillRange02 = 3f;
    [SerializeField] float skillSpeed02 = 500f;
    [SerializeField] float startSkillCooldown02 = 10f;
    [SerializeField] float poisonDamage = 2f;
    [SerializeField] float poisonDuration = 5f;
    [SerializeField] DamageOverTimeEffectType effectType;
    private float currSkillCooldown02 = 0;
    [Tooltip("The attack collider gameobject of attack 02.")]
    [SerializeField] GameObject spellPrefab02;
    [SerializeField] Transform spellOrigin02;

    [Space]

    [Header("Skill 03")]
    [SerializeField] string skillName03;
    [TextArea(2, 3)]
    [SerializeField] string skillDescription03;
    [SerializeField] float skillRange03 = 3f;
    [SerializeField] float startSkillCooldown03 = 10f;
    private float currSkillCooldown03 = 0;
    [SerializeField] float jumpForce = 1f;
    [SerializeField] float jumpHeight = 1f;
    [SerializeField] float jumpTimer = 2f;
    private float curJumpTimer = 0;
    private bool isJumping = false;
    [SerializeField] LayerMask groundLayer;
    [Tooltip("The attack collider gameobject of attack 03.")]



    private void Start()
    {
        manager = GetComponent<EnemyManager>();
        basicAttackRange = skillRange01;

        currSkillCooldown01 = 0f;
        currSkillCooldown02 = 0f;
        currSkillCooldown03 = 0f;

        clawAttackCollider.SetActive(false);
        attackColliderScript01 = clawAttackCollider.GetComponent<EnemyAttackCollider>();
    }

    public void CombatHandler()
    {
        CooldownHandler();
        JumpHandler();
        
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
            currSkillCooldown03 <= 0 && playerDis >= skillRange03)
            canUseSkill = true;
        else
            canUseSkill = false;
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
            attackColliderScript01.OnApplyDamageEvent += ApplyDamageAttack01;
            currSkillCooldown01 = startSkillCooldown01;
        }
        if (currSkillCooldown02 <= 0 && playerDistance <= skillRange02 && !anim.GetBool(StringData.IsInteracting))
        {
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack02);
            currSkillCooldown02 = startSkillCooldown02;
        }
        if (currSkillCooldown03 <= 0 && playerDistance >= skillRange03 && !anim.GetBool(StringData.IsInteracting))
        {
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack03);
            currSkillCooldown03 = startSkillCooldown03;
            InstantiateSpell03();
        }
    }

    private void JumpHandler()
    {
        manager.anim.SetBool(StringData.IsJumping, isJumping);
        if (isJumping)
        {
            if (curJumpTimer <= 0)
            {
                RaycastHit hit;
                Vector3 origin = transform.position;

                Debug.DrawRay(origin, -transform.up, Color.red);
                if (Physics.Raycast(origin, -transform.up, out hit, 0.1f, groundLayer))
                {
                    isJumping = false;
                    manager.agent.enabled = true;
                    manager.rb.isKinematic = true;
                }
            }
            else if (curJumpTimer > jumpTimer / 2f)
            {
                manager.rb.AddForce(new Vector3(transform.forward.x, jumpHeight, transform.forward.z) * jumpForce * Time.deltaTime, ForceMode.Impulse);
            }
            else if (curJumpTimer > 0)
            {
                manager.rb.AddForce(new Vector3(transform.forward.x, -jumpHeight, transform.forward.z) * jumpForce * 3f * Time.deltaTime, ForceMode.Impulse);
            }
            curJumpTimer -= Time.deltaTime;
            return;
        }
    }

    #region Instantiate Methods

    public void InstantiateSpell02()
    {
        GameObject go = Instantiate(spellPrefab02, spellOrigin02.position, spellOrigin02.rotation);
        var proj = go.GetComponent<EnemyProjectile>();
        proj.Init(skillSpeed02, skillRange02);
        proj.OnHitPlayerEvent += ApplyDamageAttack02;
    }


    public void InstantiateSpell03()
    {
        manager.agent.enabled = false;
        curJumpTimer = jumpTimer;
        isJumping = true;
        manager.rb.isKinematic = false;
    }

    #endregion

    #region Attack Damage & Effect Methods

    private void ApplyDamageAttack01()
    {
        float damage = manager.Stats.MagicalPower.Value - PlayerManager.Instance.Stats.MagicResistance.Value;

        PlayerManager.Instance.Stats.CurrentHealth -= damage;

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Physical);
        attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack01;
    }

    private void ApplyDamageAttack02()
    {
        float damage = manager.Stats.MagicalPower.Value - PlayerManager.Instance.Stats.MagicResistance.Value;

        PlayerManager.Instance.Stats.CurrentHealth -= damage;

        // apply poison
        DamageOverTimeEffect effect = new DamageOverTimeEffect(poisonDamage, poisonDuration, 0.5f, effectType);
        PlayerManager.Instance.StatusEffectManager.ApplyStatusEffects(effect);
        Debug.Log("Player took " + damage + " from " + skillName02);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Magical);
    }

    #endregion

    #region Colliders

    public void EnableClawAttackCollider()
    {
        clawAttackCollider.SetActive(true);
    }

    public void DisableClawAttackCollider()
    {
        clawAttackCollider.SetActive(false);
        attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack01;
    }

    #endregion
}
