using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class partscript : MonoBehaviour
{
    public RectTransform part;
    public RectTransform playbuttonHome;
    public RectTransform playButton;
    public RectTransform playbuttonDown;
    public RectTransform pausebuttonHome;
    public RectTransform PauseButton;
    public RectTransform pausebuttonDown;
    public GameObject timetotal;
    public GameObject timecurrent;
   
    public GameObject ff;
    public GameObject rewid;
    public Slider slider;
    public float x = 659.0627f;
    public float y1= 153;
    public float y2 = 79.52582f;
    
    // Start is called before the first frame update
    void Start()
    {
        onpause();
        
    }
    public void onplay()
    {
        
        part.sizeDelta = new Vector2(x, y1);
        rewid.SetActive(true);
        ff.SetActive(true);
        slider.gameObject.SetActive(true);
        timecurrent.SetActive(true);
        timetotal.SetActive(true);
        playButton.sizeDelta = playbuttonDown.sizeDelta;
        playButton.position = playbuttonDown.position;
        PauseButton.sizeDelta = pausebuttonDown.sizeDelta;
        PauseButton.position = pausebuttonDown.position;
       
    }
    public void onpause()
    {
       
        
            part.sizeDelta = new Vector2(x, y2);
            ff.SetActive(false);
            rewid.SetActive(false);
            slider.gameObject.SetActive(false);
            timecurrent.SetActive(false);
            timetotal.SetActive(false);
            playButton.sizeDelta = playbuttonHome.sizeDelta;
            playButton.position = playbuttonHome.position;
            PauseButton.sizeDelta = pausebuttonHome.sizeDelta;
            PauseButton.position = pausebuttonHome.position;
        

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
