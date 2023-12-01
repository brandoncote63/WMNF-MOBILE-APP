using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STARTOFF : MonoBehaviour
{

    public GameObject HOLDER1;
    public GameObject HOLDER2;
    public GameObject HOLDER3;
    public GameObject HOLDER4;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("one", .2f);
    }

    public void one()
    {
        HOLDER1.SetActive(true);
        Invoke("two", .2f);

    }
    public void two()
    {
        HOLDER2.SetActive(true);
        Invoke("three", .2f);
    }
    public void three()
    {
        HOLDER3.SetActive(true);
        Invoke("four", .2f);
    }
    public void four()
    {
        HOLDER4.SetActive(true);
    }

}
