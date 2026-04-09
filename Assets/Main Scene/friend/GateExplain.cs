using UnityEngine;

public class GateExplain : MonoBehaviour
{
    public followPlayer companion;   // الرفيق
    public Transform player;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Triggered {other.gameObject.name}");
        if (other.CompareTag("Player"))
        {        Debug.Log("Match Tag");

            companion.ExplainGates(player);
            
        }
    }
}