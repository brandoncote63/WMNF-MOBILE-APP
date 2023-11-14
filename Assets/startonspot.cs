using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startonspot : MonoBehaviour
{
    public GameObject refrecs;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = refrecs.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
