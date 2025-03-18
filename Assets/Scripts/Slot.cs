using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return; // Verifica se o objeto é nulo

        GeneratedDraggableItem draggableItem = dropped.GetComponent<GeneratedDraggableItem>();
        if (draggableItem == null) return; // Verifica se o componente é nulo

        if (transform.childCount == 0)
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