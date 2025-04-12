using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoopBox : MonoBehaviour
{
    public InventoryGrid inventoryGrid;
    public Image backButton, frontButton;
    public Sprite backEnabled, backDisabled, frontEnabled, frontDisabled;
    public TMP_Text counterText;

    public int counter = 1;
    private int lowerLimit = 1, upperLimit = 9;

    public void Start()
    {
        UpdateUI();
    }

    public void TrashPressed() 
    {
        AudioManager.Instance.Play("dump");
        inventoryGrid.ResetSlots();
    }

    public void AddCounter()
    {
        if (counter < upperLimit)
        {
            counter++;
            UpdateUI(); 
        }
    }

    public void DecrementCounter()
    {
        if (counter > lowerLimit)
        {
            counter--;
            UpdateUI(); 
        }
    }

    public void UpdateUI()
    {
        counterText.text = counter.ToString();
        backButton.sprite = counter > lowerLimit ? backEnabled : backDisabled;
        frontButton.sprite = counter < upperLimit ? frontEnabled : frontDisabled;
    }
}