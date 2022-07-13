using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //components 
    private Animator anim;
    private PlayerManager manager;
    private PlayerControls controls;
    private PlayerAttackCollider weaponCol;

    // combat
    [SerializeField] GameObject weaponColGO;

    //stats
    [Range(1, 2)]
    [SerializeField] float heavyAttackRatio = 1.5f;
    [SerializeField] float heavyAttackCost = 10f;

    // variables
    [SerializeField] float startComboTimer = 1f;
    private float curComboTimer;

    [SerializeField] GameObject magicAttack01Prefab;
    [SerializeField] Transform magicAttack01Origin;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack01Damage;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack01Speed = 500f;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack01Range = 200f;
    [SerializeField] float spellCost01;

    [SerializeField] GameObject magicAttack02Prefab;
    [SerializeField] Transform magicAttack02Origin;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack02Damage;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack02Speed = 500f;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack02Range = 200f;
    [SerializeField] float spellCost02;
    [SerializeField] float startShadowAuraTimer = 10f;
    private float curShadowAuraTimer = 10f;

    [SerializeField] GameObject magicAttack03Prefab;
    [SerializeField] Transform magicAttack03Origin;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack03Damage;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack03Speed = 500f;
    [Range(0, 1000)]
    [SerializeField] float baseMagicAttack03Range = 200f;
    [SerializeField] float spellCost03;


    private void Start()
    {
        anim = GetComponent<Animator>();
        manager = PlayerManager.Instance;
        controls = PlayerControls.Instance;
        weaponCol = GameObject.FindGameObjectWithTag(StringData.PlayerWeaponTag).GetComponentInChildren<PlayerAttackCollider>();

        controls.OnLightAttackEvent += OnLightAttack;
        controls.OnHeavyAttackEvent += OnHeavyAttack;
        controls.OnMagicAttackEvent += OnMagicAttack;

        weaponColGO.SetActive(false);
        curComboTimer = startComboTimer;
        curShadowAuraTimer = startShadowAuraTimer;
    }

    public void CombatUpdate()
    {
        CombatTimerHandler();
        ShadowAuraHandler();
    }

    private void CombatTimerHandler()
    {
        if (curComboTimer <= 0 && manager.CombatIdx > 0)
        {
            manager.CombatIdx = 0;
            curComboTimer = startComboTimer;
        }
        else
            curComboTimer -= Time.deltaTime;
    }

    private void OnLightAttack()
    {
        if (PlayerManager.Instance.StatusEffectManager.IsStunned) return;

        PlayerManager.Instance.IsLightAttacking = true;

        switch (manager.CombatIdx)
        {
            case 0:
                PlayerManager.Instance.ComboIdx = 1;
                anim.Play(StringData.LightAttack01);
                break;
            case 1:
                PlayerManager.Instance.ComboIdx = 2;
                anim.Play(StringData.LightAttack02);
                break;
            case 2:
                PlayerManager.Instance.ComboIdx = 3;
                anim.Play(StringData.LightAttack03);
                break;
        }

        weaponCol.OnAttackEvent += ApplyLightAttackDamage;
        ComboIndexHandler();
    }

    public void ApplyLightAttackDamage(EnemyManager enemy)
    {
        float physicalDamage = manager.Stats.PhysicalPower.Value - enemy.Stats.Armor.Value;
        physicalDamage = Mathf.Clamp(physicalDamage, 0, physicalDamage);

        enemy.Stats.CurrentHealth -= physicalDamage;

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, enemy.popupPos);
        go.GetComponent<DamagePopup>().Init(physicalDamage, DamageType.Physical);
    }

    private void OnHeavyAttack()
    {
        if (PlayerManager.Instance.StatusEffectManager.IsStunned) return;
        if (!PlayerManager.Instance.HasEnoughStamina(heavyAttackCost)) return;

        PlayerManager.Instance.IsHeavyAttacking = true;

        switch (manager.CombatIdx)
        {
            case 0:
                PlayerManager.Instance.ComboIdx = 1;
                anim.Play(StringData.HeavyAttack01);
                break;
            case 1:
                PlayerManager.Instance.ComboIdx = 2;
                anim.Play(StringData.HeavyAttack02);
                break;
            case 2:
                PlayerManager.Instance.ComboIdx = 3;
                anim.Play(StringData.HeavyAttack03);
                break;
        }

        weaponCol.OnAttackEvent += ApplyHeavyAttackDamage;
        ComboIndexHandler();
    }

    public void ApplyHeavyAttackDamage(EnemyManager enemy)
    {
        float physicalDamage = manager.Stats.PhysicalPower.Value * heavyAttackRatio - enemy.Stats.Armor.Value;
        physicalDamage = Mathf.Clamp(physicalDamage, 0, physicalDamage);

        enemy.Stats.CurrentHealth -= physicalDamage;

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, enemy.popupPos);
        go.GetComponent<DamagePopup>().Init(physicalDamage, DamageType.Physical);
    }

    private void OnMagicAttack()
    {
        if (PlayerManager.Instance.StatusEffectManager.IsStunned) return;

        PlayerManager.Instance.IsMagicAttacking = true;

        switch (manager.MagicIdx)
        {
            case 0:
                MagicSpell01();
                break;
            case 1:
                MagicSpell02();
                break;
            case 2:
                MagicSpell03();
                break;
        }
    }

    private void MagicSpell01()
    {
        if (!PlayerManager.Instance.CanUseSpell(spellCost01)) return;

        anim.Play(StringData.MagicAttack01);
    }

    private void MagicSpell02()
    {
        if (!PlayerManager.Instance.CanUseSpell(spellCost02)) return;

        anim.Play(StringData.MagicAttack02);
    }

    private void MagicSpell03()
    {
        if (!PlayerManager.Instance.CanUseSpell(spellCost03)) return;

        anim.Play(StringData.MagicAttack03);
    }

    public void InstantiateMagicAttack01()
    {
        var go = Instantiate(magicAttack01Prefab, magicAttack01Origin.position, magicAttack01Origin.transform.rotation);
        var proj = go.GetComponent<Projectile>();
        proj.Init(baseMagicAttack01Speed, baseMagicAttack01Range);
        proj.OnHitEvent += ApplyMagicDamage01;
    }

    private void ApplyMagicDamage01(GameObject target)
    {
        EnemyManager enemy;
        if (GameManager.Instance.Enemies.TryGetValue(target.name, out EnemyManager e))
            enemy = e;
        else
            enemy = target.GetComponent<EnemyManager>();

        float magicalDamage = manager.Stats.MagicalPower.Value + baseMagicAttack01Damage - enemy.Stats.MagicResistance.Value;
        magicalDamage = Mathf.Clamp(magicalDamage, 0, magicalDamage);

        enemy.Stats.CurrentHealth -= magicalDamage;

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, enemy.popupPos);
        go.GetComponent<DamagePopup>().Init(magicalDamage, DamageType.Magical);
    }


    public void InstantiateMagicAttack02()
    {
        var go = Instantiate(magicAttack02Prefab, magicAttack02Origin.transform, false);
        ActivateShadowAura();
    }

    private void ShadowAuraHandler()
    {
        if (weaponCol.shadowAura.activeInHierarchy)
        {
            if (curShadowAuraTimer <= 0)
            {
                curShadowAuraTimer = startShadowAuraTimer;
                weaponCol.shadowAura.SetActive(false);
            }
            else
                curShadowAuraTimer -= Time.deltaTime;
        }
    }

    private void ActivateShadowAura()
    {
        weaponCol.shadowAura.SetActive(true);
        curShadowAuraTimer = startShadowAuraTimer;
        TempStatModifier physicalMod = new TempStatModifier(10f, startShadowAuraTimer, StatType.Physical, StatModType.Flat); 
        TempStatModifier magicalMod = new TempStatModifier(10f, startShadowAuraTimer, StatType.Magical, StatModType.Flat);

        PlayerManager.Instance.StatusEffectManager.ApplyStatBuffs(physicalMod);
        PlayerManager.Instance.StatusEffectManager.ApplyStatBuffs(magicalMod);
    }

    private void ApplyMagicDamage02(GameObject target)
    {
        EnemyManager enemy;
        if (GameManager.Instance.Enemies.TryGetValue(target.name, out EnemyManager e))
            enemy = e;
        else
            enemy = target.GetComponent<EnemyManager>();

        float magicalDamage = manager.Stats.MagicalPower.Value + baseMagicAttack02Damage - enemy.Stats.MagicResistance.Value;
        magicalDamage = Mathf.Clamp(magicalDamage, 0, magicalDamage);

        enemy.Stats.CurrentHealth -= magicalDamage;

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, enemy.popupPos);
        go.GetComponent<DamagePopup>().Init(magicalDamage, DamageType.Magical);
    }


    public void InstantiateMagicAttack03()
    {
        var go = Instantiate(magicAttack01Prefab, magicAttack01Origin.position, magicAttack01Origin.transform.rotation);
        var proj = go.GetComponent<Projectile>();
        proj.OnHitEvent += ApplyMagicDamage03;
    }

    private void ApplyMagicDamage03(GameObject target)
    {
        EnemyManager enemy;
        if (GameManager.Instance.Enemies.TryGetValue(target.name, out EnemyManager e))
            enemy = e;
        else
            enemy = target.GetComponent<EnemyManager>();

        float magicalDamage = manager.Stats.MagicalPower.Value + baseMagicAttack03Damage - enemy.Stats.MagicResistance.Value;
        magicalDamage = Mathf.Clamp(magicalDamage, 0, magicalDamage);

        enemy.Stats.CurrentHealth -= magicalDamage;

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, enemy.popupPos);
        go.GetComponent<DamagePopup>().Init(magicalDamage, DamageType.Magical);
    }


    private void ComboIndexHandler()
    {
        manager.CombatIdx++;
        curComboTimer = startComboTimer;

        if (manager.CombatIdx > 2)
            manager.CombatIdx = 0;
    }

    private void EnableCollider()
    {
        weaponColGO.SetActive(true);
    }

    private void DisableCollider()
    {
        weaponColGO.SetActive(false);
        weaponCol.OnAttackEvent -= ApplyHeavyAttackDamage;
        weaponCol.OnAttackEvent -= ApplyLightAttackDamage;
        PlayerManager.Instance.IsHeavyAttacking = false;
        PlayerManager.Instance.IsLightAttacking = false;
    }
}
