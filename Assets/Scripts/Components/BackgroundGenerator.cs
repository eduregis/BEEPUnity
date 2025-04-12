using UnityEngine;
using UnityEngine.UI;

public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField] private GameObject backgroundPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateBackground();
    }

    private void GenerateBackground()
    {
        GameObject background = Instantiate(backgroundPrefab, transform);

        // 1. Obtém ou adiciona o componente Canvas
        Canvas bgCanvas = background.GetComponent<Canvas>();
        if (bgCanvas == null) 
        {
            bgCanvas = background.AddComponent<Canvas>();
        }

        // 2. Configura como Screen Space - Camera
        bgCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        // 3. Atribui a câmera principal da cena
        bgCanvas.worldCamera = Camera.main; // Ou use uma referência específica

        // 4. Garante que fique atrás de outros Canvases
        bgCanvas.overrideSorting = true;
        bgCanvas.sortingOrder = -100; // Valor baixo para ficar no fundo

        // 5. (Opcional) Adiciona GraphicRaycaster se for interativo
        if (!background.GetComponent<GraphicRaycaster>())
        {
            background.AddComponent<GraphicRaycaster>();
        }

        // 6. Coloca no início da hierarquia para garantir ordem de renderização
        background.transform.SetAsFirstSibling();
    }
}
