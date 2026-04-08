using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StatePresentationManager : MonoBehaviour
{
    public static StatePresentationManager Instance;

    [Header("References")]
    public Transform xrCamera;
    public Transform xrRigRoot;

    [Header("Slide")]
    public GameObject slidePrefab;

    [Header("Slide Spawn")]
    public Transform slideSpawnPoint;

    [Header("States")]
    public StatePresentation play;
    public StatePresentation shock;
    public StatePresentation transitionalPhase1;
    public StatePresentation denial;
    public StatePresentation anger;
    public StatePresentation bargaining;
    public StatePresentation depression;
    public StatePresentation acceptance;

    public Action bothFinishedCallback;

    AudioSource currentMusic;
    readonly List<AudioSource> allMusics = new List<AudioSource>();

    GameObject currentSlide;
    SlideController currentSlideCtrl;
    Action slideFinishedHandler;

    Coroutine voiceCo;
    Coroutine slideCo;
    Coroutine finishCo;
    Coroutine destroySlideCo;

    bool voiceFinished;
    bool slideFinished;

    readonly List<AudioSource> currentLoops = new List<AudioSource>();
    readonly List<ParticleSystem> currentParticles = new List<ParticleSystem>();

    int runId = 0;
    bool finishedInvoked = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CacheAllMusics();
        StopAllMusicsImmediate();
        StopExtras();
    }

    void CacheAllMusics()
    {
        allMusics.Clear();
        AddMusic(play);
        AddMusic(shock);
        AddMusic(transitionalPhase1);
        AddMusic(denial);
        AddMusic(anger);
        AddMusic(bargaining);
        AddMusic(depression);
        AddMusic(acceptance);
    }

    void AddMusic(StatePresentation s)
    {
        if (s != null && s.music != null && !allMusics.Contains(s.music))
            allMusics.Add(s.music);
    }

    void StopAllMusicsImmediate()
    {
        for (int i = 0; i < allMusics.Count; i++)
        {
            if (allMusics[i] == null) continue;
            allMusics[i].Stop();
            allMusics[i].volume = 1f;
        }
    }

    public void PlayState(StatePresentation state)
    {
        if (voiceCo != null) StopCoroutine(voiceCo);
        if (slideCo != null) StopCoroutine(slideCo);
        if (finishCo != null) StopCoroutine(finishCo);
        if (destroySlideCo != null) StopCoroutine(destroySlideCo);

        runId++;
        finishedInvoked = false;
        int myRun = runId;

        DestroyCurrentSlide();
        StopExtras();

        voiceFinished = (state == null || state.voiceOver == null);
        slideFinished = (state == null || slidePrefab == null || state.slideTexture == null);

        if (state == null)
        {
            voiceFinished = true;
            slideFinished = true;
            TryFinishBoth(state, myRun);
            return;
        }

        LightingManager.Instance?.SetLighting(state.lightColor, state.lightIntensity);

        SwitchMusic(state);

        PlayExtras(state);

        if (state.voiceOver != null)
            voiceCo = StartCoroutine(VoiceRoutine(state, myRun));

        if (!slideFinished)
            slideCo = StartCoroutine(SlideRoutine(state, myRun));
        else
            TryFinishBoth(state, myRun);

    }

    void SwitchMusic(StatePresentation state)
    {
        for (int i = 0; i < allMusics.Count; i++)
        {
            var m = allMusics[i];
            if (m == null) continue;
            if (m == state.music) continue;

            if (m.isPlaying)
                AudioFader.Instance.FadeOut(m, state.musicFadeOut);
        }

        currentMusic = state.music;

        if (currentMusic != null && !currentMusic.isPlaying)
            AudioFader.Instance.FadeIn(currentMusic, state.musicFadeIn, 1f);
    }

    IEnumerator VoiceRoutine(StatePresentation state, int myRun)
    {
        yield return new WaitForSeconds(state.voiceDelay);

        if (myRun != runId) yield break;

        AudioMixerDucker.Instance?.Duck(state.duckIn);

        state.voiceOver.Play();

        while (state.voiceOver != null && state.voiceOver.isPlaying)
        {
            if (myRun != runId) yield break;
            yield return null;
        }

        if (myRun != runId) yield break;

        AudioMixerDucker.Instance?.Unduck(state.duckOut);

        voiceFinished = true;

        TryFinishBoth(state, myRun);
        if (voiceFinished && GameStateManager.Instance.currentState == GameState.TransitionalPhase1)
        {
            GameStateManager.Instance.ChangeState(GameState.Denial);
        }

        if (voiceFinished && GameStateManager.Instance.currentState == GameState.Bargaining)
        {
            yield return new WaitForSeconds(5f);
            GameStateManager.Instance.ChangeState(GameState.Depression);
        }
    }

    IEnumerator SlideRoutine(StatePresentation state, int myRun)
    {
        yield return new WaitForSeconds(state.slideDelay);

        if (myRun != runId) yield break;

        DestroyCurrentSlide();

        if (slideSpawnPoint == null || slidePrefab == null || state.slideTexture == null)
        {
            slideFinished = true;
            TryFinishBoth(state, myRun);
            yield break;
        }

        currentSlide = Instantiate(slidePrefab, slideSpawnPoint);
        currentSlide.name = "Slide_Runtime";

        currentSlide.transform.localPosition = Vector3.zero;
        currentSlide.transform.localRotation = Quaternion.identity;
        currentSlide.transform.localScale = Vector3.one * 0.5f;

        currentSlideCtrl = currentSlide.GetComponentInChildren<SlideController>(true);
        if (currentSlideCtrl == null)
        {
            Debug.LogError("SlideRoutine: SlideController not found!");
            slideFinished = true;
            TryFinishBoth(state, myRun);
            yield break;
        }

        currentSlideCtrl.SetTexture(state.slideTexture);

        if (slideFinishedHandler != null)
            currentSlideCtrl.OnFinished -= slideFinishedHandler;

        slideFinishedHandler = () =>
        {
            if (this == null) return;
            if (!isActiveAndEnabled) return;
            if (myRun != runId) return;

            slideFinished = true;
            TryFinishBoth(state, myRun);

            if (destroySlideCo != null)
                StopCoroutine(destroySlideCo);

            destroySlideCo = StartCoroutine(DestroySlideAfter(state.slideStayAfter, myRun));
        };

        currentSlideCtrl.OnFinished += slideFinishedHandler;

        var anim = currentSlide.GetComponentInChildren<Animator>(true);
        if (anim != null)
        {
            anim.enabled = true;
            anim.Rebind();
            anim.Update(0f);
            anim.Play(0, 0, 0f);
            anim.Update(0f);
        }

        float animLen = currentSlideCtrl.GetAnimLength();
        float fallback = state.slideDurationFallback;

        if (fallback <= 0.01f) fallback = (animLen > 0f) ? animLen + 0.2f : 5f;
        if (animLen > 0f && fallback < animLen) fallback = animLen + 0.2f;

        currentSlideCtrl.StartFallbackFinish(this, fallback);
    }

    IEnumerator DestroySlideAfter(float seconds, int myRun)
    {
        if (seconds < 0f) seconds = 0f;
        yield return new WaitForSeconds(seconds);

        if (myRun != runId) yield break;
        DestroyCurrentSlide();
    }

    void OnDestroy()
    {
        if (voiceCo != null) StopCoroutine(voiceCo);
        if (slideCo != null) StopCoroutine(slideCo);
        if (finishCo != null) StopCoroutine(finishCo);
        if (destroySlideCo != null) StopCoroutine(destroySlideCo);

        if (currentSlideCtrl != null && slideFinishedHandler != null)
            currentSlideCtrl.OnFinished -= slideFinishedHandler;
    }

    void DestroyCurrentSlide()
    {
        if (currentSlideCtrl != null && slideFinishedHandler != null)
            currentSlideCtrl.OnFinished -= slideFinishedHandler;

        slideFinishedHandler = null;

        if (currentSlide != null)
            Destroy(currentSlide);

        currentSlide = null;
        currentSlideCtrl = null;
    }

    void TryFinishBoth(StatePresentation state, int myRun)
    {
        if (myRun != runId) return;
        if (finishedInvoked) return;
        if (!voiceFinished || !slideFinished) return;

        finishedInvoked = true;

        if (finishCo != null) StopCoroutine(finishCo);
        finishCo = StartCoroutine(FinishRoutine(myRun));
    }

    IEnumerator FinishRoutine(int myRun)
    {
        if (myRun != runId) yield break;

        var cb = bothFinishedCallback;
        bothFinishedCallback = null;

        cb?.Invoke();
        yield break;
    }

    public void DuckMusic(float seconds) => AudioMixerDucker.Instance?.Duck(seconds);
    public void UnduckMusic(float seconds) => AudioMixerDucker.Instance?.Unduck(seconds);

    void PlayExtras(StatePresentation s)
    {
        if (s == null) return;

        if (s.playParticles != null)
        {
            for (int i = 0; i < s.playParticles.Length; i++)
            {
                var ps = s.playParticles[i];
                if (ps == null) continue;

                ps.gameObject.SetActive(true);
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Clear(true);
                ps.Play(true);

                if (!currentParticles.Contains(ps))
                    currentParticles.Add(ps);
            }
        }

        if (s.stopParticles != null)
        {
            for (int i = 0; i < s.stopParticles.Length; i++)
            {
                var ps = s.stopParticles[i];
                if (ps == null) continue;
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Clear(true);
            }
        }

        if (s.loopSfx != null)
        {
            for (int i = 0; i < s.loopSfx.Length; i++)
            {
                var a = s.loopSfx[i];
                if (a == null) continue;

                a.loop = true;
                AudioFader.Instance?.FadeIn(a, s.loopSfxFadeIn, 1f);

                if (!currentLoops.Contains(a))
                    currentLoops.Add(a);
            }
        }
    }

    void StopExtras()
    {
        for (int i = 0; i < currentLoops.Count; i++)
        {
            var a = currentLoops[i];
            if (a == null) continue;
            AudioFader.Instance?.FadeOut(a, 0.6f);
        }
        currentLoops.Clear();

        for (int i = 0; i < currentParticles.Count; i++)
        {
            var ps = currentParticles[i];
            if (ps == null) continue;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Clear(true);
        }
        currentParticles.Clear();
    }
}