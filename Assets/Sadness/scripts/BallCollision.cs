using UnityEngine;
using System;

public class BallCollision : MonoBehaviour
{
    public event Action OnBallHit;

    // ≈–« ﬂ«‰ «·ﬂÊ·«Ìœ— ··„‰ÿﬁ… Normal Collider
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EventArea"))
        {
            OnBallHit?.Invoke();
        }
    }

    // ≈–« «” Œœ„  Trigger ··ﬂÊ·«Ìœ—
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EventArea"))
        {
            OnBallHit?.Invoke();
        }
    }
}
