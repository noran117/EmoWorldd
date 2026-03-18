using UnityEngine;

public class SaberGrabState : MonoBehaviour
{
    public bool isHeld;

    public void OnGrabbed()
    {
        isHeld = true;
        Debug.Log(gameObject.name + " grabbed");

        if (GameStartGate.Instance != null)
            GameStartGate.Instance.Check();
    }

    public void OnReleased()
    {
        isHeld = false;
        Debug.Log(gameObject.name + " released");
    }
}