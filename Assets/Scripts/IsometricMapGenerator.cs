using System.Collections.Generic;
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

    public GameObject boxPrefab; // Prefab da caixa
    public Box[,] boxesMatrix; // Matriz para rastrear caixas

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
    public void SetMapMatrix(int[,] matrix, List<Vector2Int> initialBoxPositions = null)
    {
        mapMatrix = matrix;
        boxesMatrix = new Box[matrix.GetLength(0), matrix.GetLength(1)];
        GenerateMap();

        // Adiciona caixas iniciais se fornecidas
        if (initialBoxPositions != null)
        {
            foreach (var position in initialBoxPositions)
            {
                if (position.y >= 0 && position.y < matrix.GetLength(0) &&
                    position.x >= 0 && position.x < matrix.GetLength(1) &&
                    matrix[position.y, position.x] != 0) // Só coloca em tiles válidos
                {
                    CreateBoxAtPosition(position);
                }
            }
        }
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

    public void UpdateTileVisual(int x, int y)
    {
        // Atualiza o visual do tile (encaixe)
        Tile tile = GetTileAtPosition(x, y);
        if (tile != null)
        {
            tile.fittingBox.gameObject.SetActive(mapMatrix[y, x] == 2);
        }

        // Atualiza visual da caixa (se houver)
        if (boxesMatrix[y, x] != null)
        {
            boxesMatrix[y, x].isInFittingSpot = mapMatrix[y, x] == 2;
            // Aqui você pode atualizar o visual da caixa (ex: mudar cor se estiver no encaixe)
        }
    }

    private Tile GetTileAtPosition(int x, int y)
    {
        // Implemente uma forma de encontrar o Tile na posição (x,y)
        // Pode ser um dicionário ou percorrer os filhos
        foreach (Transform child in transform)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile != null && tile.x == x && tile.y == y)
            {
                return tile;
            }
        }
        return null;
    }

    private void CreateBoxAtPosition(Vector2Int position)
    {
        if (boxesMatrix[position.y, position.x] == null)
        {
            GameObject boxObj = Instantiate(boxPrefab, transform);
            Box box = boxObj.GetComponent<Box>();
            box.Initialize(position);
            boxesMatrix[position.y, position.x] = box;
            UpdateTileVisual(position.x, position.y);
        }
    }

    public bool AddBox(Vector2Int position, Box box)
    {
        if (mapMatrix[position.y, position.x] != 0 && boxesMatrix[position.y, position.x] == null)
        {
            boxesMatrix[position.y, position.x] = box;
            box.MoveTo(position);
            UpdateTileVisual(position.x, position.y);
            return true;
        }
        return false;
    }

    public Box RemoveBox(Vector2Int position)
    {
        Box box = boxesMatrix[position.y, position.x];
        if (box != null)
        {
            boxesMatrix[position.y, position.x] = null;
            Destroy(box.gameObject); // Destrói o GameObject da caixa
            UpdateTileVisual(position.x, position.y);
        }
        return box;
    }

    public bool HasBoxAt(Vector2Int position)
    {
        return boxesMatrix[position.y, position.x] != null;
    }

    public bool CanPlaceBoxAt(Vector2Int position)
{
    return position.y >= 0 && position.y < mapMatrix.GetLength(0) &&
           position.x >= 0 && position.x < mapMatrix.GetLength(1) &&
           mapMatrix[position.y, position.x] != 0 &&
           boxesMatrix[position.y, position.x] == null;
}
}