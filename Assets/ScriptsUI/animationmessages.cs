using UnityEngine;
using System.Collections;

public class UIMessagePop : MonoBehaviour
{
    public float popDuration = 0.3f;
    public float stayDuration = 2f;
    public float hideDuration = 0.4f;

    public float popScale = 1.1f;
    public float shakeAmount = 4f;

    public AudioSource popSound;

    CanvasGroup cg;
    Vector3 startScale;
    Vector3 startPos;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();

        startScale = transform.localScale;
        startPos = transform.localPosition;

        cg.alpha = 0;
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        ShowMessage(); //  ŸÂ— ·Õ«·Â«
    }

    public void ShowMessage()
    {
        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        if (popSound) popSound.Play();

        float t = 0;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float p = t / popDuration;

            transform.localScale = Vector3.Lerp(Vector3.zero, startScale * popScale, p);
            cg.alpha = p;
            transform.localPosition = startPos + Random.insideUnitSphere * shakeAmount;

            yield return null;
        }

        transform.localScale = startScale;
        transform.localPosition = startPos;
        cg.alpha = 1;

        yield return new WaitForSeconds(stayDuration);

        t = 0;
        while (t < hideDuration)
        {
            t += Time.deltaTime;
            float p = t / hideDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, p);
            cg.alpha = 1 - p;

            yield return null;
        }

        cg.alpha = 0;
    }
}
