using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class matchsize : MonoBehaviour
{
    public RectTransform text;
    public RectTransform bubble;
    public float EI;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("NOW", .2F);
        
        
        
    }
    public void NOW()
    {
        EI = text.rect.width + 30;
        bubble.sizeDelta = new Vector2(EI, 39.6F);

    }

   
}
