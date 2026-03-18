using UnityEngine;

public class GameStartGate : MonoBehaviour
{
    public static GameStartGate Instance;

    public SaberGrabState leftSaber;
    public SaberGrabState rightSaber;

    void Awake()
    {
        Instance = this;
    }

    public void Check()
    {
        bool leftHeld = leftSaber != null && leftSaber.isHeld;
        bool rightHeld = rightSaber != null && rightSaber.isHeld;
        bool both = leftHeld && rightHeld;

        Debug.Log("Gate Check | left = " + leftHeld + " | right = " + rightHeld + " | both = " + both);

        if (SaberGameManager.Instance == null) return;

        if (both)
        {
            SaberGameManager.Instance.SetRunning(true);
        }
    }
}