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
public class Player : MonoBehaviourPun, IPunObservable
{

    [Header("Player stats")]
    public int maxHealth = 100, health;
    public float speed = 5f;
    public float jumpSpeed = 2f;
    public float gravity = 9.81f;
    public float maxDodgeTime = 50f;
    public float dodgeSpeed = 2f;

    [Header("HUD")]
    public Text nameTag;
    public GameObject loadingPanel;
    public GameObject settingsMenu;
    public GameObject gameOverPanel;
    public PartyHealth partyHealth;
    public Image barraVida;
    public TMP_Text lifeText;
    public Text scoreLabel;
    public DamagePopupText popupTextPrefab;
    public ChatManager chatManager;
    public TrailRenderer trailRenderrer;

    private CharacterController controller;
    private Vector3 moveDir = Vector3.zero;
    private float currentDodgeTime = 0;
    private Vector3 startPos;
    private PlayerState state = PlayerState.Idle;
    private Camera m_camera;
    private Animator animator;

    private bool isLoading = false;
    public GameObject gunHolder;
    private GunHolder gunHolderScript;
    private AudioManager audioManager;

    private DebugController debugController;

    private bool invincible;


    private Coroutine walkSoundCoroutine;

    // Valores que deben sincronizados
    private int latestSelectedGun;
    // Start is called beforz the first frame update
    void Start()
    {
        health = maxHealth;
        startPos = transform.position;
        nameTag.text = photonView.Owner.NickName;
        controller = GetComponent<CharacterController>();
        debugController = GetComponent<DebugController>();
        audioManager = GetComponent<AudioManager>();
        m_camera = Camera.main;
        animator = GetComponent<Animator>();
        SetAnimation("isIdle", true);
        StartCoroutine("WaitForLoad");
        UpdateHealthUI();
        gunHolderScript = gunHolder.GetComponent<GunHolder>();
        photonView.RPC(nameof(SetupPartyHealthBars), RpcTarget.AllBufferedViaServer);
        StartCoroutine(PlayRepeatingWalkSoundEffect());
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

    void ApplyGravity()
    {
        if (!controller.isGrounded)
            moveDir.y -= gravity * Time.deltaTime;
    }

    void SetDirection()
    {
        if (!CanMove)
        {
            moveDir.x = 0;
            moveDir.z = 0;
            return;
        }
        Vector3 newDir;
        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.z = Input.GetAxis("Vertical");
        newDir = m_camera.transform.TransformDirection(moveDir.x, 0, moveDir.z);
        moveDir.x = newDir.x;
        moveDir.z = newDir.z;
    }
    void Movement()
    {

        switch (state)
        {
            case PlayerState.Idle:
                SetDirection();
                if (CanMove && controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                    state = PlayerState.Jumping;
                    SetAnimation("isJumping", true);
                }



                if (moveDir.x != 0f || moveDir.z != 0f)
                {
                    state = PlayerState.Moving;
                    SetAnimation("isRunning", true);
                }
                break;
            case PlayerState.Jumping:
                if (controller.isGrounded)
                {
                    if (moveDir.x == 0f && moveDir.z == 0f)
                    {
                        state = PlayerState.Idle;
                        SetAnimation("isIdle", true);
                        trailRenderrer.emitting = false;
                    }
                    else
                    {
                        state = PlayerState.Moving;
                        SetAnimation("isRunning", true);
                        trailRenderrer.emitting = false;
                    }
                    audioManager.Play("Step");
                }
                break;

            case PlayerState.Moving:
                SetDirection();

                if (CanMove && controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                    state = PlayerState.Jumping;
                    SetAnimation("isJumping", true);

                }


                if (moveDir.x == 0f && moveDir.z == 0f)
                {
                    state = PlayerState.Idle;
                    SetAnimation("isIdle", true);
                }
                else
                {
                    if (CanMove && Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        state = PlayerState.Dodging;
                        currentDodgeTime = 0;
                        moveDir *= dodgeSpeed;
                        moveDir.y = 0;
                        trailRenderrer.emitting = true;
                        SetAnimation("isDashing", true);
                        audioManager.Play("Dodge");
                        photonView.RPC(nameof(PlayAudioRPC), RpcTarget.Others, "Dodge");
                    }
                }
                break;
            case PlayerState.Dodging:
                if (CanMove && controller.isGrounded && Input.GetButton("Jump"))
                {
                    moveDir.y = jumpSpeed;
                    state = PlayerState.Jumping;
                    SetAnimation("isJumping", true);
                    audioManager.Stop("Step");
                }


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
                    trailRenderrer.emitting = false;
                }
                break;
        }
        ApplyGravity();

        controller.Move(moveDir * speed * Time.deltaTime);
    }

    [PunRPC]
    void PlayAudioRPC(string clipName)
    {
        audioManager.Play(clipName);
    }

    IEnumerator PlayRepeatingWalkSoundEffect(float time = 0.3f)
    {
        while (true)
        {
            if ((moveDir.x != 0 || moveDir.z != 0) && controller.isGrounded)
            {
                if (state == PlayerState.Dodging)
                {
                    time = 0.1f;
                }
                else
                {
                    time = 0.3f;
                }
                audioManager.Play("Step");
                photonView.RPC(nameof(PlayAudioRPC), RpcTarget.Others, "Step");

            }
            yield return new WaitForSeconds(time);
        }

    }

    public bool IsChatting { get { return chatManager.IsChatting; } }
    public bool CanMove { get { return !isLoading && state != PlayerState.Dead && !IsChatting && !debugController.showConsole && !settingsMenu.activeInHierarchy; } }

    // Update is called once per frame
    void Update()
    {

        CheckSettingsPressed();
        CheckCanUseWeapon();
        if (state == PlayerState.Dead)
        {
            moveDir = Vector3.zero;
            return;
        }
        Movement();
        CheckStillOnMap();

        if (!CanMove)
            return;

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(cameraRay, out float rayLength))
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

    void CheckSettingsPressed()
    {
        if (!IsChatting && Input.GetKeyDown(KeyCode.Escape))
        {
            settingsMenu.SetActive(!settingsMenu.activeInHierarchy);
        }
    }

    void CheckCanUseWeapon()
    {
        if (gunHolderScript.SelectedGun == null)
            return;
        if(CanMove)
        {
            gunHolderScript.SelectedGun.SetActive(true);
        }
        else
        {
            gunHolderScript.SelectedGun.SetActive(false);
        }
    }


    void CheckDead()
    {
        if (health <= 0)
        {
            state = PlayerState.Dead;
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
    private void CreateFloatingText(string text, bool heal = false)
    {
        DamagePopupText instance = Instantiate(popupTextPrefab, transform);
        if(heal)
        {
            instance.damageText.color = Color.green;
        }
        instance.SetText(text);
    }

    [PunRPC]
    void SetupPartyHealthBars()
    {
        partyHealth.UpdateHealth(photonView.Owner, health, maxHealth);
    }

    [PunRPC]
    public void ReduceHealth(int amount)
    {
        if (invincible)
            return;
        health -= amount;
        UpdateHealthUI();
        CreateFloatingText("-" + amount);
        partyHealth.UpdateHealth(photonView.Owner, health, maxHealth);
        if (health <= 0)
        {
            state = PlayerState.Dead;
            animator.Play("Dead");
        }
    }

    [PunRPC]
    public void IncreaseHealth(int amount)
    {
        health += amount;

        if (maxHealth < health)
            health = maxHealth;

        UpdateHealthUI();
        CreateFloatingText("+" + amount, true);

        partyHealth.UpdateHealth(photonView.Owner, health, maxHealth);
    }

    public void UpdateHealthUI()
    {
        barraVida.fillAmount = (float)health / maxHealth;
        lifeText.text = health.ToString() + " / " + maxHealth.ToString();
        if (health <= 50 && health >= 31)
        {
            barraVida.color = new Color32(38, 143, 205, 255);
        }
        else if (health <= 30 && health >= 16)
        {
            barraVida.color = new Color32(60, 166, 228, 255);
        }
        else if (health <= 15 && health >= 6)
        {
            barraVida.color = new Color32(146, 213, 252, 255);
        }
        else if (health <= 5 && health > 0)
        {
            barraVida.color = new Color32(177, 223, 250, 255);
        }
        else if (health <= 0)
        {
            gameOverPanel.SetActive(true);
            lifeText.text = "";
        }
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(maxHealth);
            stream.SendNext(gunHolderScript.selectedGunIndex);
            stream.SendNext(state);
            stream.SendNext(trailRenderrer.emitting);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            maxHealth = (int)stream.ReceiveNext();
            latestSelectedGun = (int)stream.ReceiveNext();
            state = (PlayerState)stream.ReceiveNext();
            trailRenderrer.emitting = (bool)stream.ReceiveNext();
        }
    }

    public int Health { get { return health; } }

    public bool IsAlive { get { return state != PlayerState.Dead; } }


    public PlayerState State {  get { return state; } }
    public void Revive() { state = PlayerState.Idle; }

    public void ToggleInvincible()
    {
        invincible = !invincible;
    }

    public void aumentarBalas()
    {
        GameObject gun = _getSelectedGun();
        ProjectileGun scriptGun = gun.GetComponent<ProjectileGun>();
        scriptGun.moreBullets();
    }
}
