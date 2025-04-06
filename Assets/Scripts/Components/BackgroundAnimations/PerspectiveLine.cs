using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class PerspectiveLine : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private float lineHeight = 800f;
    [SerializeField] private float lineWidth = 2f;
    [SerializeField] private Color lineColor = Color.white;
    
    [Header("Perspective Settings")]
    [Range(90f, 170f)]
    [SerializeField] private float maxAngle = 120f;
    [SerializeField] private float centerZone = 0.3f;
    
    private RectTransform rectTransform;
    private Image lineImage;
    private Canvas canvas;
    private float canvasWidth;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        lineImage = GetComponent<Image>();
        
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("PerspectiveLine: Nenhum Canvas encontrado!");
                return;
            }
        }
        
        canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        InitializeLine();
    }
    
    private void InitializeLine()
    {
        lineImage.color = lineColor;
        rectTransform.sizeDelta = new Vector2(lineWidth, lineHeight);
        UpdatePerspective();
    }
    
    public void SetPosition(float normalizedXPos)
    {
        // Posição X baseada na posição normalizada (0 a 1)
        float xPos = Mathf.Lerp(-canvasWidth/2, canvasWidth/2, normalizedXPos);
        rectTransform.anchoredPosition = new Vector2(xPos, 0);
        UpdatePerspective();
    }
    
    private void UpdatePerspective()
    {
        float xPos = rectTransform.anchoredPosition.x;
        float normalizedX = (xPos * 2f) / canvasWidth; // Converte para -1 a 1
        
        if (Mathf.Abs(normalizedX) <= centerZone)
        {
            rectTransform.localRotation = Quaternion.Euler(0, 0, 90f);
            return;
        }
        
        float tiltFactor = (Mathf.Abs(normalizedX) - centerZone) / (1f - centerZone);
        float angle = Mathf.Lerp(90f, maxAngle, tiltFactor);
        angle = normalizedX > 0 ? 180f - angle : angle;
        
        rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}