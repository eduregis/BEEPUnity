using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public InventoryGrid playerGrid, function1Grid, function2Grid, conditionalIfGrid, conditionalElseGrid;

    void Start()
    {
        // Define a matriz do mapa
        int[,] mapMatrix = new int[,]
        {
            {1, 1, 1, 1, 0},
            {1, 1, 0, 1, 1},
        };

        // Define a posição inicial do robô
        Vector2Int initialPosition = new Vector2Int(0, 0);

        // Configura o mapa e a posição inicial do robô
        IsometricMapGenerator.Instance.SetMapMatrix(mapMatrix);
        RobotController.Instance.SetInitialPosition(initialPosition);
        RobotController.Instance.OnStepCompleted += HandleStepCompleted;
    }

    private IEnumerator ExecuteCommands() 
    {
        // Exemplo de sequência de comandos
        List<string> commands = playerGrid.GetCommandList();
        
        yield return new WaitForSeconds(1f);
        RobotController.Instance.ExecuteCommands(commands);
    }

    public void OnPlayerPressed()
    {
        StartCoroutine(ExecuteCommands());
    }

    private void HandleStepCompleted(string step)
    {
        Debug.Log("Passo concluído: " + step);
    }

    void OnDestroy()
    {
        RobotController.Instance.OnStepCompleted -= HandleStepCompleted;
    }
}