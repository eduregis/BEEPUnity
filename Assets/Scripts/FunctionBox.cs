using TMPro;
using UnityEngine;

public class FunctionBox : MonoBehaviour
{
    public InventoryGrid inventoryGrid;
    public TMP_Text counter;

    public void TrashPressed() 
    {
        inventoryGrid.ResetSlots();
    }
}
