using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject follow;
    public float speed = 2.0f;
    public float turnSpeed = 5f;
    public float smoothFactor = 0.5f;

    public float zoomSpeed = 0.2f;
    public float zoomMin = 10f;
    public float zoomMax = 50f;
    private float zoom = 10f;
    
    private Vector3 offset;
    

    void Start()
    {
        offset = transform.position - follow.transform.position;
    }
    void LateUpdate()
    {
        if (follow != null)
        {

            if (Input.GetMouseButton(1))
            {
                Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up);
                camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right);

                float scroll = Input.GetAxis("Mouse ScrollWheel");
                zoom += Input.GetAxis("Mouse ScrollWheel");

                if (zoom > zoomMax)
                {
                    zoom = zoomMax;
                    scroll = 0;
                }

                if(zoom < zoomMin)
                {
                    zoom = zoomMin;
                    scroll = 0;
                }

                offset = camTurnAngle * offset;
                print(zoom);
                offset += transform.forward * scroll * zoomSpeed;
                
            }


            Vector3 newPos = follow.transform.position + offset;

            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);

            transform.LookAt(follow.transform);
        }
    }


}
