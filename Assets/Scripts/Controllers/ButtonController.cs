using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour
{
    public enum ActionType { LoadScene, CustomAction }

    [Header("Configuration")]
    public ActionType actionType = ActionType.LoadScene;
    public string sceneName;
    public UnityEvent customAction;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        GetComponent<Button>().interactable = false;
        AudioManager.Instance.Play("defaultButton");

        switch (actionType)
        {
            case ActionType.LoadScene:
                if (!string.IsNullOrEmpty(sceneName))
                {
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    Debug.LogError("Scene name not set!");
                }
                break;

            case ActionType.CustomAction:
                customAction.Invoke();
                break;
        }
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveListener(OnButtonClicked);
    }
}