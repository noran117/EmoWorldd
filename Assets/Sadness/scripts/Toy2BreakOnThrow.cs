using UnityEngine;
using System.Collections;
using BNG;

public class Toy2BreakOnThrow : MonoBehaviour
{
    [Header("Broken Model Prefab")]
    public GameObject brokenModelObject;

    [Header("Arrow Settings")]
    public GameObject arrowObject;

    [Header("Break Settings")]
    public float nextStateDelay = 0.9f;

    [Header("Audio")]
    public AudioClip breakClip;

    private bool broken = false;

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

        if (brokenModelObject == null)
            return;
        if (arrowObject != null)
            arrowObject.SetActive(false);

        brokenModelObject.SetActive(true);

        if (breakClip != null)
        {
            AudioSource.PlayClipAtPoint(breakClip, brokenModelObject.transform.position);
        }

        Rigidbody[] allRigidbodies = brokenModelObject.GetComponentsInChildren<Rigidbody>(true);
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

        SnapZone[] snapZones = brokenModelObject.GetComponentsInChildren<SnapZone>(true);
        foreach (var zone in snapZones)
        {
            if (zone != null)
                zone.enabled = false;
        }

        StartCoroutine(EnableSnapZonesLater(brokenModelObject));

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