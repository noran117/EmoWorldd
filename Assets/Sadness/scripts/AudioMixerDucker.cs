using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioMixerDucker : MonoBehaviour
{
    public static AudioMixerDucker Instance;

    public AudioMixer mixer;
    public string musicVolumeParam = "MusicVolDb";
    public float normalDb = 0f;
    public float duckDb = -35f;

    Coroutine co;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Duck(float seconds)
    {
        SetTarget(duckDb, seconds);
    }

    public void Unduck(float seconds)
    {
        SetTarget(normalDb, seconds);
    }

    void SetTarget(float target, float seconds)
    {
        if (mixer == null) return;

        if (co != null) StopCoroutine(co);

        float current;
        if (!mixer.GetFloat(musicVolumeParam, out current))
            current = normalDb;

        co = StartCoroutine(Tween(current, target, seconds));
    }

    IEnumerator Tween(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            mixer.SetFloat(musicVolumeParam, to);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            mixer.SetFloat(musicVolumeParam, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        mixer.SetFloat(musicVolumeParam, to);
    }
}
