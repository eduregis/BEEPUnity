using UnityEngine;
using UnityEngine.UI;

public class DropArea : MonoBehaviour
{
    public int slotCount = 12; // Número de slots
    public float slotWidth = 30f; // Largura de cada slot
    public float slotHeight = 25f; // Altura de cada slot
    public float spacing = -5f; // Espaçamento entre os slots
    public Vector2 startPosition = new Vector2(-150, 0); // Posição inicial do primeiro slot
    public Sprite slotSprite; // Sprite para o visual do slot

    private GameObject[] slots; // Array para armazenar os slots

    private void Start()
    {
        GenerateSlots(); // Gera os slots ao iniciar
    }

    // Gera os slots programaticamente
    private void GenerateSlots()
    {
        slots = new GameObject[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            // Calcula a posição do slot
            float x = startPosition.x + i * (slotWidth + spacing);
            float y = startPosition.y;
            Vector2 slotPosition = new Vector2(x, y);

            // Cria um objeto vazio para representar o slot
            GameObject slot = new GameObject("Slot " + i);
            slot.transform.SetParent(transform); // Define a DropArea como pai
            slot.transform.localPosition = slotPosition;

            Image slotImage = slot.AddComponent<Image>();
            slotImage.sprite = slotSprite; // Define o sprite do slot
            slotImage.rectTransform.sizeDelta = new Vector2(slotWidth, slotHeight);

            // Adiciona um Collider2D ao slot para detectar colisões
            BoxCollider2D collider = slot.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(slotWidth, slotHeight);
            collider.isTrigger = true; // Define como trigger

            slots[i] = slot; // Armazena o slot no array
        }
    }

    // Verifica se um ponto está dentro de algum slot
    public bool IsPointInsideAnySlot(Vector2 position)
    {
        foreach (GameObject slot in slots)
        {
            if (slot.GetComponent<Collider2D>().bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    // Retorna o índice do slot que contém o ponto
    public int GetSlotIndexAtPosition(Vector2 position)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<Collider2D>().bounds.Contains(position))
            {
                return i;
            }
        }
        return -1; // Retorna -1 se o ponto não estiver em nenhum slot
    }

    // Coloca um objeto no slot correspondente
    public bool PlaceObjectInSlot(GameObject obj, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length && slots[slotIndex].transform.childCount == 0)
        {
            // Define o slot como pai do objeto
            obj.transform.SetParent(slots[slotIndex].transform);
            obj.transform.localPosition = Vector2.zero; // Centraliza o objeto no slot

            // Reativa o CanvasGroup do bloco na dropzone
            CanvasGroup objCanvasGroup = obj.GetComponent<CanvasGroup>();
            if (objCanvasGroup != null)
            {
                objCanvasGroup.blocksRaycasts = true;
            }

            Debug.Log("Objeto colocado no slot: " + slotIndex);
            return true; // Retorna true se o objeto foi colocado com sucesso
        }
        else
        {
            Debug.Log("Slot ocupado ou fora dos limites: " + slotIndex);
            return false; // Retorna false se o slot estiver ocupado ou fora dos limites
        }
    }

    public void ClearChildrenInSlotPosition(int index)
    {
        Destroy(slots[index].transform.GetChild(0).gameObject);
    }
}