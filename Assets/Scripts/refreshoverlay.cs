using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class refreshoverlay : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform vlayoutgroup;

    void Start()
    {
        InvokeRepeating("now", .2f, .2f);
    }

    // Update is called once per frame
    void now()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(vlayoutgroup);
    }
}
