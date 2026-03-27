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

    private GameObject spawnedToy2;

    void Start()
    {
        UpdateVisual();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBreak) return;
        if (finished) return;
        if (Time.time - lastHitTime < hitCooldown) return;
        if (!other.CompareTag("Hammer")) return;

        lastHitTime = Time.time;
        RegisterHit(other);
    }

    public void EnableBreaking()
    {
        canBreak = true;
    }

    void RegisterHit(Collider other)
    {
        if (breakStages == null || breakStages.Length == 0) return;
        if (hitCount >= breakStages.Length - 1) return;

        hitCount++;
        UpdateVisual();
        PlayEffects(other);
        IncreaseAngerLight();

        if (hitCount == breakStages.Length - 1)
        {
            finished = true;
            StartCoroutine(HideAndSpawnToy2());
        }
    }

    void UpdateVisual()
    {
        if (breakStages == null) return;

        for (int i = 0; i < breakStages.Length; i++)
        {
            if (breakStages[i] != null)
                breakStages[i].SetActive(i == hitCount);
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
        }
    }

    void IncreaseAngerLight()
    {
        if (roomLight == null) return;

        roomLight.color = angerColor;
        roomLight.intensity += lightIncreasePerHit;
    }

    IEnumerator HideAndSpawnToy2()
    {
        yield return new WaitForSeconds(disappearDelay);

        gameObject.SetActive(false);

        if (toy2Prefab != null && toy2SpawnPoint != null)
        {
            spawnedToy2 = Instantiate(toy2Prefab, toy2SpawnPoint.position, toy2SpawnPoint.rotation);
            spawnedToy2.SetActive(true);
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