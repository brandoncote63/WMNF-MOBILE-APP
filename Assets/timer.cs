using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject holder;
    public int secounds =5;
       
    void Start()
    {
        Invoke("now", secounds);
    }

   public void now()
    {
        holder.SetActive(true);
    }
}
