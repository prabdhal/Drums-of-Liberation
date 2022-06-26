using UnityEngine;

public class TempStatDebuffEffect : MonoBehaviour
{
    public TempStatDebuffType TempStatDebuffType;
    public float Amount { get; }
    public float Duration { get; set; }
    public float TickInterval { get; }

    public TempStatDebuffEffect(float amount, float duration, float interval, TempStatDebuffType type)
    {
        Amount = amount;
        Duration = duration;
        TickInterval = interval;
        TempStatDebuffType = type;
    }
}
