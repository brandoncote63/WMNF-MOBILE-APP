using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos;

public class listenpagelogic : MonoBehaviour
{
    public GameObject playbuuton;
    public GameObject pausebutton;
    
    public MediaPlayer[] mediaPlayers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Stopped()
    {
        foreach (var player in mediaPlayers)
        {
           if (player != null)
           {
               player.Control.Stop();
          }
      }
    }

    public void playbroadcast(string url)
    {
        playbuuton.SetActive(false);
        pausebutton.SetActive(true);
        mediaPlayers[0].OpenMedia(new MediaPath(url, MediaPathType.AbsolutePathOrURL), autoPlay: true);
    }
}
