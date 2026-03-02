using UnityEngine;
using System;

public class BallCollision : MonoBehaviour
{
    public event Action OnBallHit;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EventArea"))
        {
            OnBallHit?.Invoke();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EventArea"))
        {
            OnBallHit?.Invoke();
        }
    }
}
