using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipJar : MonoBehaviour
{
    public void tip()
    {
        Application.OpenURL("https://www.wmnf.org/support/donate");
    }
}
