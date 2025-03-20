using UnityEngine;

public class IsometricMapGenerator : MonoBehaviour
{
    public GameObject tilePrefab; // Prefab do tile isométrico
    public int[,] mapMatrix = new int[,]
    {
        {1, 1, 1, 1},
        {0, 1, 1, 0},
        {0, 1, 1, 1}
    };

    public float tileWidth = 64f; // Largura do tile
    public float tileHeight = 48f; // Altura do tile

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Obtém a posição do GameObject que contém o script
        Vector3 originPosition = transform.position;

        // Calcula o deslocamento para centralizar o mapa
        int mapWidth = mapMatrix.GetLength(1);
        int mapHeight = mapMatrix.GetLength(0);
        Vector3 offset = new Vector3(
                    (mapWidth - mapHeight) * ((tileWidth / 2) - 2),
                    -((mapWidth + mapHeight) / 2) * ((tileHeight / 2) - 9),
                    0
                );

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (mapMatrix[y, x] == 1)
                {
                    // Calcula a posição isométrica relativa ao centro
                    Vector3 tilePosition = new Vector3(
                        (x - y) * ((tileWidth / 2) - 2),
                        -(x + y) * ((tileHeight / 2) - 9),
                        0
                    );

                    // Ajusta a posição para considerar o centro do mapa
                    tilePosition += originPosition - offset;

                    // Instancia o tile na posição calculada
                    GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                    Tile tileScript = tile.GetComponent<Tile>();
                    tileScript.Initialize(mapMatrix, x, y);
                }
            }
        }
    }
}