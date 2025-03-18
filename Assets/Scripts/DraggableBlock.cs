using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject draggedObject; // Objeto que será arrastado (a cópia)
    public bool isOriginal = true; // Booleano para controlar se é o objeto original
    private CanvasGroup canvasGroup; // Referência ao CanvasGroup do objeto

    private void Start()
    {
        // Obtém o componente CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(gameObject.name);
        // Cria uma cópia do objeto original
        draggedObject = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);

        // Define o booleano isOriginal como false para o clone
        DraggableBlock cloneDraggable = draggedObject.GetComponent<DraggableBlock>();
        if (cloneDraggable != null)
        {
           cloneDraggable.isOriginal = false;
        }

        // Traz o objeto arrastado para a frente
        draggedObject.transform.SetAsLastSibling();

        // Desativa temporariamente o Raycast do objeto original
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }

        // Ativa o Collider da cópia
        Collider2D collider = draggedObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        // Adiciona um Rigidbody2D à cópia (se não tiver)
        Rigidbody2D rb = draggedObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = draggedObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic; // Define como Kinematic
        }

        DropArea dropArea = FindAnyObjectByType<DropArea>();
        if (dropArea != null)
        {
            int slotIndex = dropArea.GetSlotIndexAtPosition(draggedObject.transform.position);
            if (slotIndex != -1) {
                dropArea.ClearChildrenInSlotPosition(slotIndex);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            // Move o objeto arrastado para a posição do cursor/dedo
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                draggedObject.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint
            );
            draggedObject.transform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            // Obtém a DropArea
            DropArea dropArea = FindAnyObjectByType<DropArea>();
            if (dropArea != null)
            {
                // Verifica se o objeto foi solto dentro de algum slot
                int slotIndex = dropArea.GetSlotIndexAtPosition(draggedObject.transform.position);
                if (slotIndex != -1)
                {
                    // Tenta colocar o objeto no slot correspondente
                    bool placed = dropArea.PlaceObjectInSlot(draggedObject, slotIndex);
                    if (placed)
                    {
                        // Se o objeto foi colocado no slot, reativa o CanvasGroup do bloco na dropzone
                        CanvasGroup draggedCanvasGroup = draggedObject.GetComponent<CanvasGroup>();
                        if (draggedCanvasGroup != null)
                        {
                            draggedCanvasGroup.blocksRaycasts = true;
                        }
                    }
                    else
                    {
                        // Se não foi possível colocar no slot (slot ocupado), destrói o objeto arrastado
                        Destroy(draggedObject);
                    }
                }
                else
                {
                    // Se não foi solto dentro de um slot, destrói o objeto arrastado
                    Destroy(draggedObject);
                }
            }
        }

        // Reativa o Raycast do objeto original (se ele ainda existir)
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }
}