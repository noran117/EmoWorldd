using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatePresentationManager : MonoBehaviour
{
    public static StatePresentationManager Instance;

    [Header("References")]
    public Transform xrCamera;
    public Transform xrRigRoot;

    [Header("Slide")]
    public GameObject slidePrefab;

    [Header("Slide Spawn (Child of camera)")]
    public Transform slideSpawnPoint;

    [Header("States")]
    public StatePresentation play;
    public StatePresentation shock;
    public StatePresentation denial;
    public StatePresentation anger;
    public StatePresentation bargaining;
    public StatePresentation depression;
    public StatePresentation acceptance;

    public System.Action bothFinishedCallback;

    AudioSource currentMusic;
    readonly List<AudioSource> allMusics = new List<AudioSource>();

    GameObject currentSlide;
    SlideController currentSlideCtrl;

    Coroutine voiceCo;
    Coroutine slideCo;
    Coroutine finishCo;
    Coroutine destroySlideCo;

    bool voiceFinished;
    bool slideFinished;

    readonly List<AudioSource> currentLoops = new List<AudioSource>();
    readonly List<ParticleSystem> currentParticles = new List<ParticleSystem>();

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

        DestroyCurrentSlide();

        StopExtras();

        voiceFinished = (state.voiceOver == null);
        slideFinished = (slidePrefab == null || state.slideTexture == null);

        LightingManager.Instance?.SetLighting(state.lightColor, state.lightIntensity);
        SwitchMusic(state);

        PlayExtras(state);

        if (state.voiceOver != null)
            voiceCo = StartCoroutine(VoiceRoutine(state));

        if (!slideFinished)
            slideCo = StartCoroutine(SlideRoutine(state));
        else
            TryFinishBoth(state);
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

    IEnumerator VoiceRoutine(StatePresentation state)
    {
        Debug.Log("VOICE START: " + state.voiceOver.name);

        yield return new WaitForSeconds(state.voiceDelay);

        AudioMixerDucker.Instance?.Duck(state.duckIn);

        state.voiceOver.Play();

        while (state.voiceOver != null && state.voiceOver.isPlaying)
            yield return null;

        AudioMixerDucker.Instance?.Unduck(state.duckOut);

        voiceFinished = true;

        Debug.Log("VOICE FINISHED");

        TryFinishBoth(state);
    }

    IEnumerator SlideRoutine(StatePresentation state)
    {
        yield return new WaitForSeconds(state.slideDelay);

        DestroyCurrentSlide();

        if (slideSpawnPoint == null || slidePrefab == null || state.slideTexture == null)
        {
            Debug.LogError("SlideRoutine: slideSpawnPoint/slidePrefab/slideTexture is not found!");
            slideFinished = true;
            TryFinishBoth(state);
            yield break;
        }

        currentSlide = Instantiate(slidePrefab, slideSpawnPoint);
        currentSlide.name = "Slide_Runtime";

        currentSlide.transform.localPosition = new Vector3(0f, 0f, -7.5f);
        currentSlide.transform.localRotation = Quaternion.identity;
        currentSlide.transform.localScale = Vector3.one * 5f;

        currentSlideCtrl = currentSlide.GetComponentInChildren<SlideController>(true);
        if (currentSlideCtrl == null)
        {
            Debug.LogError("SlideRoutine: SlideController is not found");
            slideFinished = true;
            TryFinishBoth(state);
            yield break;
        }

        currentSlideCtrl.SetTexture(state.slideTexture);

        var anim = currentSlide.GetComponentInChildren<Animator>(true);
        if (anim != null)
        {
            anim.enabled = true;
            anim.Play(0, 0, 0f);
        }

        currentSlideCtrl.OnFinished += () =>
        {
            slideFinished = true;
            Debug.Log("SLIDE FINISHED");
            TryFinishBoth(state);

            if (destroySlideCo != null) StopCoroutine(destroySlideCo);
            destroySlideCo = StartCoroutine(DestroySlideAfter(state.slideStayAfter));
        };

        float animLen = currentSlideCtrl.GetAnimLength();
        float fallback = state.slideDurationFallback;

        if (fallback <= 0.01f) fallback = (animLen > 0f) ? animLen + 0.2f : 5f;
        if (animLen > 0f && fallback < animLen) fallback = animLen + 0.2f;

        currentSlideCtrl.StartFallbackFinish(this, fallback);
        Debug.Log("SLIDE FINISHED: " +
    (state.slideTexture != null ? state.slideTexture.name : "NoSlide"));
    }

    IEnumerator DestroySlideAfter(float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        yield return new WaitForSeconds(seconds);
        DestroyCurrentSlide();
    }

    void DestroyCurrentSlide()
    {
        if (currentSlide != null) Destroy(currentSlide);
        currentSlide = null;
        currentSlideCtrl = null;
    }

    void TryFinishBoth(StatePresentation state)
    {
        Debug.Log("TryFinishBoth => voice=" + voiceFinished +
                  " slide=" + slideFinished);

        if (!voiceFinished || !slideFinished)
            return;

        Debug.Log("BOTH TRUE - CALLING CALLBACK NOW");

        bothFinishedCallback?.Invoke();
    }

    IEnumerator FinishRoutine(StatePresentation state)
    {
        Debug.Log("FinishRoutine called");

        bothFinishedCallback?.Invoke();
        // bothFinishedCallback = null;

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