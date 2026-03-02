using UnityEngine;
using System.Collections;

public class SlideWorldPresenter : MonoBehaviour
{
    public Transform xrCamera;
    public GameObject slidePrefab;

    public float distance = 2f;
    public Vector3 offset = new Vector3(0f, -0.15f, 0f);

    GameObject current;
    Coroutine co;

    static readonly int MainTex = Shader.PropertyToID("_MainTex");

    public void Show(Texture tex, float showAfterSeconds, float extraSecondsAfterAnim)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(ShowRoutine(tex, showAfterSeconds, extraSecondsAfterAnim));
    }

    IEnumerator ShowRoutine(Texture tex, float delay, float extra)
    {
        yield return new WaitForSeconds(delay);

        DestroyCurrent();

        var cam = xrCamera != null ? xrCamera : (Camera.main != null ? Camera.main.transform : null);
        if (cam == null || slidePrefab == null) yield break;

        current = Instantiate(slidePrefab);
        current.name = "Slide_Runtime";

        ApplyTexture(current, tex);

        float animLen = PlayAnimAndGetLength(current);

        float t = 0f;
        float total = animLen + Mathf.Max(0.1f, extra);

        while (t < total)
        {
            t += Time.deltaTime;
            FollowCamera(cam);
            yield return null;
        }

        DestroyCurrent();
    }

    void FollowCamera(Transform cam)
    {
        if (current == null) return;

        Vector3 pos = cam.position + cam.forward * distance + cam.TransformVector(offset);
        current.transform.position = pos;
        current.transform.rotation = Quaternion.LookRotation(cam.forward, Vector3.up);
    }

    void ApplyTexture(GameObject go, Texture tex)
    {
        if (tex == null) return;

        var r = go.GetComponentInChildren<MeshRenderer>(true);
        if (r == null) return;

        var block = new MaterialPropertyBlock();
        r.GetPropertyBlock(block);
        block.SetTexture(MainTex, tex);
        r.SetPropertyBlock(block);
    }

    float PlayAnimAndGetLength(GameObject go)
    {
        var anim = go.GetComponentInChildren<Animator>(true);
        if (anim == null || anim.runtimeAnimatorController == null) return 0f;

        float len = 0f;
        var clips = anim.runtimeAnimatorController.animationClips;
        if (clips != null && clips.Length > 0) len = clips[0].length;

        anim.enabled = true;
        anim.Play(0, 0, 0f);

        return len;
    }

    void DestroyCurrent()
    {
        if (current != null) Destroy(current);
        current = null;
    }
}
