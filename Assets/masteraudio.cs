using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class masteraudio : MonoBehaviour
{
    public MediaPlayer[] mediaPlayers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   public void stopOthers(MediaPlayer player)
    {
        foreach (MediaPlayer m in mediaPlayers)
        {
            if (m == player)
            {

            }
            else { m.Stop(); }
        }
    }
}
