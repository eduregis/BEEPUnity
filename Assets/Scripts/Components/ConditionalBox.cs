using UnityEngine;


public class ConditionalBox : MonoBehaviour
{
    public InventoryGrid inventoryIfGrid, inventoryElseGrid;

    public void TrashPressed() 
    {
        AudioManager.Instance.Play("dump");
        inventoryIfGrid.ResetSlots();
        inventoryElseGrid.ResetSlots();
    }
}
