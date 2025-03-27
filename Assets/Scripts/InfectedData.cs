using UnityEngine;

public class InfectedData : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isInfected = true;
    
    public void Initialize(Vector2Int position) 
    {
        gridPosition = position;

        int depth = Utils.CalculateIsoDepth(position, 1);
        transform.SetSiblingIndex(depth);
        
        GetComponent<RectTransform>().anchoredPosition = 
        IsometricMapGenerator.Instance.GetTileRect(position)?.anchoredPosition + 
        Vector3.up * 40f ?? Vector2.zero;
    }
}