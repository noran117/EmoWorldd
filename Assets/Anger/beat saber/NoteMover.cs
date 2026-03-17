using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float speed = 2f;

    void Update()
    {
        transform.position -= transform.right * speed * Time.deltaTime;
    }
}