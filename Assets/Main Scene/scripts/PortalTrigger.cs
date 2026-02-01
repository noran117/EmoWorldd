using UnityEngine;
using UnityEngine.Events;

public class PortalTrigger : MonoBehaviour
{
    public UnityEvent onEnter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onEnter.Invoke();
        }
    }
}
