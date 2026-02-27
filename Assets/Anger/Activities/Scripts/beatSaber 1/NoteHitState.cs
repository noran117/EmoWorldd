using UnityEngine;

public class NoteHitState : MonoBehaviour
{
    public bool canBeHit;
    public bool wasHit;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitZone"))
            canBeHit = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("HitZone")) return;

        canBeHit = false;

        if (!wasHit)
        {
            // Miss (اختياري: نداء GameManager)
            // GameManager.Instance.Miss();
        }

        Destroy(gameObject);
    }
}