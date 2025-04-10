using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class RetroSunsetUIController : MonoBehaviour
{
    [Header("Sun Appearance")]
    [ColorUsage(true, true)]
    public Color mainColor = new Color(1f, 0.2f, 0.4f, 1f);
    [ColorUsage(true, true)]
    public Color secondaryColor = new Color(1f, 0.6f, 0.2f, 1f);
    [Range(0, 1)]
    public float sunSize = 0.5f;
    [Range(0, 1)]
    public float sunEdgeSoftness = 0.2f;

    [Header("Stripes Settings")]
    [ColorUsage(true, true)]
    public Color stripesColor = new Color(0f, 0.8f, 1f, 1f);
    [Range(0, 0.5f)]
    public float stripesWidth = 0.1f;
    [Range(0, 0.5f)]
    public float stripesSoftness = 0.05f;
    public float stripesSpeed = 1f;
    public float stripesSpacing = 0.5f;

    private Material materialInstance;
    private Image image;

    private Material sunsetMaterial;

    void OnEnable()
    {
        CreateMaterialIfNeeded();
        UpdateMaterialProperties();
    }

    void Update()
    {
        UpdateMaterialProperties();
    }

    void OnDisable()
    {
        if (sunsetMaterial != null && !Application.isPlaying)
        {
            DestroyImmediate(sunsetMaterial);
        }
    }

    void OnValidate()
    {
        UpdateMaterialProperties();
    }

    private void CreateMaterialIfNeeded()
    {
        if (sunsetMaterial == null)
        {
            image = GetComponent<Image>();
            sunsetMaterial = new Material(Shader.Find("UI/80sSunset"));
            image.material = sunsetMaterial;
            materialInstance = sunsetMaterial;
        }
    }


    void UpdateMaterialProperties()
    {
        CreateMaterialIfNeeded();

        if (materialInstance == null) return;

        // Update sun properties
        materialInstance.SetColor("_MainColor", mainColor);
        materialInstance.SetColor("_SecondaryColor", secondaryColor);
        materialInstance.SetFloat("_SunSize", sunSize);
        materialInstance.SetFloat("_SunEdgeSoftness", sunEdgeSoftness);

        // Update stripes properties
        materialInstance.SetColor("_StripesColor", stripesColor);
        materialInstance.SetFloat("_StripesWidth", stripesWidth);
        materialInstance.SetFloat("_StripesSoftness", stripesSoftness);
        materialInstance.SetFloat("_StripesSpeed", stripesSpeed);
        materialInstance.SetFloat("_StripesSpacing", stripesSpacing);

        image.SetMaterialDirty();
    }

}