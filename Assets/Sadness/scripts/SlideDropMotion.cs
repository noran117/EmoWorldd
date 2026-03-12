using UnityEngine;
using System.Collections;

public class SlideDropMotion : MonoBehaviour
{
    public Vector3 startOffset = new Vector3(0, 4f, 0);
    public Vector3 endOffset = Vector3.zero;

    public float duration = 1f;

    Transform target;

    void OnEnable()
    {
        target = transform;

        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        Vector3 startPos = target.localPosition + startOffset;
        Vector3 endPos = target.localPosition + endOffset;

        target.localPosition = startPos;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.SmoothStep(0, 1, time / duration);
            target.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        target.localPosition = endPos;
    }
}