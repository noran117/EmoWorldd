using UnityEngine;

public class TreeSway : MonoBehaviour
{
    [Header("Wind Settings")]
    public float swaySpeed = 1.9f;       // ”—⁄… «·«Â “«“
    public float swayAmount = 3f;        // „ﬁœ«— «·„Ì·«‰
    public float randomOffset = 0f;      // ≈÷«›… «Œ ·«› »”Ìÿ ·ﬂ· ‘Ã—…

    private Quaternion startRot;

    void Start()
    {
        startRot = transform.rotation;

        // «Œ ·«› »”Ìÿ »Ì‰ ﬂ· ‘Ã—… (ÿ»Ì⁄Ì!)
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Õ—ﬂ… ”Ì‰ + »Ì—·Ì‰ ‰ÊÌ“ ··‰⁄Ê„…
        float sway = Mathf.Sin((Time.time + randomOffset) * swaySpeed) * swayAmount;
        float noise = Mathf.PerlinNoise(Time.time * 0.5f, randomOffset) * swayAmount * 0.3f;

        float finalSway = sway + noise;

        //  ÿ»Ìﬁ „Ì·«‰ ⁄·Ï „ÕÊ— X √Ê Z Õ”» « Ã«Â «·—ÌÕ
        transform.rotation = startRot * Quaternion.Euler(finalSway, 0f, 0f);
    }
}
