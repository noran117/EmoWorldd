using UnityEngine;
using BNG;

public class BargainingManager : MonoBehaviour
{
    public static BargainingManager Instance;

    private bool puzzleFinished = false;
    private bool presentationFinished = false;

    [Header("Hint Arrow (Over Hammer)")]
    public GameObject arrowObject;            
    public Transform hammerTransform;         
    public Grabbable hammerGrabbable;        
    public Vector3 arrowOffset = new Vector3(0f, 0.25f, 0f); 

    bool arrowActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (arrowObject != null)
            arrowObject.SetActive(false);
    }

    public void ResetFlags()
    {
        puzzleFinished = false;
        presentationFinished = false;
    }

    public void StartBargaining()
    {
        ResetFlags();
        ShowArrow();
    }

    private void Update()
    {
        if (!arrowActive) return;
        if (GameStateManager.Instance == null) return;
        if (GameStateManager.Instance.currentState != GameState.Bargaining) return;

        if (hammerGrabbable != null && hammerGrabbable.BeingHeld)
        {
            HideArrow();
        }
    }

    void ShowArrow()
    {
        if (arrowObject == null || hammerTransform == null) return;

        arrowObject.transform.position = hammerTransform.position + arrowOffset;

        arrowObject.transform.rotation = Quaternion.identity;

        arrowObject.SetActive(true);
        arrowActive = true;
    }

    void HideArrow()
    {
        if (arrowObject == null) return;

        arrowObject.SetActive(false);
        arrowActive = false;
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

        HideArrow();
        GameStateManager.Instance.ChangeState(GameState.Depression);
    }
}