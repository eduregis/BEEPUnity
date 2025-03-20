using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Start()
    {
        // Define a matriz do mapa
        int[,] mapMatrix = new int[,]
        {
            {1, 1, 1, 1},
            {1, 1, 0, 1},
        };

        // Define a posição inicial do robô
        Vector2Int initialPosition = new Vector2Int(0, 0);

        // Configura o mapa e a posição inicial do robô
        IsometricMapGenerator.Instance.SetMapMatrix(mapMatrix);
        RobotController.Instance.SetInitialPosition(initialPosition);

        StartCoroutine(ExecuteCommands());
    }

    private IEnumerator ExecuteCommands() 
    {
        // Exemplo de sequência de comandos
        List<string> commands = new List<string> { "Move", "Move", "TurnRight", "Move" };

        yield return new WaitForSeconds(1f);
        RobotController.Instance.ExecuteCommands(commands);
    }
}