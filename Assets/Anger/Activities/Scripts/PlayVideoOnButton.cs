using UnityEngine;
using UnityEngine.Video;

public class PlayVideoOnButton : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource bgSound;

    public void PlayVideo()
    {
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            if (bgSound != null)
                bgSound.Play();
        }
        else
        {
             if (bgSound != null)
                bgSound.Pause();
                
            videoPlayer.Play();

        }
    }
}
