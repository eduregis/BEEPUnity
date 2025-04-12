using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public void GoToSettings()
    {
        CanvasFadeController.Instance.ShowCanvas(Constants.MenuType.Settings);
        AudioManager.Instance.Play("defaultButton");
    }

}
