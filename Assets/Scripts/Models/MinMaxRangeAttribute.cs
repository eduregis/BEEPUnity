using UnityEngine;

public class MinMaxRangeAttribute : PropertyAttribute
{
    public float Min;
    public float Max;

    public MinMaxRangeAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }
}
