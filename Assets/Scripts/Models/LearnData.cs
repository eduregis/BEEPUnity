using UnityEngine;

[CreateAssetMenu(fileName = "Learn", menuName = "New Learns/Learn Data")]
public class LearnData : ScriptableObject
{
    public string id; // Identificador único
    public Constants.LearnTag tag;
    public string title;
    [TextArea(3,10)] public string description;
    public Sprite[] sprites;
}