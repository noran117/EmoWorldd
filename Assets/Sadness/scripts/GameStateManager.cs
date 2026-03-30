using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
        //StartCoroutine(StartAfterDelay());
        ChangeState(GameState.Anger);


    }

    IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(10f);

        ChangeState(GameState.Play);
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

                if (StatePresentationManager.Instance == null)
                {
                    return;
                }

                StatePresentationManager.Instance.bothFinishedCallback = () =>
                {
                    if (brotherMovement != null)
                        StatePresentationManager.Instance.DuckMusic(0.5f);

                    if (brotherMovement != null)
                        brotherMovement.BeginSequence();

                    StatePresentationManager.Instance.UnduckMusic(0.5f);
                };
                StatePresentationManager.Instance.PlayState(
                    StatePresentationManager.Instance.play
                );

                break;

            case GameState.Shock:
                if (shockTrigger != null) shockTrigger.StartShock();
                else 
                    Debug.LogError("shockTrigger NULL");
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
                            Debug.LogError("BargainingManager.Instance NULL");
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

                    StartCoroutine(LoadSceneAfterDelay());
                };

                StatePresentationManager.Instance.PlayState(
                    StatePresentationManager.Instance.acceptance
                );

                AcceptanceManager.Instance.StartAcceptance();
                break;

        }
    }
    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(30f);
        SceneManager.LoadScene("Main_Scene");
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
