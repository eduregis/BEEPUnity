using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isInFittingSpot = false;
    
    public void Initialize(Vector2Int position)
    {
        gridPosition = position;
        GetComponent<RectTransform>().anchoredPosition = 
            IsometricMapGenerator.Instance.GetTileRect(position)?.anchoredPosition ?? Vector2.zero;
    }
    
    public void MoveTo(Vector2Int newPosition)
    {
        gridPosition = newPosition;
        RectTransform tileRect = IsometricMapGenerator.Instance.GetTileRect(newPosition);
        if (tileRect != null)
        {
            GetComponent<RectTransform>().anchoredPosition = tileRect.anchoredPosition;
        }
    }
}