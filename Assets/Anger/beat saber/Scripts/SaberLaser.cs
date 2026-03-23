using UnityEngine;

public class SaberLaser : MonoBehaviour
{
    public float minCutSpeed = 0f;

    private Vector3 lastPos;
    private float currentSpeed;
    private SaberColor saberColor;

    void Start()
    {
        lastPos = transform.position;
        saberColor = GetComponentInParent<SaberColor>();

        Debug.Log(gameObject.name + " START | saberColor = " + (saberColor != null ? saberColor.color.ToString() : "NULL"));
    }

    void Update()
    {
        currentSpeed = (transform.position - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
      //  Debug.Log("---- ON TRIGGER ENTER with: " + other.name);

        if (SaberGameManager.Instance == null)
        {
            Debug.Log("STOP: SaberGameManager.Instance is NULL");
            return;
        }

        if (!SaberGameManager.Instance.gameRunning)
        {
            Debug.Log("STOP: gameRunning is FALSE");
            return;
        }

        //if (currentSpeed < minCutSpeed)
        //{
        //    Debug.Log("STOP: Saber too slow = " + currentSpeed);
        //    return;
        //}

        NoteHitState state = other.GetComponentInParent<NoteHitState>();
        NoteColor noteColor = other.GetComponentInParent<NoteColor>();

        if (state == null)
        {
            Debug.Log("STOP: No NoteHitState on " + other.name);
            return;
        }

        if (noteColor == null)
        {
            Debug.Log("STOP: No NoteColor on " + other.name);
            return;
        }

        if (saberColor == null)
        {
            Debug.Log("STOP: saberColor is NULL");
            return;
        }

        Debug.Log("HIT CHECK | saber = " + saberColor.color + " | note = " + noteColor.color + " | wasHit = " + state.wasHit + " | speed = " + currentSpeed);

        if (state.wasHit)
        {
            Debug.Log("STOP: Note already hit");
            return;
        }

        if (noteColor.color == saberColor.color)
        {
            Debug.Log("CORRECT HIT");

            state.wasHit = true;
            SaberGameManager.Instance.AddScore(10);
            SaberGameManager.Instance.PlayCorrect();

            Destroy(state.gameObject);
        }
        else
        {
            Debug.Log("WRONG HIT");
            SaberGameManager.Instance.AddScore(-10);
            SaberGameManager.Instance.PlayWrong();
        }
    }
}