using UnityEngine;

public class MachineSpin : MonoBehaviour
{
    public float speed = 200f;
    bool spinning = false;

    void Update()
    {
        if (spinning)
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }

    public void StartSpin()
    {
        spinning = true;
        Invoke("StopSpin", 2f);
    }

    void StopSpin()
    {
        spinning = false;
    }
}
