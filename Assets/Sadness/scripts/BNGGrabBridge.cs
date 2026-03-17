using UnityEngine;
using BNG;

[RequireComponent(typeof(Grabbable))]
[RequireComponent(typeof(SaberGrabState))]
public class BNGGrabBridge : MonoBehaviour
{
    private Grabbable grabbable;
    private SaberGrabState grabState;

    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        grabState = GetComponent<SaberGrabState>();
    }

    public void Grabbed()
    {
        grabState.OnGrabbed();
    }

    public void Released()
    {
        grabState.OnReleased();
    }
}