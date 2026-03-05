using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reads original colors from BlackOnStart and lerps each material back
/// to its original color with a Perlin-noise-driven magical reveal effect.
///
/// Call  Reveal()  from anywhere (or enable Auto Reveal in the Inspector).
/// </summary>
public class MagicalReveal : MonoBehaviour
{
    public bool DebugReveal = false;
    [Header("References")]
    [Tooltip("Must be on the same GameObject as BlackOnStart, or assigned manually.")]
    public BlackOnStart blackOnStart;

    [Header("Reveal Settings")]
    [Tooltip("Total time (seconds) the reveal animation runs per material.")]
    public float revealDuration = 2.5f;

    [Tooltip("How wild the noise-driven color shimmer is during the transition.")]
    [Range(0f, 1f)]
    public float noiseIntensity = 0.4f;

    [Tooltip("Speed at which the Perlin noise scrolls.")]
    public float noiseSpeed = 3f;

    [Tooltip("Start the reveal automatically on Start (after a short delay).")]
    public bool autoReveal = true;

    [Tooltip("Seconds to wait before the auto-reveal begins.")]
    public float autoRevealDelay = 0.5f;

    // One coroutine token per material so they can be cancelled individually.
    private readonly Dictionary<Material, Coroutine> _coroutines = new();

    private void Start()
    {
        if (blackOnStart == null)
            blackOnStart = GetComponent<BlackOnStart>();

        if (blackOnStart == null)
            return;

        if (autoReveal)
            StartCoroutine(AutoRevealRoutine());
    }

    private void Update()
    {
        if(!DebugReveal) return;
        DebugReveal = false;
        Reveal();
    }
    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Reveal all materials that are still black.</summary>
    public void Reveal() => StartCoroutine(RevealAll());

    /// <summary>Reveal a single specific material.</summary>
    public void RevealMaterial(Material mat)
    {
        if (!blackOnStart.OriginalColors.TryGetValue(mat, out Color target)) return;
        if (_coroutines.TryGetValue(mat, out var existing) && existing != null)
            StopCoroutine(existing);
        _coroutines[mat] = StartCoroutine(RevealRoutine(mat, target));
    }

    // ── Coroutines ────────────────────────────────────────────────────────────

    private IEnumerator AutoRevealRoutine()
    {
        yield return new WaitForSeconds(autoRevealDelay);
        yield return RevealAll();
    }

    private IEnumerator RevealAll()
    {
        // Wait one frame so BlackOnStart.Start() has definitely run.
        yield return null;

        foreach (var kvp in blackOnStart.OriginalColors)
        {
            Material mat = kvp.Key;
            Color target = kvp.Value;

            // Stagger each material slightly for a wave-like feel.
            float stagger = Random.Range(0f, revealDuration * 0.3f);
            if (_coroutines.TryGetValue(mat, out var old) && old != null)
                StopCoroutine(old);
            _coroutines[mat] = StartCoroutine(RevealRoutine(mat, target, stagger));
        }
    }

    private IEnumerator RevealRoutine(Material mat, Color target, float delay = 0f)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float noiseOffsetX = Random.Range(0f, 100f);
        float noiseOffsetY = Random.Range(0f, 100f);

        Color startColor = mat.color;

        bool hasEmission = blackOnStart.OriginalEmissions.TryGetValue(mat, out Color targetEmission);
        Color startEmission = hasEmission ? mat.GetColor("_EmissionColor") : Color.black;

        // Re-enable emission keyword so the color change is visible.
        if (hasEmission) mat.EnableKeyword("_EMISSION");

        while (elapsed < revealDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / revealDuration);

            float envelope = Mathf.Sin(t * Mathf.PI);
            float noiseVal = Mathf.PerlinNoise(
                noiseOffsetX + elapsed * noiseSpeed,
                noiseOffsetY + elapsed * noiseSpeed * 0.7f);
            float shimmer = (noiseVal - 0.5f) * 2f * noiseIntensity * envelope;

            Color lerpedColor = Color.Lerp(startColor, target, t);
            mat.color = new Color(
                Mathf.Clamp01(lerpedColor.r + shimmer),
                Mathf.Clamp01(lerpedColor.g + shimmer),
                Mathf.Clamp01(lerpedColor.b + shimmer),
                lerpedColor.a);

            if (hasEmission)
            {
                Color lerpedEmission = Color.Lerp(startEmission, targetEmission, t);
                mat.SetColor("_EmissionColor", new Color(
                    Mathf.Clamp01(lerpedEmission.r + shimmer),
                    Mathf.Clamp01(lerpedEmission.g + shimmer),
                    Mathf.Clamp01(lerpedEmission.b + shimmer),
                    lerpedEmission.a));
            }

            yield return null;
        }

        mat.color = target;
        if (hasEmission) mat.SetColor("_EmissionColor", targetEmission);
        _coroutines.Remove(mat);
    }
}