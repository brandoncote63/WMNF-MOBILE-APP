using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;

public class AVProVideoAudioPlayer4 : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    public Button startPauseButton; // Assign the start/pause button in the Unity Inspector

    private bool hasStarted = false; // Flag to track whether playback has started

    private void Start()
    {
        // Stop the media player initially
        mediaPlayer.Control.Stop();

        // Add a click event to the start/pause button
        startPauseButton.onClick.AddListener(ToggleMediaPlayer);
    }

    public void ToggleMediaPlayer()
    {
        if (!hasStarted)
        {
            // If playback hasn't started, play the media
            mediaPlayer.Control.Play();
            hasStarted = true;
        }
        else if (mediaPlayer.Control.IsPlaying())
        {
            // If the media player is playing, pause it
            mediaPlayer.Control.Pause();
        }
        else
        {
            // If the media player is paused or stopped, resume playing
            mediaPlayer.Control.Play();
        }
    }
}
