using UnityEngine;
using System.Collections;

public class CanvasFadeController : MonoBehaviour
{
    [Header("Configurações de Fade")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    private CanvasGroup canvasGroup;
    private GameObject currentCanvasInstance;
    public GameObject canvasPrefab;
    private static CanvasFadeController _instance;

    public static CanvasFadeController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<CanvasFadeController>();
                
                if (_instance == null)
                {
                    GameObject obj = new GameObject("CanvasFadeController");
                    _instance = obj.AddComponent<CanvasFadeController>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowDialogue(string name)
    {
        AppSettings.DialogueName = name;
        
        // Carrega o diálogo primeiro para garantir que está disponível
        Dialogue dialogue = Resources.Load<Dialogue>($"Dialogues/Dialogue_{name}");
        if (dialogue == null)
        {
            Debug.LogError($"Diálogo não encontrado: Dialogue_{name}");
            return;
        }

        if (currentCanvasInstance != null)
        {
            HideCanvas(() => InstantiateNewCanvas(canvasPrefab));
        }
        else
        {
            InstantiateNewCanvas(canvasPrefab);
        }
    }

    public void HideCanvas(System.Action onComplete = null)
    {
        if (currentCanvasInstance != null)
        {
            StartCoroutine(FadeCanvas(1f, 0f, fadeOutDuration, false, () => 
            {
                Destroy(currentCanvasInstance);
                currentCanvasInstance = null;
                onComplete?.Invoke();
            }));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void InstantiateNewCanvas(GameObject prefab)
    {
        currentCanvasInstance = Instantiate(prefab);
        
        // Configura o CanvasGroup
        canvasGroup = currentCanvasInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = currentCanvasInstance.AddComponent<CanvasGroup>();
        }
        
        // Configuração inicial
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Inicia o fade in
        StartCoroutine(FadeCanvas(0f, 1f, fadeInDuration, true));
    }

    private IEnumerator FadeCanvas(float startAlpha, float endAlpha, float duration, bool enableInteraction, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
        canvasGroup.interactable = enableInteraction;
        canvasGroup.blocksRaycasts = enableInteraction;
        
        onComplete?.Invoke();
    }
}