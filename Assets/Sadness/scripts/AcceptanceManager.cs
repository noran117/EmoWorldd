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

        presentationFinished = false;
        bookFinished = false;

        SetObjectsActive(disableOnStart, false);
        SetObjectsActive(enableOnStart, true);

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

        if (started) return;
        started = true;

        if (outsideYellowLight != null)
            outsideYellowLight.SetActive(false);

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
        SetObjectsActive(enableOnStart, false);
        SetObjectsActive(disableOnStart, true);

        GameStateManager.Instance.ChangeState(GameState.Ending);
    }
}