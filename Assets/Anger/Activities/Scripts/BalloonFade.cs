using UnityEngine;

public class BalloonFade : MonoBehaviour
{
    public float shrinkSpeed = 1f; 
    private bool isHit = false;

    void Update()
    {
        if (isHit)
        {
            transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
            if (transform.localScale.x <= 0.05f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Hit()
    {
        isHit = true;
    }
}
