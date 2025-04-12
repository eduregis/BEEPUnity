using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollToTop : MonoBehaviour
{
    void Start()
    {
        // Rola para o topo ao iniciar
        GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    // Chamar este método se atualizar o conteúdo
    public void ForceToTop()
    {
        Canvas.ForceUpdateCanvases();
        GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        Canvas.ForceUpdateCanvases();
    }
}