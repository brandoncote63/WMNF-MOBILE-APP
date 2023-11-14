using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonSunController : MonoBehaviour
{
    public GameObject[] pages;
    public GameObject[] boldtext;
    public GameObject[] text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDay(int day)
    {
        foreach(GameObject i in pages)
        {
            i.SetActive(false);

        }
        foreach (GameObject i in boldtext)
        {
            i.SetActive(false);

        }
        foreach (GameObject i in text)
        {
            i.SetActive(true);

        }

        text[day].SetActive(false);
        pages[day].SetActive(true);
        boldtext[day].SetActive(true);
    }
}
