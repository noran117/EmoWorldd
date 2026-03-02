using UnityEngine;

public class arrowScript : MonoBehaviour
{
    public float floatSpeed = 0.38f;
    public float floatHeight = 0.02f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
