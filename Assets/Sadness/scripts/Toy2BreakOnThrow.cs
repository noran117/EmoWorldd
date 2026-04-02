using UnityEngine;
using System.Collections;
using BNG;

public class Toy2BreakOnThrow : MonoBehaviour
{
    [Header("Broken Model Prefab")]
    public GameObject brokenModelPrefab;

    [Header("Broken Spawn Point")]
    public Transform brokenSpawnPoint;

    [Header("Break Settings")]
    public float nextStateDelay = 0.9f;

    [Header("Audio")]
    public AudioClip breakClip;

    private bool broken = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (broken) return;
        if (!collision.gameObject.CompareTag("Floor")) return;

        BreakNow();
    }

    void BreakNow()
    {
        broken = true;

        GameObject spawnedBroken = null;

        if (brokenModelPrefab != null && brokenSpawnPoint != null)
        {
            spawnedBroken = Instantiate(
                brokenModelPrefab,
                brokenSpawnPoint.position,
                brokenSpawnPoint.rotation
            );

            spawnedBroken.SetActive(true);

            if (breakClip != null)
            {
                AudioSource.PlayClipAtPoint(breakClip, spawnedBroken.transform.position);
            }

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
                {
                    snapZones[i].enabled = false;
                }
            }

            StartCoroutine(EnableSnapZonesLater(spawnedBroken));
        }
        else
        {
            Debug.LogWarning("Broken Model Prefab or Broken Spawn Point is missing!");
        }

        DisableOriginalPenguin();

        StartCoroutine(GoNext());
    }

    void DisableOriginalPenguin()
    {
        Collider[] cols = GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] != null)
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
            if (renderers[i] != null)
                renderers[i].enabled = false;
        }

        Grabbable[] grabs = GetComponentsInChildren<Grabbable>(true);
        for (int i = 0; i < grabs.Length; i++)
        {
            if (grabs[i] != null)
                grabs[i].enabled = false;
        }
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