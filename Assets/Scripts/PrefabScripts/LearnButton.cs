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
    private LearnData _myData;

    public void Initialize(LearnData data)
    {
        _myData = data;
        _titleText.text = data.title;
        _decorationLine.color = GetColorByTag(data.tag);
    }

    private Color GetColorByTag(Constants.LearnTag tag)
    {
        return tag switch
        {
            Constants.LearnTag.Interface => interfaceColor,
            Constants.LearnTag.Concepts => conceptsColor,
            Constants.LearnTag.Characters => charactersColor,
            _ => Color.white
        };
    }

    public void OnClick()
    {
        LearnUIManager.Instance.DisplayLearnData(_myData);
    }
}