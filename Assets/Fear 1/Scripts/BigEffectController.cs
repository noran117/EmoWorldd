using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BigEffectController : MonoBehaviour
{
    public static BigEffectController Instance;

    public int ghostCounter = 0;
    public int totalGhosts = 12;

    [Header("Skybox Settings")]
    public Material skyboxMaterial;
    public Texture2D initialPanorama;
    public Texture2D finalPanorama;
    public float initialExposure = 0.5f;
    public float exposurePerGhost = 0.05f;
    public float exposureTransitionDuration = 1.5f;

    [Header("Final Skybox Transition")]
    public float finalTransitionDuration = 4f;
    public float finalExposure = 1f;

    [Header("Lighting Settings")]
    public Light sceneLight;

    [Header("Final Effects")]
    public ParticleSystem finalParticles;
    public AudioSource finalMusic;

    void Awake()
    {
        Instance = this;
        skyboxMaterial.SetFloat("_Exposure", initialExposure);

        if (sceneLight != null)
            sceneLight.intensity = 0f;

        skyboxMaterial.SetTexture("_Tex1", initialPanorama);
        skyboxMaterial.SetTexture("_Tex2", finalPanorama);
        skyboxMaterial.SetFloat("_Blend", 0f);
        skyboxMaterial.SetFloat("_Exposure", initialExposure);
        RenderSettings.skybox = skyboxMaterial;
    }

    public void GhostDestroyed()
    {
        ghostCounter++;

        //float exposure = skyboxMaterial.GetFloat("_Exposure");
        //skyboxMaterial.SetFloat("_Exposure", exposure + exposurePerGhost);

        // DynamicGI.UpdateEnvironment();
        float currentExposure = skyboxMaterial.GetFloat("_Exposure");
        float targetExposure = currentExposure + exposurePerGhost;
        StartCoroutine(SmoothExposureTransition(currentExposure, targetExposure));

        /*if (ghostCounter >= 3)
        {
            PlayFinalEffect();
            StartCoroutine(FinalSkyboxTransition());
        }*/
        // انتهت الأشباح
        if (ghostCounter >= totalGhosts)
        {
            PlayFinalEffect();
            StartCoroutine(FinalSkyboxTransition());
            StartCoroutine(LoadSceneAfterDelay());
        }
    }
    IEnumerator FinalSkyboxTransition()
    {
        float elapsed = 0f;
        float startBlend = skyboxMaterial.GetFloat("_Blend");
        float startExposure = skyboxMaterial.GetFloat("_Exposure");

        while (elapsed < finalTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / finalTransitionDuration));

            skyboxMaterial.SetFloat("_Blend", Mathf.Lerp(startBlend, 1f, smoothT));
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(startExposure, finalExposure, smoothT));

            DynamicGI.UpdateEnvironment();
            yield return null;
        }
        skyboxMaterial.SetFloat("_Blend", 1f);
        skyboxMaterial.SetFloat("_Exposure", finalExposure);
        DynamicGI.UpdateEnvironment();
    }
    IEnumerator SmoothExposureTransition(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < exposureTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / exposureTransitionDuration);

            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(from, to, smoothT));

            DynamicGI.UpdateEnvironment();
            yield return null;
        }

        skyboxMaterial.SetFloat("_Exposure", to);
        DynamicGI.UpdateEnvironment();
    }

    void PlayFinalEffect()
    {
        if (finalParticles != null)
            finalParticles.Play();

        if (finalMusic != null)
            finalMusic.Play();

        if (sceneLight != null)
            sceneLight.intensity = 0.5f;

    }
    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(30f);
        SceneManager.LoadScene("Main_Scene");
    }
}