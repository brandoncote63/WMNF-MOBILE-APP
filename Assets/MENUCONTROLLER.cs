using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MENUCONTROLLER : MonoBehaviour
{
    public GameObject[] RED;
    public GameObject[] GREY;
    public GameObject[] views;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MUSIC()
    {
        views[0].SetActive(true);
        views[1].SetActive(false);
        views[2].SetActive(false);
        views[3].SetActive(false);
        views[4].SetActive(false);
        views[5].SetActive(false);
        

        RED[0].SetActive(true);
        RED[1].SetActive(false);
        RED[2].SetActive(false);
        RED[3].SetActive(false);
        RED[4].SetActive(false);
        RED[5].SetActive(false);
       

        GREY[0].SetActive(false);
        GREY[1].SetActive(true);
        GREY[2].SetActive(true);
        GREY[3].SetActive(true);
        GREY[4].SetActive(true);
        GREY[5].SetActive(true);
        
    }
    public void ARCHIVES()
    {
        views[0].SetActive(false);
        views[1].SetActive(true);
        views[2].SetActive(false);
        views[3].SetActive(false);
        views[4].SetActive(false);
        views[5].SetActive(false);

        RED[0].SetActive(false);
        RED[1].SetActive(true);
        RED[2].SetActive(false);
        RED[3].SetActive(false);
        RED[4].SetActive(false);
        RED[5].SetActive(false);


        GREY[0].SetActive(true);
        GREY[1].SetActive(false);
        GREY[2].SetActive(true);
        GREY[3].SetActive(true);
        GREY[4].SetActive(true);
        GREY[5].SetActive(true);

    }

    public void NEWS()
    {
        views[0].SetActive(false);
        views[1].SetActive(false);
        views[2].SetActive(true);
        views[3].SetActive(false);
        views[4].SetActive(false);
        views[5].SetActive(false);


        RED[0].SetActive(false);
        RED[1].SetActive(false);
        RED[2].SetActive(true);
        RED[3].SetActive(false);
        RED[4].SetActive(false);
        RED[5].SetActive(false);


        GREY[0].SetActive(true);
        GREY[1].SetActive(true);
        GREY[2].SetActive(false);
        GREY[3].SetActive(true);
        GREY[4].SetActive(true);
        GREY[5].SetActive(true);

    }
    public void Events()
    {
        views[0].SetActive(false);
        views[1].SetActive(false);
        views[2].SetActive(false);
        views[3].SetActive(true);
        views[4].SetActive(false);
        views[5].SetActive(false);

        RED[0].SetActive(false);
        RED[1].SetActive(false);
        RED[2].SetActive(false);
        RED[3].SetActive(true);
        RED[4].SetActive(false);
        RED[5].SetActive(false);


        GREY[0].SetActive(true);
        GREY[1].SetActive(true);
        GREY[2].SetActive(true);
        GREY[3].SetActive(false);
        GREY[4].SetActive(true);
        GREY[5].SetActive(true);

    }
    public void Scedual()

    {
        views[0].SetActive(false);
        views[1].SetActive(false);
        views[2].SetActive(false);
        views[3].SetActive(false);
        views[4].SetActive(true);
        views[5].SetActive(false);

        RED[0].SetActive(false);
        RED[1].SetActive(false);
        RED[2].SetActive(false);
        RED[3].SetActive(false);
        RED[4].SetActive(true);
        RED[5].SetActive(false);


        GREY[0].SetActive(true);
        GREY[1].SetActive(true);
        GREY[2].SetActive(true);
        GREY[3].SetActive(true);
        GREY[4].SetActive(false);
        GREY[5].SetActive(true);

    }
    public void Support()


    {
        views[0].SetActive(false);
        views[1].SetActive(false);
        views[2].SetActive(false);
        views[3].SetActive(false);
        views[4].SetActive(false);
        views[5].SetActive(true);


        RED[0].SetActive(false);
        RED[1].SetActive(false);
        RED[2].SetActive(false);
        RED[3].SetActive(false);
        RED[4].SetActive(false);
        RED[5].SetActive(true);


        GREY[0].SetActive(true);
        GREY[1].SetActive(true);
        GREY[2].SetActive(true);
        GREY[3].SetActive(true);
        GREY[4].SetActive(true);
        GREY[5].SetActive(false);

    }
}
