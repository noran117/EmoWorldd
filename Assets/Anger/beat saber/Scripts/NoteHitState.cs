using UnityEngine;

public class NoteHitState : MonoBehaviour
{
    public bool canBeHit;
    public bool wasHit;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitZone"))
        {
            canBeHit = true;
            Debug.Log(gameObject.name + " entered HitZone");
        }

        if (other.CompareTag("DestroyZone"))
        {
            Debug.Log(gameObject.name + " entered DestroyZone");
            Destroy(gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HitZone"))
        {
            canBeHit = false;
            Debug.Log(gameObject.name + " exited HitZone");
        }
    }
}