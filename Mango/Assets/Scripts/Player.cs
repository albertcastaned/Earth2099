using System;
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
    Jumping,
    Dodging,
    Dead
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAnimator))]
public class Player : MonoBehaviourPun
{

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
    private new Camera camera;
    private Animator animator;

    private float fps = 0f, timer, refresh;
    // Start is called beforz the first frame update
    void Start()
    {
        startPos = transform.position;
        nameTag.text = DEBUG ? "Player" : photonView.Owner.NickName;
        controller = GetComponent<CharacterController>();
        camera = Camera.main;
        animator = GetComponent<Animator>();
        SetAnimation("isIdle", true);
        if (DEBUG)
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


    void ResetAnimationParameters()
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }
    }
    void SetAnimation(string parameter, bool value)
    {

        ResetAnimationParameters();
        animator.SetBool(parameter, value);
    }

    void Movement()
    {
        Vector3 newDir;

        switch (state)
        {
            case PlayerState.Idle:
                moveDir.x = Input.GetAxis("Horizontal");
                moveDir.z = Input.GetAxis("Vertical");
                newDir = camera.transform.TransformDirection(moveDir.x, 0, moveDir.z);
                moveDir.x = newDir.x;
                moveDir.z = newDir.z;
                if (controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                    state = PlayerState.Jumping;
                    SetAnimation("isJumping", true);

                }
                if (!controller.isGrounded)
                    moveDir.y -= gravity * Time.deltaTime;



                if (moveDir.x != 0f || moveDir.z != 0f)
                {
                    state = PlayerState.Moving;
                    SetAnimation("isRunning", true);

                }
                break;
            case PlayerState.Jumping:
                if (!controller.isGrounded)
                    moveDir.y -= gravity * Time.deltaTime;
                else
                {
                    if (moveDir.x == 0f && moveDir.z == 0f)
                    {
                        state = PlayerState.Idle;
                        SetAnimation("isIdle", true);

                    }
                    else
                    {
                        state = PlayerState.Moving;
                        SetAnimation("isRunning", true);
                    }
                }
                break;

            case PlayerState.Moving:
                moveDir.x = Input.GetAxis("Horizontal");
                moveDir.z = Input.GetAxis("Vertical");
                newDir = camera.transform.TransformDirection(moveDir.x, 0, moveDir.z);
                moveDir.x = newDir.x;
                moveDir.z = newDir.z;
                if (controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                    state = PlayerState.Jumping;
                    SetAnimation("isJumping", true);

                }
                if (!controller.isGrounded)
                    moveDir.y -= gravity * Time.deltaTime;

                if (moveDir.x == 0f && moveDir.z == 0f)
                {
                    state = PlayerState.Idle;
                    SetAnimation("isIdle", true);

                }
                else
                {
                    if(Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        state = PlayerState.Dodging;
                        currentDodgeTime = 0;
                        moveDir *= dodgeSpeed;
                        moveDir.y = 0;

                        SetAnimation("isDashing", true);

                    }
                }
                break;
            case PlayerState.Dodging:
                if (controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                    state = PlayerState.Jumping;
                    SetAnimation("isJumping", true);

                }

                if (!controller.isGrounded)
                    moveDir.y -= gravity * Time.deltaTime;
                if (currentDodgeTime < maxDodgeTime)
                {
                    currentDodgeTime += 0.1f;
                }
                else
                {
                    if (moveDir.x == 0f && moveDir.z == 0f)
                    {
                        state = PlayerState.Moving;
                        SetAnimation("isRunning", true);
                    }
                    else
                    {
                        state = PlayerState.Idle;
                        SetAnimation("isIdle", true);
                    }
                }
                break;
        }
        controller.Move(moveDir * speed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFPS();
        if (!isLoaded)
            return;
        Movement();
        CheckStillOnMap();


        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (ground.Raycast(cameraRay, out rayLength))
        {
            Vector3 playerAimDirection = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, playerAimDirection, Color.blue);

            if (state != PlayerState.Dodging)
            {
                transform.LookAt(
                    new Vector3(playerAimDirection.x, transform.position.y, playerAimDirection.z)
                );
            }

        }


    }

    void CalculateFPS()
    {
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) fps = (int)(1f / timelapse);

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

    private void CheckStillOnMap()
    {
        if(transform.position.y < -50f)
        {
            transform.position = new Vector3(startPos.x, startPos.y + 5f, startPos.z);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 300, 5, 200, 25), state.ToString());
        GUI.Label(new Rect(Screen.width - 300, 50, 200, 25), "FPS: " + fps);


    }
}
