using System;
using Photon.Pun;
using RSG;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    Idle,
    Moving,
    Dodging,
    Dead
}

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviourPun
{
    public PlayerState state = PlayerState.Idle;
    public Text nameTag;

    public float speed = 5f;
    public float jumpSpeed = 2f;
    public float gravity = 9.81f;
    public float maxDodgeTime = 50f;
    public float dodgeSpeed = 2f;

    private CharacterController controller;
    private Vector3 moveDir = Vector3.zero;
    private float currentDodgeTime = 0;

    // Start is called beforz the first frame update
    void Start()
    {
        nameTag.text = photonView.Owner.NickName;
        controller = GetComponent<CharacterController>();
    }
    void Movement()
    {
        switch(state)
        {
            case PlayerState.Idle:
                moveDir.x = Input.GetAxis("Horizontal");

                if (controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                }
                moveDir.y -= gravity * Time.deltaTime;
                moveDir.z = Input.GetAxis("Vertical");

                if(moveDir != Vector3.zero)
                {
                    state = PlayerState.Moving;
                }
                break;
            case PlayerState.Moving:
                moveDir.x = Input.GetAxis("Horizontal");

                if (controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                }
                moveDir.y -= gravity * Time.deltaTime;
                moveDir.z = Input.GetAxis("Vertical");

                if (moveDir == Vector3.zero)
                {
                    state = PlayerState.Idle;
                }else
                {
                    if(Input.GetButtonDown("Fire2"))
                    {
                        state = PlayerState.Dodging;
                        currentDodgeTime = 0;
                        moveDir *= dodgeSpeed;
                        moveDir.y = 0;
                    }
                }
                break;
            case PlayerState.Dodging:
                if(currentDodgeTime < maxDodgeTime)
                {
                    currentDodgeTime += 0.1f;
                }
                else
                {
                    state = PlayerState.Idle;
                }
                break;
        }

        controller.Move(moveDir * speed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

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
        try
        {
            Instantiate(gun.GetComponent<Gun>().bullet, gun.transform.position, transform.rotation);
        }
        catch
        {
            // ignored
        }
    }

    private GameObject _getSelectedGun()
    {
        var selectedGun = transform.Find("GunHolder").gameObject.GetComponent<GunHolder>();
        return selectedGun.SelectedGun;
    }
}
