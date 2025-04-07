using System.Collections.Generic;
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
    private int currentItem = -1;


    private List<CommandItem> commandItems = new List<CommandItem>();

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

    public void ResetSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
           for (int j = slots[i].transform.childCount - 1; j >= 0; j--)
            {
                DestroyImmediate(slots[i].transform.GetChild(j).gameObject);
            }
            slots[i].Highlight(false);
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
                 AudioManager.Instance.Play("dropBlock");
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

    public List<string> GetCommandList() 
    {
        List<string> commandList = new List<string>();
        commandItems.Clear();
        currentItem = -1;

        // Itera sobre todos os slots
        for (int i = 0; i < slotCount; i++)
        {
            // Verifica se o slot não está vazio
            if (!slots[i].IsEmpty())
            {
                // Obtém o primeiro filho do slot (que deve ser o CommandItem)
                Transform child = slots[i].transform.GetChild(0);
                CommandItem commandItem = child.GetComponent<CommandItem>();
                commandItems.Add(commandItem);

                // Se o CommandItem existir, adiciona o tipo de comando à lista
                if (commandItem != null)
                {
                    commandList.Add(commandItem.commandType.ToString());
                }
            }
        }
        return commandList;
    }

    public void ResetHighlights() 
    {
        currentItem = -1;
        for (int i = 0; i < commandItems.Count; i++) 
        {
            commandItems[i].HighlightItem(false);
        }
    }

    public void HighlightCurrentStep() 
    {
        currentItem++;
        for (int i = 0; i < commandItems.Count; i++) 
        {
            commandItems[i].HighlightItem(i == currentItem);
        }
    }
}