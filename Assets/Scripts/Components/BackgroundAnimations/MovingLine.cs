using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class MovingLine : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float minSpeed = 20f;
    
    [Header("Line Settings")]
    [SerializeField] private float lineHeight = 2f;
    [SerializeField] private Color lineColor = Color.white;
    
    private RectTransform rectTransform;
    private Image lineImage;
    private Canvas canvas;
    private float canvasHeight;
    private float startYPosition;
    private float targetYPosition; // Nova posição alvo (1/3 do canvas)
    
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
                Debug.LogError("MovingLine: Nenhum Canvas encontrado na cena!");
                enabled = false;
                return;
            }
        }
        
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        startYPosition = rectTransform.anchoredPosition.y;
        targetYPosition = startYPosition + (canvasHeight / 3f); // 1/3 do canvas a partir da posição inicial
        
        InitializeLine();
    }
    
    private void InitializeLine()
    {
        lineImage.color = lineColor;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, lineHeight);
    }
    
    private void Update()
    {
        // Calcula o progresso normalizado (0 a 1) da linha em relação ao trajeto
        float currentY = rectTransform.anchoredPosition.y;
        float progress = Mathf.InverseLerp(startYPosition, targetYPosition, currentY);
        
        // Interpola a velocidade entre maxSpeed e minSpeed baseado no progresso
        float currentSpeed = Mathf.Lerp(maxSpeed, minSpeed, progress);
        
        // Move a linha para cima
        rectTransform.anchoredPosition += Vector2.up * currentSpeed * Time.deltaTime;
        
        // Destrói a linha quando atinge 1/3 do canvas
        if (rectTransform.anchoredPosition.y >= targetYPosition)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetInitialPosition(Vector2 position)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        startYPosition = position.y;
        targetYPosition = startYPosition + (canvasHeight / 3f);
    }
}