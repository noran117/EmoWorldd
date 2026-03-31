using UnityEngine;

public class BalloonImpact : MonoBehaviour
{
    public GameObject splashPrefab;
    private AudioSource balloonHit;
    private void Start()
    {
        balloonHit = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Canvas")) return;

        ContactPoint contact = collision.contacts[0];

        Quaternion rot = Quaternion.LookRotation(contact.normal);
        rot *= Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        GameObject splash = Instantiate(
            splashPrefab,
            contact.point + contact.normal * 0.02f,
            rot
        );
        if (balloonHit is not null)
        {
            balloonHit.Play();
            Debug.Log("soundPlayed");
            //
        }
        Renderer r = splash.GetComponent<Renderer>();
        Balloon balloon = GetComponent<Balloon>();

        if (r != null && balloon != null)
        {
            Color softColor = Color.Lerp(balloon.balloonColor, Color.white, 0.4f);
            r.material.color = softColor;
        }

        Destroy(gameObject);
    }
}
