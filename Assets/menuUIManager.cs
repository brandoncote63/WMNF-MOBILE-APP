using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class menuUIManager : MonoBehaviour
{
    public Image[] image;
    public Color color;

    public void page(int i)
    {
        foreach (Image w in image)
        {
            w.color = color;
        }
        image[i].color = Color.white;
    }
}
