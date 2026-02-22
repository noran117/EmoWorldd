using UnityEngine;

public class PaintCanvas : MonoBehaviour
{
    public GameObject splashPrefab;

    [Header("Random Settings")]
    public Vector2 randomScaleRange = new Vector2(0.5f, 0.65f);
    public Vector2 randomDepthOffset = new Vector2(0.01f, 0.03f);

    private void OnCollisionEnter(Collision collision)
    {
        Balloon balloon = collision.gameObject.GetComponent<Balloon>();
        if (balloon == null) return;

        ContactPoint contact = collision.contacts[0];

        float depthOffset = Random.Range(randomDepthOffset.x, randomDepthOffset.y);
        Vector3 spawnPos = contact.point + contact.normal * depthOffset;

        Quaternion rotation = Quaternion.LookRotation(contact.normal);

        rotation *= Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        GameObject splash = Instantiate(splashPrefab, spawnPos, rotation);

        float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
        splash.transform.localScale = new Vector3(
            randomScale,
            randomScale,
            randomScale * 0.15f 
        );

        Renderer r = splash.GetComponent<Renderer>();
        if (r != null)
        {

            Color softColor = Color.Lerp(
                balloon.balloonColor,
                Color.white,
                0.4f);

            r.material.color = softColor;

        }

        BalloonFade fade = collision.gameObject.GetComponent<BalloonFade>();
        if (fade != null) fade.Hit();
        else Destroy(collision.gameObject);
    }
}
