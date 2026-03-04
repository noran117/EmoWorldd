using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reads baked color data from MaterialColorData and reveals all materials
/// back to their original colors using one of two effects:
///
///   RevealMagical()  — Perlin-noise shimmer burst, fades cleanly at start/end
///   RevealSmooth()   — Pure smoothstep lerp, no flicker, calm and clean
///
/// Optimizations:
///   • Single coroutine drives ALL materials — no per-material coroutine overhead.
///   • Materials batched into a flat struct array (no boxing, no dictionary hits per frame).
///   • Emission re-enable deferred to first frame of the coroutine.
///   • WaitForEndOfFrame reused (cached).
/// </summary>
[RequireComponent(typeof(MaterialColorData))]
public class MaterialReveal : MonoBehaviour
{
    [Header("Timing")]
    public float revealDuration  = 2.5f;
    [Tooltip("Max random start delay per material, creates a staggered wave.")]
    public float maxStagger      = 0.4f;
    public bool  autoReveal      = true;
    public float autoRevealDelay = 0.5f;

    [Header("Magical Effect")]
    [Range(0f, 1f)] public float noiseIntensity = 0.35f;
    public float noiseSpeed = 3f;

    // ── Internals ─────────────────────────────────────────────────────────────

    private MaterialColorData _data;
    private Coroutine         _activeRoutine;

    // Flat job-like struct — avoids per-frame dictionary lookups in the hot loop.
    private struct MatJob
    {
        public Material mat;
        public Color    startColor;
        public Color    targetColor;
        public bool     hasEmission;
        public Color    startEmission;
        public Color    targetEmission;
        public float    startTime;        // absolute time this material begins
        public float    noiseOffX;
        public float    noiseOffY;
    }

    private static readonly WaitForEndOfFrame _waitEOF = new WaitForEndOfFrame();

    // ── Unity ─────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _data = GetComponent<MaterialColorData>();
    }

    private void Start()
    {
        if (!autoReveal) return;
        StartCoroutine(AutoStart());
    }

    private IEnumerator AutoStart()
    {
        if (autoRevealDelay > 0f) yield return new WaitForSeconds(autoRevealDelay);
        RevealMagical();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Perlin-noise shimmer that bursts mid-transition then settles.</summary>
    public void RevealMagical()  => StartEffect(magical: true);

    /// <summary>Smooth, flicker-free smoothstep lerp to original colors.</summary>
    public void RevealSmooth()   => StartEffect(magical: false);

    // ── Core ──────────────────────────────────────────────────────────────────

    private void StartEffect(bool magical)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(magical ? RunMagical() : RunSmooth());
    }

    /// <summary>Build the flat job array from baked snapshot data.</summary>
    private MatJob[] BuildJobs()
    {
        var snaps = _data.snapshots;
        // Pre-count so we allocate exactly once.
        int total = 0;
        foreach (var s in snaps) if (s.renderer != null) total += s.entries.Length;

        var jobs = new MatJob[total];
        int idx  = 0;
        float now = Time.time;

        foreach (var snap in snaps)
        {
            if (snap.renderer == null) continue;
            var mats = snap.renderer.materials;   // instance array

            foreach (var e in snap.entries)
            {
                if (e.materialIndex >= mats.Length) continue;
                var mat = mats[e.materialIndex];
                if (mat == null) continue;

                jobs[idx] = new MatJob
                {
                    mat            = mat,
                    startColor     = mat.color,
                    targetColor    = e.originalColor,
                    hasEmission    = e.hasEmission,
                    startEmission  = e.hasEmission ? mat.GetColor("_EmissionColor") : Color.black,
                    targetEmission = e.originalEmission,
                    startTime      = now + Random.Range(0f, maxStagger),
                    noiseOffX      = Random.Range(0f, 100f),
                    noiseOffY      = Random.Range(0f, 100f),
                };

                if (e.hasEmission) mat.EnableKeyword("_EMISSION");
                idx++;
            }
        }

        return jobs;
    }

    // ── Effect: Magical (Perlin noise shimmer) ────────────────────────────────

    private IEnumerator RunMagical()
    {
        yield return null;   // let Awake/Start finish on all objects
        var jobs     = BuildJobs();
        int remaining = jobs.Length;

        while (remaining > 0)
        {
            float now = Time.time;
            remaining = 0;

            for (int i = 0; i < jobs.Length; i++)
            {
                MatJob j = jobs[i];
                if (j.mat == null) continue;

                float elapsed = now - j.startTime;
                if (elapsed < 0f) { remaining++; continue; }

                float t = elapsed / revealDuration;

                if (t >= 1f)
                {
                    j.mat.color = j.targetColor;
                    if (j.hasEmission) j.mat.SetColor("_EmissionColor", j.targetEmission);
                    jobs[i].mat = null;
                    continue;
                }

                remaining++;

                float envelope = Mathf.Sin(t * Mathf.PI);
                float noise    = Mathf.PerlinNoise(
                    j.noiseOffX + elapsed * noiseSpeed,
                    j.noiseOffY + elapsed * noiseSpeed * 0.7f);
                float shimmer  = (noise - 0.5f) * 2f * noiseIntensity * envelope;

                ApplyColor(j, t, shimmer);
            }

            yield return null;
        }

        _activeRoutine = null;
    }

    // ── Effect: Smooth (pure smoothstep, no noise) ────────────────────────────

    private IEnumerator RunSmooth()
    {
        yield return null;
        var jobs      = BuildJobs();
        int remaining = jobs.Length;

        while (remaining > 0)
        {
            float now = Time.time;
            remaining = 0;

            for (int i = 0; i < jobs.Length; i++)
            {
                MatJob j = jobs[i];
                if (j.mat == null) continue;

                float elapsed = now - j.startTime;
                if (elapsed < 0f) { remaining++; continue; }

                float t = elapsed / revealDuration;

                if (t >= 1f)
                {
                    j.mat.color = j.targetColor;
                    if (j.hasEmission) j.mat.SetColor("_EmissionColor", j.targetEmission);
                    jobs[i].mat = null;
                    continue;
                }

                remaining++;

                float st = t * t * (3f - 2f * t);
                ApplyColor(j, st, 0f);
            }

            yield return null;
        }

        _activeRoutine = null;
    }

    // ── Shared color write ────────────────────────────────────────────────────

    private static void ApplyColor(MatJob j, float t, float shimmer)
    {
        Color c = Color.LerpUnclamped(j.startColor, j.targetColor, t);
        j.mat.color = new Color(
            Mathf.Clamp01(c.r + shimmer),
            Mathf.Clamp01(c.g + shimmer),
            Mathf.Clamp01(c.b + shimmer),
            c.a);

        if (!j.hasEmission) return;
        Color e = Color.LerpUnclamped(j.startEmission, j.targetEmission, t);
        j.mat.SetColor("_EmissionColor", new Color(
            Mathf.Clamp01(e.r + shimmer),
            Mathf.Clamp01(e.g + shimmer),
            Mathf.Clamp01(e.b + shimmer),
            e.a));
    }
}
