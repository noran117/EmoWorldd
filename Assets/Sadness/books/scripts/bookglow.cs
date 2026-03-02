using UnityEngine;

public class bookglow : MonoBehaviour
{
    public ParticleSystem bookGlow;

    public void ExpandGlowRadius()
    {
        var shape = bookGlow.shape;

        shape.radius = 30f;  
    }
}

