using UnityEngine;
using UnityEngine.Video;

public class PlayVideoOnButton : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public void PlayVideo()
    {
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play(); 
        }
    }
}
