using UnityEngine;
using System.Collections;
using BNG;

public class Toy2BreakOnThrow : MonoBehaviour
{
    [Header("Broken Model Prefab")]
    public GameObject brokenModelPrefab;

    [Header("Break Settings")]
    public float breakImpactThreshold = 2.0f; // حالياً غير مستخدم
    public float nextStateDelay = 0.5f;

    [Header("Broken Spawn Fix")]
    public float brokenSpawnYOffset = 0.05f;

    private bool broken = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (broken) return;
        if (!collision.gameObject.CompareTag("Floor")) return;

        Debug.Log("=== PENGUIN HIT FLOOR ===");
        Debug.Log("Healthy penguin position = " + transform.position);
        Debug.Log("Hit floor object = " + collision.gameObject.name);
        Debug.Log("Contact count = " + collision.contactCount);

        BreakNow(collision);
    }

    void BreakNow(Collision collision)
    {
        broken = true;

        GameObject spawnedBroken = null;

        if (brokenModelPrefab != null)
        {
            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = transform.rotation;

            Debug.Log("Initial spawnPosition from healthy penguin = " + spawnPosition);

            if (collision != null && collision.contactCount > 0)
            {
                ContactPoint contact = collision.GetContact(0);

                float heightOffset = brokenSpawnYOffset;

                Collider col = brokenModelPrefab.GetComponentInChildren<Collider>();
                if (col != null)
                {
                    heightOffset += col.bounds.extents.y;
                }

                Debug.Log("Contact point = " + contact.point);
                Debug.Log("Height offset used = " + heightOffset);

                spawnPosition = contact.point + Vector3.up * heightOffset;
            }

            Debug.Log("FINAL broken penguin spawnPosition = " + spawnPosition);
            Debug.DrawLine(transform.position, spawnPosition, Color.red, 5f);

            spawnedBroken = Instantiate(
                brokenModelPrefab,
                spawnPosition,
                spawnRotation
            );

            spawnedBroken.SetActive(true);

            Debug.Log("Broken penguin spawned name = " + spawnedBroken.name);
            Debug.Log("Broken penguin actual world position = " + spawnedBroken.transform.position);

            Rigidbody[] allRigidbodies = spawnedBroken.GetComponentsInChildren<Rigidbody>(true);
            for (int i = 0; i < allRigidbodies.Length; i++)
            {
                if (allRigidbodies[i] != null)
                {
                    allRigidbodies[i].linearVelocity = Vector3.zero;
                    allRigidbodies[i].angularVelocity = Vector3.zero;
                    allRigidbodies[i].isKinematic = true;
                    allRigidbodies[i].useGravity = false;
                }
            }

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

        Collider[] cols = GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }

        Rigidbody[] originalRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < originalRigidbodies.Length; i++)
        {
            if (originalRigidbodies[i] != null)
            {
                originalRigidbodies[i].linearVelocity = Vector3.zero;
                originalRigidbodies[i].angularVelocity = Vector3.zero;
                originalRigidbodies[i].isKinematic = true;
                originalRigidbodies[i].useGravity = false;
            }
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }

        Grabbable[] grabs = GetComponentsInChildren<Grabbable>(true);
        for (int i = 0; i < grabs.Length; i++)
        {
            if (grabs[i] != null)
                grabs[i].enabled = false;
        }

        StartCoroutine(GoNext());
    }

    IEnumerator EnableSnapZonesLater(GameObject spawnedBroken)
    {
        yield return new WaitForSeconds(0.1f);

        if (spawnedBroken == null) yield break;

        SnapZone[] snapZones = spawnedBroken.GetComponentsInChildren<SnapZone>(true);
        for (int i = 0; i < snapZones.Length; i++)
        {
            if (snapZones[i] != null)
                snapZones[i].enabled = true;
        }
    }

    IEnumerator GoNext()
    {
        yield return new WaitForSeconds(nextStateDelay);

        gameObject.SetActive(false);

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Bargaining);
        }
    }
}