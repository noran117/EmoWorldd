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
            if (item == g) return true;
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
            if (obj != null) obj.SetActive(active);
    }

    public void StartDepression()
    {
        SetObjectsActive(disableOnStart, false);
        SetObjectsActive(enableOnStart, true);

        LockAllInteractions();

        AudioFader.Instance?.FadeIn(breathingSfx, 2f, 0.4f);
        AudioFader.Instance?.FadeIn(heartbeatSfx, 2f, 0.3f);

        XRRigSlowMovement.Instance?.StartDepressionSlowdown();
    }

    public void EndDepression()
    {
        AudioFader.Instance?.FadeOut(breathingSfx, 1.5f);
        AudioFader.Instance?.FadeOut(heartbeatSfx, 1.5f);

        SetObjectsActive(enableOnStart, false);
        SetObjectsActive(disableOnStart, true);

        XRRigSlowMovement.Instance?.ResetSpeed();
        UnlockAllInteractions();
    }
}