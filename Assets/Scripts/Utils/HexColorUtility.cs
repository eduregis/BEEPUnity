// HexColorUtility.cs
using UnityEngine;

public static class HexColorUtility
{
    /// <summary>
    /// Converte um hexadecimal (formato "#RRGGBB" ou "#RRGGBBAA") para Color
    /// </summary>
    public static Color HexToColor(string hex)
    {
        hex = hex.TrimStart('#');

        if (hex.Length == 6)
        {
            // Sem alpha (opaco)
            return new Color(
                HexToFloat(hex[..2]),
                HexToFloat(hex.Substring(2, 2)),
                HexToFloat(hex.Substring(4, 2)),
                1f);
        }
        else if (hex.Length == 8)
        {
            // Com alpha
            return new Color(
                HexToFloat(hex[..2]),
                HexToFloat(hex.Substring(2, 2)),
                HexToFloat(hex.Substring(4, 2)),
                HexToFloat(hex.Substring(6, 2)));
        }
        else
        {
            Debug.LogError($"Formato hexadecimal inválido: {hex}. Use #RRGGBB ou #RRGGBBAA");
            return Color.magenta; // Cor de fallback visível
        }
    }

    /// <summary>
    /// Converte um hexadecimal (formato "#RRGGBB" ou "#RRGGBBAA") para Color32
    /// </summary>
    public static Color32 HexToColor32(string hex)
    {
        hex = hex.TrimStart('#');

        if (hex.Length == 6)
        {
            // Sem alpha (255)
            return new Color32(
                HexToByte(hex[..2]),
                HexToByte(hex.Substring(2, 2)),
                HexToByte(hex.Substring(4, 2)),
                255);
        }
        else if (hex.Length == 8)
        {
            // Com alpha
            return new Color32(
                HexToByte(hex.Substring(0, 2)),
                HexToByte(hex.Substring(2, 2)),
                HexToByte(hex.Substring(4, 2)),
                HexToByte(hex.Substring(6, 2)));
        }
        else
        {
            Debug.LogError($"Formato hexadecimal inválido: {hex}. Use #RRGGBB ou #RRGGBBAA");
            return new Color32(255, 0, 255, 255); // Magenta como fallback
        }
    }

    private static float HexToFloat(string hex)
    {
        return HexToByte(hex) / 255f;
    }

    private static byte HexToByte(string hex)
    {
        return byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
    }
}