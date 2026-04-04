using UnityEngine;
using System.Collections;

public class ShockTrigger : MonoBehaviour
{
    public charactermovement brotherMovement;

    public Animator targetAnimator;
    public string animationTrigger = "LightFall";

    public GameObject targetObject;
    public Material newMaterial;

    public Material shockSkybox;

    public ParticleSystem particleToPlay;
    public ParticleSystem particleToStop;

    public AudioSource electricSound;

    public GameObject exposedCable;
    public GameObject planeObject;

    [Header("Stones to stop glow on shock")]
    public Stone[] stonesToStopGlow;

    bool triggered;
    Material originalSkybox;


    public followPlayer companion;//
    void Awake()
    {
        originalSkybox = RenderSettings.skybox;
    }

    void Start()
    {
        if (exposedCable != null) exposedCable.SetActive(false);
        StopPS(particleToPlay);
        StopPS(particleToStop);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.GetComponentInParent<charactermovement>() == null) return;
        if (GameStateManager.Instance.currentState != GameState.Play) return;

        GameStateManager.Instance.ChangeState(GameState.Shock);
    }

    public void StartShock()
    {
        if (triggered) return;
        triggered = true;
        StartCoroutine(ShockSequence());
    }

    IEnumerator ShockSequence()
    {
        StopAllStoneGlow();

        if (companion != null)
            companion.ReactToShock();

        electricSound?.Play();

        StopPS(particleToStop);
        PlayPS(particleToPlay);

        if (planeObject != null)
        {
            planeObject.SetActive(true);
            yield return new WaitForSeconds(0.12f);
            planeObject.SetActive(false);
        }

        if (exposedCable != null) exposedCable.SetActive(true);

        if (targetAnimator != null) targetAnimator.SetTrigger(animationTrigger);

        if (targetObject != null && newMaterial != null)
        {
            var r = targetObject.GetComponent<MeshRenderer>();
            if (r != null) r.material = newMaterial;
        }

        if (brotherMovement != null) brotherMovement.gameObject.SetActive(false);

        ApplySkybox(shockSkybox);

        yield return new WaitForSeconds(0.8f);

        StatePresentationManager.Instance.bothFinishedCallback = () =>
        {
            ApplySkybox(originalSkybox);
            GameStateManager.Instance.ChangeState(GameState.TransitionalPhase1);
        };

        StatePresentationManager.Instance.PlayState(StatePresentationManager.Instance.shock);

         foreach (Stone stone in stonesToStopGlow)
        {
            if (stone != null)
                stone.gameObject.SetActive(false);
        }
    }

    void StopAllStoneGlow()
    {

        foreach (Stone stone in stonesToStopGlow)
        {
            if (stone != null)
                stone.DisableGlow();
        }
    }

    void StopPS(ParticleSystem ps)
    {
        if (ps == null) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Clear(true);
    }

    void PlayPS(ParticleSystem ps)
    {
        if (ps == null) return;
        ps.gameObject.SetActive(true);
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Clear(true);
        ps.Play(true);
    }

    void ApplySkybox(Material sky)
    {
        if (sky == null) return;
        RenderSettings.skybox = sky;
        DynamicGI.UpdateEnvironment();
    }
}