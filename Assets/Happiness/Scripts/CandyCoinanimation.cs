using UnityEngine;

public class CandyCoinanimation : MonoBehaviour
{
    [Header("ÅÚÏÇÏÇÊ ÈÓíØÉ")]
    public float rotateSpeed = 100f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.1f;

    private float startY;
    private float offset;

    void Start()
    {
        startY = transform.position.y;
        // ÃæİÓÊ ÚÔæÇÆí ÚÔÇä ãÇ íÊÍÑßæÇ ßáåã ßÃäåã ÑæÈæÊÇÊ
        offset = Random.Range(0f, 10f);
    }

    void Update()
    {
        // 1. ÇáÏæÑÇä (ÃÎİ ØÑíŞÉ ÏæÑÇä İí íæäÊí)
        transform.Rotate(Vector3.up * (rotateSpeed * Time.deltaTime));

        // 2. ÇáØİæ (ÍÓÇÈ ÑíÇÖí ÈÓíØ ÌÏÇğ)
        float newY = startY + Mathf.Sin(Time.time * floatSpeed + offset) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}