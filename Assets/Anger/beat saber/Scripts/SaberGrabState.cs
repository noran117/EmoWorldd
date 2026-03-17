using UnityEngine;

public class SaberGrabState : MonoBehaviour
{
    public bool isHeld;

    public void OnGrabbed()
    {
        isHeld = true;
        Debug.Log(gameObject.name + " grabbed");
        GameStartGate.Instance.Check();
    }

    public void OnReleased()
    {
        isHeld = false;
        Debug.Log(gameObject.name + " released");
        GameStartGate.Instance.Check();
    }
}