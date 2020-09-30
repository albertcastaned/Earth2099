using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject follow;
    public float speed = 2.0f;
    public Vector3 offset = Vector3.one;


    void LateUpdate()
    {
        if (follow != null)
        {
            float interpolation = speed * Time.deltaTime;

            Vector3 position = transform.position;
            position.y = Mathf.Lerp(transform.position.y, follow.transform.position.y + offset.y, interpolation);
            position.x = Mathf.Lerp(transform.position.x, follow.transform.position.x + offset.x, interpolation);
            position.z = Mathf.Lerp(transform.position.z, follow.transform.position.z + offset.z, interpolation);

            transform.position = position;
        } 
    }
}
