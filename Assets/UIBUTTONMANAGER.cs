using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBUTTONMANAGER : MonoBehaviour
{
    public Button[] PASUEBUTT;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PASUSE(int NUM)
    {
       foreach (Button I in PASUEBUTT)
        {
            if (I == PASUEBUTT[NUM]) { }
            else {

                if (I.isActiveAndEnabled) { I.onClick.Invoke();  }


               }
        }
    }
}
