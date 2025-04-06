using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class FinalRetroGridController : MonoBehaviour
{
    [Header("Grid Appearance")]
    public Color gridColor = new Color(0, 1, 1, 1);
    public Color backgroundColor = new Color(0, 0, 0.2f, 1);
    [Range(10, 200)] public float gridDensity = 80f;
    [Range(0.01f, 0.1f)] public float lineWidth = 0.03f;
    
    [Header("Animation Settings")]
    public Vector2 scrollSpeed = new Vector2(0.3f, 1f);
    public bool reverseAnimation = false;
    
    [Header("Perspective Settings")]
    [Range(0, 1)] public float perspectiveStrength = 0.7f;
    [Range(0.1f, 0.9f)] public float horizonPosition = 0.6f;
    [Range(0.01f, 0.5f)] public float fadeLength = 0.2f;
    public enum HorizonDirection { Up, Down }
    public HorizonDirection horizonDirection = HorizonDirection.Up;

    private Material gridMaterial;

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
        if (gridMaterial != null && !Application.isPlaying)
        {
            DestroyImmediate(gridMaterial);
        }
    }

    void OnValidate()
    {
        UpdateMaterialProperties();
    }

    private void CreateMaterialIfNeeded()
    {
        if (gridMaterial == null)
        {
            Image image = GetComponent<Image>();
            gridMaterial = new Material(Shader.Find("UI/80sObliqueGridFinal"));
            image.material = gridMaterial;
        }
    }

    private void UpdateMaterialProperties()
    {
        if (gridMaterial == null)
            CreateMaterialIfNeeded();

        gridMaterial.SetColor("_GridColor", gridColor);
        gridMaterial.SetColor("_BackgroundColor", backgroundColor);
        gridMaterial.SetFloat("_GridDensity", gridDensity);
        gridMaterial.SetFloat("_LineWidth", lineWidth);
        gridMaterial.SetVector("_ScrollSpeed", scrollSpeed);
        gridMaterial.SetFloat("_ReverseAnimation", reverseAnimation ? 1 : 0);
        gridMaterial.SetFloat("_Perspective", perspectiveStrength);
        gridMaterial.SetFloat("_HorizonPosition", horizonPosition);
        gridMaterial.SetFloat("_HorizonDirection", horizonDirection == HorizonDirection.Up ? 1 : -1);
        gridMaterial.SetFloat("_FadeLength", fadeLength);
    }

    public void SetHorizonDirection(bool upwards)
    {
        horizonDirection = upwards ? HorizonDirection.Up : HorizonDirection.Down;
        UpdateMaterialProperties();
    }
}