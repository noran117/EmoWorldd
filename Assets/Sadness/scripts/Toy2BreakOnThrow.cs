using UnityEngine;
using System.Collections;
using BNG;

public class Toy2BreakOnThrow : MonoBehaviour
{
    [Header("Broken Model Prefab")]
    public GameObject brokenModelPrefab;

    [Header("Broken Spawn Anchor")]
    public Transform brokenSpawnAnchor;

    [Header("Break Settings")]
    public float nextStateDelay = 0.9f;

    [Header("Audio")]
    public AudioClip breakClip;

    private bool broken = false;

    void Awake()
    {
        if (brokenSpawnAnchor == null)
        {
            GameObject point = GameObject.Find("BrokenSpawnAnchor");

            if (point != null)
            {
                brokenSpawnAnchor = point.transform;
                Debug.Log("Spawn point FOUND");
            }
            else
            {
                Debug.LogWarning("BrokenSpawnAnchor NOT FOUND in scene!");
            }
        }

        if (brokenModelPrefab == null)
        {
            Debug.LogWarning("Broken Model Prefab is NOT assigned!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (broken) return;
        if (!collision.gameObject.CompareTag("Floor")) return;

        Debug.Log("Penguin hit floor → break");

        BreakNow();
    }

    void BreakNow()
    {
        broken = true;

        if (brokenModelPrefab == null)
        {
            Debug.LogWarning("No brokenModelPrefab!");
            return;
        }

        if (brokenSpawnAnchor == null)
        {
            Debug.LogWarning("No BrokenSpawnAnchor!");
            return;
        }

        GameObject spawnedBroken = Instantiate(
            brokenModelPrefab,
            brokenSpawnAnchor.position,
            brokenSpawnAnchor.rotation
        );

        spawnedBroken.SetActive(true);

        Debug.Log("Spawned at: " + spawnedBroken.transform.position);

        if (breakClip != null)
        {
            AudioSource.PlayClipAtPoint(breakClip, spawnedBroken.transform.position);
        }

        Rigidbody[] allRigidbodies = spawnedBroken.GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in allRigidbodies)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        SnapZone[] snapZones = spawnedBroken.GetComponentsInChildren<SnapZone>(true);
        foreach (var zone in snapZones)
        {
            if (zone != null)
                zone.enabled = false;
        }

        StartCoroutine(EnableSnapZonesLater(spawnedBroken));

        DisableOriginalPenguin();

        StartCoroutine(GoNext());
    }

    void DisableOriginalPenguin()
    {
        Collider[] cols = GetComponentsInChildren<Collider>(true);
        foreach (var c in cols)
        {
            if (c != null)
                c.enabled = false;
        }

        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in rbs)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        Renderer[] rends = GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends)
        {
            if (r != null)
                r.enabled = false;
        }

        Grabbable[] grabs = GetComponentsInChildren<Grabbable>(true);
        foreach (var g in grabs)
        {
            if (g != null)
                g.enabled = false;
        }
    }

    IEnumerator EnableSnapZonesLater(GameObject spawnedBroken)
    {
        yield return new WaitForSeconds(0.1f);

        if (spawnedBroken == null) yield break;

        SnapZone[] snapZones = spawnedBroken.GetComponentsInChildren<SnapZone>(true);
        foreach (var zone in snapZones)
        {
            if (zone != null)
                zone.enabled = true;
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