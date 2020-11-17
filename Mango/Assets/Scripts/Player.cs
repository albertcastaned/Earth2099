using Mango.Game;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
public class Player : MonoBehaviourPun, IPunObservable
{

    [Header("Player stats")]
    public int maxHealth = 100;
    public float speed = 5f;
    public float jumpSpeed = 2f;
    public float gravity = 9.81f;
    public float maxDodgeTime = 50f;
    public float dodgeSpeed = 2f;

    [Header("HUD")]
    public Text nameTag;
    public GameObject loadingPanel;
    public Image barraVida;
    public Text lifeText;
    public DamagePopupText popupTextPrefab;

    [Header("Debug")]
    public bool DEBUG = false;

    private CharacterController controller;
    private Vector3 moveDir = Vector3.zero;
    private float currentDodgeTime = 0;
    private Vector3 startPos;
    private PlayerState state = PlayerState.Idle;
    private Camera m_camera;
    private Animator animator;
    private int health;
    private bool isLoading = false;
    public GameObject gunHolder;

    // Valores que deben sincronizados
    private int latestSelectedGun;
    // Start is called beforz the first frame update
    void Start()
    {
        health = maxHealth;
        startPos = transform.position;
        nameTag.text = DEBUG ? "Player" : photonView.Owner.NickName;
        controller = GetComponent<CharacterController>();
        m_camera = Camera.main;
        animator = GetComponent<Animator>();
        SetAnimation("isIdle", true);
        StartCoroutine("WaitForLoad");
    }



    public IEnumerator WaitForLoad()
    { 

        isLoading = true;
        loadingPanel.SetActive(true);
        yield return new WaitUntil(() => RoomController.Instance.isLoading == false);
        RoomController.Instance.SetLoading(false);
        loadingPanel.SetActive(false);
        isLoading = false;

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
                newDir = m_camera.transform.TransformDirection(moveDir.x, 0, moveDir.z);
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
                newDir = m_camera.transform.TransformDirection(moveDir.x, 0, moveDir.z);
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
        if (isLoading || state == PlayerState.Dead)
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
        if (!photonView.IsMine)
        {
            gunHolder.GetComponent<GunHolder>().SelectGun(latestSelectedGun);

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
    [PunRPC]
    public void ReduceHealth(int amount)
    {
        Debug.Log("Player receives damage");
        health -= amount;
        UpdateHealthUI();
        CreateFloatingText("-" + amount);
        if (health <= 0)
        {
            state = PlayerState.Dead;
        }
    }

    private void UpdateHealthUI()
    {
        barraVida.fillAmount = (float)health / maxHealth;
        lifeText.text = health.ToString() + " / " + maxHealth.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gunHolder.GetComponent<GunHolder>().selectedGunIndex);
            stream.SendNext(health);
            stream.SendNext(state);
        }
        else
        {
            int oldHealth = health;

            latestSelectedGun = (int)stream.ReceiveNext();
            health = (int)stream.ReceiveNext();
            state = (PlayerState)stream.ReceiveNext();

            if (health != oldHealth)
                UpdateHealthUI();
        }
    }

    public int Health { get { return health; } }

    public bool IsAlive { get { return state != PlayerState.Dead; } }

    private void CreateFloatingText(string text)
    {
        DamagePopupText instance = Instantiate(popupTextPrefab, transform);

        instance.SetText(text);
    }

    public PlayerState State {  get { return state; } }

    [PunRPC]
    public void Shoot()
    {
        var gun = _getSelectedGun();
        var shooter = gun.GetComponent<ProjectileGun>();
        if (shooter != null)
        {
            shooter.Shoot();
        }
    }
}
