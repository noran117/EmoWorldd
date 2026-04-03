using UnityEngine;
using UnityEngine.UI;

public class VRSettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource musicSource;
    public Slider audioSlider;
    public float defaultVolume = 0.5f;

    [Header("Lighting Settings")]
    public Slider lightSlider;
    public float defaultLight = 1.0f;

    void Start()
    {
        // 1. ÅÚÏÇÏ ÇáÕæÊ
        float savedVolume = PlayerPrefs.GetFloat("MusicVol", defaultVolume);
        audioSlider.value = savedVolume;
        musicSource.volume = savedVolume;
        audioSlider.onValueChanged.AddListener(UpdateVolume);

        // 2. ÅÚÏÇÏ ÇáÅÖÇÁÉ (Skybox)
        float savedLight = PlayerPrefs.GetFloat("SkyLight", defaultLight);
        lightSlider.value = savedLight;
        RenderSettings.skybox.SetFloat("_Exposure", savedLight); // ÇáÊÍßã İí ÓØæÚ ÇáÓãÇÁ
        lightSlider.onValueChanged.AddListener(UpdateLight);
    }

    void UpdateVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVol", value); // ÍİÙ ÇáŞíãÉ
    }

    void UpdateLight(float value)
    {
        // äÚÏá ŞíãÉ ÇáÜ Exposure İí ÇáÜ Skybox
        RenderSettings.skybox.SetFloat("_Exposure", value);
        PlayerPrefs.SetFloat("SkyLight", value); // ÍİÙ ÇáŞíãÉ

        // ÊÍÏíË ÇáÅÖÇÁÉ İí ÇáãÔåÏ áÊÚßÓ ÊÛííÑ ÇáÓãÇÁ İæÑÇğ
        DynamicGI.UpdateEnvironment();
    }
}