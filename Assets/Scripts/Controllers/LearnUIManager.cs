using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class LearnUIManager : MonoBehaviour
{
    public static LearnUIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Transform _contentParent;
    [SerializeField] private LearnButton _learnButtonPrefab;
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Animation Settings")]
    [SerializeField] private float _buttonFadeInDuration = 0.3f;

    private List<LearnButton> _activeButtons = new List<LearnButton>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Opcional: DontDestroyOnLoad(gameObject) se precisar persistir entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
        DOTween.Init();
    }

    public void ShowFilteredItems(List<LearnData> items)
    {
        ClearContent();
        
        foreach (var item in items)
        {
            var button = Instantiate(_learnButtonPrefab, _contentParent);
            button.Initialize(item);
            AnimateButtonAppearance(button);
            _activeButtons.Add(button);
        }

        ScrollToTop();
    }

    private void AnimateButtonAppearance(LearnButton button)
    {
        // Configura estado inicial
        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, _buttonFadeInDuration).SetEase(Ease.OutQuad);
    }

    private void ClearContent()
    {
        foreach (var button in _activeButtons)
        {
            button.transform.DOKill(); // Cancela animações pendentes
            Destroy(button.gameObject);
        }
        _activeButtons.Clear();
    }

    private void ScrollToTop()
    {
        _scrollRect.verticalNormalizedPosition = 1;
    }
}