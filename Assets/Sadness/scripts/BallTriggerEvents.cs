using UnityEngine;
using System.Collections;

public class BallTriggerEvents : MonoBehaviour
{
    [Header("Object to change Material")]
    public GameObject targetObject;
    public Material newMaterial;

    [Header("Skybox")]
    public Material oldSkybox;
    public Material newSkybox;
    public float skyboxDuration = 5f;

    [Header("Particles")]
    public ParticleSystem particleToStop;     
    public ParticleSystem particleToPlay;    
    public float stopParticleDuration = 2f;     
    public float playParticleDuration = 3f;  

    [Header("Plane Object")]
    public GameObject planeObject;          
    [Header("Animation")]
    public Animator targetAnimator;
    public string animationTrigger = "Play";

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private bool triggered = false;

    private void Start()
    {
        if (planeObject != null) planeObject.SetActive(false);

        if (particleToStop != null) particleToStop.Stop();
        if (particleToPlay != null) particleToPlay.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.CompareTag("Ball"))   
        {
            triggered = true;
            StartCoroutine(TriggerSequence());
        }
    }

    IEnumerator TriggerSequence()
    {
        if (targetObject != null && newMaterial != null)
        {
            MeshRenderer mesh = targetObject.GetComponent<MeshRenderer>();
            if (mesh != null)
                mesh.material = newMaterial;
        }

        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;
            DynamicGI.UpdateEnvironment();
        }

        if (particleToStop != null) particleToStop.Play();
        if (particleToPlay != null) particleToPlay.Play();

        yield return new WaitForSeconds(stopParticleDuration);
        if (particleToStop != null) particleToStop.Stop();

        yield return new WaitForSeconds(playParticleDuration - stopParticleDuration);
        if (particleToPlay != null) particleToPlay.Stop();

        if (planeObject != null) planeObject.SetActive(true);

        if (targetAnimator != null && !string.IsNullOrEmpty(animationTrigger))
            targetAnimator.SetTrigger(animationTrigger);

        if (backgroundMusic != null)
            backgroundMusic.Stop();

        yield return new WaitForSeconds(skyboxDuration);
        if (oldSkybox != null)
        {
            RenderSettings.skybox = oldSkybox;
            DynamicGI.UpdateEnvironment();
        }
    }
}
