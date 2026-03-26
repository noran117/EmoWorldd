using UnityEngine;

public class BookTouchTrigger : MonoBehaviour
{
    public AcceptanceManager acceptanceManager;
    public string handTag = "PlayerHand";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag(handTag)) return;

        triggered = true;

        if (acceptanceManager != null)
            acceptanceManager.OnBookTouched();
    }
}