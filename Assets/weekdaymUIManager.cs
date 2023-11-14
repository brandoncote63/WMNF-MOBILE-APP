using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class weekdaymUIManager : MonoBehaviour
{
    public TextMeshProUGUI[] text;
    public GameObject[] DAY;
    public Color color;


    private void Start()
    {
        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Monday)
        {
            weekday(0);
        }
        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Tuesday)
        {
            weekday(1);
        }
        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Wednesday)
        {
            weekday(2);
        }
        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Thursday)
        {
            weekday(3);
        }
        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Friday)
        {
            weekday(4);
        }
        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Saturday)
        {
            weekday(5);
        }
        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Sunday)
        {
            weekday(6);
        }
        DAY[1].SetActive(true);
    }
    public void weekday(int i)
    {
         foreach (TextMeshProUGUI t in text)
        {
            t.color = color;
        }
        foreach (GameObject I in DAY)
        {
            I.SetActive(false);
        }
        text[i].color = Color.white;
        DAY[i].SetActive(true);
    }
}
