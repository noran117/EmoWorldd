using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] notes;
    public Transform[] points;

    public float bpm = 130f;
    public int spawnEveryBeats = 2;

    float beatInterval;
    Coroutine spawnRoutine;

    void Start()
    {
        beatInterval = (60f / bpm) * spawnEveryBeats;
        Debug.Log("Spawner START | notes = " + notes.Length + " | points = " + points.Length);
    }

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
            Spawn();
            yield return new WaitForSeconds(beatInterval);
        }
    }

    void Spawn()
    {
        if (notes == null || notes.Length == 0)
        {
            Debug.LogError("notes array is empty!");
            return;
        }

        if (points == null || points.Length == 0)
        {
            Debug.LogError("points array is empty!");
            return;
        }

        var prefab = notes[Random.Range(0, notes.Length)];
        var p = points[Random.Range(0, points.Length)];

        if (prefab == null || p == null)
        {
            Debug.LogError("Prefab or SpawnPoint is null!");
            return;
        }

        GameObject note = Instantiate(prefab, p.position, p.rotation);

        note.transform.Rotate(0f, 90f, 0f);
        Debug.Log("Spawned note: " + note.name);
    }
}