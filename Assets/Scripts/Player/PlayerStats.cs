using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats
{
  public int playerLevel = 1;
  public float playerXP = 0;
  private float xpScaleFactor = 1.5f;
  private float baseXP = 1000f;
  public CharacterStat MaxHealth { get; set; }
  [SerializeField]
  private float maxHealthVal;
  public float CurrentHealth { get; set; }
  public CharacterStat MaxStamina { get; set; }
  [SerializeField]
  private float maxStaminaVal;
  public float CurrentStamina { get; set; }
  public CharacterStat MaxMana { get; set; }
  [SerializeField]
  private float maxManaVal;
  public float CurrentMana { get; set; }
  public CharacterStat PhysicalPower { get; set; }
  [SerializeField]
  private float physicalPowerVal;
  public CharacterStat MagicalPower { get; set; }
  [SerializeField]
  private float magicalPowerVal;
  public CharacterStat Armor { get; set; }
  [SerializeField]
  private float armorVal;
  public CharacterStat MagicResistance { get; set; }
  [SerializeField]
  private float magicResistanceVal;
  public CharacterStat AttackSpeed { get; set; }
  [SerializeField]
  private float attackSpeed;
  public CharacterStat MovementSpeed { get; set; }
  [SerializeField]
  private float movementSpeedVal;


  public void Init()
  {
    MaxHealth = new CharacterStat(maxHealthVal);
    MaxStamina = new CharacterStat(maxStaminaVal);
    MaxMana = new CharacterStat(maxManaVal);
    PhysicalPower = new CharacterStat(physicalPowerVal);
    MagicalPower = new CharacterStat(magicalPowerVal);
    Armor = new CharacterStat(armorVal);
    MagicResistance = new CharacterStat(magicResistanceVal);
    MovementSpeed = new CharacterStat(movementSpeedVal);
    AttackSpeed = new CharacterStat(attackSpeed);

    CurrentHealth = MaxHealth.Value;
    CurrentStamina = MaxStamina.Value;
    CurrentMana = MaxMana.Value;
  }

  public void Update()
  {
    CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth.Value);
    CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, MaxStamina.Value);
    CurrentMana = Mathf.Clamp(CurrentMana, 0f, MaxMana.Value);

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

  public void AddPlayerExperience(float xp)
  {
    playerXP += xp;
    float requiredXP = baseXP * Mathf.Pow(playerLevel, xpScaleFactor);

    if (playerXP >= requiredXP)
    {
      float surplus = playerXP - requiredXP;
      playerLevel++;
      playerXP = surplus;
    }
  }
}