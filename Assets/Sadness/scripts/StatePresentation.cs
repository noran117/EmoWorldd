using UnityEngine;

[System.Serializable]
public class StatePresentation
{
    public AudioSource voiceOver;
    public float voiceDelay = 0f;

    public AudioSource music;

    public Color lightColor = Color.white;
    public float lightIntensity = 1f;

    public Texture slideTexture;
    public float slideDelay = 0f;
    public float slideDistance = 2.0f;
    public Vector3 slideOffset = new Vector3(0f, -0.15f, 0f);
    public float slideDurationFallback = 5f;
    public float slideStayAfter = 5f;

    public float musicFadeIn = 1.0f;
    public float musicFadeOut = 1.2f;

    public float duckIn = 0.2f;
    public float duckOut = 0.4f;

    [Header("Extras (Particles / SFX)")]
    public ParticleSystem[] playParticles;   
    public ParticleSystem[] stopParticles;   
    public AudioSource[] loopSfx;           

    public float loopSfxFadeIn = 0.8f;
    public float loopSfxFadeOut = 0.8f;
}