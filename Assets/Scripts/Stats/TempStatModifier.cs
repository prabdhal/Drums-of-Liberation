public class TempStatModifier
{
    public readonly float Value;
    public float Duration;
    public readonly StatType StatType;
    public readonly StatModType Type;
    public readonly int Order;
    public readonly object Source;

    public TempStatModifier(float value, float duration, StatType statType, StatModType type, int order, object source)
    {
        Value = value;
        Duration = duration;
        StatType = statType;
        Type = type;
        Order = order;
        Source = source;
    }

    public TempStatModifier(float value, float duration, StatType statType, StatModType type) : this(value, duration, statType, type, (int)type, null) { }

    public TempStatModifier(float value, float duration, StatType statType, StatModType type, int order) : this(value, duration, statType, type, order, null) { }

    public TempStatModifier(float value, float duration, StatType statType, StatModType type, object source) : this(value, duration, statType, type, (int)type, source) { }
}
