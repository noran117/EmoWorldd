using UnityEngine;

public class BigEffectController : MonoBehaviour
{
    public ParticleSystem finalParticles;
    public Light sceneLight;

    public void PlayFinalEffect()
    {
        finalParticles.Play();

        if (sceneLight != null)
        {
            sceneLight.intensity = 1.8f;
        }
    }
}
