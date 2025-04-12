using UnityEngine;
using UnityEngine.UI;

public class LearnTag : MonoBehaviour
{
    [Header("Referências")]

    public Constants.LearnTag learnTag;
    public Button button;
    public Image lightLine;
    public Color color;
    
    void Start()
    {
        lightLine.color = color;
    }
  
    public void SetColor(Color newColor)
    {
        color = newColor;
        lightLine.color = newColor;
    }

}
