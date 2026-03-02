using UnityEngine;

public class CloudUIController : MonoBehaviour
{
    public GameObject mainCloud;
    public GameObject settingsCloud;

    public void OpenSettings()
    {
        mainCloud.SetActive(false);
        settingsCloud.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsCloud.SetActive(false);
        mainCloud.SetActive(true);
    }
}
