using UnityEngine;
using UnityEngine.EventSystems;

public class GeneratedDraggableItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Transform parentToReturnTo = null;
    public CommandItem commandItem; // Referência ao CommandItem
    private InventoryGrid currentGrid;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        commandItem = GetComponent<CommandItem>(); // Obtém o CommandItem
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.Instance.Play("grabBlock");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (AppSettings.IsPlaying)
        {
            canvasGroup.blocksRaycasts = true;
            eventData.pointerDrag = null; // Cancela o drag
            return;
        }

        parentToReturnTo = transform.parent;
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Se IsPlaying for TRUE, não faz nada
        if (AppSettings.IsPlaying) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        InventoryGrid grid = eventData.pointerEnter.GetComponentInParent<InventoryGrid>();
        
        if (grid != null)
        {
            currentGrid = grid;
            grid.HighlightAvailableSlot(true);
        } 
        else 
        {
            if (currentGrid != null) 
            {
                currentGrid.HighlightAvailableSlot(false);
                currentGrid.ShiftItems();
                currentGrid = null;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Se IsPlaying for TRUE, não faz nada
        if (AppSettings.IsPlaying) return;

        canvasGroup.blocksRaycasts = true;

        // Verifica se o objeto foi solto em um slot válido
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("Slot"))
        {
            // Se foi solto em um slot válido, define o novo parent
            transform.SetParent(parentToReturnTo);
            transform.localPosition = Vector3.zero; // Centraliza no slot
        }
        else
        {
            // Se não foi solto em um slot válido, destrói o objeto
            Destroy(gameObject);
        }
    }
}