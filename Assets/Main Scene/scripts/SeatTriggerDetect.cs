using UnityEngine;

public class SeatTriggerDetect : MonoBehaviour
{
    public TrainMove trainMove;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trainMove.StartMoving();
        }
    }
}
