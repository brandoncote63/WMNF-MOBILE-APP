using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class AVProVideoAudioPlayer3 : MonoBehaviour
{
    public MediaPlayer mediaPlayer;

    private void Start()
    {
        // Stop the media player initially
        mediaPlayer.Control.Stop();
    }

    public void StartMediaPlayer()
    {
        // Start the media player
        mediaPlayer.Control.Play();
    }
}
