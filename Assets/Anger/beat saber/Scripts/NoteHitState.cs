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
        }

        if (other.CompareTag("DestroyZone"))
        {
            DestroyNow();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HitZone"))
        {
            canBeHit = false;
        }
    }

    public void DestroyNow()
    {
        Debug.Log("Destroying: " + gameObject.name);
        Destroy(gameObject);
    }
}