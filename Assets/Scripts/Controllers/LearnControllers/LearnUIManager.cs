using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    [SerializeField] private ScrollToTop _descriptionScroll;
    [SerializeField] private Image _iconImage;

    [Header("Animation Settings")]
    [SerializeField] private float _buttonFadeInDuration = 0.3f;
    private Coroutine currentAnimation; // Guarda a animação atual
    private bool isPlayingAnimation; // Flag para controle

    private List<LearnButton> _activeButtons = new List<LearnButton>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        if (data == null)
        {
            Debug.LogError("LearnData is null!");
            return;
        }

        AudioManager.Instance.Play("defaultButton");
        
        _learnDisplayPanel.SetActive(true);
        // Preenche os conteúdos
        _titleText.text = data.title;
        _descriptionText.text = data.description;
        _descriptionScroll.ForceToTop();

        // Interrompe a animação atual se estiver rodando
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            isPlayingAnimation = false;
        }

        // Se não houver sprites, esconde a imagem
        if (data == null || data.sprites == null || data.sprites.Length == 0)
        {
            SetImageAlpha(0f);
            return;
        }

        // Mostra a imagem e inicia a animação em loop
        SetImageAlpha(1f);
        currentAnimation = StartCoroutine(PlayAnimationLoop(data.sprites));
    }

    private IEnumerator PlayAnimationLoop(Sprite[] frames)
    {
        isPlayingAnimation = true;
        int currentFrame = 0;

        while (isPlayingAnimation) // Loop infinito controlado
        {
            if (frames == null || frames.Length == 0) yield break;

            // Atualiza o sprite
            _iconImage.sprite = frames[currentFrame];
            _iconImage.preserveAspect = true;

            // Avança para o próximo frame (ou volta ao início)
            currentFrame = (currentFrame + 1) % frames.Length;

            // Espera um tempo antes do próximo frame
            yield return new WaitForSeconds(0.1f); // Ajuste o tempo conforme necessário
        }
    }

    private void SetImageAlpha(float alpha)
    {
        if (_iconImage != null)
        {
            Color newColor = _iconImage.color;
            newColor.a = alpha;
            _iconImage.color = newColor;
        }
    }

    // Método para parar a animação manualmente (opcional)
    public void StopAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            isPlayingAnimation = false;
        }
    }
}