using UnityEngine;

public class Stone : MonoBehaviour
{
    public ParticleSystem glowParticles;

    private void Awake()
    {
        if (glowParticles != null)
        {
            glowParticles.gameObject.SetActive(true);
            glowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            glowParticles.Clear(true);
        }
        else
        {
            Debug.LogWarning("Stone on " + gameObject.name + " has no glowParticles assigned.");
        }
    }

    public void EnableGlow()
    {
        if (glowParticles == null)
        {
            Debug.LogWarning("EnableGlow failed: glowParticles is NULL on " + gameObject.name);
            return;
        }

        Debug.Log("EnableGlow called on " + gameObject.name);

        glowParticles.gameObject.SetActive(true);
        glowParticles.transform.position = transform.position + Vector3.up * 0.1f;

        glowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        glowParticles.Clear(true);
        glowParticles.Play(true);
    }

    public void DisableGlow()
    {
        if (glowParticles == null) return;

        Debug.Log("DisableGlow called on " + gameObject.name);

        glowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        glowParticles.Clear(true);
    }
}