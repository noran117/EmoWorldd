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
        Debug.Log("EnableGlow called on " + gameObject.name);

        if (glowParticles != null)
            glowParticles.Play();
    }

    public void DisableGlow()
    {
        Debug.Log("DisableGlow called on " + gameObject.name);

        if (glowParticles != null)
            glowParticles.Stop();
    }
}
