using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class AVProVideoAudioPlayer : MonoBehaviour
{
    public MediaPlayer[] mediaPlayers; // An array of MediaPlayer components for your 8 different streams
    public AudioSource[] audioSources;


    private void Start()
    {
        // Stop all media players initially
      //  foreach (var player in mediaPlayers)
      //  {
       //     if (player != null)
       //     {
       //         player.Control.Stop();
       //     }
        //}
    }

    public void StartMediaPlayer(int playerIndex)
    {
        PlayOnlyOne(10);
        // Ensure the playerIndex is within bounds
        if (playerIndex >= 0 && playerIndex < mediaPlayers.Length)
        {
            // Stop all media players except the one specified by playerIndex
            for (int i = 0; i < mediaPlayers.Length; i++)
            {
                if (i != playerIndex)
                {
                    mediaPlayers[i].Control.Stop();
                }
            }

            // Start the specified media player
            mediaPlayers[playerIndex].Control.Play();
        }
        else
        {
            Debug.LogError("Invalid playerIndex: " + playerIndex);
        }
    }

    public void StopMediaPlayer(int playerIndex)
    {
        // Ensure the playerIndex is within bounds
        if (playerIndex >= 0 && playerIndex < mediaPlayers.Length)
        {
            // Stop the specified media player
            mediaPlayers[playerIndex].Control.Stop();
        }
        else
        {
            Debug.LogError("Invalid playerIndex: " + playerIndex);
        }
    }

    public void PlayOnlyOne(int audioNum)
    {
        for (int i = 0; i < mediaPlayers.Length; i++)
        {
            
            
                mediaPlayers[i].Control.Stop();
            
        }
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (i != audioNum)
            {
                audioSources[i].Stop();
            }
        }

    }
}
