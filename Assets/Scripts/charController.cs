using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class charController : MonoBehaviour
{

    public float transitionSpeed = 3f, transitionRotationSpeed = 3f;

    Vector3 targetGridPos, targetRotation;

    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector3 targetPosition = targetGridPos;

        if (targetRotation.y > 270f && targetRotation.y < 361f) targetRotation.y = 0f;
        if (targetRotation.y < 0f) targetRotation.y = 270f;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * transitionRotationSpeed);
    }

    public void RotateLeft() { if (AtRest) targetRotation -= Vector3.up * 90f; }
    public void RotateRight() { if (AtRest) targetRotation += Vector3.up * 90f; }
    public void MoveForward() { if (AtRest) targetGridPos += transform.forward * 2; }
    public void MoveBackward() { if (AtRest) targetGridPos -= transform.forward * 2; }
 

    bool AtRest
    {
        get
        {
            return (Vector3.Distance(transform.position, targetGridPos) < 0.05f) && (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.05f);
        }
    }
}
