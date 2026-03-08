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
            Debug.Log("SnapZones found = " + snapZones.Length);

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

        gameObject.SetActive(false);

        StartCoroutine(GoNext());
    }

    IEnumerator EnableSnapZonesLater(GameObject spawnedBroken)
    {
        yield return null;

        if (spawnedBroken == null) yield break;

        SnapZone[] snapZones = spawnedBroken.GetComponentsInChildren<SnapZone>(true);
        Debug.Log("Enabling SnapZones Count = " + snapZones.Length);

        for (int i = 0; i < snapZones.Length; i++)
        {
            if (snapZones[i] != null)
            {
                snapZones[i].enabled = true;
                Debug.Log("Enabled SnapZone on: " + snapZones[i].gameObject.name);
            }
        }
    }

    IEnumerator GoNext()
    {
        yield return new WaitForSeconds(nextStateDelay);
        GameStateManager.Instance.ChangeState(GameState.Bargaining);
    }
}