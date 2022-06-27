using System.Collections.Generic;
using UnityEngine;

public class ForestWitchCombat : MonoBehaviour, ICombat
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

    [Header("Skill 01")]
    [SerializeField] string skillName01;
    [TextArea(2, 3)]
    [SerializeField] string skillDescription01;
    [SerializeField] float skillRange01 = 3f;
    [SerializeField] float skillSpeed01 = 500f;
    [SerializeField] float startSkillCooldown01 = 5f;
    private float currSkillCooldown01 = 0;
    [SerializeField] TempStatDebuffType skillDebuffType;
    [SerializeField] float skillStunDuration01;
    [SerializeField] GameObject spellPrefab01;
    [SerializeField] Transform spellOrigin01;

    [Space]

    [Header("Skill 02")]
    [SerializeField] string skillName02 = "";
    [TextArea(2, 3)]
    [SerializeField] string skillDescription02 = "";
    [SerializeField] float skillRange02 = 3f;
    [SerializeField] float startSkillCooldown02 = 10f;
    private float currSkillCooldown02 = 10f;
    [SerializeField] GameObject[] spellPrefabs02;
    [SerializeField] GameObject spellPrefab02;
    private GameObject creatureSummon;
    [SerializeField] Transform[] spawnPoints;
    private Transform spellOrigin02;
    [SerializeField] int maxSpiderlings = 5;
    private List<EnemyManager> spiderlings = new List<EnemyManager>();
    private int summonIdx = 1;
    public bool CanSummon { get { return spiderlings.Count < maxSpiderlings; }  }

    [Space]

    [Header("Skill 03")]
    [SerializeField] string skillName03 = "";
    [SerializeField]
    [TextArea(2, 3)]
    string skillDescription03 = "";
    [SerializeField] float skillRange03 = 3f;
    [SerializeField] float startSkillCooldown03 = 10f;
    private float currSkillCooldown03 = 10f;
    [SerializeField] GameObject teleportEffect;
    [SerializeField] Transform teleportEffectOrigin;
    public Transform[] TeleportLocations { get { return teleportLocations; } }
    [SerializeField] Transform[] teleportLocations;
    private Transform teleportLocation;


    private void Start()
    {
        manager = GetComponent<EnemyManager>();
        basicAttackRange = skillRange01;

        currSkillCooldown01 = 0f;
        currSkillCooldown02 = 0f;
        currSkillCooldown03 = 0f;
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
            currSkillCooldown02 <= 0 && playerDis <= skillRange02 && CanSummon ||
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
            anim.SetBool(StringData.IsInteracting, true);
            anim.Play(StringData.Attack01);
            currSkillCooldown01 = startSkillCooldown01;
        }
        if (currSkillCooldown02 <= 0 && playerDistance <= skillRange02 && !anim.GetBool(StringData.IsInteracting) && spiderlings.Count < maxSpiderlings)
        {
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

    #region Instantiate Methods

    public void InstantiateSpell01()
    {
        GameObject go = Instantiate(spellPrefab01, spellOrigin01.position, spellOrigin01.rotation);
        var proj = go.GetComponent<EnemyProjectile>();
        proj.Init(skillSpeed01, skillRange01);
        proj.OnHitPlayerEvent += ApplyDamageAttack01;

    }

    public void InstantiateSpell02()
    {
        if (spiderlings.Count >= maxSpiderlings) return;

        creatureSummon = spellPrefabs02[Random.Range(0, spellPrefabs02.Length)];
        spellOrigin02 = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject go = Instantiate(spellPrefab02, spellOrigin02.position, spellOrigin02.rotation);
        GameObject creature = Instantiate(creatureSummon, spellOrigin02.position, spellOrigin02.rotation);
        ISummon summon = creature.GetComponent<ISummon>();
        summon.Summoner = transform;

        EnemyManager spiderManager = creature.GetComponent<EnemyManager>();
        spiderManager.name = spiderManager.name + " " + summonIdx;
        spiderManager.OnDeathEvent += OnDeath_RemoveSpiderling;
        GameManager.Instance.AddEnemy(spiderManager);

        spiderlings.Add(spiderManager);
        summonIdx++;
    }


    public void InstantiateSpell03()
    {
        GameObject go = Instantiate(teleportEffect, transform.position, teleportEffectOrigin.rotation);
    }

    public void Teleport()
    {
        float furthestDistance = 0f;
        foreach (Transform loc in teleportLocations)
        {
            float distance = Vector3.Distance(PlayerManager.Instance.transform.position, loc.position);
            if (distance >= furthestDistance)
            {
                furthestDistance = distance;
                teleportLocation = loc;
            }
        }

        manager.agent.Warp(teleportLocation.position);
        GameObject go = Instantiate(teleportEffect, transform.position, teleportEffectOrigin.rotation);
    }

    #endregion

    #region Attack Damage & Effect Methods

    private void ApplyDamageAttack01()
    {
        float damage = manager.Stats.MagicalPower.Value - PlayerManager.Instance.Stats.MagicResistance.Value;

        PlayerManager.Instance.Stats.CurrentHealth -= damage;

        // apply stun
        TempStatDebuffEffect effect = new TempStatDebuffEffect(0, skillStunDuration01, 1f, skillDebuffType);
        PlayerManager.Instance.StatusEffectManager.ApplyDebuffEffects(effect);
        Debug.Log("Player took " + damage + " from " + skillName01);

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
        go.GetComponent<DamagePopup>().Init(damage, DamageType.Magical);
        //attackColliderScript01.OnApplyDamageEvent -= ApplyDamageAttack01;
    }

    #endregion

    private void OnDeath_RemoveSpiderling(EnemyManager enemy)
    {
        spiderlings.Remove(enemy);
        GameManager.Instance.RemoveEnemy(enemy);
    }
}
