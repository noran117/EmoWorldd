using UnityEngine;

public class WallHitEffect : MonoBehaviour
{
    [Header("Particle Effect")]
    public GameObject hitEffectPrefab;

    [Header("Hit Sounds")]
    public AudioClip[] hitSounds;
    private AudioSource audioSource;

    //[Header("Hit Square")]
    //public GameObject hitSquarePrefab;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Ball")) return;

        Vector3 hitPoint = collision.contacts[0].point;

        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(effect, ps.main.duration + 0.5f);
            }
            else
            {
                Destroy(effect, 1f);
            }
        }

        if (hitSounds.Length > 0)
        {
            AudioClip randomClip = hitSounds[Random.Range(0, hitSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }

        //if (hitSquarePrefab != null)
        //{
        //    Quaternion squareRotation = Quaternion.LookRotation(collision.contacts[0].normal);

        //    GameObject square = Instantiate(hitSquarePrefab, hitPoint, squareRotation);

        //    Destroy(square, 0.5f);
        //}
    }
}
