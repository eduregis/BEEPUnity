using UnityEngine;
using UnityEngine.UI;

public class NeonEffect : MonoBehaviour
{
    private float pulseSpeed = 2f; // Velocidade da pulsação
    private float timeOffset;      // Offset de tempo para variar o efeito entre os tiles
    public Image lightEffect;
    private Color color1 = HexColorUtility.HexToColor("#e332aa");
    private Color color2 = HexColorUtility.HexToColor("#1479da");


    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Sin(Time.time * pulseSpeed + timeOffset) * 0.5f + 0.5f;
        Color currentColor = Color.Lerp(color1, color2, t);
        if (lightEffect.gameObject.activeSelf)
            lightEffect.color = currentColor;
    }
}
