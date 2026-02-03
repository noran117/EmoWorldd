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

    public AnimationStateController companion;



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

        transform.localScale = Vector3.one * Random.Range(0.5f, 1.1f);

        if (col != null) col.enabled = true;
        if (rend != null) rend.enabled = true;

        floatDir = new Vector3(
            Random.Range(horizontalSpeed, horizontalSpeed),
            Random.Range(0.25f, floatSpeed),
            Random.Range(horizontalSpeed, horizontalSpeed)
        );
    }

    void Update()
    {
        transform.position += floatDir * Time.deltaTime;

        transform.position += new Vector3(
            Mathf.Sin(Time.time * 1.5f) * 0.003f,
            Mathf.Sin(Time.time * 2.2f) * 0.004f,
            Mathf.Cos(Time.time * 1.7f) * 0.003f
        );
        if (!isPopping && transform.position.y >= maxHeight)
        {
            isPopping = true;
            StartCoroutine(PopRoutine());
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (isPopping) return;
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("Bubble popped by player hand.");
            StartCoroutine(PopRoutine());
            isPopping = true;

            companion.ReactToBubble();
        }
        Debug.Log("Bubble collided with: " + other.name);
    }

    IEnumerator PopRoutine()
    {
        if (spawner == null)
        {
            Debug.LogWarning("Bubble has no spawner assigned!");
        }

        if (col != null) col.enabled = false;

        isPopping = true;

        if (spawner != null && spawner.popAudioSource != null && popSound != null && popSound.clip != null)
        {
            spawner.popAudioSource.PlayOneShot(popSound.clip);
        }


        if (popParticles != null) popParticles.Play();

        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < popScaleTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, start * 1.5f, t / popScaleTime);
            yield return null;
        }

        if (rend != null) rend.enabled = false;

        float waitTime = (popSound != null && popSound.clip != null) ? popSound.clip.length : 0f;
        yield return new WaitForSeconds(waitTime);

        if (spawner != null)
            spawner.ReturnToPool(gameObject);
        else
            gameObject.SetActive(false);
    }

}
