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
        bool both = leftSaber != null && rightSaber != null &&
                    leftSaber.isHeld && rightSaber.isHeld;

        Debug.Log("Both sabers held = " + both);

        if (SaberGameManager.Instance == null) return;

        SaberGameManager.Instance.SetRunning(both);
    }
}