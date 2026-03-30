using UnityEngine;
using System.Collections;
using BNG;

public class Toy2BreakOnThrow : MonoBehaviour
{
    [Header("Broken Model Prefab")]
    public GameObject brokenModelPrefab;

    [Header("Break Settings")]
    public float nextStateDelay = 0.9f;

    [Header("Broken Spawn Settings")]
    public Vector3 brokenSpawnOffset = Vector3.zero;

    [Header("Ground Ray Settings")]
    public float rayStartHeight = 3f;
    public float rayDistance = 20f;

    [Header("Audio")]
    public AudioClip breakClip;

    private bool broken = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (broken) return;
        if (!collision.gameObject.CompareTag("Floor")) return;

        BreakNow(collision);
    }

    void BreakNow(Collision collision)
    {
        broken = true;

        GameObject spawnedBroken = null;

        if (brokenModelPrefab != null)
        {
            // 1) نبدأ من مكان البنجوين السليم الحقيقي
            Vector3 spawnPosition = transform.position;

            // 2) نجيب الأرض من نفس Collider الأرض اللي خبط فيه
            //    هذا أدق من contact.point لوحده
            Ray ray = new Ray(
                new Vector3(transform.position.x, transform.position.y + rayStartHeight, transform.position.z),
                Vector3.down
            );

            RaycastHit floorHit;
            if (collision.collider.Raycast(ray, out floorHit, rayDistance))
            {
                spawnPosition = new Vector3(transform.position.x, floorHit.point.y, transform.position.z);
            }
            else if (collision.contactCount > 0)
            {
                // fallback
                ContactPoint contact = collision.GetContact(0);
                spawnPosition = new Vector3(transform.position.x, contact.point.y, transform.position.z);
            }

            // 3) نفس دوران الجسم الحالي
            Quaternion spawnRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

            // 4) اعمل Spawn
            spawnedBroken = Instantiate(brokenModelPrefab, spawnPosition, spawnRotation);
            spawnedBroken.SetActive(true);

            // 5) أول تصحيح: خلي أسفل المجسم يلامس الأرض
            SnapObjectToFloor(spawnedBroken, spawnPosition.y);

            // 6) Offset إضافي من Inspector
            spawnedBroken.transform.position += brokenSpawnOffset;

            // 7) إعادة تصحيح مرة ثانية بعد الـ offset
            float targetFloorY = spawnPosition.y + brokenSpawnOffset.y;
            SnapObjectToFloor(spawnedBroken, targetFloorY);

            if (breakClip != null)
            {
                AudioSource.PlayClipAtPoint(breakClip, spawnedBroken.transform.position);
            }

            // وقف الفيزيكس أول لحظة حتى ما يطير
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

            // عطل الـ SnapZones مؤقتاً
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

        // عطّل البنجوين السليم
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

    void SnapObjectToFloor(GameObject obj, float floorY)
    {
        if (obj == null) return;

        float bottomY;
        if (TryGetLowestColliderY(obj, out bottomY))
        {
            float yFix = floorY - bottomY;
            obj.transform.position += new Vector3(0f, yFix, 0f);
            return;
        }

        if (TryGetLowestRendererY(obj, out bottomY))
        {
            float yFix = floorY - bottomY;
            obj.transform.position += new Vector3(0f, yFix, 0f);
        }
    }

    bool TryGetLowestColliderY(GameObject obj, out float bottomY)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);

        bool found = false;
        bottomY = 0f;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider c = colliders[i];
            if (c == null) continue;
            if (!c.enabled) continue;

            if (!found)
            {
                bottomY = c.bounds.min.y;
                found = true;
            }
            else
            {
                if (c.bounds.min.y < bottomY)
                    bottomY = c.bounds.min.y;
            }
        }

        return found;
    }

    bool TryGetLowestRendererY(GameObject obj, out float bottomY)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);

        bool found = false;
        bottomY = 0f;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null) continue;
            if (!r.enabled) continue;

            if (!found)
            {
                bottomY = r.bounds.min.y;
                found = true;
            }
            else
            {
                if (r.bounds.min.y < bottomY)
                    bottomY = r.bounds.min.y;
            }
        }

        return found;
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