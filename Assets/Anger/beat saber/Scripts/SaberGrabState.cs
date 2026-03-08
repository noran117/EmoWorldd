using UnityEngine;

public class SaberGrabState : MonoBehaviour
{
    public bool isHeld;

    public void OnGrabbed()
    {
        isHeld = true;
        GameStartGate.Instance.Check();
    }

    public void OnReleased()
    {
        isHeld = false;
        GameStartGate.Instance.Check();
    }
}