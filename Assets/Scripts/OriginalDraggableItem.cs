using UnityEngine;
using UnityEngine.EventSystems;

public class OriginalDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject generatedDraggableItemPrefab; // Prefab da cópia gerada
    private GameObject draggedItem; // Cópia que está sendo arrastada
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Gera uma cópia do bloco original
        draggedItem = Instantiate(generatedDraggableItemPrefab, canvas.transform);
        draggedItem.transform.position = transform.position;

        // Configura a cópia para seguir o cursor
        GeneratedDraggableItem draggableItem = draggedItem.GetComponent<GeneratedDraggableItem>();
        draggableItem.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move a cópia gerada
        if (draggedItem != null)
        {
            GeneratedDraggableItem draggableItem = draggedItem.GetComponent<GeneratedDraggableItem>();
            draggableItem.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Finaliza o arrasto da cópia
        if (draggedItem != null)
        {
            GeneratedDraggableItem draggableItem = draggedItem.GetComponent<GeneratedDraggableItem>();
            draggableItem.OnEndDrag(eventData);

            // Verifica se a cópia foi solta em um slot válido
            if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("Slot"))
            {
                // Destrói a cópia se não foi solta em um slot válido
                Destroy(draggedItem);
            }

            draggedItem = null;
        }
    }
}