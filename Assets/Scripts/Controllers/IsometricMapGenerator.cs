using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IsometricMapGenerator : MonoBehaviour
{
    // Instância única do IsometricMapGenerator
    public static IsometricMapGenerator Instance { get; private set; }

    public GameObject tilePrefab; // Prefab do tile isométrico
    public int[,] mapMatrix; // Matriz do mapa (será definida pelo PlayerController)
    public float tileWidth = 64f; // Largura do tile
    public float tileHeight = 48f; // Altura do tile
    private float createAndDestroySpeed = 0.5f;

    public GameObject boxPrefab; // Prefab da caixa
    public GameObject infectedDataPrefab; // Prefab do dado infectado
    public Box[,] boxesMatrix; // Matriz para rastrear caixas
    public List<InfectedData> infectedDatas; // Vetor para armazenar Dados Infectados
    private List<Tile> tiles;

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
                    matrix[position.y, position.x] != (int)Constants.TileType.Empty) // Só coloca em tiles válidos
                {
                    StartCoroutine(CreateBoxAtPosition(position));
                }
            }
        }
    }

    void GenerateMap()
    {
        // Limpa o mapa anterior (se houver)
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RobotController>() == null) 
            {
                Destroy(child.gameObject);
            }
        }

        // Obtém a posição do GameObject que contém o script
        Vector3 originPosition = transform.position;

        // Calcula o deslocamento para centralizar o map
        int mapWidth = mapMatrix.GetLength(1);
        int mapHeight = mapMatrix.GetLength(0);
        Vector3 offset = Utils.CalculateOffset();
        
        tiles = new List<Tile>();

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (mapMatrix[y, x] != (int)Constants.TileType.Empty)
                {
                    // Calcula a posição isométrica relativa ao centro
                    Vector3 tilePosition = Utils.CalculateTilePosition(x, y);

                    // Ajusta a posição para considerar o centro do mapa
                    tilePosition += originPosition - offset;

                    // Instancia o tile na posição calculada
                    GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                    Tile tileScript = tile.GetComponent<Tile>();
                    tiles.Add(tileScript);
                    tileScript.Initialize(mapMatrix, x, y, mapMatrix[y, x] == (int)Constants.TileType.Fitting);
                }
            }
        }

        infectedDatas = new List<InfectedData>();

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Calcula a posição isométrica relativa ao centro
                Vector3 tilePosition = Utils.CalculateTilePosition(x, y);

                // Ajusta a posição para considerar o centro do mapa
                tilePosition += originPosition - offset;

                // Instancia o dado infectado na posição calculada, se existir
                if (mapMatrix[y, x] == (int)Constants.TileType.InfectedData)
                {
                    GameObject infectedData = Instantiate(infectedDataPrefab, tilePosition, Quaternion.identity, transform);
                    InfectedData infectedDataScript = infectedData.GetComponent<InfectedData>();
                    infectedDataScript.Initialize(new Vector2Int(x, y));
                    infectedDatas.Add(infectedDataScript);
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
            tile.fittingBox.gameObject.SetActive(mapMatrix[y, x] == (int)Constants.TileType.Fitting);
        }

        // Atualiza visual da caixa (se houver)
        if (boxesMatrix[y, x] != null)
        {
            boxesMatrix[y, x].isInFittingSpot = mapMatrix[y, x] == (int)Constants.TileType.Fitting;
            // Aqui você pode atualizar o visual da caixa (ex: mudar cor se estiver no encaixe)
        }
    }

    public void ConcludedPhase()
    {
        foreach (Tile tile in tiles)
        {
            tile.concluded = true;
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

    public IEnumerator CreateBoxAtPosition(Vector2Int position)
    {
        yield return new WaitForSeconds(createAndDestroySpeed);
        if (CanPlaceBoxAt(position))
        {
            GameObject boxObj = Instantiate(boxPrefab, transform);
            Box box = boxObj.GetComponent<Box>();
            box.Initialize(position);
            boxesMatrix[position.y, position.x] = box;
            
            // Posiciona corretamente
            RectTransform boxRect = boxObj.GetComponent<RectTransform>();
            RectTransform tileRect = GetTileRect(position);
            if (tileRect != null)
            {
                boxRect.anchoredPosition = tileRect.anchoredPosition;
                boxRect.SetAsLastSibling();
            }
            UpdateTileVisual(position.x, position.y);
        }
    }

    public Box RemoveBox(Vector2Int position)
    {
        Box box = boxesMatrix[position.y, position.x];
        if (box != null)
        {
            boxesMatrix[position.y, position.x] = null;
            StartCoroutine(DestroyBox(box));
            UpdateTileVisual(position.x, position.y);
        }
        return box;
    }

    public IEnumerator DestroyBox(Box box)
    {
        yield return new WaitForSeconds(createAndDestroySpeed);
        Destroy(box.gameObject);
    }

    public bool HasBoxAt(Vector2Int position)
    {
        return boxesMatrix[position.y, position.x] != null;
    }

    public bool HasInfectedDataAt(Vector2Int position)
    {
        return mapMatrix[position.y, position.x] == (int)Constants.TileType.InfectedData;
    }
    

    public bool CanPlaceBoxAt(Vector2Int position)
    {
        return position.y >= 0 && position.y < mapMatrix.GetLength(0) &&
            position.x >= 0 && position.x < mapMatrix.GetLength(1) &&
            mapMatrix[position.y, position.x] != (int)Constants.TileType.Empty &&
            mapMatrix[position.y, position.x] != (int)Constants.TileType.InfectedData &&
            boxesMatrix[position.y, position.x] == null;
    }

    public RectTransform GetTileRect(Vector2Int position)
    {
        foreach (Transform child in transform)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile != null && tile.x == position.x && tile.y == position.y)
            {
                return child.GetComponent<RectTransform>();
            }
        }
        return null;
    }

    public bool IsValidPosition(Vector2Int position)
    {
        return position.y >= 0 && position.y < mapMatrix.GetLength(0) &&
            position.x >= 0 && position.x < mapMatrix.GetLength(1) &&
            mapMatrix[position.y, position.x] != (int)Constants.TileType.Empty;
    }

    public void RecoveringData(Vector2Int position)
    {
        foreach (InfectedData infectedData in infectedDatas)
        {
            if (position == infectedData.gridPosition && infectedData.isInfected)
            {
                infectedData.RecoveringData();
            }
        }
    }

    public bool CheckRecoveredData()
    {
        foreach (InfectedData infectedData in infectedDatas)
        {
            if (infectedData.isInfected)
                return false;
        }
        return true;
    }
}