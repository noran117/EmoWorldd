using UnityEngine;
using System.Collections;
using BNG;

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
    public float disappearDelay = 0.3f;
    public GameObject toy2Prefab;
    public Transform toy2SpawnPoint;

    [Header("Hide This When Toy2 Appears")]
    public GameObject carRootToHide;

    [Header("Hammer In Anger")]
    public GameObject hammerObject;
    public GameObject arrowObject;
    public Transform hammerTransform;
    public Grabbable hammerGrabbable;

    [Header("Hit Control")]
    public float hitCooldown = 0.25f;
    public float minHammerSpeed = 0.5f;

    private int hitCount = 0;
    private bool canBreak = false;
    private bool finished = false;
    private float lastHitTime = -999f;

    private GameObject spawnedToy2;

    void Start()
    {
        UpdateVisual();

        if (hammerObject != null)
            hammerObject.SetActive(true);

        if (arrowObject != null)
            arrowObject.SetActive(false);

        EnableBreaking();
    }

    void Update()
    {
        if (hammerGrabbable != null && hammerGrabbable.BeingHeld)
        {
            HideArrow();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canBreak) return;
        if (finished) return;

        if (Time.time - lastHitTime < hitCooldown)
            return;

        bool hammerHit =
            other.CompareTag("Hammer") ||
            (other.transform.parent != null && other.transform.parent.CompareTag("Hammer")) ||
            (other.transform.root != null && other.transform.root.CompareTag("Hammer"));

        if (!hammerHit)
            return;

        Rigidbody rb = other.attachedRigidbody;

        if (rb == null && other.transform.root != null)
            rb = other.transform.root.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        float speed = rb.linearVelocity.magnitude;
        // float speed = rb.velocity.magnitude;

        if (speed < minHammerSpeed)
            return;

        lastHitTime = Time.time;
        RegisterHit(other);
    }

    public void EnableBreaking()
    {
        canBreak = true;
        ShowArrow();
    }

    void ShowArrow()
    {
        if (arrowObject == null || hammerTransform == null) return;

        arrowScript arrowFollow = arrowObject.GetComponent<arrowScript>();
        if (arrowFollow != null)
        {
            arrowFollow.target = hammerTransform;
        }

        arrowObject.SetActive(true);
    }

    void HideArrow()
    {
        if (arrowObject == null) return;
        arrowObject.SetActive(false);
    }

    void RegisterHit(Collider other)
    {
        if (breakStages == null || breakStages.Length == 0)
            return;

        if (hitCount >= breakStages.Length - 1)
            return;

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
        if (breakStages == null)
            return;

        for (int i = 0; i < breakStages.Length; i++)
        {
            if (breakStages[i] != null)
            {
                bool shouldBeActive = (i == hitCount);
                breakStages[i].SetActive(shouldBeActive);
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
        }
    }

    void IncreaseAngerLight()
    {
        if (roomLight == null)
            return;

        roomLight.color = angerColor;
        roomLight.intensity += lightIncreasePerHit;
    }

    IEnumerator HideAndSpawnToy2()
    {
        yield return new WaitForSeconds(disappearDelay);

        if (toy2Prefab != null && toy2SpawnPoint != null)
        {
            spawnedToy2 = Instantiate(toy2Prefab, toy2SpawnPoint.position, toy2SpawnPoint.rotation);
            spawnedToy2.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Toy1: toy2Prefab أو toy2SpawnPoint Null!");
        }

        HideArrow();

        if (hammerObject != null)
            hammerObject.SetActive(false);

        if (carRootToHide != null)
        {
            carRootToHide.SetActive(false);
        }
        else
        {
            Debug.LogWarning("carRootToHide is NULL!");
        }
    }

    public GameObject GetSpawnedToy2()
    {
        return spawnedToy2;
    }
}