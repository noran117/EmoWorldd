using UnityEngine;
using System.Collections;
using System;

public class Cardsanimate : MonoBehaviour
{
    public GameObject memoriesLight;

    [Header("Book")]
    public Animator bookAnimator;
    public ParticleSystem bookClosedEffect;
    public float closedGlowTime = 2f;
    public float delayBeforeOpen = 3f;

    [Header("Memories")]
    public Animator mem1;
    public Animator mem2;
    public Animator mem3;
    public Animator mem4;

    public float delay1 = 0.5f;
    public float delay2 = 0.8f;
    public float delay3 = 0.8f;
    public float delay4 = 0.8f;

    public ParticleSystem bookSparks;

    [Header("Finish")]
    public float finishDelayAfterLastMemory = 2.0f; 

    public event Action onFinished;

    private bool started = false;
    private Coroutine memCo;

    void Start()
    {
        if (bookClosedEffect != null)
            bookClosedEffect.Play();

        if (bookSparks != null)
            bookSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (memoriesLight != null)
            memoriesLight.SetActive(false);
    }

    public void StartMemories()
    {
        if (started) return;
        started = true;

        if (memCo != null) StopCoroutine(memCo);
        memCo = StartCoroutine(OpenBookThenMemories());
    }

    IEnumerator OpenBookThenMemories()
    {
        yield return new WaitForSeconds(closedGlowTime);
        yield return new WaitForSeconds(delayBeforeOpen);

        if (bookClosedEffect != null)
        {
            bookClosedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (bookSparks != null) bookSparks.Play();
        }

        if (bookAnimator != null)
            bookAnimator.SetTrigger("Open");

        yield return new WaitForSeconds(0.5f);

        if (memoriesLight != null)
            memoriesLight.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (mem1 != null) { yield return new WaitForSeconds(delay1); mem1.SetTrigger("Play1"); }
        if (mem2 != null) { yield return new WaitForSeconds(delay2); mem2.SetTrigger("Play2"); }
        if (mem3 != null) { yield return new WaitForSeconds(delay3); mem3.SetTrigger("Play3"); }
        if (mem4 != null) { yield return new WaitForSeconds(delay4); mem4.SetTrigger("Play4"); }

        yield return new WaitForSeconds(finishDelayAfterLastMemory);

        onFinished?.Invoke();
    }
}