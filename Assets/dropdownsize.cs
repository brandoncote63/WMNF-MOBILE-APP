using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dropdownsize : MonoBehaviour
{
    public RectTransform mask;
    public GameObject content;
    public RectTransform contentrt;
   
    public GameObject buttons;
    public bool isdroped;
    public bool revers;
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rt = content.GetComponent<RectTransform>();
    }
    public void dropdownnow()
    {
        isdroped = true;
    }
    public void Updropdow()
    {
        isdroped = false;
    }
    public void rreset()
    {
        revers = true;
    }

    public void set()
    {
        if (revers)
        {
            content.GetComponent<reversOrder>().ReverseOrderOfChildren();
            revers = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
       
        if (content.transform.childCount == 1)
        {
            buttons.SetActive(false);
           
        }
      

        if (content.transform.childCount == 2)
        {
            buttons.SetActive(false);
            mask.sizeDelta = new Vector2(666.3f, contentrt.rect.height);
        }

        if (content.transform.childCount == 3)
        {
            buttons.SetActive(false);
            mask.sizeDelta = new Vector2(666.3f, contentrt.rect.height);
        }

        if (content.transform.childCount == 4)
        {
            buttons.SetActive(false);
            mask.sizeDelta = new Vector2(666.3f, contentrt.rect.height);
        }

        if (content.transform.childCount >= 5 && !isdroped)
        {
            set();
            buttons.SetActive(true);
            mask.sizeDelta = new Vector2(666.3f, 385f);
            
          
        }
        if (content.transform.childCount >= 5 && isdroped)
        {
            buttons.SetActive(true);
            mask.sizeDelta = new Vector2(666.3f, contentrt.rect.height) ;
        }
        
    }
}
