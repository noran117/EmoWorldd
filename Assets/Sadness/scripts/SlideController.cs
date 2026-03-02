using UnityEngine;
using System;
using System.Collections;

public class SlideController : MonoBehaviour
{
    public MeshRenderer quadRenderer;
    public event Action OnFinished;

    bool finished;
    Coroutine fallbackCo;

    static readonly int MainTex = Shader.PropertyToID("_MainTex");
    static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

    MaterialPropertyBlock block;

    void Awake()
    {
        if (quadRenderer == null)
            quadRenderer = GetComponentInChildren<MeshRenderer>();

        block = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        finished = false;
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

        var clipsInfo = anim.GetCurrentAnimatorClipInfo(0);
        if (clipsInfo != null && clipsInfo.Length > 0 && clipsInfo[0].clip != null)
            return clipsInfo[0].clip.length;

        var clips = anim.runtimeAnimatorController.animationClips;
        float max = 0f;
        if (clips != null)
        {
            for (int i = 0; i < clips.Length; i++)
                if (clips[i] != null && clips[i].length > max) max = clips[i].length;
        }
        return max;
    }


    public void StartFallbackFinish(MonoBehaviour runner, float seconds)
    {
        if (runner == null) return;
        if (fallbackCo != null) runner.StopCoroutine(fallbackCo);
        fallbackCo = runner.StartCoroutine(Fallback(seconds));
    }

    IEnumerator Fallback(float s)
    {
        yield return new WaitForSeconds(s);
        Finish();
    }

    public void Finish()
    {
        if (finished) return;
        finished = true;
        OnFinished?.Invoke();
    }
}
