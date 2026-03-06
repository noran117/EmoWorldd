using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasketManager : MonoBehaviour
{
    public static BasketManager Instance;

    [Header("Basket Root")]
    public GameObject basketRoot;

    [Header("Stones")]
    public Stone[] stones;

    [Header("Dissolve Settings")]
    [SerializeField] private float dissolveTime = 1.2f;

    [Tooltip("Use dissolve-like property if found")]
    [SerializeField] private bool useDissolve = true;

    [Tooltip("Use vertical dissolve if found")]
    [SerializeField] private bool useVertical = false;

    [Header("Start / End Values")]
    [SerializeField] private float hiddenValue = 1.1f;
    [SerializeField] private float shownValue = 0f;

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

    class MaterialEntry
    {
        public Material mat;
        public string dissolveProp;
        public string verticalProp;
        public int dissolveId;
        public int verticalId;
    }

    private readonly List<MaterialEntry> basketEntries = new List<MaterialEntry>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        CacheBasketMaterials();

        SetDissolveInstant(hiddenValue);

        if (basketRoot != null)
            basketRoot.SetActive(false);

        if (stones != null)
        {
            for (int i = 0; i < stones.Length; i++)
            {
                if (stones[i] != null)
                    stones[i].gameObject.SetActive(false);
            }
        }
    }

    void CacheBasketMaterials()
    {
        basketEntries.Clear();

        if (basketRoot == null) return;

        var renderers = basketRoot.GetComponentsInChildren<Renderer>(true);

        foreach (var r in renderers)
        {
            if (r == null) continue;

            var mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                Material mat = mats[i];
                if (mat == null) continue;

                string dissolveProp = FindFirstExistingProperty(mat, DissolveCandidates);
                string verticalProp = FindFirstExistingProperty(mat, VerticalCandidates);

                var entry = new MaterialEntry();
                entry.mat = mat;
                entry.dissolveProp = dissolveProp;
                entry.verticalProp = verticalProp;

                if (!string.IsNullOrEmpty(dissolveProp))
                    entry.dissolveId = Shader.PropertyToID(dissolveProp);

                if (!string.IsNullOrEmpty(verticalProp))
                    entry.verticalId = Shader.PropertyToID(verticalProp);

                basketEntries.Add(entry);

                Debug.Log(
                    $"Basket material: {mat.name} | Shader: {(mat.shader != null ? mat.shader.name : "NoShader")} | DissolveProp: {dissolveProp} | VerticalProp: {verticalProp}"
                );
            }
        }

        if (basketEntries.Count == 0)
            Debug.LogWarning("BasketManager: No materials found on basketRoot.");
    }

    string FindFirstExistingProperty(Material mat, string[] candidates)
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

        if (basketEntries.Count == 0)
            CacheBasketMaterials();

        if (dissolveRoutine != null)
            StopCoroutine(dissolveRoutine);

        basketRoot.SetActive(true);

        if (stones != null)
        {
            for (int i = 0; i < stones.Length; i++)
            {
                if (stones[i] != null)
                    stones[i].gameObject.SetActive(true);
            }
        }

        SetDissolveInstant(hiddenValue);

        dissolveRoutine = StartCoroutine(ShowBasketRoutine());
    }

    private IEnumerator ShowBasketRoutine()
    {
        yield return null;

        Debug.Log("Stones Count = " + (stones != null ? stones.Length : 0));

        if (stones != null)
        {
            for (int i = 0; i < stones.Length; i++)
            {
                if (stones[i] != null)
                    stones[i].EnableGlow();
            }
        }

        yield return StartCoroutine(AppearDissolve());
    }

    public void HideBasket()
    {
        if (basketRoot == null) return;

        if (dissolveRoutine != null)
            StopCoroutine(dissolveRoutine);

        dissolveRoutine = StartCoroutine(HideBasketRoutine());
    }

    private IEnumerator HideBasketRoutine()
    {
        if (stones != null)
        {
            for (int i = 0; i < stones.Length; i++)
            {
                if (stones[i] != null)
                    stones[i].DisableGlow();
            }
        }

        yield return StartCoroutine(VanishAndDisable());
    }

    IEnumerator AppearDissolve()
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / dissolveTime);

            float value = Mathf.Lerp(hiddenValue, shownValue, t);
            ApplyDissolve(value);

            yield return null;
        }

        ApplyDissolve(shownValue);
        dissolveRoutine = null;
    }

    IEnumerator VanishAndDisable()
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / dissolveTime);

            float value = Mathf.Lerp(shownValue, hiddenValue, t);
            ApplyDissolve(value);

            yield return null;
        }

        ApplyDissolve(hiddenValue);

        if (stones != null)
        {
            for (int i = 0; i < stones.Length; i++)
            {
                if (stones[i] != null)
                    stones[i].gameObject.SetActive(false);
            }
        }

        basketRoot.SetActive(false);
        dissolveRoutine = null;
    }

    void ApplyDissolve(float amount)
    {
        if (basketEntries.Count == 0) return;

        for (int i = 0; i < basketEntries.Count; i++)
        {
            var e = basketEntries[i];
            if (e == null || e.mat == null) continue;

            if (useDissolve && !string.IsNullOrEmpty(e.dissolveProp) && e.mat.HasProperty(e.dissolveProp))
                e.mat.SetFloat(e.dissolveId, amount);

            if (useVertical && !string.IsNullOrEmpty(e.verticalProp) && e.mat.HasProperty(e.verticalProp))
                e.mat.SetFloat(e.verticalId, amount);
        }
    }

    void SetDissolveInstant(float amount)
    {
        ApplyDissolve(amount);
    }
}