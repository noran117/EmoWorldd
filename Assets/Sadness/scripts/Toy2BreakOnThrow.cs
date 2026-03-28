using UnityEngine;
using System.Collections;
using BNG;

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

        GameObject spawnedBroken = null;

        if (brokenModelPrefab != null)
        {
            spawnedBroken = Instantiate(
                brokenModelPrefab,
                transform.position,
                transform.rotation
            );

            SnapZone[] snapZones = spawnedBroken.GetComponentsInChildren<SnapZone>(true);
            for (int i = 0; i < snapZones.Length; i++)
            {
                if (snapZones[i] != null)
                    snapZones[i].enabled = false;
            }

            StartCoroutine(EnableSnapZonesLater(spawnedBroken));
        }
        else
        {
            Debug.LogWarning("Broken Model Prefab not assigned!");
        }

        Collider[] cols = GetComponentsInChildren<Collider>();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }

        Grabbable grab = GetComponent<Grabbable>();
        if (grab != null)
        {
            grab.enabled = false;
        }

        StartCoroutine(GoNext());
    }

    IEnumerator EnableSnapZonesLater(GameObject spawnedBroken)
    {
        yield return null;

        if (spawnedBroken == null) yield break;

        SnapZone[] snapZones = spawnedBroken.GetComponentsInChildren<SnapZone>(true);

        for (int i = 0; i < snapZones.Length; i++)
        {
            if (snapZones[i] != null)
            {
                snapZones[i].enabled = true;
            }
        }
    }

    IEnumerator GoNext()
    {
        yield return new WaitForSeconds(nextStateDelay);

        gameObject.SetActive(false);

        GameStateManager.Instance.ChangeState(GameState.Bargaining);
    }
}