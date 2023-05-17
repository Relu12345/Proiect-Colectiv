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

    public static uint selection;

    private void Start()
    {
        selection = 0;
        controller = GetComponent<charController>();
    }

    private void Update()
    {
        if (selection == 1 || Input.GetKey(W))
        {
            controller.MoveForward();
            selection = 0;
        }
        if (selection == 2 || Input.GetKey(S))
        {
            controller.MoveBackward();
            selection = 0;
        }
        if (selection == 3 || Input.GetKey(A))
        {
            controller.RotateLeft();
            selection = 0;
        }
        if (selection == 4 || Input.GetKey(D))
        {
            controller.RotateRight();
            selection = 0;
        }
    }
}
