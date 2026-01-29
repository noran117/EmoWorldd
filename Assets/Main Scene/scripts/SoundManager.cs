using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    //How we can call it 
    // SoundManager.Instance.PlaySFX(jumpSound);   // when the player jumps 

    // SoundManager.Instance.PlayUISound(clickSound); // when click ------ etc...





    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("UI Elements")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;
    public Toggle muteToggle;

    private const string MUSIC_VOL = "MusicVolume";
    private const string SFX_VOL = "SFXVolume";
    private const string UI_VOL = "UIVolume";
    private const string MUTE_STATE = "Muted";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        LoadAudioSettings();
    }

    private void Start()
    {
        if (musicSlider) 
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider) 
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if (uiSlider) 
            uiSlider.onValueChanged.AddListener(SetUIVolume);
        if (muteToggle) 
            muteToggle.onValueChanged.AddListener(ToggleMute);
    }

    // Music Volume Control
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOL, volume);
    }

    // Sound Effects Volume Control
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOL, volume);
    }

    // UI Sounds Volume Control
    public void SetUIVolume(float volume)
    {
        if (uiSource) uiSource.volume = volume;
        PlayerPrefs.SetFloat(UI_VOL, volume);
    }

    // Mute / Unmute All Sounds
    public void ToggleMute(bool isMuted)
    {
        float volume = isMuted ? 0f : 1f;
        musicSource.volume = isMuted ? 0f : PlayerPrefs.GetFloat(MUSIC_VOL, 0.75f);
        sfxSource.volume = isMuted ? 0f : PlayerPrefs.GetFloat(SFX_VOL, 0.75f);
        if (uiSource) uiSource.volume = isMuted ? 0f : PlayerPrefs.GetFloat(UI_VOL, 0.75f);

        PlayerPrefs.SetInt(MUTE_STATE, isMuted ? 1 : 0);
    }

    // Play Sound Effects
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // Play UI Sound
    public void PlayUISound(AudioClip clip)
    {
        if (clip != null && uiSource)
            uiSource.PlayOneShot(clip);
    }

    // Load Saved Settings
    private void LoadAudioSettings()
    {
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOL, 0.75f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOL, 0.75f);
        float uiVol = PlayerPrefs.GetFloat(UI_VOL, 0.75f);
        bool isMuted = PlayerPrefs.GetInt(MUTE_STATE, 0) == 1;

        if (musicSlider) musicSlider.value = musicVol;
        if (sfxSlider) sfxSlider.value = sfxVol;
        if (uiSlider) uiSlider.value = uiVol;
        if (muteToggle) muteToggle.isOn = isMuted;

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
        SetUIVolume(uiVol);
        ToggleMute(isMuted);
    }
}
