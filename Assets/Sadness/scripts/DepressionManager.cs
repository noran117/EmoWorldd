using UnityEngine;
using BNG;

public class DepressionManager : MonoBehaviour
{
    public static DepressionManager Instance;

    [Header("Visual Objects")]
    public GameObject[] enableOnStart;
    public GameObject[] disableOnStart;

    [Header("SFX")]
    public AudioSource breathingSfx;
    public AudioSource heartbeatSfx;
    [Range(0f, 1f)] public float breathingTargetVolume = 0.4f;
    [Range(0f, 1f)] public float heartbeatTargetVolume = 0.3f;

    [Header("Lock Interaction")]
    public Grabbable[] ignore;

    Grabbable[] allGrabbables;
    bool locked = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        allGrabbables = FindObjectsOfType<Grabbable>(true);
    }

    bool IsIgnored(Grabbable g)
    {
        if (ignore == null) return false;

        foreach (var item in ignore)
        {
            if (item == g)
                return true;
        }

        return false;
    }

    void LockAllInteractions()
    {
        if (locked) return;
        locked = true;

        foreach (var g in allGrabbables)
        {
            if (g == null) continue;
            if (IsIgnored(g)) continue;

            g.enabled = false;
        }
    }

    void UnlockAllInteractions()
    {
        foreach (var g in allGrabbables)
        {
            if (g == null) continue;
            g.enabled = true;
        }

        locked = false;
    }

    void SetObjectsActive(GameObject[] arr, bool active)
    {
        if (arr == null) return;

        foreach (var obj in arr)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    void StartLoopedSound(AudioSource src, float targetVolume, string label)
    {
        if (src == null)
        {
            return;
        }

        src.loop = true;
        src.playOnAwake = false;
        src.volume = 0f;

        if (!src.isPlaying)
            src.Play();

        if (AudioFader.Instance != null)
        {
            AudioFader.Instance.FadeIn(src, 2f, targetVolume);
        }
        else
        {
            src.volume = targetVolume;
        }
    }

    void StopLoopedSound(AudioSource src, string label)
    {
        if (src == null) return;

        if (AudioFader.Instance != null)
        {
            AudioFader.Instance.FadeOut(src, 1.5f);
        }
        else
        {
            src.Stop();
            Debug.LogWarning("DepressionManager: AudioFader not found, stopped " + label + " مباشرة");
        }
    }

    public void StartDepression()
    {
        SetObjectsActive(disableOnStart, false);
        SetObjectsActive(enableOnStart, true);

        LockAllInteractions();

        StartLoopedSound(breathingSfx, breathingTargetVolume, "Breathing");
        StartLoopedSound(heartbeatSfx, heartbeatTargetVolume, "Heartbeat");

        if (XRRigSlowMovement.Instance != null)
        {
            XRRigSlowMovement.Instance.StartDepressionSlowdown();
        }
        else
        {
            Debug.LogWarning("DepressionManager: XRRigSlowMovement.Instance is NULL");
        }
    }

    public void EndDepression()
    {
        StopLoopedSound(breathingSfx, "Breathing");
        StopLoopedSound(heartbeatSfx, "Heartbeat");

        SetObjectsActive(enableOnStart, false);
        SetObjectsActive(disableOnStart, true);

        if (XRRigSlowMovement.Instance != null)
        {
            XRRigSlowMovement.Instance.ResetSpeed();
        }
        else
        {
            Debug.LogWarning("DepressionManager: XRRigSlowMovement.Instance is NULL");
        }

        UnlockAllInteractions();
    }
}