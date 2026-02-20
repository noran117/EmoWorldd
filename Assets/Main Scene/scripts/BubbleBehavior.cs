using UnityEngine;
using System.Collections;

public class BubbleBehavior : MonoBehaviour
{
    [Header("Pop Settings")]
    public ParticleSystem popParticles;
    public AudioSource popSound;
    public float popScaleTime = 0.12f;

[Header("Floating Movement")]
    public float floatSpeed = 0.4f;
    public float horizontalSpeed = 0.25f;

[Header("Auto Pop Settings")]
    public float maxHeight = 2.5f;

    private bool isPopping = false;
    private BubbleSpawner spawner;
    private Collider col;
    private Renderer rend;
    private Vector3 floatDir;

    void Awake()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();

        if (!popSound) popSound = GetComponent<AudioSource>();
        if (!popParticles) popParticles = GetComponent<ParticleSystem>();
    }

    public void Init(BubbleSpawner owner)
    {
        spawner = owner;
        isPopping = false;

        transform.localScale = Vector3.one * Random.Range(0.5f, 1.1f);

        if (col) col.enabled = true;
        if (rend) rend.enabled = true;

        floatDir = new Vector3(
            Random.Range(-horizontalSpeed, horizontalSpeed),
            Random.Range(0.25f, floatSpeed),
            Random.Range(-horizontalSpeed, horizontalSpeed)
        );
    }

    void Update()
    {
        if (isPopping) return;

        transform.position += floatDir * Time.deltaTime;

        transform.position += new Vector3(
            Mathf.Sin(Time.time * 1.5f) * 0.003f,
            Mathf.Sin(Time.time * 2.2f) * 0.004f,
            Mathf.Cos(Time.time * 1.7f) * 0.003f
        );

        if (transform.position.y >= maxHeight)
        {
            StartCoroutine(PopRoutine());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPopping) return;

        if (other.CompareTag("PlayerHand"))
        {
            StartCoroutine(PopRoutine());
        }
    }

    IEnumerator PopRoutine()
    {
        if (isPopping) yield break;
        isPopping = true;

        if (col) col.enabled = false;

        if (spawner != null && spawner.popAudioSource != null && popSound != null && popSound.clip != null)
        {
            spawner.popAudioSource.PlayOneShot(popSound.clip);
        }

        if (popParticles) popParticles.Play();

        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < popScaleTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, start * 1.5f, t / popScaleTime);
            yield return null;
        }

        if (rend) rend.enabled = false;

        yield return new WaitForSeconds(
            popSound != null && popSound.clip != null ? popSound.clip.length : 0f
        );

        if (spawner != null)
            spawner.ReturnToPool(this);
        else
            gameObject.SetActive(false);
    }
}