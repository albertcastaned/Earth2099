using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviourPun
{
    public Text nameTag;

    public float speed = 5f;
    public float jumpForce = 2.0f;

    private Rigidbody rb;
    private bool isOnGround;

    // Start is called before the first frame update
    void Start()
    {
        nameTag.text = photonView.Owner.NickName;
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay()
    {
        if (!isOnGround && rb.velocity.y == 0)
        {
            isOnGround = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.position += new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isOnGround = false;
        }

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (ground.Raycast(cameraRay, out rayLength))
        {
            Vector3 playerAimDirection = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, playerAimDirection, Color.blue);

            transform.LookAt(
                new Vector3(playerAimDirection.x, transform.position.y, playerAimDirection.z)
            );
        }
    }
    
    
    /**
     * TODO: fix the starting position of the bullet, in order to look like going out of the gun.
     */
    [PunRPC]
    private void FireProjectile()
    {
        var gun = _getSelectedGun();
        Instantiate(gun.GetComponent<Gun>().bullet, gun.transform.position, transform.rotation);
    }

    private GameObject _getSelectedGun()
    {
        var selectedGun = transform.Find("GunHolder").gameObject.GetComponent<GunHolder>();
        return selectedGun.SelectedGun;
    }
}
