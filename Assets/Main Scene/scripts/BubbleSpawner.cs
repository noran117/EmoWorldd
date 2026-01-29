using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [Header("Bubble Prefab & Pool")]
    public GameObject bubblePrefab;
    public int poolSize = 10;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public float spawnRange = 0.5f;
    public int maxBubblesActive = 10;
    public AudioSource popAudioSource;

    [Header("Plane Reference")]
    public Transform plane;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private int activeBubbles = 0;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject b = Instantiate(bubblePrefab);
            b.SetActive(false);
            pool.Enqueue(b);
        }

        InvokeRepeating(nameof(TrySpawnBubble), spawnInterval, spawnInterval);
    }

    void TrySpawnBubble()
    {
        if (activeBubbles >= maxBubblesActive) return;
        if (pool.Count == 0) return;

        GameObject bubble = pool.Dequeue();

        Vector3 origin = plane.position;

        Vector3 randomOffset = Random.insideUnitSphere * spawnRange;
        randomOffset.y = 0.2f;

        bubble.transform.position = origin + randomOffset;

        bubble.SetActive(true);
        bubble.GetComponent<BubbleBehavior>().Init(this);

        activeBubbles++;
    }

    public void ReturnToPool(GameObject bubble)
    {
        bubble.SetActive(false);
        pool.Enqueue(bubble);
        activeBubbles--;
    }
}
