using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] notes;
    public Transform[] points;

    public float bpm = 130f;
    public int spawnEveryBeats = 2;

    float beatInterval;
    float timer;
    bool canSpawn = false;

    void Start()
    {
        beatInterval = (60f / bpm) * spawnEveryBeats;
    }

    void Update()
    {
        if (!canSpawn) return;

        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            Spawn();
            timer = 0f;
        }
    }

    public void SetSpawnerRunning(bool value)
    {
        canSpawn = value;
        if (!value) timer = 0f;
    }

    void Spawn()
    {
        if (notes == null || notes.Length == 0) return;
        if (points == null || points.Length == 0) return;

        var prefab = notes[Random.Range(0, notes.Length)];
        var p = points[Random.Range(0, points.Length)];

        GameObject note = Instantiate(prefab, p.position, p.rotation);
        note.transform.Rotate(Vector3.forward, 90f * Random.Range(0, 4), Space.Self);
    }
}