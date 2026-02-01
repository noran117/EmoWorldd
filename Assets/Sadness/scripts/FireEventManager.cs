using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeableElement
{
    public GameObject obj;          // «·⁄‰’— ›Ì «·„‘Âœ
    public Texture beforeTexture;   // «· ﬂ” ‘— ﬁ»· «·Õ«œÀ
    public Texture afterTexture;    // «· ﬂ” ‘— »⁄œ «·Õ«œÀ
}

public class FireEventManager : MonoBehaviour
{
    [Header("Skybox")]
    public Material normalSkybox;
    public Material accidentSkybox;

    [Header("Environment Textures")]
    public List<ChangeableElement> elements;

    [Header("Effects")]
    public ParticleSystem sparkParticle; // «·‘—«—…
    public GameObject smokePlane;         // œŒ«‰ (Plane)
    public GameObject wirePrefab;         // «·”·ﬂ
    public Transform wireSpawnPoint;

    [Header("Timing")]
    public float delayBeforeSmoke = 1.5f;

    private bool eventTriggered = false;

    void Start()
    {
        // ≈Ìﬁ«› «·„ƒÀ—«  »«·»œ«Ì…
        if (sparkParticle != null)
            sparkParticle.Stop();

        if (smokePlane != null)
            smokePlane.SetActive(false);

        // ÷»ÿ «· ﬂ” ‘— «·ÿ»Ì⁄Ì ·ﬂ· «·⁄‰«’—
        foreach (var el in elements)
        {
            if (el.obj != null && el.beforeTexture != null)
            {
                Renderer r = el.obj.GetComponent<Renderer>();
                if (r != null)
                    r.material.mainTexture = el.beforeTexture;
            }
        }

        // ÷»ÿ «·”ﬂ«Ì»Êﬂ” «·ÿ»Ì⁄Ì
        if (normalSkybox != null)
        {
            RenderSettings.skybox = normalSkybox;
            DynamicGI.UpdateEnvironment();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // «·ﬂ—…  œŒ· „‰ÿﬁ… EventArea
        if (!eventTriggered && other.CompareTag("EventArea"))
        {
            eventTriggered = true;
            StartCoroutine(FireSequence());
        }
    }

    IEnumerator FireSequence()
    {
        // 1?? ‘—«—…
        if (sparkParticle != null)
            sparkParticle.Play();

        // 2??  €ÌÌ— «·”ﬂ«Ì»Êﬂ”
        if (accidentSkybox != null)
        {
            RenderSettings.skybox = accidentSkybox;
            DynamicGI.UpdateEnvironment();
        }

        // 3??  €ÌÌ—  ﬂ” ‘—«  «·»Ì∆…
        foreach (var el in elements)
        {
            if (el.obj != null && el.afterTexture != null)
            {
                Renderer r = el.obj.GetComponent<Renderer>();
                if (r != null)
                    r.material.mainTexture = el.afterTexture;
            }
        }

        // 4?? «‰ Ÿ«—
        yield return new WaitForSeconds(delayBeforeSmoke);

        // 5?? ŸÂÊ— «·”·ﬂ
        if (wirePrefab != null && wireSpawnPoint != null)
            Instantiate(wirePrefab, wireSpawnPoint.position, wireSpawnPoint.rotation);

        // 6?? ŸÂÊ— «·œŒ«‰
        if (smokePlane != null)
            smokePlane.SetActive(true);
    }
}
