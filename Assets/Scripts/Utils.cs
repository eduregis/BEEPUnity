using UnityEngine;

public static class Utils
{
    // Valores fixos
    public const float TileWidth = 64f;
    public const float TileHeight = 48f;
    public const float AssetAdjust = 10f; // Valor padrão para o ajuste

    // Método para calcular o offset
    public static Vector3 CalculateOffset()
    {
        int mapWidth = IsometricMapGenerator.Instance.mapMatrix.GetLength(1);
        int mapHeight = IsometricMapGenerator.Instance.mapMatrix.GetLength(0);

        Vector3 offset = new Vector3(
            (mapWidth - mapHeight) * ((TileWidth / 2) - 2),
            -((mapWidth + mapHeight) / 2) * ((TileHeight / 2) - 9),
            0
        );

        return offset;
    }

    // Método para calcular a posição do tile
    public static Vector3 CalculateTilePosition(float x, float y, float assetAdjust = AssetAdjust)
    {
        Vector3 tilePosition = new Vector3(
            (x - y) * ((TileWidth / 2) - 2),
            -(x + y) * ((TileHeight / 2) - 9) + assetAdjust,
            0
        );

        return tilePosition;
    }

    public static int CalculateIsoDepth(Vector2Int position, int layerOffset = 0) 
    {
        return -(position.x + position.y) * 10 + layerOffset;
    }
}