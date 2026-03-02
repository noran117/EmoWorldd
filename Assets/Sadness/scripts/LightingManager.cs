using UnityEngine;
using System.Collections;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance;

    [Header("Main Light")]
    public Light directionalLight;

    [Header("Play")]
    public Color playColor = new Color(1f, 0.95f, 0.85f);
    public float playIntensity = 1.2f;

    [Header("Shock")]
    public Color shockColor = new Color(1f, 1f, 1f);
    public float shockIntensity = 2.5f;

    [Header("Denial")]
    public Color denialColor = new Color(0.7f, 0.75f, 0.85f);
    public float denialIntensity = 0.8f;

    [Header("Anger")]
    public Color angerColor = new Color(1f, 0.3f, 0.3f);
    public float angerIntensity = 1.8f;

    [Header("Bargaining")]
    public Color bargainingColor = new Color(0.5f, 0.5f, 0.7f);
    public float bargainingIntensity = 1.0f;

    [Header("Depression")]
    public Color depressionColor = new Color(0.5f, 0.55f, 0.6f);
    public float depressionIntensity = 0.4f;

    [Header("Acceptance")]
    public Color acceptanceColor = new Color(1f, 0.9f, 0.75f);
    public float acceptanceIntensity = 1.3f;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    private void Awake()
    {
        Instance = this;
        Debug.Log("LightingManager Awake ✔");
    }

    public void SetLighting(Color color, float intensity)
    {
        Debug.Log("SetLighting CALLED -> Target Intensity: " + intensity);

        StopAllCoroutines();
        StartCoroutine(FadeLight(color, intensity, fadeDuration));
    }

    IEnumerator FadeLight(Color targetColor, float targetIntensity, float duration)
    {
        Debug.Log("FadeLight STARTED");

        float time = 0f;
        Color startColor = directionalLight.color;
        float startIntensity = directionalLight.intensity;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            directionalLight.color = Color.Lerp(startColor, targetColor, t);
            directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }

        directionalLight.color = targetColor;
        directionalLight.intensity = targetIntensity;

        Debug.Log("FadeLight FINISHED ✔");
    }

    public void PlayPhase()
    {
        Debug.Log("Lighting Phase -> PLAY");
        SetLighting(playColor, playIntensity);
    }

    public void ShockPhase()
    {
        Debug.Log("Lighting Phase -> SHOCK");
        SetLighting(shockColor, shockIntensity);
    }

    public void DenialPhase()
    {
        Debug.Log("Lighting Phase -> DENIAL");
        SetLighting(denialColor, denialIntensity);
    }

    public void AngerPhase()
    {
        Debug.Log("Lighting Phase -> ANGER");
        SetLighting(angerColor, angerIntensity);
    }

    public void BargainingPhase()
    {
        Debug.Log("Lighting Phase -> BARGAINING");
        SetLighting(bargainingColor, bargainingIntensity);
    }

    public void DepressionPhase()
    {
        Debug.Log("Lighting Phase -> DEPRESSION");
        SetLighting(depressionColor, depressionIntensity);
    }

    public void AcceptancePhase()
    {
        Debug.Log("Lighting Phase -> ACCEPTANCE");
        SetLighting(acceptanceColor, acceptanceIntensity);
    }
}