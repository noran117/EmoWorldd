using UnityEngine;
using System.Collections;

public class Toy2BreakOnThrow : MonoBehaviour
{
    [Header("Broken Model Prefab")]
    public GameObject brokenModelPrefab;

    [Header("Break Settings")]
    public float breakImpactThreshold = 2.0f;
    public float nextStateDelay = 0.5f;

    private bool broken = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (broken) return;

        if (!collision.gameObject.CompareTag("Floor")) return;

        float impact = collision.relativeVelocity.magnitude;

        if (impact >= breakImpactThreshold)
        {
            BreakNow();
        }
    }

    void BreakNow()
    {
        broken = true;

        if (brokenModelPrefab != null)
        {
            Instantiate(
                brokenModelPrefab,
                transform.position,
                transform.rotation
            );
        }
        else
        {
            Debug.LogWarning("Broken Model Prefab not assigned!");
        }

        gameObject.SetActive(false);

        StartCoroutine(GoNext());
    }

    IEnumerator GoNext()
    {
        yield return new WaitForSeconds(nextStateDelay);
        GameStateManager.Instance.ChangeState(GameState.Bargaining);
    }
}
