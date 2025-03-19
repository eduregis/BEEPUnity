using UnityEngine;

[ExecuteInEditMode] // Permite visualizar as mudanças no Editor
public class InventoryGrid : MonoBehaviour
{
    [Header("Slot Settings")]
    public GameObject slotPrefab; // Prefab do slot
    public int slotCount = 5; // Número de slots
    public float spacing = -5f; // Espaçamento entre os slots

    [Header("Slot Size")]
    public float slotWidth = 30f; // Largura do slot
    public float slotHeight = 25f; // Altura do slot
    private Slot[] slots;

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

        slots = new Slot[slotCount];

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

            // Obtém o componente Slot do objeto instanciado
            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent != null)
            {
                // Atribui a referência do InventoryGrid ao Slot
                slotComponent.grid = this;
                slots[i] = slotComponent;
            }
            else
            {
                Debug.LogError("Componente Slot não encontrado no prefab.");
            }
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

    public void CheckAvailableSlot(GameObject dropped)
    {
        // Procura o primeiro slot vazio disponível
        for (int i = 0; i < slotCount; i++)
        {
            if (slots[i].IsEmpty())
            {
                slots[i].FillSlot(dropped);
                return;
            }
        }

        Debug.Log("Nenhum slot vazio disponível.");
    }

    public void HighlightAvailableSlot(bool enabled)
    {
        bool firstEnabled = false;
        // Procura o primeiro slot vazio disponível
        for (int i = 0; i < slotCount; i++)
        {
            if (slots[i].IsEmpty() && !firstEnabled)
            {
                slots[i].Highlight(enabled);
                firstEnabled = true;
            } 
            else 
            {
                slots[i].Highlight(false);
            }
        }
    }

    public void ShiftItems()
    {
        for (int i = 0; i + 1 < slotCount; i++)
        {
            if (slots[i].IsEmpty() && !slots[i + 1].IsEmpty()) 
            {
                Transform child = slots[i + 1].transform.GetChild(0);
                child.SetParent(slots[i].transform);
                child.localPosition = Vector3.zero;
            }
        }
    }
}