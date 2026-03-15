using UnityEngine;

public class GateExplain : MonoBehaviour
{
    public followPlayer companion;   // الرفيق
    public Transform player;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            companion.ExplainGates(player);
        }
    }
}