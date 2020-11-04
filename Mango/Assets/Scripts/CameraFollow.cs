using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform centerPoint;
    public Transform follow;
    public float speed = 2.0f;
    public float turnSpeed = 5f;
    public float smoothFactor = 0.5f;

    public float zoomSpeed = 2f;
    public float zoomMin = 5f;
    public float zoomMax = 30f;
    private float zoom = 15f;

    private float mouseX, mouseY = -30f;


    void Start()
    {
        transform.localPosition = new Vector3(0, 0, -20f);

    }
    void LateUpdate()
    {
        if (follow != null)
        {

            zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);

            transform.localPosition = new Vector3(0, 0, zoom);

            if(Input.GetMouseButton(1))
            {
                mouseX += Input.GetAxis("Mouse X") * turnSpeed;
                mouseY -= Input.GetAxis("Mouse Y") * turnSpeed;
            }
            mouseY = Mathf.Clamp(mouseY, -60f, 0f);
            transform.LookAt(centerPoint);
            centerPoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);

            centerPoint.position = new Vector3(follow.position.x, follow.position.y, follow.position.z);
        }
    }


}
