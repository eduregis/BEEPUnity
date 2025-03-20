using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
     public RobotController robotController;

    void Start()
    {
         // Exemplo de sequência de comandos
        List<string> commands = new List<string> { "Move", "Move", "TurnRight", "Move" };

        // Acessa a instância única do RobotController
        RobotController robotController = RobotController.Instance;

        // Executa a sequência de comandos no robô
        if (robotController != null)
        {
            robotController.ExecuteCommands(commands);
        }
        else
        {
            Debug.LogError("RobotController não encontrado!");
        }
    }
}
