using UnityEngine;

public class CandyCoin : MonoBehaviour
{
    public AudioClip collectSound;   
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CoinManager.Instance.CollectCoin();
            audioSource.PlayOneShot(collectSound);
            Destroy(gameObject, collectSound.length);
        }
    }
}
