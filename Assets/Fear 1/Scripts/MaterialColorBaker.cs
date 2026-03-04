#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-only tool: scans all Renderers under the target root, stores
/// original base + emission colors into MaterialColorData, then blacks
/// out every valid material.
///
/// Place this file inside an  Editor/  folder  OR  keep it anywhere —
/// the #if UNITY_EDITOR guards strip it from builds automatically.
///
/// Usage: select the GameObject that has MaterialColorData attached,
///        then click  "Bake & Black Out"  in the Inspector.
/// </summary>
[CustomEditor(typeof(MaterialColorData))]
public class MaterialColorBaker : Editor
{
    private GameObject _targetRoot;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var data = (MaterialColorData)target;

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Baker", EditorStyles.boldLabel);

        _targetRoot = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Target Root", "Root whose children will be scanned. Defaults to this GameObject."),
            _targetRoot, typeof(GameObject), true);

        EditorGUILayout.HelpBox(
            "Bake & Black Out: scans all Renderers, saves colors, then sets them to black.\n" +
            "Restore: sets all materials back to their baked original colors.",
            MessageType.None);

        EditorGUILayout.BeginHorizontal();

        GUI.color = new Color(0.9f, 0.5f, 0.5f);
        if (GUILayout.Button("Bake & Black Out", GUILayout.Height(28)))
            BakeAndBlackOut(data);

        GUI.color = new Color(0.5f, 0.9f, 0.5f);
        if (GUILayout.Button("Restore Colors", GUILayout.Height(28)))
            RestoreColors(data);

        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

        if (data.snapshots != null && data.snapshots.Length > 0)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField($"Baked: {data.snapshots.Length} renderer(s)", EditorStyles.miniLabel);
        }
    }

    // ── Bake ──────────────────────────────────────────────────────────────────

    private void BakeAndBlackOut(MaterialColorData data)
    {
        GameObject root = _targetRoot != null ? _targetRoot : data.gameObject;

        Undo.RecordObject(data, "Bake Material Colors");

        var renderers  = root.GetComponentsInChildren<Renderer>(includeInactive: true);
        var snapList   = new List<RendererColorSnapshot>(renderers.Length);

        foreach (var r in renderers)
        {
            // r.materials creates instances; use sharedMaterials to read, then
            // switch to instances only for the ones we actually modify.
            var shared = r.sharedMaterials;
            var entries = new List<MaterialColorEntry>(shared.Length);

            // Determine which slots need modification before allocating instances.
            bool anyValid = false;
            for (int i = 0; i < shared.Length; i++)
            {
                var mat = shared[i];
                if (mat == null || !mat.HasProperty("_Color"))
                {
                    if (mat != null && !mat.HasProperty("_Color"))
                        Debug.LogWarning($"[Baker] SKIPPED '{mat.name}' on '{r.gameObject.name}' — no '_Color' (shader: {mat.shader.name}).");
                    continue;
                }
                anyValid = true;
            }

            if (!anyValid) continue;

            // Now get instances so we don't dirty shared assets.
            Undo.RecordObject(r, "Bake Material Colors");
            var instMats = r.materials;   // allocates instances

            for (int i = 0; i < instMats.Length; i++)
            {
                var mat = instMats[i];
                if (mat == null || !mat.HasProperty("_Color")) continue;

                var entry = new MaterialColorEntry
                {
                    materialIndex   = i,
                    originalColor   = mat.color,
                    hasEmission     = mat.HasProperty("_EmissionColor"),
                    originalEmission = Color.black
                };

                if (entry.hasEmission)
                    entry.originalEmission = mat.GetColor("_EmissionColor");

                entries.Add(entry);
                Undo.RecordObject(mat, "Bake Material Colors");

                mat.color = Color.black;
                if (entry.hasEmission)
                {
                    mat.SetColor("_EmissionColor", Color.black);
                    mat.DisableKeyword("_EMISSION");
                }

                EditorUtility.SetDirty(mat);
            }

            r.materials = instMats;
            EditorUtility.SetDirty(r);

            if (entries.Count > 0)
                snapList.Add(new RendererColorSnapshot { renderer = r, entries = entries.ToArray() });
        }

        data.snapshots = snapList.ToArray();
        EditorUtility.SetDirty(data);
        Debug.LogWarning($"[Baker] Baked {snapList.Count} renderer(s) under '{root.name}'.");
    }

    // ── Restore ───────────────────────────────────────────────────────────────

    private void RestoreColors(MaterialColorData data)
    {
        if (data.snapshots == null || data.snapshots.Length == 0) return;

        foreach (var snap in data.snapshots)
        {
            if (snap.renderer == null) continue;
            Undo.RecordObject(snap.renderer, "Restore Material Colors");

            var mats = snap.renderer.materials;
            foreach (var e in snap.entries)
            {
                if (e.materialIndex >= mats.Length) continue;
                var mat = mats[e.materialIndex];
                if (mat == null) continue;

                Undo.RecordObject(mat, "Restore Material Colors");
                mat.color = e.originalColor;
                if (e.hasEmission)
                {
                    mat.SetColor("_EmissionColor", e.originalEmission);
                    mat.EnableKeyword("_EMISSION");
                }
                EditorUtility.SetDirty(mat);
            }
            snap.renderer.materials = mats;
            EditorUtility.SetDirty(snap.renderer);
        }
    }
}
#endif
