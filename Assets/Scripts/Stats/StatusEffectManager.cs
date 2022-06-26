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

                damageOverTimeEffects[i].Duration -= 0.5f;
                Debug.Log("Status Effect: " + damageOverTimeEffects[i].DamageOverTimeEffectType + " deals " + damageOverTimeEffects[i].Damage
                  + " current duration: " + damageOverTimeEffects[i].Duration);
            }
            damageOverTimeEffects.RemoveAll(effect => effect.Duration <= 0);
            //StatsPanel.Instance.UpdateUI();
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
            yield return new WaitForSeconds(0.5f);

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

                tempStatDebuffEffects[i].Duration -= 0.5f;
                Debug.Log("Status Effect: " + tempStatDebuffEffects[i].TempStatDebuffType + " is in effect slowing target by " + tempStatDebuffEffects[i].Amount + " current duration left: " + tempStatDebuffEffects[i].Duration);
            }

            if (rootCount <= 0) IsRooted = false;
            if (stunCount <= 0) IsStunned = false;
            if (slowCount <= 0) IsSlowed = false;
            else ApplySlow(slowAmount);

            tempStatDebuffEffects.RemoveAll(effect => effect.Duration <= 0);
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
