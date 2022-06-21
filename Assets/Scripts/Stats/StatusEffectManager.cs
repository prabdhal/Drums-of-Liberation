using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StatusEffectManager : MonoBehaviour
{
  public bool HasStatusEffect { get { return damageOverTimeEffects.Count > 0; } }
  public List<DamageOverTimeEffect> damageOverTimeEffects = new List<DamageOverTimeEffect>();

  public void ApplyStatusEffects(DamageOverTimeEffect effect)
  {
    damageOverTimeEffects.Add(effect);
    if (damageOverTimeEffects.Count <= 1)
      StartCoroutine(StatusEffectHandler());
  }

  private IEnumerator StatusEffectHandler()
  {
    while (damageOverTimeEffects.Count > 0)
    {
      yield return new WaitForSeconds(1f);

      for (int i = 0; i < damageOverTimeEffects.Count - 1; i++)
      {
        PlayerManager.Instance.Stats.CurrentHealth -= damageOverTimeEffects[i].Damage;
        damageOverTimeEffects[i].Duration -= 0.5f;
        Debug.Log("Status Effect: " + damageOverTimeEffects[i].DamageOverTimeEffectType + " deals " + damageOverTimeEffects[i].Damage
          + " current duration: " + damageOverTimeEffects[i].Duration);
      }
      damageOverTimeEffects.RemoveAll(effect => effect.Duration <= 0);
      //StatsPanel.Instance.UpdateUI();
    }
  }

  public void ClearStatusEffects(DamageOverTimeEffectType type = DamageOverTimeEffectType.All)
  {
    switch (type)
    {
      case DamageOverTimeEffectType.Bleed:
        damageOverTimeEffects.RemoveAll(effect => effect.DamageOverTimeEffectType.Equals(DamageOverTimeEffectType.Bleed));
        break;
      case DamageOverTimeEffectType.Burn:
        damageOverTimeEffects.RemoveAll(effect => effect.DamageOverTimeEffectType.Equals(DamageOverTimeEffectType.Burn));
        break;
      case DamageOverTimeEffectType.Poison:
        damageOverTimeEffects.RemoveAll(effect => effect.DamageOverTimeEffectType.Equals(DamageOverTimeEffectType.Poison));
        break;
      default:
        damageOverTimeEffects.Clear();
        break;
    }
    //StatsPanel.Instance.UpdateUI();
  }
}
