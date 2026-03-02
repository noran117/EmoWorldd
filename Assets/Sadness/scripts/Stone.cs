using UnityEngine;

public class Stone : MonoBehaviour
{
    public ParticleSystem glowParticles;

    private void Awake()
    {
        if (glowParticles != null)
            glowParticles.Stop();
    }

    public void EnableGlow()
    {
        if (glowParticles != null)
            glowParticles.Play();
    }

    public void DisableGlow()
    {
        if (glowParticles != null)
            glowParticles.Stop();
    }
}
