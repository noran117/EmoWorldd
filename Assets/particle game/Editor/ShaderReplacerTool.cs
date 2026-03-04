using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ShaderReplacerTool : EditorWindow
{
    // ── Drag targets ──────────────────────────────────────────────────────────
    private GameObject  targetObject;
    private Shader      newShader;

    // ── State ─────────────────────────────────────────────────────────────────
    // Only the *direct* children are shown as rows; nested children are processed
    // recursively when the user clicks "Update" for that row.
    private List<GameObject> pendingChildren  = new List<GameObject>();
    private List<string>     successLog       = new List<string>();
    private Vector2          scrollPending;
    private Vector2          scrollLog;

    // ── Open window ───────────────────────────────────────────────────────────
    [MenuItem("Tools/Shader Replacer")]
    public static void OpenWindow()
    {
        var win = GetWindow<ShaderReplacerTool>("Shader Replacer");
        win.minSize = new Vector2(480, 560);
    }

    // ── GUI ───────────────────────────────────────────────────────────────────
    private void OnGUI()
    {
        DrawHeader();
        DrawDragFields();
        DrawPopulateButton();
        EditorGUILayout.Space(6);
        DrawPendingList();
        EditorGUILayout.Space(6);
        DrawSuccessLog();
        DrawFooterButtons();
    }

    // ── Sections ──────────────────────────────────────────────────────────────

    private void DrawHeader()
    {
        var style = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize  = 14,
            alignment = TextAnchor.MiddleCenter
        };
        GUILayout.Space(8);
        GUILayout.Label("Shader Replacer", style);
        GUILayout.Space(4);
        EditorGUILayout.HelpBox(
            "1. Drag a GameObject whose children need a shader update.\n" +
            "2. Drag the new Shader.\n" +
            "3. Click \"Populate List\" to load direct children.\n" +
            "4. Click \"Update\" next to each child to apply — the row disappears when done.",
            MessageType.Info);
        GUILayout.Space(6);
    }

    private void DrawDragFields()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Setup", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        targetObject = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Parent Object", "Drag the GameObject whose children you want to update."),
            targetObject, typeof(GameObject), true);
        if (EditorGUI.EndChangeCheck())
            RefreshPendingList();   // auto-refresh when object changes

        newShader = (Shader)EditorGUILayout.ObjectField(
            new GUIContent("New Shader", "Drag the replacement shader here."),
            newShader, typeof(Shader), false);

        if (newShader != null)
        {
            EditorGUILayout.HelpBox($"Selected shader: {newShader.name}", MessageType.None);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawPopulateButton()
    {
        EditorGUI.BeginDisabledGroup(targetObject == null);
        if (GUILayout.Button("↺  Populate / Refresh List", GUILayout.Height(28)))
            RefreshPendingList();
        EditorGUI.EndDisabledGroup();
    }

    private void DrawPendingList()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label($"Pending Children  ({pendingChildren.Count} remaining)", EditorStyles.boldLabel);

        if (pendingChildren.Count == 0)
        {
            EditorGUILayout.HelpBox(
                targetObject == null
                    ? "No parent object selected."
                    : "All children have been updated! ✓",
                MessageType.None);
        }
        else
        {
            scrollPending = EditorGUILayout.BeginScrollView(scrollPending, GUILayout.MinHeight(160), GUILayout.MaxHeight(300));

            // Iterate a copy so we can remove while iterating
            var toRemove = new List<GameObject>();
            foreach (var child in pendingChildren.ToList())
            {
                if (child == null) { toRemove.Add(child); continue; }

                EditorGUILayout.BeginHorizontal("helpbox");

                // Icon + name
                var icon = EditorGUIUtility.ObjectContent(child, typeof(GameObject)).image;
                GUILayout.Label(new GUIContent(icon), GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.Label(child.name, GUILayout.MinWidth(160));

                // Material count preview (across all nested renderers)
                int matCount = CountMaterials(child);
                GUILayout.Label($"({matCount} material{(matCount != 1 ? "s" : "")})", EditorStyles.miniLabel, GUILayout.Width(90));

                // Ping button
                if (GUILayout.Button("◎", GUILayout.Width(26)))
                    EditorGUIUtility.PingObject(child);

                // Update button
                EditorGUI.BeginDisabledGroup(newShader == null);
                var btnStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold };
                GUI.color = new Color(0.4f, 0.9f, 0.4f);
                if (GUILayout.Button("Update ✓", btnStyle, GUILayout.Width(80)))
                {
                    ApplyShaderToChild(child);
                    toRemove.Add(child);
                }
                GUI.color = Color.white;
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
            }

            foreach (var go in toRemove)
                pendingChildren.Remove(go);

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSuccessLog()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label($"Success Log  ({successLog.Count} entries)", EditorStyles.boldLabel);

        scrollLog = EditorGUILayout.BeginScrollView(scrollLog, GUILayout.MinHeight(80), GUILayout.MaxHeight(180));

        if (successLog.Count == 0)
        {
            GUILayout.Label("No updates yet.", EditorStyles.miniLabel);
        }
        else
        {
            var logStyle = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true };
            foreach (var entry in successLog)
            {
                GUI.color = new Color(0.6f, 1f, 0.6f);
                GUILayout.Label(entry, logStyle);
                GUI.color = Color.white;
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawFooterButtons()
    {
        GUILayout.Space(4);
        EditorGUILayout.BeginHorizontal();

        // Update ALL remaining at once
        EditorGUI.BeginDisabledGroup(pendingChildren.Count == 0 || newShader == null);
        GUI.color = new Color(1f, 0.75f, 0.3f);
        if (GUILayout.Button("⚡ Update ALL Remaining", GUILayout.Height(28)))
        {
            foreach (var child in pendingChildren.ToList())
            {
                if (child != null) ApplyShaderToChild(child);
            }
            pendingChildren.Clear();
        }
        GUI.color = Color.white;
        EditorGUI.EndDisabledGroup();

        // Clear log
        if (GUILayout.Button("Clear Log", GUILayout.Height(28), GUILayout.Width(90)))
            successLog.Clear();

        EditorGUILayout.EndHorizontal();
    }

    // ── Core logic ────────────────────────────────────────────────────────────

    private void RefreshPendingList()
    {
        pendingChildren.Clear();
        if (targetObject == null) return;

        foreach (Transform child in targetObject.transform)
            pendingChildren.Add(child.gameObject);

        Repaint();
    }

    /// <summary>
    /// Recursively walks <paramref name="root"/> (and all its nested children),
    /// finds every Renderer, and replaces each material's shader.
    /// Texture2D "basemap" → "base" mapping is applied automatically.
    /// </summary>
    private void ApplyShaderToChild(GameObject root)
    {
        int updatedMats = 0;
        int updatedRenderers = 0;

        // GetComponentsInChildren includes the root itself and all nested children
        var renderers = root.GetComponentsInChildren<Renderer>(includeInactive: true);

        foreach (var renderer in renderers)
        {
            bool rendererTouched = false;

            // Work on sharedMaterials to avoid creating unnecessary instances
            var materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];
                if (mat == null) continue;

                Undo.RecordObject(mat, "Replace Shader");

                // ── Texture mapping: old "basemap" / "_BaseMap" → new "base" / "_Base" ──
                Texture2D savedBaseMap = null;

                // Try common old-shader property names for the base/albedo texture
                string[] baseMapCandidates = { "_BaseMap", "_MainTex", "_BaseTex", "basemap" };
                foreach (var propName in baseMapCandidates)
                {
                    if (mat.HasProperty(propName))
                    {
                        savedBaseMap = mat.GetTexture(propName) as Texture2D;
                        if (savedBaseMap != null) break;
                    }
                }

                string previousShaderName = mat.shader != null ? mat.shader.name : "none";
                mat.shader = newShader;

                // Apply the texture to the new shader's "base" / "_Base" property
                if (savedBaseMap != null)
                {
                    string[] newBaseProps = { "_Base", "base", "_BaseMap", "_MainTex" };
                    bool applied = false;
                    foreach (var propName in newBaseProps)
                    {
                        if (mat.HasProperty(propName))
                        {
                            mat.SetTexture(propName, savedBaseMap);
                            applied = true;
                            break;
                        }
                    }

                    string texNote = applied
                        ? $"texture '{savedBaseMap.name}' remapped"
                        : $"texture '{savedBaseMap.name}' NOT remapped (property not found on new shader)";

                    LogEntry(renderer, mat, previousShaderName, texNote);
                }
                else
                {
                    LogEntry(renderer, mat, previousShaderName, "no base texture to remap");
                }

                EditorUtility.SetDirty(mat);
                updatedMats++;
                rendererTouched = true;
            }

            if (rendererTouched) updatedRenderers++;
        }

        successLog.Add($"✓  [{root.name}]  — {updatedRenderers} renderer(s), {updatedMats} material(s) updated.");
        Debug.Log($"[ShaderReplacer] '{root.name}': updated {updatedMats} material(s) across {updatedRenderers} renderer(s).");
    }

    private void LogEntry(Renderer renderer, Material mat, string oldShader, string texNote)
    {
        string path = GetHierarchyPath(renderer.transform);
        string msg  = $"  • {path} / {mat.name}  |  {oldShader} → {newShader.name}  |  {texNote}";
        successLog.Add(msg);
        Debug.Log($"[ShaderReplacer] {msg}");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static int CountMaterials(GameObject root)
    {
        return root.GetComponentsInChildren<Renderer>(true)
                   .Sum(r => r.sharedMaterials.Count(m => m != null));
    }

    private static string GetHierarchyPath(Transform t)
    {
        var parts = new List<string>();
        while (t != null) { parts.Insert(0, t.name); t = t.parent; }
        return string.Join("/", parts);
    }
}