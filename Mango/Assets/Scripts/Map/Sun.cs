using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public float speed = 10f;

    private Material skyboxRuntime;
    // Start is called before the first frame update
    void Start()
    {
        skyboxRuntime = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.right, speed * Time.deltaTime);
        transform.LookAt(Vector3.zero);
        skyboxRuntime.SetFloat("_Rotation", Time.time * 1f);

    }
}
