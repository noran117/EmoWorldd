using UnityEngine;

public class SaberDebug : MonoBehaviour
{
    public float minCutSpeed = 0.8f;

    Vector3 prevPos;
    Vector3 velocity;

    void Start() => prevPos = transform.position;

    void Update()
    {
        velocity = (transform.position - prevPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        prevPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        var note = other.GetComponentInParent<NoteHitState>();
        if (note == null) return;
        if (!note.canBeHit) return;

        if (velocity.magnitude < minCutSpeed) return;

        note.wasHit = true;
        Destroy(note.gameObject);
    }
}