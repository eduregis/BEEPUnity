using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "MyDialogues/Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(3, 10)]
    public List<string> descriptionTexts;
    public List<string> learnIdentifiers;
}