using UnityEngine;

public class arrowScript : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0.25f, 0f);

    public float floatSpeed = 0.38f;
    public float floatHeight = 0.02f;

    void Update()
    {
        if (target == null) return;

        Vector3 basePos = target.position + offset;
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = basePos + new Vector3(0f, yOffset, 0f);
        transform.rotation = Quaternion.identity;
    }
}