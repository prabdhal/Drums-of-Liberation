using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStats
{
  public CharacterStat MaxHealth { get; set; }
  [SerializeField]
  private float maxHealthVal;
  public float CurrentHealth { get; set; }
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
  public CharacterStat MovementSpeed { get; set; }
  [SerializeField]
  private float movementSpeedVal;


  public void Init()
  {
    MaxHealth = new CharacterStat(maxHealthVal);
    MaxMana = new CharacterStat(maxManaVal);
    PhysicalPower = new CharacterStat(physicalPowerVal);
    MagicalPower = new CharacterStat(magicalPowerVal);
    Armor = new CharacterStat(armorVal);
    MagicResistance = new CharacterStat(magicResistanceVal);
    MovementSpeed = new CharacterStat(movementSpeedVal);

    CurrentHealth = MaxHealth.Value;
    CurrentMana = MaxMana.Value;
  }

  public void Update()
  {
    CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth.Value);
    CurrentMana = Mathf.Clamp(CurrentMana, 0f, MaxMana.Value);

    MaxHealth.Update();
    MaxMana.Update();
    PhysicalPower.Update();
    MagicalPower.Update();
    Armor.Update();
    MagicResistance.Update();
    MovementSpeed.Update();
  }
}