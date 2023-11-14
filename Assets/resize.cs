using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resize : MonoBehaviour
{
    public RectTransform content;
    public RectTransform mask;
    public int childcount;
    public float hieght;
    public float sizeOfElement;
    public int OFFSET;
    public int OFFSET2;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("setHight", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setHight()
    {
       
        childcount = content.transform.childCount;

        hieght = (childcount * sizeOfElement) + (215 + OFFSET); 

        content.sizeDelta = new Vector2(0, hieght);

        if (mask != null)
        {
            mask.sizeDelta = new Vector2(mask.rect.width, (childcount * sizeOfElement) + OFFSET2);

        }
       
    }


}
