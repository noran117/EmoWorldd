using UnityEngine;
using System.Collections;

public enum GameState
{
    Play,
    Shock,
    Denial,
    Anger,
    Bargaining,
    Depression,
    Acceptance,
    Ending
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public GameState currentState;
    public charactermovement brotherMovement;
    public ShockTrigger shockTrigger;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        //ChangeState(GameState.Play);
        ChangeState(GameState.Depression);

    }

    public void ChangeState(GameState newState)
    {
        Debug.Log("ChangeState: " + newState);
        currentState = newState;
        EnterState(newState);
    }
    void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Play:

                Debug.Log("Entered PLAY state");

                Debug.Log("StatePresentationManager.Instance = " + StatePresentationManager.Instance);

                if (StatePresentationManager.Instance == null)
                {
                    Debug.LogError("StatePresentationManager.Instance is NULL");
                    return;
                }

                Debug.Log("play state data = " + StatePresentationManager.Instance.play);

                StatePresentationManager.Instance.bothFinishedCallback = () =>
                {
                    Debug.Log("Play presentation finished");

                    if (brotherMovement != null)
                        StatePresentationManager.Instance.DuckMusic(0.5f);

                    if (brotherMovement != null)
                        brotherMovement.BeginSequence();

                    StatePresentationManager.Instance.UnduckMusic(0.5f);

                    Debug.Log("Slide + Voice Over finished .. starting brother sequence");
                };

                Debug.Log("About to call PlayState");
                StatePresentationManager.Instance.PlayState(
                    StatePresentationManager.Instance.play
                );
                Debug.Log("PlayState called");

                break;
            //case GameState.Play:

            //    StatePresentationManager.Instance.bothFinishedCallback = () =>
            //    {
            //        if (brotherMovement != null)
            //        StatePresentationManager.Instance.DuckMusic(0.5f);
            //        brotherMovement.BeginSequence();
            //        StatePresentationManager.Instance.UnduckMusic(0.5f);
            //        Debug.Log("Slide + Voice Over finished .. starting brother sequence");
            //    };

            //    StatePresentationManager.Instance.PlayState(
            //        StatePresentationManager.Instance.play
            //    );

            //    break;

            case GameState.Shock:
                if (shockTrigger != null) shockTrigger.StartShock();
                else Debug.LogError("shockTrigger NULL");
                break;

            case GameState.Denial:

                DenialManager.Instance.StartDenial();

                StatePresentationManager.Instance.bothFinishedCallback = () =>
                {
                    Debug.Log("DENIAL finished -> going to ANGER");
                    DenialManager.Instance.EndDenial();
                    ChangeState(GameState.Anger);
                };

                StatePresentationManager.Instance.PlayState(
                    StatePresentationManager.Instance.denial
                );

                break;



            case GameState.Anger:
                StatePresentationManager.Instance.PlayState(
                    StatePresentationManager.Instance.anger
                );
                EnableAngerObjects();
                break;


            case GameState.Bargaining:
                {
                    if (BargainingManager.Instance != null)
                        BargainingManager.Instance.StartBargaining();
                    else
                        Debug.LogError("BargainingManager.Instance NULL");


                    StatePresentationManager.Instance.bothFinishedCallback = () =>
                    {
                        if (BargainingManager.Instance != null)
                            BargainingManager.Instance.OnPresentationFinished();
                        else
                            Debug.LogError("BargainingManager.Instance NULL (OnPresentationFinished)");
                    };

                    StatePresentationManager.Instance.PlayState(
                        StatePresentationManager.Instance.bargaining
                    );

   
                    PuzzleManager pm = Object.FindFirstObjectByType<PuzzleManager>(FindObjectsInactive.Include);

                    if (pm != null)
                    {
                        pm.EnablePuzzle();
                    }
                    else
                    {
                        Debug.LogError("PuzzleManager is not found");
                    }

                    break;
                }



            case GameState.Depression:

                DepressionManager.Instance.StartDepression();

                StatePresentationManager.Instance.bothFinishedCallback = () =>
                {
                    DepressionManager.Instance.EndDepression();

                    ChangeState(GameState.Acceptance);
                };

                StatePresentationManager.Instance.PlayState(
                    StatePresentationManager.Instance.depression
                );

                break;



            case GameState.Acceptance:

                StatePresentationManager.Instance.bothFinishedCallback = () =>
                {
                    AcceptanceManager.Instance.NotifyPresentationFinished();
                };

                StatePresentationManager.Instance.PlayState(
                    StatePresentationManager.Instance.acceptance
                );

                AcceptanceManager.Instance.StartAcceptance();
                break;

        }
    }

    void StartPlay()
    {
        if (brotherMovement != null)
            brotherMovement.BeginSequence();
    }
    void EnableAngerObjects()
    {
        ToyBreakableInAngerState[] toys = Object.FindObjectsByType<ToyBreakableInAngerState>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        foreach (var toy in toys)
            toy.EnableBreaking();
    }

}
