using UnityEngine;
using System.Collections;

public class BasketManager : MonoBehaviour
{
    public static BasketManager Instance;

    [Header("Basket Root")]
    public GameObject basketRoot;

    [Header("Stones")]
    public GameObject stonesRoot;

    [Header("Dissolve Settings")]
    [SerializeField] private float dissolveTime = 0.75f;

    [Tooltip("dissolve")]
    [SerializeField] private bool useDissolve = true;

    [Tooltip("Vertical dissolve")]
    [SerializeField] private bool useVertical = false;

    [Header("Shader Property Names (Optional)")]
    [SerializeField] private string dissolvePropertyName = "_DissolveAmount";

    [SerializeField] private string verticalPropertyName = "_VerticalDissolve";

    private int _dissolveAmountId;
    private int _verticalDissolveId;

    private Material[] basketMats;
    private Coroutine dissolveRoutine;

    private static readonly string[] DissolveCandidates =
    {
        "_DissolveAmount",
        "_Dissolve",
        "_DissolveValue",
        "_DissolveFactor",
        "_DissolveThreshold",
        "_Cutoff",          
        "_AlphaClipThreshold"
    };

    private static readonly string[] VerticalCandidates =
    {
        "_VerticalDissolve",
        "_Vertical",
        "_VerticalAmount"
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (basketRoot != null)
        {
            CacheBasketMaterials();

            ResolveShaderPropertyIds();

            SetDissolveInstant(1.1f);

            basketRoot.SetActive(false);
        }

        if (stonesRoot != null)
            stonesRoot.SetActive(false);
    }

    private void CacheBasketMaterials()
    {
        if (basketRoot == null) { basketMats = null; return; }

        var renderers = basketRoot.GetComponentsInChildren<Renderer>(true);
        var list = new System.Collections.Generic.List<Material>();

        foreach (var r in renderers)
        {
            if (r == null) continue;
            var mats = r.materials;
            for (int j = 0; j < mats.Length; j++)
                if (mats[j] != null) list.Add(mats[j]);
        }

        basketMats = list.ToArray();
    }

    private void ResolveShaderPropertyIds()
    {
        if (basketMats == null || basketMats.Length == 0) return;

        var mat = basketMats[0];
        if (mat == null) return;

        Debug.Log("Basket shader: " + (mat.shader != null ? mat.shader.name : "NoShader"));

        string dissolveName = dissolvePropertyName;

        if (string.IsNullOrEmpty(dissolveName) || !mat.HasProperty(dissolveName))
        {
            dissolveName = FindFirstExistingProperty(mat, DissolveCandidates);
            if (!string.IsNullOrEmpty(dissolveName))
                Debug.Log("Basket dissolve property detected: " + dissolveName);
            else
                Debug.LogWarning("Basket dissolvePropertyName.");
        }

        if (!string.IsNullOrEmpty(dissolveName))
        {
            dissolvePropertyName = dissolveName; 
            _dissolveAmountId = Shader.PropertyToID(dissolvePropertyName);
        }

        string verticalName = verticalPropertyName;

        if (string.IsNullOrEmpty(verticalName) || !mat.HasProperty(verticalName))
        {
            verticalName = FindFirstExistingProperty(mat, VerticalCandidates);
            if (!string.IsNullOrEmpty(verticalName))
                Debug.Log("Basket vertical dissolve property detected: " + verticalName);
        }

        if (!string.IsNullOrEmpty(verticalName))
        {
            verticalPropertyName = verticalName;
            _verticalDissolveId = Shader.PropertyToID(verticalPropertyName);
        }

        Debug.Log("Has dissolve? " + (useDissolve && !string.IsNullOrEmpty(dissolvePropertyName) && mat.HasProperty(dissolvePropertyName)));
        Debug.Log("Has vertical? " + (useVertical && !string.IsNullOrEmpty(verticalPropertyName) && mat.HasProperty(verticalPropertyName)));
    }

    private string FindFirstExistingProperty(Material mat, string[] candidates)
    {
        if (mat == null || candidates == null) return null;

        for (int i = 0; i < candidates.Length; i++)
        {
            string p = candidates[i];
            if (!string.IsNullOrEmpty(p) && mat.HasProperty(p))
                return p;
        }
        return null;
    }

    public void ShowBasket()
    {
        if (basketRoot == null) return;

        if (basketMats == null || basketMats.Length == 0)
        {
            CacheBasketMaterials();
            ResolveShaderPropertyIds();
        }

        basketRoot.SetActive(true);

        if (stonesRoot != null)
            stonesRoot.SetActive(true);

        if (dissolveRoutine != null) StopCoroutine(dissolveRoutine);
        dissolveRoutine = StartCoroutine(AppearDissolve());

        foreach (Stone stone in basketRoot.GetComponentsInChildren<Stone>(true))
            stone.EnableGlow();
    }

    public void HideBasket()
    {
        if (basketRoot == null) return;

        foreach (Stone stone in basketRoot.GetComponentsInChildren<Stone>(true))
            stone.DisableGlow();

        if (dissolveRoutine != null) StopCoroutine(dissolveRoutine);
        dissolveRoutine = StartCoroutine(VanishAndDisable());
    }

    private IEnumerator AppearDissolve()
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dissolveTime;
            float lerped = Mathf.Lerp(1.1f, 0f, t);

            ApplyDissolve(lerped);
            yield return null;
        }

        ApplyDissolve(0f);
    }

    private IEnumerator VanishAndDisable()
    {
        if (stonesRoot != null)
            stonesRoot.SetActive(false);

        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dissolveTime;
            float lerped = Mathf.Lerp(0f, 1.1f, t);

            ApplyDissolve(lerped);
            yield return null;
        }

        ApplyDissolve(1.1f);

        basketRoot.SetActive(false);
    }

    private void ApplyDissolve(float amount)
    {
        if (basketMats == null) return;

        for (int i = 0; i < basketMats.Length; i++)
        {
            var m = basketMats[i];
            if (m == null) continue;

            if (useDissolve && !string.IsNullOrEmpty(dissolvePropertyName) && m.HasProperty(dissolvePropertyName))
                m.SetFloat(_dissolveAmountId, amount);

            if (useVertical && !string.IsNullOrEmpty(verticalPropertyName) && m.HasProperty(verticalPropertyName))
                m.SetFloat(_verticalDissolveId, amount);
        }
    }

    private void SetDissolveInstant(float amount)
    {
        ApplyDissolve(amount);
    }
}