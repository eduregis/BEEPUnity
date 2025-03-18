using UnityEngine;

[ExecuteInEditMode] // Permite visualizar as mudanças no Editor
public class InventoryGrid : MonoBehaviour
{
    [Header("Slot Settings")]
    public GameObject slotPrefab; // Prefab do slot
    public int slotCount = 5; // Número de slots
    public float spacing = 10f; // Espaçamento entre os slots

    [Header("Slot Size")]
    public float slotWidth = 50f; // Largura do slot
    public float slotHeight = 50f; // Altura do slot

    private void Start()
    {
        if (Application.isPlaying) // Gera slots apenas durante a execução
        {
            GenerateSlots();
        }
    }

    private void OnDestroy()
    {
        // Limpa os slots ao destruir o objeto (incluindo ao parar o jogo no Editor)
        ClearSlots();
    }

    // Gera os slots programaticamente
    public void GenerateSlots()
    {
        if (slotPrefab == null)
        {
            Debug.LogError("Slot Prefab não foi atribuído.");
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            // Instancia o slot
            GameObject slot = Instantiate(slotPrefab, transform);

            // Define o nome do slot
            slot.name = $"Slot_{i + 1}";

            // Calcula a posição horizontal
            float xPosition = i * (slotWidth + spacing);
            slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, 0);

            // Define o tamanho do slot
            slot.GetComponent<RectTransform>().sizeDelta = new Vector2(slotWidth, slotHeight);
        }
    }

    // Remove todos os slots existentes
    public void ClearSlots()
    {
        // Destroi os slots de trás para frente para evitar problemas de índice
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}