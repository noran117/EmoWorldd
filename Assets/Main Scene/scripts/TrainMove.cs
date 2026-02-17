using UnityEngine;

public class TrainMove : MonoBehaviour
{
    public Transform destination;
    public float speed = 2f;

    private bool isMoving = false;

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, destination.position) <= 0.1f)
        {
            transform.position = destination.position;
            isMoving = false;
        }
    }

    public void StartMoving()
    {
        isMoving = true;
    }
}
