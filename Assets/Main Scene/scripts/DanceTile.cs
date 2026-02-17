using UnityEngine;

public class DanceTile : MonoBehaviour
{
    [Header("Colors")]
    public Color[] possibleColors;

    [Header("Sounds")]
    public AudioClip[] possibleSounds;

    private AudioSource audioSource;
    private Renderer rend;
    private Color originalColor;
    private bool isActive;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material = new Material(rend.material); 
        originalColor = rend.material.color;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; 
    }

    public void Activate()
    {
        if (isActive) return;
        isActive = true;

        if (possibleColors.Length > 0)
            rend.material.color = possibleColors[Random.Range(0, possibleColors.Length)];

        if (possibleSounds.Length > 0)
            audioSource.PlayOneShot(possibleSounds[Random.Range(0, possibleSounds.Length)]);
    }

    public void Deactivate()
    {
        if (!isActive) return;
        isActive = false;
        rend.material.color = originalColor;
    }
    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Activate();
    }
}

private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Deactivate();
    }
}

}
