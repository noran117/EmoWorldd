using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class Drum : MonoBehaviour
{
    [Header("Hit Settings")]
    public float minHitForce = 0.5f;   
    public float hitCooldown = 0.08f;  

    [Header("Visual (Optional)")]
    public bool useColorFlash = true;
    public Renderer drumRenderer;      
    public Color hitColor = Color.yellow;
    public float colorFlashDuration = 0.08f;

    private AudioSource audioSource;
    private float lastHitTime = -999f;
    private Color originalColor;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        if (useColorFlash && drumRenderer != null)
            originalColor = drumRenderer.material.color;
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

        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }

        lastHitTime = Time.time;

        if (useColorFlash && drumRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashColor());
        }
    }

    IEnumerator FlashColor()
    {
        drumRenderer.material.color = hitColor;
        yield return new WaitForSeconds(colorFlashDuration);
        drumRenderer.material.color = originalColor;
    }
}