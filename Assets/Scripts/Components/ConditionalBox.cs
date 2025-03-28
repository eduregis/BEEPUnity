using UnityEngine;


public class ConditionalBox : MonoBehaviour
{
    public InventoryGrid inventoryIfGrid, inventoryElseGrid;

    public void TrashPressed() 
    {
        inventoryIfGrid.ResetSlots();
        inventoryElseGrid.ResetSlots();
    }
}
