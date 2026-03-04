using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable snapshot of a renderer's material colors.
/// Baked at edit-time, consumed at runtime — zero allocation on play.
/// </summary>
[Serializable]
public class RendererColorSnapshot
{
    public Renderer           renderer;
    public MaterialColorEntry[] entries;
}

[Serializable]
public class MaterialColorEntry
{
    public int   materialIndex;   // index into renderer.sharedMaterials
    public Color originalColor;
    public bool  hasEmission;
    public Color originalEmission;
}

/// <summary>
/// MonoBehaviour that holds the baked snapshot list and exposes fast
/// lookup structures for the reveal scripts.
/// Populated exclusively by the Editor tool — never at runtime.
/// </summary>
public class MaterialColorData : MonoBehaviour
{
    [HideInInspector] public RendererColorSnapshot[] snapshots = Array.Empty<RendererColorSnapshot>();

    // ── Runtime lookup (built once in Awake, O(1) access) ────────────────────
    // Key: (renderer instanceID << 8 | materialIndex)  →  entry
    // Using a plain array-backed struct would be even faster, but Dictionary
    // is fine here since we build it once and never modify it.
    [NonSerialized] public Dictionary<long, MaterialColorEntry> Lookup;

    private void Awake()
    {
        Lookup = new Dictionary<long, MaterialColorEntry>(snapshots.Length * 2);
        foreach (var snap in snapshots)
        {
            if (snap.renderer == null) continue;
            int id = snap.renderer.GetInstanceID();
            foreach (var e in snap.entries)
                Lookup[(long)id << 16 | (uint)e.materialIndex] = e;
        }
    }
}
