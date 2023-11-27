using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class menuUIManager : MonoBehaviour
{
    public Image[] image;
    public Color color;
    public Button[] buttons;
    public GameObject IOS;
    public GameObject ANDR;
    public void Start()
    {
#if UNITY_IPHONE

           IOS.SetActive(true);
        ANDR.SetActive(false);
#endif
#if UNITY_ANDROID
        IOS.SetActive(false);
        ANDR.SetActive(true);
#endif
    }
    public void page(int i)
    {
     
        foreach(Button b in buttons)
        {
            b.onClick.Invoke();
        }

        foreach (Image w in image)
        {
            w.color = color;
        }
        image[i].color = Color.white;
    }
}
