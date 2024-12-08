using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HotkeyCantClickLayer : MonoBehaviour
{
    [SerializeField] private GameObject cantclicklayer1;

    private float disabletime = 0.05f;
    private float disabletimer;

    private DateTime startdate;
    private DateTime currentdate;
    private float seconds;
    private void OnEnable()
    {
        Keybindinputmanager.disablecantclicklayer += startcountdown;
    }
    private void OnDisable()
    {
        Keybindinputmanager.disablecantclicklayer -= startcountdown;
    }

    private void startcountdown()
    {
        StartCoroutine("disablelayer");
    }
    IEnumerator disablelayer()
    {
        startdate = DateTime.Now;
        disabletimer = 0f;
        while (disabletimer < disabletime)
        {
            currentdate = DateTime.Now;
            seconds = currentdate.Ticks - startdate.Ticks;
            disabletimer = seconds * 0.0000001f;
            yield return null;
        }
        cantclicklayer1.SetActive(false);
    }
}
