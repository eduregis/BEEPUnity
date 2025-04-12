using UnityEngine;

[System.Serializable]
public struct MinMaxVector2
{
    public float min;
    public float max;

    public MinMaxVector2(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(min, max);
    }
}
