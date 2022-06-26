using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //components 
    private Animator anim;
    private PlayerManager manager;
    private PlayerControls controls;
    private PlayerAttackCollider weaponCol;

    // combat
    [SerializeField]
    private GameObject weaponColGO;

    //stats
    [SerializeField]
    [Range(1, 2)]
    private float heavyAttackRatio = 1.5f;

    // variables
    [SerializeField]
    private float startComboTimer = 1f;
    private float curComboTimer;

    [SerializeField]
    private GameObject magicAttack01Prefab;
    [SerializeField]
    private Transform magicAttack01Origin;
    [SerializeField]
    [Range(0, 1000)]
    private float baseMagicAttack01Damage;
    [SerializeField]
    [Range(0, 1000)]
    private float baseMagicAttack01Speed = 500f;
    [SerializeField]
    [Range(0, 1000)]
    private float baseMagicAttack01Range = 200f;

    [SerializeField]
    private GameObject magicAttack02Prefab;
    [SerializeField]
    private Transform magicAttack02Origin;
    [SerializeField]
    [Range(0, 1000)]
    private float baseMagicAttack02Damage;
    [Range(0, 1000)]
    private float baseMagicAttack02Speed = 500f;
    [SerializeField]
    [Range(0, 1000)]
    private float baseMagicAttack02Range = 200f;

    [SerializeField]
    private GameObject magicAttack03Prefab;
    [SerializeField]
    private Transform magicAttack03Origin;
    [SerializeField]
    [Range(0, 1000)]
    private float baseMagicAttack03Damage;
    [Range(0, 1000)]
    private float baseMagicAttack03Speed = 500f;
    [SerializeField]
    [Range(0, 1000)]
    private float baseMagicAttack03Range = 200f;


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
    }

    public void CombatUpdate()
    {
        CombatTimerHandler();
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

        switch (manager.CombatIdx)
        {
            case 0:
                anim.Play(StringData.LightAttack01);
                break;
            case 1:
                anim.Play(StringData.LightAttack02);
                break;
            case 2:
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
        Debug.Log(enemy.name + " was attacked and took " + physicalDamage + " points of light physical damage");

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, enemy.popupPos);
        go.GetComponent<DamagePopup>().Init(physicalDamage, DamageType.Physical);
    }

    private void OnHeavyAttack()
    {
        if (PlayerManager.Instance.StatusEffectManager.IsStunned) return;

        switch (manager.CombatIdx)
        {
            case 0:
                anim.Play(StringData.HeavyAttack01);
                break;
            case 1:
                anim.Play(StringData.HeavyAttack02);
                break;
            case 2:
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
        Debug.Log(enemy.name + " was attacked and took " + physicalDamage + " points of heavy physical damage");

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, enemy.popupPos);
        go.GetComponent<DamagePopup>().Init(physicalDamage, DamageType.Physical);
    }

    private void OnMagicAttack()
    {
        if (PlayerManager.Instance.StatusEffectManager.IsStunned) return;

        switch (manager.MagicIdx)
        {
            case 0:
                anim.Play(StringData.MagicAttack01);
                break;
            case 1:
                anim.Play(StringData.MagicAttack02);
                break;
            case 2:
                anim.Play(StringData.MagicAttack03);
                break;
        }
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
        //Debug.Log(enemy.name + " was attacked and took " + magicalDamage + " points of magical damage");

        // add damage popup 
        GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, enemy.popupPos);
        go.GetComponent<DamagePopup>().Init(magicalDamage, DamageType.Magical);
    }


    public void InstantiateMagicAttack02()
    {
        var go = Instantiate(magicAttack01Prefab, magicAttack01Origin.position, magicAttack01Origin.transform.rotation);
        var proj = go.GetComponent<Projectile>();
        proj.OnHitEvent += ApplyMagicDamage02;
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
        Debug.Log(enemy.name + " was attacked and took " + magicalDamage + " points of magical damage");

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
        Debug.Log(enemy.name + " was attacked and took " + magicalDamage + " points of magical damage");

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
    }
}
