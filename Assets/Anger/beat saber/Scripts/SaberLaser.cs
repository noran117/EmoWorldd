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

    }

    void Update()
    {
        currentSpeed = (transform.position - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (SaberGameManager.Instance == null)
        {
            return;
        }

        if (!SaberGameManager.Instance.gameRunning)
        {
            return;
        }

        NoteHitState state = other.GetComponentInParent<NoteHitState>();
        NoteColor noteColor = other.GetComponentInParent<NoteColor>();

        if (state == null)
        {
            return;
        }

        if (noteColor == null)
        {
            return;
        }

        if (saberColor == null)
        {
            return;
        }

        if (state.wasHit)
        {
            return;
        }

        if (noteColor.color == saberColor.color)
        {
            state.wasHit = true;
            SaberGameManager.Instance.AddScore(10);
            SaberGameManager.Instance.PlayCorrect();

            Destroy(state.gameObject);
        }
        else
        {
            SaberGameManager.Instance.AddScore(-10);
            SaberGameManager.Instance.PlayWrong();
        }
    }
}