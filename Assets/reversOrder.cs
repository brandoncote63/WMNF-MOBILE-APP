using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reversOrder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReverseOrderOfChildren()
    {
        for (var i = 0; i < transform.childCount - 1; i++)
        {
            
            transform.GetChild(1).SetSiblingIndex(transform.childCount - 1 - i);
        }
    }
}
