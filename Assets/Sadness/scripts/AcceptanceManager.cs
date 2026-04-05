using UnityEngine;
using System.Collections;

public class AcceptanceManager : MonoBehaviour
{
    public static AcceptanceManager Instance;

    [Header("Outside Hint")]
    public GameObject outsideYellowLight;
    public AudioSource outsideCallSfx;
    public float outsideHintDelay = 0.5f;

    [Header("Acceptance Start Particles")]
    public ParticleSystem acceptanceStartParticles;

    [Header("Book")]
    public Cardsanimate cardsAnimate;
    public Animator bookAnimator;
    public string openTriggerName = "Open";
    public bool useCardsAnimate = true;

    bool started;
    bool acceptanceActive = false;

    bool presentationFinished = false;
    bool bookFinished = false;


    public followPlayer companion;
   // public GameObject message;
   // public Transform player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void StopAcceptanceStartParticles()
    {
        if (acceptanceStartParticles != null)
        {
            acceptanceStartParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            acceptanceStartParticles.gameObject.SetActive(false);
        }
    }

    void PlayAcceptanceStartParticles()
    {
        if (acceptanceStartParticles != null)
        {
            acceptanceStartParticles.gameObject.SetActive(true);
            acceptanceStartParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            acceptanceStartParticles.Play();
        }
    }

    public void StartAcceptance()
    {
        started = false;
        acceptanceActive = true;

        presentationFinished = false;
        bookFinished = false;


        PlayAcceptanceStartParticles();

        if (cardsAnimate != null)
        {
            cardsAnimate.PrepareForAcceptance();
        }

        StartCoroutine(OutsideHintRoutine());
    }

    IEnumerator OutsideHintRoutine()
    {
        yield return new WaitForSeconds(outsideHintDelay);

        if (outsideYellowLight != null)
            outsideYellowLight.SetActive(true);

        if (outsideCallSfx != null)
        {
            outsideCallSfx.loop = false;
            outsideCallSfx.Play();
        }
    }

    public void OnBookTouched()
    {

        if (!acceptanceActive) return;
        if (started) return;

        started = true;

         if (companion != null)
        {
            companion.PlayAcceptanceMoment();
        }

        if (outsideYellowLight != null)
            outsideYellowLight.SetActive(false);

        StopAcceptanceStartParticles();

        if (cardsAnimate != null)
        {
            cardsAnimate.StartAcceptanceParticles();
            cardsAnimate.onFinished -= OnBookSequenceFinished;
            cardsAnimate.onFinished += OnBookSequenceFinished;
        }

        if (useCardsAnimate && cardsAnimate != null)
        {
            cardsAnimate.StartMemories();
        }
        else
        {
            if (bookAnimator != null)
            {
                bookAnimator.SetTrigger(openTriggerName);
                bookFinished = true;
                TryEndAcceptance();
            }
        }
    }

    void OnBookSequenceFinished()
    {
        bookFinished = true;
        TryEndAcceptance();
    }

    public void NotifyPresentationFinished()
    {
        presentationFinished = true;
        TryEndAcceptance();
    }

    void TryEndAcceptance()
    {
        if (!presentationFinished) return;
        if (!bookFinished) return;

        EndAcceptance();
    }

    void EndAcceptance()
    {
        acceptanceActive = false;
        started = false;

        StopAcceptanceStartParticles();

        if (cardsAnimate != null)
            cardsAnimate.CleanupAfterAcceptance();

        GameStateManager.Instance.ChangeState(GameState.Ending);
    }
}