using UnityEngine;

public class StartMove : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("player entered");
            GetComponent<BNG.MoveToWaypoint>().IsActive = true;
        }
    }

}
