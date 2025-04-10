using UnityEngine;

public class Panettone : MonoBehaviour
{
    public void SwitchTheme()
    {
        ThemeManager.Instance.ToggleTheme();
    }
}
