using UnityEngine;
using BNG;
using System.Collections;

public class DenialManager : MonoBehaviour
{















    public static DenialManager Instance;

    [Header("Audio")]
    public AudioSource houseCall;

    [Header("Ignore")]
    public Grabbable[] ignore;

    Grabbable[] allGrabbables;
    bool[] wasEnabled;
    bool locked = false;

    public followPlayer companion;//


    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        CacheGrabbables();
    }

    void CacheGrabbables()
    {
        allGrabbables = FindObjectsOfType<Grabbable>(true);

        wasEnabled = new bool[allGrabbables.Length];
        for (int i = 0; i < allGrabbables.Length; i++)
            wasEnabled[i] = allGrabbables[i] != null && allGrabbables[i].enabled;
    }

    bool IsIgnored(Grabbable g)
    {
        if (ignore == null) return false;
        foreach (var item in ignore)
            if (item == g) return true;
        return false;
    }

    public void StartDenial()
    {
        LockAllInteractions();
        houseCall?.Play();




        if (companion != null)
        {
            companion.PlayDenialSequence(); // 
        }


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

    public void EndDenial()
    {
        UnlockAllInteractions();
    }

    void UnlockAllInteractions()
    {
        if (!locked) return;

        for (int i = 0; i < allGrabbables.Length; i++)
        {
            if (allGrabbables[i] == null) continue;
            allGrabbables[i].enabled = wasEnabled[i];
        }

        locked = false;
    }
}