using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraColorConfig : MonoBehaviour
{
    public Transform centerPoint;

    public float speed = 2.0f;
    public float turnSpeed = 5f;
    public float smoothFactor = 0.5f;
    private float mouseX, mouseY = -30f;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X") * turnSpeed;
            mouseY -= Input.GetAxis("Mouse Y") * turnSpeed;
        }
        transform.LookAt(centerPoint);
        centerPoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);

        centerPoint.position = Vector3.zero;
    }
}
