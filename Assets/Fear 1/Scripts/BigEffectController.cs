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
    public float initialExposure = 0.5f;
    public float exposurePerGhost = 0.05f;
    [Header("Lighting Settings")]
    public Light sceneLight;

    [Header("Final Effects")]
    public ParticleSystem finalParticles;
    public AudioSource finalMusic;

    void Awake()
    {
        Instance = this;
        skyboxMaterial.SetFloat("_Exposure", initialExposure);
    }

    public void GhostDestroyed()
    {
        ghostCounter++;

        // تغيير السماء تدريجياً
        float exposure = skyboxMaterial.GetFloat("_Exposure");
        skyboxMaterial.SetFloat("_Exposure", exposure + exposurePerGhost);

        DynamicGI.UpdateEnvironment();

        // لو انتهت الأشباح
        if (ghostCounter >= totalGhosts)
        {
            PlayFinalEffect();
            StartCoroutine(LoadSceneAfterDelay());
        }
    }

    void PlayFinalEffect()
    {
        if (finalParticles != null)
            finalParticles.Play();

        if (finalMusic != null)
            finalMusic.Play();

        if (sceneLight != null)
            sceneLight.intensity = 1.8f;

    }
    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(30f);
        SceneManager.LoadScene("Main Scene");
    }
}