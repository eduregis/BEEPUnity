using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsSliderHandle : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.Instance.Play("grabBlock");
    }
}