using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// On Start, sets every material on every Renderer in all children (recursive)
/// to black. Stores original colors so other scripts can read them later.
/// </summary>
public class BlackOnStart : MonoBehaviour
{
    [Tooltip("Root object whose children will be affected. Defaults to this GameObject if left empty.")]
    public GameObject targetRoot;

    // Public so MagicalReveal (or any other script) can read the originals.
    public Dictionary<Material, Color> OriginalColors    { get; private set; } = new();
    public Dictionary<Material, Color> OriginalEmissions { get; private set; } = new();

    private void Start()
    {
        if (targetRoot == null) targetRoot = gameObject;

        var renderers = targetRoot.GetComponentsInChildren<Renderer>(includeInactive: true);

        foreach (var r in renderers)
        {
            // Use instance materials so we don't corrupt shared assets.
            var mats = r.materials;
            foreach (var mat in mats)
            {
                if (mat == null) continue;

                if (!mat.HasProperty("_Color"))
                {
                    Debug.LogWarning($"[BlackOnStart] SKIPPED '{mat.name}' on '{r.gameObject.name}' — no '_Color' property (shader: {mat.shader.name}).");
                    continue;
                }

                OriginalColors[mat] = mat.color;
                mat.color = Color.black;

                if (mat.HasProperty("_EmissionColor"))
                {
                    OriginalEmissions[mat] = mat.GetColor("_EmissionColor");
                    mat.SetColor("_EmissionColor", Color.black);
                    // Disable the emission keyword so the black actually takes effect.
                    mat.DisableKeyword("_EMISSION");
                }
            }
            // Write the (now-modified) instance array back.
            r.materials = mats;
        }
    }
}