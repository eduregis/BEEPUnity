using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class RetroSunsetController : MonoBehaviour
{
    [Header("Sun Colors")]
    public Color sunColor1 = new Color(1, 0.5f, 0, 1);
    public Color sunColor2 = new Color(1, 0.9f, 0.2f, 1);
    [Range(0f, 1f)] public float opacity = 1f;
    
    [Header("Sun Settings")]
    [Range(0.1f, 1f)] public float sunSize = 0.3f;
    [Range(0.1f, 1f)] public float visiblePercentage = 0.7f;
    
    [Header("Stripes Settings")]
    [Range(1f, 20f)] public float stripesDensity = 5f;
    [MinMaxRange(0.1f, 5f)] public MinMaxVector2 stripesSpeed = new MinMaxVector2(0.5f, 2f);
    [MinMaxRange(0.01f, 0.2f)] public MinMaxVector2 stripesWidth = new MinMaxVector2(0.02f, 0.05f);

    private Material sunMaterial;
    private Image image;

    // Custom attribute para UI amigável no Inspector (editor)
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

    void OnEnable()
    {
        image = GetComponent<Image>();
        CreateMaterialIfNeeded();
        UpdateMaterialProperties();
    }

    void OnDisable()
    {
        if (sunMaterial != null)
        {
            if (Application.isPlaying)
                Destroy(sunMaterial);
            else
                DestroyImmediate(sunMaterial);
        }
    }

    void Update()
    {
        UpdateMaterialProperties();
    }

    void OnValidate()
    {
        UpdateMaterialProperties();
    }

    private void CreateMaterialIfNeeded()
    {
        if (sunMaterial == null || image.material == null)
        {
            sunMaterial = new Material(Shader.Find("UI/80sSunset"));
            image.material = sunMaterial;
        }
    }

    private void UpdateMaterialProperties()
    {
        if (sunMaterial == null || image.material == null)
        {
            CreateMaterialIfNeeded();
            return;
        }

        if (image.material != sunMaterial)
        {
            sunMaterial = new Material(image.material);
            image.material = sunMaterial;
        }

        sunMaterial.SetColor("_SunColor1", sunColor1);
        sunMaterial.SetColor("_SunColor2", sunColor2);
        sunMaterial.SetFloat("_Opacity", opacity);
        sunMaterial.SetFloat("_SunSize", sunSize);
        sunMaterial.SetFloat("_VisiblePercentage", visiblePercentage);
        sunMaterial.SetFloat("_StripesDensity", stripesDensity);
        sunMaterial.SetFloat("_MinStripesSpeed", stripesSpeed.min);
        sunMaterial.SetFloat("_MaxStripesSpeed", stripesSpeed.max);
        sunMaterial.SetFloat("_MinStripesWidth", stripesWidth.min);
        sunMaterial.SetFloat("_MaxStripesWidth", stripesWidth.max);
    }
}
