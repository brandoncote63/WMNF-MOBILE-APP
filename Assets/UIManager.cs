using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameObject[] pages;
    public GameObject[] content;
    public Image[] image;
    public Color color;

    public void Iconcolor(int i)
    {
        foreach (Image w in image)
        {
            w.color = color;
        }
        image[i].color = Color.white;
    }

    // Start is called before the first frame update
    void Start()
    {
  
        foreach (GameObject i in pages) { i.SetActive(false); }


        pages[0].SetActive(true);
        image[0].color = Color.white;



    }


  

    public void ListenPage()
    {
        foreach (GameObject i in pages) { i.SetActive(false);}

        pages[0].SetActive(true);
        Iconcolor(0);
        foreach (GameObject g in content) { g.SetActive(true); }
    }
    public void Playlist()
    {
        foreach (GameObject i in pages) { if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        foreach (GameObject g in content) { g.SetActive(true); }
        pages[1].SetActive(true);
    }
    public void Upnext()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        foreach (GameObject g in content) { g.SetActive(true); }
        pages[2].SetActive(true);
    }
    public void Archive()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        foreach (GameObject g in content) { g.SetActive(true); }
        pages[3].SetActive(true);
        Iconcolor(1);
    }
    public void ArchiveSub()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        pages[4].SetActive(true);
        foreach (GameObject g in content) { g.SetActive(true); }
    }
    public void News()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        Iconcolor(2);
        pages[5].SetActive(true);
        foreach (GameObject g in content) { g.SetActive(true); }
    }
    public void NewsSub()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        foreach (GameObject g in content) { g.SetActive(true); }
        pages[6].SetActive(true);
    }
    public void Events()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        pages[7].SetActive(true);
        Iconcolor(3);
        foreach (GameObject g in content) { g.SetActive(true); }
    }
    public void EventsSub()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        foreach (GameObject g in content) { g.SetActive(true); }
        pages[8].SetActive(true);
    }
    public void Scedual()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        foreach (GameObject g in content) { g.SetActive(true); }
        pages[9].SetActive(true);
        Iconcolor(4);
    }
    public void ScedualSub()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        foreach (GameObject g in content) { g.SetActive(true); }
        pages[10].SetActive(true);
    }
    public void Donate()
    {
        foreach (GameObject i in pages)
        {
            if (i == pages[0]) { }
            else { i.SetActive(false); }
        }
        foreach (GameObject g in content) { g.SetActive(true); }
        pages[11].SetActive(true);
        Iconcolor(5);
    }
}
