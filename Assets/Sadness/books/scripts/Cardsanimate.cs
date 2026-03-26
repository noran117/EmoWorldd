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

    [Header("Spawn Points")]
    public Transform spawn1;
    public Transform spawn2;
    public Transform spawn3;
    public Transform spawn4;

    public ParticleSystem bookSparks;

    [Header("Finish")]
    public float finishDelayAfterLastMemory = 2.0f;

    public event Action onFinished;

    private bool started = false;
    private Coroutine memCo;

    void Start()
    {
        if (memoriesLight != null)
            memoriesLight.SetActive(false);

        if (bookClosedEffect != null)
        {
            bookClosedEffect.gameObject.SetActive(false);
            bookClosedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (bookSparks != null)
        {
            bookSparks.gameObject.SetActive(false);
            bookSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void PrepareForAcceptance()
    {
        started = false;

        if (memoriesLight != null)
            memoriesLight.SetActive(false);

        if (bookClosedEffect != null)
        {
            bookClosedEffect.gameObject.SetActive(false);
            bookClosedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (bookSparks != null)
        {
            bookSparks.gameObject.SetActive(false);
            bookSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    public void CleanupAfterAcceptance()
    {
        started = false;

        if (memoriesLight != null)
            memoriesLight.SetActive(false);

        if (bookClosedEffect != null)
        {
            bookClosedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            bookClosedEffect.gameObject.SetActive(false);
        }

        if (bookSparks != null)
        {
            bookSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            bookSparks.gameObject.SetActive(false);
        }
    }

    public void StartMemories()
    {
        if (started) return;
        started = true;

        if (memCo != null) StopCoroutine(memCo);
        memCo = StartCoroutine(OpenBookThenMemories());
    }

    void MoveToSpawn(Animator mem, Transform spawn)
    {
        if (mem == null || spawn == null) return;

        mem.transform.position = spawn.position;
        mem.transform.rotation = spawn.rotation;
    }

    IEnumerator OpenBookThenMemories()
    {
        yield return new WaitForSeconds(closedGlowTime);
        yield return new WaitForSeconds(delayBeforeOpen);

        if (bookClosedEffect != null)
        {
            bookClosedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            bookClosedEffect.gameObject.SetActive(false);
        }

        if (bookSparks != null)
        {
            bookSparks.gameObject.SetActive(true);
            bookSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            bookSparks.Play();
        }

        if (bookAnimator != null)
            bookAnimator.SetTrigger("Open");

        yield return new WaitForSeconds(0.5f);

        if (memoriesLight != null)
            memoriesLight.SetActive(true);

        yield return new WaitForSeconds(1f);

        MoveToSpawn(mem1, spawn1);
        if (mem1 != null)
        {
            yield return new WaitForSeconds(delay1);
            mem1.SetTrigger("Play1");
        }

        MoveToSpawn(mem2, spawn2);
        if (mem2 != null)
        {
            yield return new WaitForSeconds(delay2);
            mem2.SetTrigger("Play2");
        }

        MoveToSpawn(mem3, spawn3);
        if (mem3 != null)
        {
            yield return new WaitForSeconds(delay3);
            mem3.SetTrigger("Play3");
        }

        MoveToSpawn(mem4, spawn4);
        if (mem4 != null)
        {
            yield return new WaitForSeconds(delay4);
            mem4.SetTrigger("Play4");
        }

        yield return new WaitForSeconds(finishDelayAfterLastMemory);

        onFinished?.Invoke();
    }
}