using Photon.Pun;
using UnityEngine;

public class Gun : MonoBehaviourPun
{
    public GameObject bullet;

    // Update is called once per frame
    void Update()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (ground.Raycast(cameraRay, out rayLength))
        {
            if (Input.GetButtonDown("Fire1") && gameObject.activeSelf)
            {
                photonView.RPC("FireProjectile", RpcTarget.All);
            }
        }
    }
}
