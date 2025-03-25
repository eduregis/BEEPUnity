// Box.cs
using UnityEngine;

public class Box : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isInFittingSpot = false;

    public void Initialize(Vector2Int position)
    {
        gridPosition = position;
    }

    public void MoveTo(Vector2Int newPosition)
    {
        gridPosition = newPosition;
        // Atualiza a posição visual (isométrica)
        transform.position = Utils.CalculateTilePosition(newPosition.x, newPosition.y);
    }
}