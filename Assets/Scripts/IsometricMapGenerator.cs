using UnityEditor;
using UnityEngine;

public class IsometricMapGenerator : MonoBehaviour
{
    // Instância única do IsometricMapGenerator
    public static IsometricMapGenerator Instance { get; private set; }

    public GameObject tilePrefab; // Prefab do tile isométrico
    public int[,] mapMatrix; // Matriz do mapa (será definida pelo PlayerController)
    public float tileWidth = 64f; // Largura do tile
    public float tileHeight = 48f; // Altura do tile

    private void Awake()
    {
        // Configura a instância única do IsometricMapGenerator
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Se já existir uma instância, destrói esta nova instância
            Debug.LogWarning("Já existe uma instância do IsometricMapGenerator. Destruindo esta nova instância.");
            Destroy(gameObject);
        }
    }

    // Método para definir a matriz do mapa
    public void SetMapMatrix(int[,] matrix)
    {
        mapMatrix = matrix;
        GenerateMap();
    }

    void GenerateMap()
    {
        // Limpa o mapa anterior (se houver)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Obtém a posição do GameObject que contém o script
        Vector3 originPosition = transform.position;

        // Calcula o deslocamento para centralizar o map
        int mapWidth = mapMatrix.GetLength(1);
        int mapHeight = mapMatrix.GetLength(0);
        Vector3 offset = Utils.CalculateOffset();
        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (mapMatrix[y, x] != 0)
                {
                    // Calcula a posição isométrica relativa ao centro
                    Vector3 tilePosition = Utils.CalculateTilePosition(x, y);

                    // Ajusta a posição para considerar o centro do mapa
                    tilePosition += originPosition - offset;

                    // Instancia o tile na posição calculada
                    GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                    Tile tileScript = tile.GetComponent<Tile>();
                    tileScript.Initialize(mapMatrix, x, y, mapMatrix[y, x] == 2);
                }
            }
        }
    }
}