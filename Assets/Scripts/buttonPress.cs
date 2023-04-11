using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.ItemRecever;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonPress : MonoBehaviour
{
    public Button button1, button2, button3;

    void Start()
    {
        UDPReceiver.msg = "";
    }

    void Update()
    {
        if (UDPReceiver.msg == "Up")
        {
            UDPReceiver.msg = "";
            button1.onClick.Invoke();
            gameObject.SetActive(false);
        }
        if (UDPReceiver.msg == "Down")
        {
            UDPReceiver.msg = "";
            button2.onClick.Invoke();
            gameObject.SetActive(false);
        }
        if (button3 != null)
        {
            if (UDPReceiver.msg == "Button")
            {
                UDPReceiver.msg = "";
                button3.onClick.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}
