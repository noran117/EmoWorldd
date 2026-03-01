using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class Drum : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip drumClip;
    public float minHitForce = 0.5f;      
    public float maxVolume = 1f;          
    public float hitCooldown = 0.1f;      

    [Header("Visual")]
    public Renderer drumRenderer;
    public Color hitColor = Color.yellow;
    public float colorFlashDuration = 0.1f;

    private AudioSource audioSource;
    private float lastHitTime;
    private Color originalColor;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (drumRenderer != null)
        {
            originalColor = drumRenderer.material.color;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Sticks"))
            return;

        if (Time.time - lastHitTime < hitCooldown)
            return;

        float hitForce = collision.relativeVelocity.magnitude;

        if (hitForce < minHitForce)
            return;

        float volume = Mathf.Clamp(hitForce / 5f, 0f, maxVolume);

        if (drumClip != null)
        {
            audioSource.PlayOneShot(drumClip, volume);
        }

        lastHitTime = Time.time;

        if (drumRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashColor());
        }
    }

    System.Collections.IEnumerator FlashColor()
    {
        drumRenderer.material.color = hitColor;
        yield return new WaitForSeconds(colorFlashDuration);
        drumRenderer.material.color = originalColor;
    }
}