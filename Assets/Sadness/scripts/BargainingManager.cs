using UnityEngine;
using BNG;

public class BargainingManager : MonoBehaviour
{
    public static BargainingManager Instance;

    private bool puzzleFinished = false;
    private bool presentationFinished = false;


    public Vector3 arrowOffset = new Vector3(0f, 0.25f, 0f);

    bool arrowActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ResetFlags()
    {
        puzzleFinished = false;
        presentationFinished = false;
    }

    public void StartBargaining()
    {
        ResetFlags();

    }

    private void Update()
    {
        if (!arrowActive) return;
        if (GameStateManager.Instance == null) return;
        if (GameStateManager.Instance.currentState != GameState.Bargaining) return;


    }


    public void OnPuzzleFinished()
    {
        puzzleFinished = true;
        TryFinish();
    }

    public void OnPresentationFinished()
    {
        presentationFinished = true;
        TryFinish();
    }

    void TryFinish()
    {
        if (!puzzleFinished) return;
        if (!presentationFinished) return;

    }
}