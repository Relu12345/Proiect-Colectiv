using System.Collections;
using System.Collections.Generic;
using Unity.ItemRecever;
using UnityEngine;

[RequireComponent(typeof(charController))]
public class charInput : MonoBehaviour
{
    charController controller;

    KeyCode W = KeyCode.W;
    KeyCode A = KeyCode.A;
    KeyCode S = KeyCode.S;
    KeyCode D = KeyCode.D;

    private void Start()
    {
        UDPReceiver.msg = "";
        controller = GetComponent<charController>();
    }

    private void Update()
    {
        if (UDPReceiver.msg == "Up" || Input.GetKey(W))
        {
            UDPReceiver.msg = "";
            controller.MoveForward();
        }
        if (UDPReceiver.msg == "Down" || Input.GetKey(S))
        {
            UDPReceiver.msg = "";
            controller.MoveBackward();
        }
        if (UDPReceiver.msg == "Left" || Input.GetKey(A))
        {
            UDPReceiver.msg = "";
            controller.RotateLeft();
        }
        if (UDPReceiver.msg == "Right" || Input.GetKey(D))
        {
            UDPReceiver.msg = "";
            controller.RotateRight();
        }
    }
}
