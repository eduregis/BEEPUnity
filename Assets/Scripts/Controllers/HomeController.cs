using UnityEngine;

public class HomeController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!AudioManager.Instance.IsPlaying("ost1"))
        {
            AudioManager.Instance.Play("ost1");
        }
    }
}
