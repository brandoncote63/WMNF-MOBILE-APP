using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;

public class masteraudio : MonoBehaviour
{
    public MediaPlayer[] mediaPlayers;
    public Button[] listenPausebuttons;
    public GameObject subpageArkive;
    public GameObject subpageScedule;
    public bool onlistenpage;
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
                Debug.Log("WE HAVE LANDED3");
                
            }
            else
            {
                m.Stop();
                
            }
        }
       


    }
    public void fixpasuetomatch()
    {
        foreach (Button b in listenPausebuttons)
        {
            Debug.Log("was not active in hhhhhhhhhi");
            
            

                b.onClick.Invoke();
                Debug.Log("WE HAVE LANDED");
            
            
        }
    }
}
