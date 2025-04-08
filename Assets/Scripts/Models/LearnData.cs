using UnityEngine;

[CreateAssetMenu(fileName = "Learn", menuName = "New Learns/Learn Data")]
public class LearnData : ScriptableObject
{
    public enum Tag { Interface, Concepts, Characters }
    
    public string id; // Identificador único
    public Tag tag;
    public string title;
    [TextArea] public string description;
    public Sprite icon;
    
    // Adicione outros campos conforme necessário
}