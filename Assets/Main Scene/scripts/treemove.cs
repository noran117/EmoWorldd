using UnityEngine;

public class treemove : MonoBehaviour
{
    public float swaySpeed = 0.3f;
    public float swayAmount = 0.1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        transform.position = startPos + new Vector3(sway, 0, 0);
    }
}