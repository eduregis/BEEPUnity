using UnityEngine;
using System.Collections;

public class CanvasFadeController : MonoBehaviour
{
    [Header("Configurações de Fade")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Prefabs de Canvas")]
    [SerializeField] private GameObject dialogueCanvasPrefab;
    [SerializeField] private GameObject settingsCanvasPrefab;

    private CanvasGroup canvasGroup;
    private GameObject currentCanvasInstance;
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

    public void ShowCanvas(Constants.MenuType type, string dialogueName = null)
    {
        GameObject selectedPrefab = null;

        switch (type)
        {
            case Constants.MenuType.Dialogue:
                selectedPrefab = dialogueCanvasPrefab;
                AppSettings.DialogueName = dialogueName;
                break;

            case Constants.MenuType.Settings:
                selectedPrefab = settingsCanvasPrefab;
                break;
        }

        if (selectedPrefab == null)
        {
            Debug.LogError($"Prefab para {type} não foi atribuído no inspector.");
            return;
        }

        if (currentCanvasInstance != null)
        {
            HideCanvas(() => InstantiateNewCanvas(selectedPrefab));
        }
        else
        {
            InstantiateNewCanvas(selectedPrefab);
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

        canvasGroup = currentCanvasInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = currentCanvasInstance.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

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
