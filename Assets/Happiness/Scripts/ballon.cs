using UnityEngine;

public class ballon : MonoBehaviour
{
    // ŞæÉ ÇáÇåÊÒÇÒ (ÎáíåÇ ÃÑŞÇã ÕÛíÑÉ ááÍÑßÉ ÇááØíİÉ)
    public float speed = 1.0f;
    public float strength = 0.2f;

    private Vector3 startPos;
    private float randomOffset;

    void Start()
    {
        startPos = transform.position;
        // ÈäÖíİ ÑŞã ÚÔæÇÆí ÚÔÇä ãÇ íÊÍÑßæÇ ßáåã ÈäİÓ ÇááÍÙÉ ÈÇáÙÈØ
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // ãÚÇÏáÉ ÈÓíØÉ ÈÊÎáí ÇáÈÇáæä íÑæÍ æííÌí ÈäÚæãÉ
        float newY = Mathf.Sin(Time.time * speed + randomOffset) * strength;
        float newX = Mathf.PerlinNoise(Time.time * speed, randomOffset) * strength;

        transform.position = startPos + new Vector3(newX, newY, 0);
    }
}