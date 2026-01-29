using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject audioSettingsPanel; 

    public void OpenSettings()
    {
        audioSettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        audioSettingsPanel.SetActive(false);
    }
}
