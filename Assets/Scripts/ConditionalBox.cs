using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionalBox : MonoBehaviour
{
    public InventoryGrid inventoryIfGrid, inventoryElseGrid;

    public void TrashPressed() 
    {
        inventoryIfGrid.ResetSlots();
        inventoryElseGrid.ResetSlots();
    }
}
