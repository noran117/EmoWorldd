using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [Header("Skybox Settings")]
    public float defaultExposure = 1.0f;

    [Header("Audio Settings")]
    public AudioSource bgMusicSource; 

    void Start()
    {
        
        float savedExposure = PlayerPrefs.GetFloat("SkyboxExposure", defaultExposure);
        RenderSettings.skybox.SetFloat("_Exposure", savedExposure);

       
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        if (bgMusicSource != null) bgMusicSource.volume = savedVolume;
    }

    public void SetSkyboxBrightness(float value)
    {
        RenderSettings.skybox.SetFloat("_Exposure", value);
        PlayerPrefs.SetFloat("SkyboxExposure", value);
    }

    
    public void SetMusicVolume(float value)
    {
        if (bgMusicSource != null)
        {
            bgMusicSource.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value); 
        }
    }
}