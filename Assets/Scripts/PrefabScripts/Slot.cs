using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
    public InventoryGrid grid;
    public Image targetImage; // Referência para a imagem que você quer modificar

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return; // Verifica se o objeto é nulo
        grid.CheckAvailableSlot(dropped);
    }

    public bool IsEmpty()
    {
        return transform.childCount == 0;
    }

    public void FillSlot(GameObject dropped)
    {
        GeneratedDraggableItem draggableItem = dropped.GetComponent<GeneratedDraggableItem>();

        if (draggableItem == null) return;

        bool validCommand = true;

        if (grid.gridType != InventoryGrid.GridType.Main)
        {
            CommandGrid.CommandType commandType = draggableItem.commandItem.commandType;

            validCommand = commandType.ToString() != grid.gridType.ToString();

            Debug.Log("Command Type: " + commandType.ToString() + ", Grid Type: " + grid.gridType.ToString());
        }

        if (!validCommand)
        {
            Destroy(draggableItem.gameObject);
            CanvasFadeController.Instance.ShowCanvas(Constants.MenuType.Dialogue, "InfiniteRecursion");
            return;
        }

        if (IsEmpty())
        {
            draggableItem.parentToReturnTo = transform;
        }
        else
        {
            Transform existingBlock = transform.GetChild(0);
            existingBlock.SetParent(draggableItem.parentToReturnTo);
            existingBlock.localPosition = Vector3.zero;
            draggableItem.parentToReturnTo = transform;
        }
    }

    public void Highlight(bool enabled)
    {
        if (targetImage != null)
        {
            Color currentColor = targetImage.color;
            currentColor.a = enabled ? 0.5f : 1f; // 1f para totalmente opaco, 0.5f para semi-transparente
            targetImage.color = currentColor;
        }
        else
        {
            Debug.LogWarning("Target Image não foi atribuída no Inspector.");
        }
    }
}