using Mango.Game;
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
    [Header("Prefabs")]
    public GameObject bullet;


    [Header("Player stats")]
    public float speed = 5f;
    public float jumpSpeed = 2f;
    public float gravity = 9.81f;
    public float maxDodgeTime = 50f;
    public float dodgeSpeed = 2f;

    [Header("HUD")]
    public Text nameTag;
    public GameObject loadingPanel;
    [Header("Debug")]
    public bool DEBUG = false;


    private CharacterController controller;
    private Vector3 moveDir = Vector3.zero;
    private float currentDodgeTime = 0;
    private Vector3 startPos;
    private bool isLoaded = false;
    private PlayerState state = PlayerState.Idle;

    // Start is called beforz the first frame update
    void Start()
    {
        startPos = transform.position;
        nameTag.text = DEBUG ? "Player" : photonView.Owner.NickName;
        controller = GetComponent<CharacterController>();
        if(DEBUG)
        {
            isLoaded = true;
        }
        else
        {
            StartCoroutine(WaitForLoad());
        }
    }

    IEnumerator WaitForLoad()
    {
        loadingPanel.SetActive(true);
        RoomController.Instance.SetLoading(true);
        yield return new WaitForSeconds(2f);
        isLoaded = true;
        RoomController.Instance.SetLoading(false);
        loadingPanel.SetActive(false);
    }
    void Movement()
    {
        Vector3 newDir;

        switch (state)
        {
            case PlayerState.Idle:
                moveDir.x = Input.GetAxis("Horizontal");
                moveDir.z = Input.GetAxis("Vertical");
                newDir = Camera.main.transform.TransformDirection(moveDir.x, 0, moveDir.z);
                moveDir.x = newDir.x;
                moveDir.z = newDir.z;
                if (controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                }

                if(!controller.isGrounded)
                    moveDir.y -= gravity * Time.deltaTime;

                if(moveDir != Vector3.zero)
                {
                    state = PlayerState.Moving;
                }
                break;
            case PlayerState.Moving:
                moveDir.x = Input.GetAxis("Horizontal");
                moveDir.z = Input.GetAxis("Vertical");
                newDir = Camera.main.transform.TransformDirection(moveDir.x, 0, moveDir.z);
                moveDir.x = newDir.x;
                moveDir.z = newDir.z;
                if (controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                }
                if (!controller.isGrounded)
                    moveDir.y -= gravity * Time.deltaTime;

                if (moveDir == Vector3.zero)
                {
                    state = PlayerState.Idle;
                }else
                {
                    if(Input.GetKeyDown(KeyCode.LeftShift))
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
        if (!isLoaded)
            return;
        Movement();

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (ground.Raycast(cameraRay, out rayLength))
        {
            Vector3 playerAimDirection = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, playerAimDirection, Color.blue);

            transform.LookAt(new Vector3(playerAimDirection.x, transform.position.y, playerAimDirection.z));

            if (Input.GetButtonDown("Fire1"))
            {
                if(DEBUG)
                {
                    FireProjectile();
                }
                else
                {
                    photonView.RPC("FireProjectile", RpcTarget.All);

                }
            }
        }
      CheckStillOnMap();


    }
    [PunRPC]
    private void FireProjectile()
    {
        Instantiate(bullet, transform.position, transform.rotation);
    }

    private void CheckStillOnMap()
    {
        if(transform.position.y < -50f)
        {
            transform.position = new Vector3(startPos.x, startPos.y + 5f, startPos.z);
        }
    }
}
