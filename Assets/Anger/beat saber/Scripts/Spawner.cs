using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
/// <summary>
/// Drop-in replacement for Spawner.cs
/// Detects beats from the AudioSource in SaberGameManager and spawns notes in sync.
///
/// HOW TO USE:
///   1. Remove (or disable) your old Spawner component.
///   2. Add this BeatSpawner component to the same GameObject.
///   3. In SaberGameManager, assign this as the Spawner reference (it still calls
///      StartSpawning / StopSpawning — same API).
///   4. Assign your note prefabs, spawn points, and the gameMusic AudioSource
///      in the Inspector.
/// </summary>

    // ── Reuse the same data classes so the Inspector looks identical ──────────

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

    // ── Inspector fields ──────────────────────────────────────────────────────

    [Header("Notes")]
    public List<NotePrefabData> notes = new List<NotePrefabData>();

    [Header("Spawn Points")]
    public List<SpawnPointData> points = new List<SpawnPointData>();

    [Header("Beat Detection")]
    [Tooltip("AudioSource that plays the game music (same one in SaberGameManager).")]
    public AudioSource gameMusic;

    [Tooltip("Beats Per Minute of the track. 117.45 for the cyberwave track.")]
    public float bpm = 117.45f;

    [Tooltip("How many beats to skip between spawns. 1 = every beat, 2 = every other beat.")]
    [Range(1, 4)] public int spawnEveryNBeats = 1;

    [Tooltip("Add a tiny random offset (seconds) so not every cube arrives at exactly the same instant.")]
    public float jitter = 0.05f;

    [Header("Limits")]
    public int maxActiveNotes = 8;

    [Header("Debug")]
    public bool randomRotationY = false;

    // ── Private state ─────────────────────────────────────────────────────────

    Coroutine spawnRoutine;
    List<GameObject> activeNotes = new List<GameObject>();

    float beatInterval;        // seconds between beats
    float nextBeatTime;        // DSP time of the next expected beat
    int   beatCounter;         // counts beats so we can skip N

    // ─────────────────────────────────────────────────────────────────────────
    // Public API  (same as old Spawner so SaberGameManager needs no changes)
    // ─────────────────────────────────────────────────────────────────────────

    public void StartSpawning()
    {
        if (spawnRoutine != null) return;

        // Recalculate in case bpm was changed at runtime
        beatInterval = 60f / bpm;
        beatCounter  = 0;

        // Align the first beat to right now (music should already be playing)
        nextBeatTime = (float)AudioSettings.dspTime;

        spawnRoutine = StartCoroutine(BeatLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    public void ClearAllNotes()
    {
        CleanupNulls();
        foreach (var n in activeNotes)
            if (n != null) Destroy(n);
        activeNotes.Clear();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Core beat loop
    // ─────────────────────────────────────────────────────────────────────────

    IEnumerator BeatLoop()
    {
        while (true)
        {
            // Wait until DSP clock reaches the next beat
            double waitUntil = nextBeatTime;
            while (AudioSettings.dspTime < waitUntil)
                yield return null;   // check every frame — very tight sync

            // Advance to the beat after this one
            nextBeatTime += beatInterval;
            beatCounter++;

            // Only spawn on every Nth beat
            if (beatCounter % spawnEveryNBeats != 0)
                continue;

            CleanupNulls();

            if (activeNotes.Count < maxActiveNotes)
            {
                // Optional tiny jitter so cubes don't look robotic
                if (jitter > 0f)
                    yield return new WaitForSeconds(Random.Range(0f, jitter));

                Spawn();
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Spawn helpers  (identical logic to original Spawner.cs)
    // ─────────────────────────────────────────────────────────────────────────

    void Spawn()
    {
        GameObject prefab      = GetRandomNotePrefab();
        Transform  spawnPoint  = GetRandomSpawnPoint();

        if (prefab == null)
        {
            Debug.LogWarning("BeatSpawner: No note prefab available!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("BeatSpawner: No enabled spawn point!");
            return;
        }

        Quaternion rot = spawnPoint.rotation;
        if (randomRotationY)
            rot *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject note = Instantiate(prefab, spawnPoint.position, rot);
        note.transform.Rotate(0f, 90f, 0f);
        activeNotes.Add(note);

        Debug.Log($"[BeatSpawner] Beat #{beatCounter} → spawned {note.name}");
    }

    GameObject GetRandomNotePrefab()
    {
        if (notes == null || notes.Count == 0) return null;

        int totalWeight = 0;
        foreach (var n in notes)
            if (n?.prefab != null) totalWeight += Mathf.Max(1, n.weight);

        if (totalWeight == 0) return null;

        int rand = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var n in notes)
        {
            if (n?.prefab == null) continue;
            current += Mathf.Max(1, n.weight);
            if (rand < current) return n.prefab;
        }

        return null;
    }

    Transform GetRandomSpawnPoint()
    {
        var enabled = new List<Transform>();
        foreach (var p in points)
            if (p?.point != null && p.enabled) enabled.Add(p.point);

        return enabled.Count == 0 ? null : enabled[Random.Range(0, enabled.Count)];
    }

    void CleanupNulls()
    {
        activeNotes.RemoveAll(n => n == null);
    }

}