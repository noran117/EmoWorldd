using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public RoomSequenceController roomController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Playerr"))
        {
            roomController.StartRoomSequence();
            gameObject.SetActive(false); // ÚÔÇä ãÇ íÚíÏåÇ
        }
    }
}
