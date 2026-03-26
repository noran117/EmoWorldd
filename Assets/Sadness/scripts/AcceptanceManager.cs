using UnityEngine;
using System.Collections;

public class AcceptanceManager : MonoBehaviour
{
    public static AcceptanceManager Instance;

    [Header("Visual Objects")]
    public GameObject[] enableOnStart;
    public GameObject[] disableOnStart;

    [Header("Outside Hint")]
    public GameObject outsideYellowLight;
    public AudioSource outsideCallSfx;
    public float outsideHintDelay = 0.5f;

    [Header("Book")]
    public Cardsanimate cardsAnimate;
    public Animator bookAnimator;
    public string openTriggerName = "OpenBook";
    public bool useCardsAnimate = true;

    bool started;
    bool acceptanceActive = false;

    bool presentationFinished = false;
    bool bookFinished = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void SetObjectsActive(GameObject[] arr, bool active)
    {
        if (arr == null) return;
        for (int i = 0; i < arr.Length; i++)
            if (arr[i] != null) arr[i].SetActive(active);
    }

    public void StartAcceptance()
    {
        started = false;
        acceptanceActive = true;

        presentationFinished = false;
        bookFinished = false;

        SetObjectsActive(disableOnStart, false);
        SetObjectsActive(enableOnStart, true);

        if (cardsAnimate != null)
            cardsAnimate.PrepareForAcceptance();

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
        Debug.Log("BOOK TOUCHED CALLED");
        Debug.Log("acceptanceActive = " + acceptanceActive);
        Debug.Log("started = " + started);

        if (!acceptanceActive) return;
        if (started) return;

        started = true;

        if (outsideYellowLight != null)
            outsideYellowLight.SetActive(false);

        if (cardsAnimate != null && cardsAnimate.bookClosedEffect != null)
        {
            cardsAnimate.bookClosedEffect.gameObject.SetActive(true);
            cardsAnimate.bookClosedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            cardsAnimate.bookClosedEffect.Play();
        }

        if (cardsAnimate != null)
        {
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

        if (cardsAnimate != null)
            cardsAnimate.CleanupAfterAcceptance();

        SetObjectsActive(enableOnStart, false);
        SetObjectsActive(disableOnStart, true);

        GameStateManager.Instance.ChangeState(GameState.Ending);
    }
}