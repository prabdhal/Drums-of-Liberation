using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private TextMeshProUGUI damageText;

    [Header("Colors")]
    [SerializeField]
    private Color physicalDamageColor;
    [SerializeField]
    private Color magicalDamageColor;
    [SerializeField]
    private Color burnDamageColor;
    [SerializeField]
    private Color poisonDamageColor;
    [SerializeField]
    private Color criticalDamageColor;
    [SerializeField]
    private Color bleedDamageColor;


    private void Start()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Destroy(gameObject, 1f);
    }

    public void Init(float damageAmount, DamageType damageType)
    {
        if (damageType.Equals(DamageType.Physical))
        {
            damageText.text = damageAmount.ToString();
            damageText.fontSize = GlobalValues.StandardDamageFontSize;
            damageText.color = physicalDamageColor;
        }
        else if (damageType.Equals(DamageType.Magical))
        {
            damageText.text = damageAmount.ToString();
            damageText.fontSize = GlobalValues.StandardDamageFontSize;
            damageText.color = magicalDamageColor;
        }
        else if (damageType.Equals(DamageType.Burn))
        {
            damageText.text = damageAmount.ToString();
            damageText.fontSize = GlobalValues.SmallDamageFontSize;
            damageText.color = burnDamageColor;
        }
        else if (damageType.Equals(DamageType.Poison))
        {
            damageText.text = damageAmount.ToString();
            damageText.fontSize = GlobalValues.SmallDamageFontSize;
            damageText.color = poisonDamageColor;
        }
        else if (damageType.Equals(DamageType.Critical))
        {
            damageText.text = damageAmount.ToString();
            damageText.fontSize = GlobalValues.CriticalDamageFontSize;
            damageText.color = criticalDamageColor;
        }
        else if (damageType.Equals(DamageType.Bleed))
        {
            damageText.text = damageAmount.ToString();
            damageText.fontSize = GlobalValues.SmallDamageFontSize;
            damageText.color = bleedDamageColor;
        }
        anim.Play(StringData.Popup);
    }
}

public enum DamageType
{
    Physical,
    Magical,
    Burn,
    Poison,
    Critical,
    Bleed
}
