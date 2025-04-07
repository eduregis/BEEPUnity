using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPhase", menuName = "MyPhases/Phase Data")]
public class PhaseData : ScriptableObject
{
    [Header("Map Configuration")]
    public int mapWidth = 5;
    public int mapHeight = 2;
    public List<int> mapData = new List<int>();

    [Header("Initial Positions")]
    public Vector2Int robotInitialPosition;
    public List<Vector2Int> boxesInitialPositions = new List<Vector2Int>();
    public int availableCommands = 9;

    // Método para validar e ajustar os dados
    public void ValidateData()
    {
        // Garante que temos dados suficientes para a matriz
        int requiredSize = mapWidth * mapHeight;
        while (mapData.Count < requiredSize)
        {
            mapData.Add(0); // Preenche com 0 (espaço vazio) se faltar dados
        }
        
        // Remove dados excedentes
        while (mapData.Count > requiredSize)
        {
            mapData.RemoveAt(mapData.Count - 1);
        }
    }

    // Método para converter a lista em matriz 2D
    public int[,] GetMapMatrix()
    {
        ValidateData(); // Garante que os dados estão corretos
        
        int[,] matrix = new int[mapHeight, mapWidth];
        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int index = y * mapWidth + x;
                matrix[y, x] = mapData[index];
            }
        }
        
        return matrix;
    }

    // Método para visualização no Inspector (opcional)
    public void OnValidate()
    {
        ValidateData();
    }
}