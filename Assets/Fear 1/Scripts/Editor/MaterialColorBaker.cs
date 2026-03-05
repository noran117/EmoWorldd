#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
            "Bake & Black Out: scans all Renderers, saves colors, sets them to black.\n" +
            "Restore: sets materials back to their baked original colors.",
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

        var renderers = root.GetComponentsInChildren<Renderer>(includeInactive: true);
        var snapList  = new List<RendererColorSnapshot>(renderers.Length);

        foreach (var r in renderers)
        {
            // sharedMaterials — reads the asset directly, no instances, no leaks.
            var sharedMats = r.sharedMaterials;
            var entries    = new List<MaterialColorEntry>(sharedMats.Length);

            for (int i = 0; i < sharedMats.Length; i++)
            {
                var mat = sharedMats[i];
                if (mat == null) continue;

                if (!mat.HasProperty("_Color"))
                {
                    Debug.LogWarning($"[Baker] SKIPPED '{mat.name}' on '{r.gameObject.name}' — no '_Color' (shader: {mat.shader.name}).");
                    continue;
                }

                var entry = new MaterialColorEntry
                {
                    materialIndex    = i,
                    originalColor    = mat.color,
                    hasEmission      = mat.HasProperty("_EmissionColor"),
                    originalEmission = Color.black
                };

                if (entry.hasEmission)
                    entry.originalEmission = mat.GetColor("_EmissionColor");

                entries.Add(entry);

                // Record the material asset itself for undo, then modify it directly.
                Undo.RecordObject(mat, "Bake Material Colors");

                mat.color = Color.black;
                if (entry.hasEmission)
                {
                    mat.SetColor("_EmissionColor", Color.black);
                    mat.DisableKeyword("_EMISSION");
                }

                EditorUtility.SetDirty(mat);
            }

            if (entries.Count > 0)
                snapList.Add(new RendererColorSnapshot { renderer = r, entries = entries.ToArray() });
        }

        data.snapshots = snapList.ToArray();
        EditorUtility.SetDirty(data);
    }

    // ── Restore ───────────────────────────────────────────────────────────────

    private void RestoreColors(MaterialColorData data)
    {
        if (data.snapshots == null || data.snapshots.Length == 0) return;

        foreach (var snap in data.snapshots)
        {
            if (snap.renderer == null) continue;

            var sharedMats = snap.renderer.sharedMaterials;

            foreach (var e in snap.entries)
            {
                if (e.materialIndex >= sharedMats.Length) continue;
                var mat = sharedMats[e.materialIndex];
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
        }
    }
}
#endif