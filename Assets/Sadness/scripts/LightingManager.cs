using UnityEngine;
using System.Collections;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance;

    [Header("Directional Light")]
    public Light directionalLight;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    Coroutine fadeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (directionalLight == null)
        {
            directionalLight = RenderSettings.sun;
        }
    }

    // ✅ لون + شدة
    public void SetLighting(Color targetColor, float targetIntensity)
    {
        if (directionalLight == null)
        {
            Debug.LogError("Directional Light is not assigned!");
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeLight(targetColor, targetIntensity));
    }

    // ✅ (اختياري) شدة فقط (يبقي اللون الحالي)
    public void SetIntensity(float targetIntensity)
    {
        if (directionalLight == null)
        {
            Debug.LogError("Directional Light is not assigned!");
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeLight(directionalLight.color, targetIntensity));
    }

    IEnumerator FadeLight(Color targetColor, float targetIntensity)
    {
        Color startColor = directionalLight.color;
        float startIntensity = directionalLight.intensity;

        float time = 0f;

        float dur = fadeDuration;
        if (dur <= 0.01f)
        {
            directionalLight.color = targetColor;
            directionalLight.intensity = targetIntensity;
            yield break;
        }

        while (time < dur)
        {
            time += Time.deltaTime;
            float t = time / dur;

            directionalLight.color = Color.Lerp(startColor, targetColor, t);
            directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null;
        }

        directionalLight.color = targetColor;
        directionalLight.intensity = targetIntensity;
    }
}