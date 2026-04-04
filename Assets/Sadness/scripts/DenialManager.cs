using UnityEngine;
using BNG;
using System.Collections;

public class DenialManager : MonoBehaviour
{
    public static DenialManager Instance;

    [Header("Audio")]
    public AudioSource houseCall;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

    }

    public void StartDenial()
    {
        houseCall?.Play();

    }
 
}