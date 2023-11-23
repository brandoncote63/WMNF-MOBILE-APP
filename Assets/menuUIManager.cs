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
