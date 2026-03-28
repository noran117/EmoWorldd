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
            SaberGameManager.Instance.SetRunning(both);
            lastBoth = both;
        }
    }
}