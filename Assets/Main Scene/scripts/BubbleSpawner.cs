using System.Collections;
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
    public AudioSource popAudioSource;

[Header("Plane Reference")]
    public Transform plane;

    private Queue<BubbleBehavior> pool = new Queue<BubbleBehavior>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            BubbleBehavior bubble = Instantiate(bubblePrefab).GetComponent<BubbleBehavior>();

            bubble.gameObject.SetActive(false);
            pool.Enqueue(bubble);
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            TrySpawnBubble();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void TrySpawnBubble()
    {
        if (pool.Count == 0) return;

        BubbleBehavior bubble = pool.Dequeue();

        Vector2 randomCircle = Random.insideUnitCircle * spawnRange;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0.2f, randomCircle.y);

        bubble.transform.position = plane.position + randomOffset;
        bubble.gameObject.SetActive(true);
        bubble.Init(this);
    }

    public void ReturnToPool(BubbleBehavior bubble)
    {
        bubble.gameObject.SetActive(false);
        pool.Enqueue(bubble);
    }
}