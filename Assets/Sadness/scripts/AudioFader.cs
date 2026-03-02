using System.Collections;
using UnityEngine;

public class AudioFader : MonoBehaviour
{
    public static AudioFader Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void FadeIn(AudioSource audio, float duration, float targetVolume)
    {
        if (audio == null) return;
        audio.volume = 0f;
        audio.Play();
        StartCoroutine(FadeAudio(audio, 0f, targetVolume, duration));
    }

    public void FadeOut(AudioSource audio, float duration)
    {
        if (audio == null) return;
        StartCoroutine(FadeAudio(audio, audio.volume, 0f, duration, true));
    }

    IEnumerator FadeAudio(AudioSource audio, float from, float to, float duration, bool stopAfter = false)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audio.volume = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        audio.volume = to;

        if (stopAfter)
            audio.Stop();
    }
}
