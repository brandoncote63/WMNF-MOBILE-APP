using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class partscript : MonoBehaviour
{
    public RectTransform part;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onplay()
    {
        part.sizeDelta = new Vector2(659.0627f, 153);
        slider.gameObject.SetActive(true);
    }
    public void onpause()
    {
        part.sizeDelta = new Vector2(659.0627f, 79.52582f);
        slider.gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
