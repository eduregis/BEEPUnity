using UnityEngine;

public class InfectedData : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isInfected = true;
    
    public void Initialize(Vector2Int position)
    {
        gridPosition = position;
        GetComponent<RectTransform>().anchoredPosition = 
            IsometricMapGenerator.Instance.GetTileRect(position)?.anchoredPosition ?? Vector2.zero;
    }
}