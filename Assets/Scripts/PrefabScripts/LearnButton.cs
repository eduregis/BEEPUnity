// LearnButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LearnButton : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Color interfaceColor = HexColorUtility.HexToColor("#aedb16");
    [SerializeField] private Color conceptsColor = HexColorUtility.HexToColor("#e332aa");
    [SerializeField] private Color charactersColor = HexColorUtility.HexToColor("#fe8305");

    [Header("Assets")]
    [SerializeField] private Image _decorationLine;
    [SerializeField] private TextMeshProUGUI _titleText;

     private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup => _canvasGroup ??= GetComponent<CanvasGroup>();

    public void Initialize(LearnData data)
    {
        _titleText.text = data.title;
        _decorationLine.color = GetColorByTag(data.tag);
    }
    
    private Color GetColorByTag(LearnData.Tag tag)
    {
        return tag switch
        {
            LearnData.Tag.Interface => interfaceColor,
            LearnData.Tag.Concepts => conceptsColor,
            LearnData.Tag.Characters => charactersColor,
            _ => Color.white
        };
    }
}