using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrainMove : MonoBehaviour
{
    public Transform destination;
    public float speed = 2f;

    private bool isMoving = false;
    public bool IsMoving => isMoving;   

    private Vector3 targetPos;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        if (!isMoving || destination == null) return;

        Vector3 newPos = Vector3.MoveTowards(
            rb.position,
            destination.position,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        if (Vector3.Distance(rb.position, destination.position) <= 0.02f)
        {
            rb.MovePosition(destination.position);
            isMoving = false;
        }
    }

    public void StartMoving()
    {
        if (destination == null) return;

        isMoving = true;
    }
}
