using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerStats
{
    [Header("Player Level Scaling")]
    public int playerLevel = 0;
    public float playerXP = 0;
    public float requiredXP = 0f;

    public float[] playerRequiredXpPerLevel;
    public float[] playerMaxHealthIncreasePerLevel;
    public float[] playerMaxManaIncreasePerLevel;
    public float[] playerMaxStaminaIncreasePerLevel;
    public float[] playerPhysicalPowerIncreasePerLevel;
    public float[] playerMagicalPowerIncreasePerLevel;
    public float[] playerArmorIncreasePerLevel;
    public float[] playerMagicResistIncreasePerLevel;
    public float[] playerHealthRegenIncreasePerLevel;
    public float[] playerManaRegenIncreasePerLevel;
    public float[] playerStaminaRegenIncreasePerLevel;


    public CharacterStat MaxHealth { get; set; }
    //[SerializeField] float maxHealthVal;
    public float CurrentHealth { get; set; }
    public CharacterStat MaxStamina { get; set; }
    //[SerializeField] float maxStaminaVal;
    public float CurrentStamina { get; set; }
    public CharacterStat MaxMana { get; set; }
    //[SerializeField] float maxManaVal;
    public float CurrentMana { get; set; }
    public CharacterStat PhysicalPower { get; set; }
    //[SerializeField] float physicalPowerVal;
    public CharacterStat MagicalPower { get; set; }
    //[SerializeField] float magicalPowerVal;
    public CharacterStat Armor { get; set; }
    //[SerializeField] float armorVal;
    public CharacterStat MagicResistance { get; set; }
    //[SerializeField] float magicResistanceVal;
    public CharacterStat AttackSpeed { get; set; }
    //[SerializeField] float attackSpeed;
    public CharacterStat MovementSpeed { get; set; }
    //[SerializeField] float movementSpeedVal;

    [Header("Health Regen Rate")]
    [SerializeField] float regenHpAmountPerTimeUnit = 1f;
    [SerializeField] float regenHpStartTimer = 5f;
    private float regenHpCurTimer = 0f;

    [Header("Mana Regen Rate")]
    [SerializeField] float regenManaAmountPerTimeUnit = 1f;
    [SerializeField] float regenManaStartTimer = 5f;
    private float regenManaCurTimer = 0f;

    [Header("Stamina Regen Rate")]
    [SerializeField] float regenStaminaAmountPerTimeUnit = 1f;
    [SerializeField] float regenStaminaStartTimer = 5f;
    private float regenStaminaCurTimer = 0f;

    [Header("Player UIs")]
    [SerializeField] Image healthBar;
    [SerializeField] Image manaBar;
    [SerializeField] Image staminaBar;
    [SerializeField] Image experienceBar;
    [SerializeField] TextMeshProUGUI playerLevelText;

    public delegate void OnLevelUp();
    public event OnLevelUp OnLevelUpEvent;

    public void Init()
    {
        if (healthBar == null)
            healthBar = GameObject.FindGameObjectWithTag(StringData.HealthBar).GetComponent<Image>();
        if (manaBar == null)
            manaBar = GameObject.FindGameObjectWithTag(StringData.ManaBar).GetComponent<Image>();
        if (staminaBar == null)
            staminaBar = GameObject.FindGameObjectWithTag(StringData.StaminaBar).GetComponent<Image>();
        if (experienceBar == null)
            experienceBar = GameObject.FindGameObjectWithTag(StringData.ExperienceBar).GetComponent<Image>();
        if (playerLevelText == null)
            playerLevelText = GameObject.FindGameObjectWithTag(StringData.PlayerLevelText).GetComponent<TextMeshProUGUI>();
        
        InitPlayerStats();
        UpdateUI(true, true, true, true);
    }

    public void Update()
    {
        MaxHealth.Update();
        MaxStamina.Update();
        MaxMana.Update();
        PhysicalPower.Update();
        MagicalPower.Update();
        Armor.Update();
        MagicResistance.Update();
        MovementSpeed.Update();
        AttackSpeed.Update();
    }

    private void InitPlayerStats()
    {
        playerLevel = PlayerDataManager.PlayerLevel;
        requiredXP = playerRequiredXpPerLevel[playerLevel];

        MaxHealth = new CharacterStat(playerMaxHealthIncreasePerLevel[playerLevel]);
        CurrentHealth = PlayerDataManager.CurrentHealth;
        MaxMana = new CharacterStat(playerMaxManaIncreasePerLevel[playerLevel]);
        CurrentMana = PlayerDataManager.CurrentMana;
        MaxStamina = new CharacterStat(playerMaxStaminaIncreasePerLevel[playerLevel]);
        CurrentStamina = PlayerDataManager.CurrentStamina;
        PhysicalPower = new CharacterStat(playerPhysicalPowerIncreasePerLevel[playerLevel]);
        MagicalPower = new CharacterStat(playerMagicalPowerIncreasePerLevel[playerLevel]);
        Armor = new CharacterStat(playerArmorIncreasePerLevel[playerLevel]);
        MagicResistance = new CharacterStat(playerMagicResistIncreasePerLevel[playerLevel]);
        MovementSpeed = new CharacterStat(3);
        AttackSpeed = new CharacterStat(0);
    }

    public void AddPlayerExperience(float xp)
    {
        playerXP += xp;

        if (playerXP >= requiredXP)
        {
            float surplus = playerXP - requiredXP;
            playerLevel++;
            playerXP = surplus;
            UpdateStatsUponLevelingUp();
            OnLevelUpEvent?.Invoke();
        }
    }

    public void UpdateStatsUponLevelingUp()
    {
        if (playerLevel > 0)
            UpdateCurrentValuesUponLevelUp();
        else
            UpdateCurrentValuesUponLevelUp(0);


        UpdateUI(true, true, true, true);
    }

    private void UpdateCurrentValuesUponLevelUp(int diff = 1)
    {
        CurrentHealth += playerMaxHealthIncreasePerLevel[playerLevel] - playerMaxHealthIncreasePerLevel[playerLevel - diff];
        CurrentMana += playerMaxManaIncreasePerLevel[playerLevel] - playerMaxManaIncreasePerLevel[playerLevel - diff];
        CurrentStamina += playerMaxStaminaIncreasePerLevel[playerLevel] - playerMaxStaminaIncreasePerLevel[playerLevel - diff];

        requiredXP = playerRequiredXpPerLevel[playerLevel];

        MaxHealth = new CharacterStat(playerMaxHealthIncreasePerLevel[playerLevel]);
        CurrentHealth = PlayerDataManager.CurrentHealth;
        MaxMana = new CharacterStat(playerMaxManaIncreasePerLevel[playerLevel]);
        CurrentMana = PlayerDataManager.CurrentMana;
        MaxStamina = new CharacterStat(playerMaxStaminaIncreasePerLevel[playerLevel]);
        CurrentStamina = PlayerDataManager.CurrentStamina;
        PhysicalPower = new CharacterStat(playerPhysicalPowerIncreasePerLevel[playerLevel]);
        MagicalPower = new CharacterStat(playerMagicalPowerIncreasePerLevel[playerLevel]);
        Armor = new CharacterStat(playerArmorIncreasePerLevel[playerLevel]);
        MagicResistance = new CharacterStat(playerMagicResistIncreasePerLevel[playerLevel]);
        MovementSpeed = new CharacterStat(3);
        AttackSpeed = new CharacterStat(0);
    }

    #region Stat Regen Rate
    public void PassiveRegen()
    {
        RegenHp();
        RegenMana();
        RegenStamina();
    }

    private void RegenHp()
    {
        if (regenHpCurTimer <= 0 && CurrentHealth < MaxHealth.Value)
        {
            regenHpCurTimer = regenHpStartTimer;
            CurrentHealth += regenHpAmountPerTimeUnit;
            UpdateHealthUI();
        }
        else
            regenHpCurTimer -= Time.deltaTime;
    }

    private void RegenMana()
    {
        if (regenManaCurTimer <= 0 && CurrentMana < MaxMana.Value)
        {
            regenManaCurTimer = regenManaStartTimer;
            CurrentMana += regenManaAmountPerTimeUnit;
            UpdateManaUI();
        }
        else
            regenManaCurTimer -= Time.deltaTime;
    }

    private void RegenStamina()
    {
        if (regenStaminaCurTimer <= 0 && CurrentStamina < MaxStamina.Value)
        {
            regenStaminaCurTimer = regenStaminaStartTimer;
            CurrentStamina += regenStaminaAmountPerTimeUnit;
            UpdateStaminaUI();
        }
        else
            regenStaminaCurTimer -= Time.deltaTime;
    }
    #endregion

    #region UI

    public void UpdateUI(bool hp, bool mana, bool stamina, bool xp)
    {
        if (hp)
            UpdateHealthUI();
        if (mana)
            UpdateManaUI();
        if (stamina)
            UpdateStaminaUI();
        if (xp)
            UpdateExperienceUI();

        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth.BaseValue);
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, MaxStamina.BaseValue);
        CurrentMana = Mathf.Clamp(CurrentMana, 0f, MaxMana.BaseValue);
    }

    private void UpdateHealthUI()
    {
        healthBar.fillAmount = CurrentHealth / MaxHealth.BaseValue;
    }

    private void UpdateManaUI()
    {
        manaBar.fillAmount = CurrentMana / MaxMana.BaseValue;
    }

    private void UpdateStaminaUI()
    {
        staminaBar.fillAmount = CurrentStamina / MaxStamina.BaseValue;
    }

    private void UpdateExperienceUI()
    {
        experienceBar.fillAmount = playerXP / requiredXP;
        playerLevelText.text = playerLevel.ToString();
    }

    #endregion
}