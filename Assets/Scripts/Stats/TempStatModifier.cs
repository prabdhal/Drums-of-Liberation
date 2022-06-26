public class TempStatModifier
{
    public readonly float Value;
    public float Duration;
    public readonly StatModType Type;
    public readonly int Order;
    public readonly object Source;

    public TempStatModifier(float value, float duration, StatModType type, int order, object source)
    {
        Value = value;
        Duration = duration;
        Type = type;
        Order = order;
        Source = source;
    }

    public TempStatModifier(float value, float duration, StatModType type) : this(value, duration, type, (int)type, null) { }

    public TempStatModifier(float value, float duration, StatModType type, int order) : this(value, duration, type, order, null) { }

    public TempStatModifier(float value, float duration, StatModType type, object source) : this(value, duration, type, (int)type, source) { }
}
