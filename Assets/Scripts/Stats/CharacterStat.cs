using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
  public float BaseValue;

  protected bool isDirty = true;
  protected float lastBaseValue;

  protected float _value;
  public virtual float Value
  {
    get
    {
      if (isDirty || lastBaseValue != BaseValue)
      {
        lastBaseValue = BaseValue;
        _value = CalculateFinalValue();
        isDirty = false;
      }
      return _value;
    }
  }

  protected readonly List<StatModifier> statModifiers;
  public readonly ReadOnlyCollection<StatModifier> StatModifiers;

  protected readonly List<TempStatModifier> tempStatModifiers;
  public readonly ReadOnlyCollection<TempStatModifier> TempStatModifiers;

  public CharacterStat()
  {
    statModifiers = new List<StatModifier>();
    tempStatModifiers = new List<TempStatModifier>();
    StatModifiers = statModifiers.AsReadOnly();
    TempStatModifiers = tempStatModifiers.AsReadOnly();
  }

  public CharacterStat(float baseValue) : this()
  {
    BaseValue = baseValue;
  }

  public virtual void AddModifier(StatModifier mod)
  {
    isDirty = true;
    statModifiers.Add(mod);
  }

  public virtual void AddTempModifier(TempStatModifier mod)
  {
    isDirty = true;
    tempStatModifiers.Add(mod);
  }

  public virtual bool RemoveModifier(StatModifier mod)
  {
    if (statModifiers.Remove(mod))
    {
      isDirty = true;
      return true;
    }
    return false;
  }

  public virtual bool RemoveTempModifier(TempStatModifier mod)
  {
    if (tempStatModifiers.Remove(mod))
    {
      isDirty = true;
      return true;
    }
    return false;
  }

  public virtual void RemoveAllModifiers()
  {
    statModifiers.Clear();
  }

  public virtual void RemoveAllTempModifiers()
  {
    tempStatModifiers.Clear();
  }

  public virtual bool RemoveAllModifiersFromSource(object source)
  {
    int numRemovals = statModifiers.RemoveAll(mod => mod.Source == source);

    if (numRemovals > 0)
    {
      isDirty = true;
      return true;
    }
    return false;
  }

  protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
  {
    if (a.Order < b.Order)
      return -1;
    else if (a.Order > b.Order)
      return 1;
    return 0;
  }

  protected virtual int CompareModifierOrder(TempStatModifier a, TempStatModifier b)
  {
    if (a.Order < b.Order)
      return -1;
    else if (a.Order > b.Order)
      return 1;
    return 0;
  }

  protected virtual float CalculateFinalValue()
  {
    float finalValue = BaseValue;
    float sumPercentAdd = 0;

    statModifiers.Sort(CompareModifierOrder);
    tempStatModifiers.Sort(CompareModifierOrder);

    for (int i = 0; i < statModifiers.Count; i++)
    {
      StatModifier mod = statModifiers[i];

      if (mod.Type == StatModType.Flat)
      {
        finalValue += mod.Value;
      }
      else if (mod.Type == StatModType.PercentAdd)
      {
        sumPercentAdd += mod.Value;

        if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
        {
          finalValue *= 1 + sumPercentAdd;
          sumPercentAdd = 0;
        }
      }
      else if (mod.Type == StatModType.PercentMult)
      {
        finalValue *= 1 + mod.Value;
      }
    }

    for (int i = 0; i < tempStatModifiers.Count; i++)
    {
      TempStatModifier mod = tempStatModifiers[i];

      if (mod.Type == StatModType.Flat)
      {
        finalValue += mod.Value;
      }
      else if (mod.Type == StatModType.PercentAdd)
      {
        sumPercentAdd += mod.Value;

        if (i + 1 >= tempStatModifiers.Count || tempStatModifiers[i + 1].Type != StatModType.PercentAdd)
        {
          finalValue *= 1 + sumPercentAdd;
          sumPercentAdd = 0;
        }
      }
      else if (mod.Type == StatModType.PercentMult)
      {
        finalValue *= 1 + mod.Value;
      }
    }

    // Workaround for float calculation errors, like displaying 12.00001 instead of 12
    return (float)Math.Round(finalValue, 4);
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
    if (TempStatModifiers.Count <= 0) return;
    //Debug.Log(string.Format("Temp mod count is {0}", TempStatModifiers.Count));

    for (int i = 0; i < TempStatModifiers.Count; i++)
    {
      //Debug.Log(string.Format("Temp mod value: {0} has duration of {1}", TempStatModifiers[i].Value, TempStatModifiers[i].Duration));
      TempStatModifiers[i].Duration -= Time.deltaTime;
      int numRemovals = tempStatModifiers.RemoveAll(mod => mod.Duration <= 0);

      if (numRemovals > 0)
        isDirty = true;
    }
  }
}
