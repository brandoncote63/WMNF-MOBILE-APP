using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class invoke : MonoBehaviour
{
    public float time;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        button = b;
        Invoke("now", time);
    }

   public void now()
    {
        button.onClick.Invoke();
    }
}
