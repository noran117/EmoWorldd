using UnityEngine;
using System;
using System.Collections;

public class SlideController : MonoBehaviour
{
    public MeshRenderer quadRenderer;
    public event Action OnFinished;

    bool finished;
    bool isQuittingOrDestroying;

    Coroutine fallbackCo;
    MonoBehaviour fallbackRunner;

    static readonly int MainTex = Shader.PropertyToID("_MainTex");
    static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

    MaterialPropertyBlock block;

    void Awake()
    {
        if (quadRenderer == null)
            quadRenderer = GetComponentInChildren<MeshRenderer>(true);

        block = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        finished = false;
        isQuittingOrDestroying = false;
    }

    void OnDisable()
    {
        StopFallback();
    }

    void OnDestroy()
    {
        isQuittingOrDestroying = true;
        StopFallback();
    }

    public void SetTexture(Texture tex)
    {
        if (quadRenderer == null || tex == null) return;

        quadRenderer.GetPropertyBlock(block);
        block.SetTexture(MainTex, tex);
        block.SetTexture(BaseMap, tex);
        quadRenderer.SetPropertyBlock(block);
    }

    public void OnSlideFinished_AnimEvent()
    {
        Finish();
    }

    public void OnSlideFinished()
    {
        Finish();
    }

    public float GetAnimLength()
    {
        var anim = GetComponentInChildren<Animator>(true);
        if (anim == null || anim.runtimeAnimatorController == null) return 0f;

        var clips = anim.runtimeAnimatorController.animationClips;
        float max = 0f;

        if (clips != null)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null && clips[i].length > max)
                    max = clips[i].length;
            }
        }

        return max;
    }

    public void StartFallbackFinish(MonoBehaviour runner, float seconds)
    {
        if (runner == null) return;

        StopFallback();

        fallbackRunner = runner;
        fallbackCo = runner.StartCoroutine(Fallback(seconds));
    }

    IEnumerator Fallback(float s)
    {
        yield return new WaitForSeconds(s);

        // لو object انمسح، لا تكمل
        if (this == null || isQuittingOrDestroying) yield break;

        Finish();
    }

    void StopFallback()
    {
        if (fallbackCo != null && fallbackRunner != null)
        {
            if (fallbackRunner != null)
                fallbackRunner.StopCoroutine(fallbackCo);
        }

        fallbackCo = null;
        fallbackRunner = null;
    }

    public void Finish()
    {
        if (finished) return;
        if (isQuittingOrDestroying) return;

        finished = true;
        StopFallback();

        OnFinished?.Invoke();
    }
}