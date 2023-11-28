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
    public Image[] imageANDROID;
    public GameObject logoIOS;
    public GameObject logoANDROID;
    public Color color;
    public bool waited;
    public float timeDelay;

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

        //#if UNITY_IPHONE


        //#endif
#if UNITY_ANDROID

        for (int i = 0; i < image.Length; i++)
        {
            image[i] = imageANDROID[i];
        }
        logoIOS.SetActive(false);
        logoANDROID.SetActive(true);
#endif

        foreach (GameObject i in pages) { i.SetActive(false); }


        pages[0].SetActive(true);
        image[0].color = Color.white;



    }

    public void delay()
    {
        waited = true;
    }
  

    public void ListenPage()
    {
        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages) { i.SetActive(false); }
            pages[0].SetActive(true);
            Iconcolor(0);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("ListenPage", timeDelay);
        }

       
        
    }
    public void Playlist()

    {
        
        if (waited)
        {
            waited = false;

            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }

            pages[1].SetActive(true);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("Playlist", timeDelay);
        }


    }
    public void Upnext()
    {
        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }

            pages[2].SetActive(true);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("Upnext", timeDelay);
        }


       
    }
    public void Archive()
    {

        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }
            content[0].SetActive(true);
            pages[3].SetActive(true);
            Iconcolor(1);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("Archive", timeDelay);
        }


       
    }
    public void ArchiveSub()
    {

        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }
            pages[4].SetActive(true);

            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("ArchiveSub", timeDelay);
        }


        
    }
    public void News()
    {
        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }
            Iconcolor(2);
            pages[5].SetActive(true);
            content[1].SetActive(true);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("News", timeDelay);
        }


     
    }
    public void NewsSub()
    {
        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }
            foreach (GameObject g in content) { g.SetActive(true); }
            pages[6].SetActive(true);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("NewsSub", timeDelay);
        }

      
    }
    public void Events()
    {
        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }
            pages[7].SetActive(true);
            Iconcolor(3);
            content[2].SetActive(true);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("Events", timeDelay);
        }


       
    }
    public void EventsSub()
    {
        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }

            pages[8].SetActive(true);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("EventsSub", timeDelay);
        }

       
    }
    public void Scedual()
    {
        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }

            pages[9].SetActive(true);
            Iconcolor(4);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("Scedual", timeDelay);
        }

      
    }
    public void ScedualSub()
    {
        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }

            pages[10].SetActive(true);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("ScedualSub", timeDelay);
        }

      
    }
    public void Donate()
    {

        if (waited)
        {
            waited = false;
            foreach (GameObject i in pages)
            {
                if (i == pages[0]) { }
                else { i.SetActive(false); }
            }

            pages[11].SetActive(true);
            Iconcolor(5);
            Invoke("delay", timeDelay);
        }
        else
        {
            Invoke("Donate", timeDelay);
        }

       
    }
}
