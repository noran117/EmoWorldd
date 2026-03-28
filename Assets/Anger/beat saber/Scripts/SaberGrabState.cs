using UnityEngine;

public class SaberGrabState : MonoBehaviour
{
    public bool isHeld;

    public void OnGrabbed()
    {
        isHeld = true;
    }

    public void OnReleased()
    {
        isHeld = false;
    }
}