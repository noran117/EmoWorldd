using UnityEngine;

public class HandTrailController : MonoBehaviour
{
    public TrailRenderer trail;
    public float movementThreshold = 0.01f;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        trail.emitting = false;
    }

    void Update()
    {
        float movement = Vector3.Distance(transform.position, lastPosition);

        trail.emitting = movement > movementThreshold;

        lastPosition = transform.position;
    }
}
