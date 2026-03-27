using UnityEngine;
using System.Collections;

public class ToyBreakableInAngerState : MonoBehaviour
{
    [Header("Breakable Settings (Toy 1)")]
    public GameObject[] breakStages;
    public ParticleSystem hitParticles;

    [Header("Lighting")]
    public Light roomLight;
    public Color angerColor = Color.red;
    public float lightIncreasePerHit = 0.3f;

    [Header("After Toy1 Broken -> Spawn Toy2")]
    public float disappearDelay = 1.0f;
    public GameObject toy2Prefab;
    public Transform toy2SpawnPoint;

    [Header("Hit Control")]
    public float hitCooldown = 0.25f;

    private int hitCount = 0;
    private bool canBreak = false;
    private bool finished = false;
    private float lastHitTime = -999f;

    private int triggerEnterCount = 0;

    private GameObject spawnedToy2;

    void Start()
    {
        Debug.Log("ToyBreakableInAngerState START on: " + gameObject.name);
        UpdateVisual();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("=== ON TRIGGER ENTER WORKED ===");
        Debug.Log("Entered by: " + other.gameObject.name);
        Debug.Log("Tag: " + other.tag);
        Debug.Log("Parent: " + (other.transform.parent != null ? other.transform.parent.name : "NO PARENT"));
        Debug.Log("Root: " + other.transform.root.name);

        if (!canBreak)
        {
            Debug.Log("STOP: canBreak = false");
            return;
        }

        if (finished)
        {
            Debug.Log("STOP: finished = true");
            return;
        }

        if (Time.time - lastHitTime < hitCooldown)
        {
            Debug.Log("STOP: cooldown");
            return;
        }

        bool hammerHit =
            other.CompareTag("Hammer") ||
            (other.transform.parent != null && other.transform.parent.CompareTag("Hammer")) ||
            (other.transform.root != null && other.transform.root.CompareTag("Hammer"));

        if (!hammerHit)
        {
            Debug.Log("STOP: not Hammer or Hammer parent/root");
            return;
        }

        Debug.Log("VALID HIT");
        lastHitTime = Time.time;
        RegisterHit(other);
    }
    public void EnableBreaking()
    {
        canBreak = true;
        Debug.Log("EnableBreaking CALLED on: " + gameObject.name);
    }

    void RegisterHit(Collider other)
    {
        Debug.Log("=== REGISTER HIT WORKED ===");
        Debug.Log("Current hitCount before = " + hitCount);

        if (breakStages == null || breakStages.Length == 0)
        {
            Debug.LogWarning("STOP: breakStages empty");
            return;
        }

        if (hitCount >= breakStages.Length - 1)
        {
            Debug.Log("STOP: already at last stage");
            return;
        }

        hitCount++;
        Debug.Log("HIT REGISTERED -> hitCount = " + hitCount);

        UpdateVisual();
        PlayEffects(other);
        IncreaseAngerLight();

        if (hitCount == breakStages.Length - 1)
        {
            finished = true;
            Debug.Log("FINAL STAGE reached");
            StartCoroutine(HideAndSpawnToy2());
        }
    }

    void UpdateVisual()
    {
        if (breakStages == null)
        {
            Debug.LogWarning("UpdateVisual: breakStages is null");
            return;
        }

        Debug.Log("UpdateVisual CALLED. Active stage index = " + hitCount);

        for (int i = 0; i < breakStages.Length; i++)
        {
            if (breakStages[i] != null)
            {
                bool shouldBeActive = (i == hitCount);
                breakStages[i].SetActive(shouldBeActive);

                Debug.Log("Stage " + i + " = " + breakStages[i].name + " -> Active: " + shouldBeActive);
            }
            else
            {
                Debug.LogWarning("Stage " + i + " is NULL");
            }
        }
    }

    void PlayEffects(Collider other)
    {
        if (hitParticles != null)
        {
            Vector3 pos = transform.position;

            if (other != null)
                pos = other.ClosestPoint(transform.position);

            hitParticles.transform.position = pos;
            hitParticles.Play();

            Debug.Log("Hit particles PLAY at position: " + pos);
        }
        else
        {
            Debug.Log("hitParticles is NULL");
        }
    }

    void IncreaseAngerLight()
    {
        if (roomLight == null)
        {
            Debug.Log("roomLight is NULL");
            return;
        }

        roomLight.color = angerColor;
        roomLight.intensity += lightIncreasePerHit;

        Debug.Log("Light changed. New intensity = " + roomLight.intensity);
    }

    IEnumerator HideAndSpawnToy2()
    {
        Debug.Log("HideAndSpawnToy2 STARTED");

        yield return new WaitForSeconds(disappearDelay);

        Debug.Log("Hiding object: " + gameObject.name);
        gameObject.SetActive(false);

        if (toy2Prefab != null && toy2SpawnPoint != null)
        {
            spawnedToy2 = Instantiate(toy2Prefab, toy2SpawnPoint.position, toy2SpawnPoint.rotation);
            spawnedToy2.SetActive(true);

            Debug.Log("Toy2 spawned successfully: " + spawnedToy2.name);
        }
        else
        {
            Debug.LogWarning("Toy1: toy2Prefab أو toy2SpawnPoint Null!");
        }
    }

    public GameObject GetSpawnedToy2()
    {
        return spawnedToy2;
    }
}