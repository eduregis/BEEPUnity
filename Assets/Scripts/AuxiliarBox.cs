using UnityEngine;
using UnityEngine.UI;

public class AuxiliarBox : MonoBehaviour
{
    public Button toggleButton; // Botão que vai controlar a animação
    public RectTransform panelRectTransform; // Referência ao RectTransform do painel
    public float animationDuration = 0.5f; // Duração da animação

    private Vector2 hiddenPosition; // Posição quando o painel está escondido
    private Vector2 visiblePosition; // Posição quando o painel está visível
    private bool isVisible = false; // Estado atual do painel

    void Start()
    {
        // Configura as posições inicial e final
        hiddenPosition = panelRectTransform.anchoredPosition;
        visiblePosition = hiddenPosition + new Vector2(0, panelRectTransform.rect.height);

        // Configura o listener do botão
        toggleButton.onClick.AddListener(TogglePanel);
    }

    void TogglePanel()
    {
        if (isVisible)
        {
            // Move o painel para a posição escondida
            StartCoroutine(AnimatePanel(hiddenPosition));
        }
        else
        {
            // Move o painel para a posição visível
            StartCoroutine(AnimatePanel(visiblePosition));
        }

        // Inverte o estado do painel
        isVisible = !isVisible;
    }

    System.Collections.IEnumerator AnimatePanel(Vector2 targetPosition)
    {
        Vector2 startPosition = panelRectTransform.anchoredPosition;
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que o painel termine exatamente na posição desejada
        panelRectTransform.anchoredPosition = targetPosition;
    }
}