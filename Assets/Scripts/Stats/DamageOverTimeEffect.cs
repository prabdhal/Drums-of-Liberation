using UnityEngine;

public class DamageOverTimeEffect : MonoBehaviour
{
    public DamageOverTimeEffectType DamageOverTimeEffectType;
    public float Damage { get; }
    public float Duration { get; set; }
    public float TickInterval { get; }

    public DamageOverTimeEffect(float damage, float duration, float interval, DamageOverTimeEffectType type)
    {
        DamageOverTimeEffectType = type;
        Damage = damage;
        Duration = duration;
        TickInterval = interval;
    }
}
