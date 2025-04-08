using UnityEngine;
using UnityEngine.UI;

public class AuxiliarBox : MonoBehaviour
{
    public enum AnimationDirection
    {
        Vertical,
        Horizontal
    }

    public Button toggleButton, trashButton; // Botão que vai controlar a animação
    public RectTransform panelRectTransform; // Referência ao RectTransform do painel
    public float animationDuration = 0.5f; // Duração da animação
    public AnimationDirection direction = AnimationDirection.Vertical; // Direção da animação

    private Vector2 hiddenPosition; // Posição quando o painel está escondido
    private Vector2 visiblePosition; // Posição quando o painel está visível
    private bool isVisible = false; // Estado atual do painel

    void Start()
    {
        // Configura as posições inicial e final baseadas na direção escolhida
        hiddenPosition = panelRectTransform.anchoredPosition;
        
        if (direction == AnimationDirection.Vertical)
        {
            visiblePosition = hiddenPosition + new Vector2(0, panelRectTransform.rect.height);
        }
        else // Horizontal
        {
            visiblePosition = hiddenPosition + new Vector2(-panelRectTransform.rect.width, 0);
        }

        // Configura o listener do botão
        toggleButton.onClick.AddListener(TogglePanel);
    }

    void TogglePanel()
    {
        if (isVisible)
        {
            // Move o painel para a posição escondida
            AudioManager.Instance.Play("dropBlock");
            StartCoroutine(AnimatePanel(hiddenPosition));
        }
        else
        {
            // Move o painel para a posição visível
            AudioManager.Instance.Play("grabBlock");
            StartCoroutine(AnimatePanel(visiblePosition));
        }

        trashButton.gameObject.SetActive(!isVisible);

        // Inverte o estado do painel
        isVisible = !isVisible;
    }

    System.Collections.IEnumerator AnimatePanel(Vector2 targetPosition)
    {
        Vector2 startPosition = panelRectTransform.anchoredPosition;
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que o painel termine exatamente na posição desejada
        panelRectTransform.anchoredPosition = targetPosition;
    }
}