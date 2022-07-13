using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StatusEffectManager : MonoBehaviour
{

    public bool HasEffect { get { return HasStatusEffect && HasDebuffEffect; } }
    public bool HasStatusEffect { get { return damageOverTimeEffects.Count > 0; } }
    public List<DamageOverTimeEffect> damageOverTimeEffects = new List<DamageOverTimeEffect>();
    public bool HasDebuffEffect { get { return tempStatDebuffEffects.Count > 0; } }
    public List<TempStatDebuffEffect> tempStatDebuffEffects = new List<TempStatDebuffEffect>();
    public bool HasStatModifierEffect { get { return tempStatModifierEffects.Count > 0; } }
    public List<TempStatModifier> tempStatModifierEffects = new List<TempStatModifier>();

    public bool IsSlowed;
    public bool IsRooted;
    public bool IsStunned;


    public void ApplyStatusEffects(DamageOverTimeEffect effect)
    {
        damageOverTimeEffects.Add(effect);
        if (damageOverTimeEffects.Count <= 1)
            StartCoroutine(StatusEffectHandler());
    }

    public void ApplyDebuffEffects(TempStatDebuffEffect effect)
    {
        tempStatDebuffEffects.Add(effect);
        if (effect.TempStatDebuffType.Equals(TempStatDebuffType.Slow))
            ApplySlow(effect.Amount);
        else if (effect.TempStatDebuffType.Equals(TempStatDebuffType.Root))
            ApplyRoot();
        else if (effect.TempStatDebuffType.Equals(TempStatDebuffType.Stun))
            ApplyStun();

        if (tempStatDebuffEffects.Count <= 1)
            StartCoroutine(DebuffEffectHandler());
    }

    public void ApplyStatBuffs(TempStatModifier mod)
    {
        tempStatModifierEffects.Add(mod);
        switch (mod.StatType)
        {
            case StatType.Health:
                PlayerManager.Instance.Stats.MaxHealth.AddTempModifier(mod);
                break;
            case StatType.Mana:
                PlayerManager.Instance.Stats.MaxMana.AddTempModifier(mod);
                break;
            case StatType.Stamina:
                PlayerManager.Instance.Stats.MaxStamina.AddTempModifier(mod);
                break;
            case StatType.Physical:
                PlayerManager.Instance.Stats.PhysicalPower.AddTempModifier(mod);
                break;
            case StatType.Magical:
                PlayerManager.Instance.Stats.MagicalPower.AddTempModifier(mod);
                break;
            case StatType.Armor:
                PlayerManager.Instance.Stats.Armor.AddTempModifier(mod);
                break;
            case StatType.MagicResist:
                PlayerManager.Instance.Stats.MagicResistance.AddTempModifier(mod);
                break;
        }

        if (tempStatModifierEffects.Count <= 1)
            StartCoroutine(TempStatModifierHandler());
    }

    private void ApplySlow(float amount)
    {
        IsSlowed = true;
    }

    private void ApplyRoot()
    {
        IsRooted = true;
    }

    private void ApplyStun()
    {
        IsStunned = true;
    }

    private IEnumerator StatusEffectHandler()
    {
        while (damageOverTimeEffects.Count > 0)
        {
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < damageOverTimeEffects.Count - 1; i++)
            {
                PlayerManager.Instance.Stats.CurrentHealth -= damageOverTimeEffects[i].Damage;
                DisplayDamagePopup(damageOverTimeEffects[i]);

                damageOverTimeEffects[i].Duration -= 1f;
                Debug.Log("Status Effect: " + damageOverTimeEffects[i].DamageOverTimeEffectType + " deals " + damageOverTimeEffects[i].Damage
                  + " current duration: " + damageOverTimeEffects[i].Duration);
            }
            damageOverTimeEffects.RemoveAll(effect => effect.Duration <= 0);
            PlayerManager.Instance.Stats.UpdateUI(true, true, true, true);
        }
    }

    private IEnumerator DebuffEffectHandler()
    {
        while (tempStatDebuffEffects.Count > 0)
        {
            float slowAmount = 0;
            float slowCount = 0;
            float rootCount = 0;
            float stunCount  = 0;
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < tempStatDebuffEffects.Count - 1; i++)
            {
                if (tempStatDebuffEffects[i].TempStatDebuffType.Equals(TempStatDebuffType.Slow))
                {
                    slowCount++;
                    slowAmount = Math.Max(slowAmount, tempStatDebuffEffects[i].Amount);
                }
                if (tempStatDebuffEffects[i].TempStatDebuffType.Equals(TempStatDebuffType.Root))
                    rootCount++;
                if (tempStatDebuffEffects[i].TempStatDebuffType.Equals(TempStatDebuffType.Stun))
                    stunCount++;

                tempStatDebuffEffects[i].Duration -= 1f;

                Debug.Log("Status Effect: " + tempStatDebuffEffects[i].TempStatDebuffType + " is in effect slowing target by " + tempStatDebuffEffects[i].Amount + " current duration left: " + tempStatDebuffEffects[i].Duration);
            }

            if (rootCount <= 0) IsRooted = false;
            if (stunCount <= 0) IsStunned = false;
            if (slowCount <= 0) IsSlowed = false;
            else ApplySlow(slowAmount);

            tempStatDebuffEffects.RemoveAll(effect => effect.Duration <= 0);
            PlayerManager.Instance.Stats.UpdateUI(true, true, true, true);
        }
    }

    private IEnumerator TempStatModifierHandler()
    {
        float timer = 10f;
        while (tempStatModifierEffects.Count > 0)
        {
            Debug.Log("First Time: " + timer);
            yield return new WaitForSeconds(1f);

            timer -= 1f;
            Debug.Log("Last Time: " + timer);
            for (int i = 0; i < tempStatModifierEffects.Count - 1; i++)
            {
                if (tempStatModifierEffects[i].Duration <= 0)
                {
                    switch (tempStatModifierEffects[i].StatType)
                    {
                        case StatType.Health:
                            PlayerManager.Instance.Stats.MaxHealth.RemoveTempModifier(tempStatModifierEffects[i]);
                            break;
                        case StatType.Mana:
                            PlayerManager.Instance.Stats.MaxMana.RemoveTempModifier(tempStatModifierEffects[i]);
                            break;
                        case StatType.Stamina:
                            PlayerManager.Instance.Stats.MaxStamina.RemoveTempModifier(tempStatModifierEffects[i]);
                            break;
                        case StatType.Physical:
                            PlayerManager.Instance.Stats.PhysicalPower.RemoveTempModifier(tempStatModifierEffects[i]);
                            break;
                        case StatType.Magical:
                            PlayerManager.Instance.Stats.MagicalPower.RemoveTempModifier(tempStatModifierEffects[i]);
                            break;
                        case StatType.Armor:
                            PlayerManager.Instance.Stats.Armor.RemoveTempModifier(tempStatModifierEffects[i]);
                            break;
                        case StatType.MagicResist:
                            PlayerManager.Instance.Stats.MagicResistance.RemoveTempModifier(tempStatModifierEffects[i]);
                            break;
                    }
                }
                Debug.Log("Spell buff duration: " + tempStatModifierEffects[i].StatType.ToString());
                Debug.Log("Spell buff duration: " + tempStatModifierEffects[i].Duration);
            }
            tempStatModifierEffects.RemoveAll(effect => effect.Duration <= 0);
            PlayerManager.Instance.Stats.UpdateUI(true, true, true, true);
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

    public void ClearDebuffEffects()
    {
        tempStatDebuffEffects.Clear();
    }

    public void ClearTempModifiersEffects()
    {
        tempStatModifierEffects.Clear();
    }

    private void DisplayDamagePopup(DamageOverTimeEffect effect)
    {
        if (effect.DamageOverTimeEffectType.Equals(DamageOverTimeEffectType.Bleed))
        {
            GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
            go.GetComponent<DamagePopup>().Init(effect.Damage, DamageType.Bleed);
        }
        else if (effect.DamageOverTimeEffectType.Equals(DamageOverTimeEffectType.Burn))
        {
            GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
            go.GetComponent<DamagePopup>().Init(effect.Damage, DamageType.Burn);
        }
        else if (effect.DamageOverTimeEffectType.Equals(DamageOverTimeEffectType.Poison))
        {
            GameObject go = Instantiate(GameManager.Instance.damagePopPrefab, PlayerManager.Instance.popupPos);
            go.GetComponent<DamagePopup>().Init(effect.Damage, DamageType.Poison);
        }
    }
}
