using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackButtonController : MonoBehaviour
{
    // Método chamado quando o botão é clicado
    public string sceneName;
    public void ReturnToLevelSelection()
    {
        GetComponent<Button>().interactable = false;
        SceneManager.LoadScene(sceneName);
    }
}