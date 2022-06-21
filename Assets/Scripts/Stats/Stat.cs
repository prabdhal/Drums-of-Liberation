using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
  public float BaseValue { get; }
  public float BonusValue { get { return TotalEquipmentModifiersValue() + TotalTempModifiersValue(); } }
  public float FinalValue { get { return BaseValue + TotalEquipmentModifiersValue() + TotalTempModifiersValue(); } }
  public bool isPercent = false;
  public List<StatModifier> EquipmentModifiers { get { return equipmentModifiers; } }
  private List<StatModifier> equipmentModifiers = new List<StatModifier>();
  public List<TempStatModifier> TempModifiers { get { return tempModifier; } }
  private List<TempStatModifier> tempModifier = new List<TempStatModifier>();

  public Stat(float baseValue)
  {
    BaseValue = baseValue;
  }

  public Stat(float baseValue, List<StatModifier> equipmentModifiers)
  {
    BaseValue = baseValue;

    foreach (StatModifier stat in this.EquipmentModifiers)
    {
      equipmentModifiers.Add(stat);
    }
  }

  public float TotalEquipmentModifiersValue()
  {
    float value = 0;
    foreach (StatModifier stat in EquipmentModifiers)
    {
      value += stat.Value;
    }
    return value;
  }

  public float TotalTempModifiersValue()
  {
    float value = 0;
    foreach (TempStatModifier stat in TempModifiers)
    {
      value += stat.Value;
    }
    return value;
  }

  /// <summary>
  /// Adds a temp stat modifier.
  /// </summary>
  /// <param name="effect"></param>
  public void AddTempModifier(TempStatModifier effect)
  {
    TempModifiers.Add(effect);
    Debug.Log("Added temp stat modifier");
  }

  /// <summary>
  /// Removes all temp stat modifiers.
  /// </summary>
  /// <param name="effect"></param>
  public void RemoveAllTempModifier()
  {
    TempModifiers.Clear();
    Debug.Log("Removed all temp modifiers in this stat");
  }

  /// <summary>
  /// Adds a specific stat modifier
  /// </summary>
  /// <param name="stat"></param>
  public void AddEquipmentModifier(StatModifier stat)
  {
    EquipmentModifiers.Add(stat);
    Debug.Log("added equipment stat modifier");
  }

  /// <summary>
  /// Removes a specific stat modifier.
  /// </summary>
  /// <param name="stat"></param>
  public void RemoveEquipmentModifier(StatModifier stat)
  {
    EquipmentModifiers.Remove(stat);
    Debug.Log("Removed equipment stat modifier");
  }

  /// <summary>
  /// Removes all equipment modifiers.
  /// </summary>
  public void RemoveAllEquipmentModifier()
  {
    EquipmentModifiers.Clear();
    Debug.Log("Removed all equipment modifiers for this stat");
  }

  /// <summary>
  /// Required to run in an update method of a MonoBehaviour script.
  /// </summary>
  public void Update()
  {
    TempModifierDurationHandler();
  }

  /// <summary>
  /// Responsible for updating the current duration of temp modifiers. 
  /// </summary>
  private void TempModifierDurationHandler()
  {
    if (TempModifiers.Count <= 0) return;

    for (int i = 0; i < TempModifiers.Count; i++)
    {
      TempModifiers[i].Duration -= Time.deltaTime;
      TempModifiers.RemoveAll(i => i.Duration <= 0);
    }
  }
}
