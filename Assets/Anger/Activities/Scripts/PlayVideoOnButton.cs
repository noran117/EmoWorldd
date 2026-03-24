using UnityEngine;
using UnityEngine.Video;

public class PlayVideoOnButton : MonoBehaviour
{
     public Renderer screenRenderer;

    public Material idleMaterial;
    public Material videoMaterial;
    public VideoPlayer videoPlayer;
    public AudioSource bgSound;

    public void PlayVideo()
    {
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            screenRenderer.material = idleMaterial;
            if (bgSound != null)
                bgSound.Play();
        }
        else
        {
             if (bgSound != null)
                bgSound.Pause();
                
            videoPlayer.Play();
            screenRenderer.material = videoMaterial;

        }
    }
}
