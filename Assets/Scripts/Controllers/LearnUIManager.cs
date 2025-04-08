using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class LearnUIManager : MonoBehaviour
{
    public static LearnUIManager Instance { get; private set; }

    [Header("Button List References")]
    [SerializeField] private Transform _contentParent;
    [SerializeField] private LearnButton _learnButtonPrefab;
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Details References")]
    [SerializeField] private GameObject _learnDisplayPanel;
    [SerializeField] private RectTransform _titleContainer;
    [SerializeField] private RectTransform _iconContainer;
    [SerializeField] private RectTransform _descriptionContainer;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _iconImage;

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

   public void DisplayLearnData(LearnData data)
    {
        _learnDisplayPanel.SetActive(true);
        // Preenche os conteúdos
        _titleText.text = data.title;
        _descriptionText.text = data.description;

        // Controle de visibilidade da imagem
        bool hasIcon = data.icon != null;
        _iconContainer.gameObject.SetActive(hasIcon);
        
        if (hasIcon)
        {
            _iconImage.sprite = data.icon;
            _iconImage.preserveAspect = true;
        }

        // Configuração dinâmica dos layouts
        ConfigureLayout(hasIcon);
    }

    private void ConfigureLayout(bool hasIcon)
    {
        // Ajusta os tamanhos preferidos
        if (hasIcon)
        {
            _iconContainer.GetComponent<LayoutElement>().preferredHeight = 200; // Valor desejado quando visível
            _descriptionContainer.GetComponent<LayoutElement>().flexibleHeight = 1;
        }
        else
        {
            _descriptionContainer.GetComponent<LayoutElement>().flexibleHeight = 1;
        }

        // Força reconstrução do layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(_descriptionContainer);
        Canvas.ForceUpdateCanvases();
    }
}