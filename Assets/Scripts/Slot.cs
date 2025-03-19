using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public InventoryGrid grid;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return; // Verifica se o objeto é nulo
        grid.CheckAvailableSlot(dropped);
    }

    public bool IsEmpty() {
        return transform.childCount == 0;
    }

    public void FillSlot(GameObject dropped) {
        Debug.Log(gameObject.name);
        GeneratedDraggableItem draggableItem = dropped.GetComponent<GeneratedDraggableItem>();
        if (draggableItem == null) return; // Verifica se o componente é nulo

        if (IsEmpty())
        {
            // Se o slot estiver vazio, aceita o bloco
            draggableItem.parentToReturnTo = transform;
        }
        else
        {
            // Se o slot já contém um bloco, troca os blocos de lugar
            Transform existingBlock = transform.GetChild(0);
            existingBlock.SetParent(draggableItem.parentToReturnTo);
            existingBlock.localPosition = Vector3.zero;
            draggableItem.parentToReturnTo = transform;
        }
    }
}