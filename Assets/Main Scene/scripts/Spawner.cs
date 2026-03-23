using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class NotePrefabData
    {
        public GameObject prefab;
        [Range(1, 10)] public int weight = 1;
    }

    [System.Serializable]
    public class SpawnPointData
    {
        public Transform point;
        public bool enabled = true;
    }

    [Header("Notes")]
    public List<NotePrefabData> notes = new List<NotePrefabData>();

    [Header("Spawn Points")]
    public List<SpawnPointData> points = new List<SpawnPointData>();

    [Header("Spawn Timing")]
    public float minSpawnDelay = 0.4f;
    public float maxSpawnDelay = 0.8f;

    [Header("Limits")]
    public int maxActiveNotes = 8;

    [Header("Debug")]
    public bool randomRotationY = false;

    Coroutine spawnRoutine;
    List<GameObject> activeNotes = new List<GameObject>();

    public void StartSpawning()
    {
        if (spawnRoutine != null) return;

        Debug.Log("StartSpawning()");
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        Debug.Log("StopSpawning()");
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            CleanupNulls();

            if (activeNotes.Count < maxActiveNotes)
            {
                Spawn();
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    void Spawn()
    {
        GameObject prefab = GetRandomNotePrefab();
        Transform spawnPoint = GetRandomSpawnPoint();

        if (prefab == null)
        {
            Debug.LogWarning("No note prefab available!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("No enabled spawn point available!");
            return;
        }

        Quaternion rot = spawnPoint.rotation;

        if (randomRotationY)
        {
            rot *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }

        GameObject note = Instantiate(prefab, spawnPoint.position, rot);
        note.transform.Rotate(0f, 90f, 0f);

        activeNotes.Add(note);

        Debug.Log("Spawned note: " + note.name + " at " + spawnPoint.name);
    }

    GameObject GetRandomNotePrefab()
    {
        if (notes == null || notes.Count == 0) return null;

        int totalWeight = 0;
        foreach (var n in notes)
        {
            if (n != null && n.prefab != null)
                totalWeight += Mathf.Max(1, n.weight);
        }

        if (totalWeight == 0) return null;

        int rand = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var n in notes)
        {
            if (n == null || n.prefab == null) continue;

            current += Mathf.Max(1, n.weight);
            if (rand < current)
                return n.prefab;
        }

        return null;
    }

    Transform GetRandomSpawnPoint()
    {
        List<Transform> enabledPoints = new List<Transform>();

        foreach (var p in points)
        {
            if (p != null && p.point != null && p.enabled)
                enabledPoints.Add(p.point);
        }

        if (enabledPoints.Count == 0) return null;

        return enabledPoints[Random.Range(0, enabledPoints.Count)];
    }

    void CleanupNulls()
    {
        activeNotes.RemoveAll(n => n == null);
    }

    public void ClearAllNotes()
    {
        CleanupNulls();

        foreach (var n in activeNotes)
        {
            if (n != null)
                Destroy(n);
        }

        activeNotes.Clear();
    }
}