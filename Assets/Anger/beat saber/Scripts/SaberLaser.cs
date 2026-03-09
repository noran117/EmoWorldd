using UnityEngine;

public class SaberLaser : MonoBehaviour
{
    public LayerMask noteLayer;
    public float castRadius = 0.05f;
    public float minCutSpeed = 0.8f;

    Vector3 prevPos;
    Vector3 velocity;

    SaberColor saberColor;

    void Start()
    {
        prevPos = transform.position;
        saberColor = GetComponent<SaberColor>();
    }

    void Update()
    {
        if (SaberGameManager.Instance != null && !SaberGameManager.Instance.gameRunning)
        {
            prevPos = transform.position;
            return;
        }

        velocity = (transform.position - prevPos) / Mathf.Max(Time.deltaTime, 0.0001f);

        Vector3 dir = transform.position - prevPos;
        float dist = dir.magnitude;

        if (dist > 0.0001f)
        {
            if (Physics.SphereCast(prevPos, castRadius, dir.normalized, out RaycastHit hit, dist, noteLayer))
            {
                TryHit(hit.collider);
            }
        }

        prevPos = transform.position;
    }

    void TryHit(Collider col)
    {
        var state = col.GetComponentInParent<NoteHitState>();
        if (state == null) return;
        if (!state.canBeHit) return;
        if (state.wasHit) return;

        var noteColor = col.GetComponentInParent<NoteColor>();
        var myColor = (saberColor != null) ? saberColor.color : SaberColorType.Red;

        state.wasHit = true;

        if (noteColor != null && noteColor.color == myColor)
        {
            SaberGameManager.Instance.AddScore(10);
            SaberGameManager.Instance.PlayCorrect();
        }
        else
        {
            SaberGameManager.Instance.PlayWrong();
        }

        Destroy(state.gameObject);
    }
}