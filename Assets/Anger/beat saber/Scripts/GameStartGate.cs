using UnityEngine;

public class GameStartGate : MonoBehaviour
{
    public static GameStartGate Instance;

    public SaberGrabState leftSaber;
    public SaberGrabState rightSaber;

    bool lastBoth;

    void Awake()
    {
        Instance = this;
        Debug.Log("GameStartGate Awake");
    }

    void Update()
    {
        if (SaberGameManager.Instance == null) return;
        if (SaberGameManager.Instance.gameLocked) return;

        bool leftHeld = leftSaber != null && leftSaber.isHeld;
        bool rightHeld = rightSaber != null && rightSaber.isHeld;
        bool both = leftHeld && rightHeld;

        if (both != lastBoth)
        {
            Debug.Log("Gate Update | left = " + leftHeld + " | right = " + rightHeld + " | both = " + both);
            SaberGameManager.Instance.SetRunning(both);
            lastBoth = both;
        }
    }
}