using UnityEngine;

[CreateAssetMenu(fileName = "Learn", menuName = "New Learns/Learn Data")]
public class LearnData : ScriptableObject
{
    public string id; // Identificador único
    public Constants.LearnTag tag;
    public string title;
    [TextArea] public string description;
    public Sprite icon;
}