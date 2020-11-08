using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class ChatManager : MonoBehaviourPun
{

    public enum ChatMessageType
    {
        PlayerMessage,
        NotificationMessage,
        GameMessage,
    }

    public Color messageColor = Color.white;
    public Color notificationColor = Color.yellow;

    public float secondsToDisplay = 3f;
    public float fadeSpeed = 1f;
    public GameObject content;
    public TMP_InputField inputField;
    public Button sendButton;
    public GameObject messagePrefab;
    public ScrollRect scrollRect;

    private CanvasGroup canvasGroup;
    private float alpha = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<PhotonView>() == null)
        {
            PhotonView photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = 1;
        }
        else
        {
            photonView.ViewID = 1;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = alpha;

        sendButton.onClick.AddListener(() => Submit());
        DeactivateChatInput();

        photonView.RPC("SendChat", RpcTarget.All, $"<b>{PhotonNetwork.NickName}</b> joined the game.", ChatManager.ChatMessageType.NotificationMessage);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (!IsChatting)
            {
                if(!IsDisplayingChat)
                    ActivateChat();
                ActivateChatInput();
            }
            else
            {
                Submit();
            }
        }

        if(Input.GetKeyUp(KeyCode.Escape) && IsChatting)
        {
            DeactivateChat();
            DeactivateChatInput();
        }
    }

    IEnumerator ChatDisplayTimer()
    {
        yield return new WaitForSeconds(secondsToDisplay);
        DeactivateChat();

    }

    IEnumerator FadeIn()
    {
        if (alpha >= 1f)
        {
            canvasGroup.alpha = 1f;
            yield return null;
        }
        for(float m_alpha = alpha; m_alpha < 1f; m_alpha += Time.deltaTime * fadeSpeed)
        {
            alpha = m_alpha;
            canvasGroup.alpha = m_alpha;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }


    IEnumerator FadeOut()
    {
        if (alpha <= 0f)
        {
            canvasGroup.alpha = 0f;
            yield return null;
        }
        for (float m_alpha = 1.0f; m_alpha > 0f; m_alpha -= Time.deltaTime * fadeSpeed)
        {
            alpha = m_alpha;
            canvasGroup.alpha = m_alpha;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    void Submit()
    { 
        string message = inputField.text;
        if (!string.IsNullOrWhiteSpace(message) && message.Length > 0)
        {
            photonView.RPC("SendChat", RpcTarget.All, $"<b>{PhotonNetwork.LocalPlayer.NickName}</b>: {message}", ChatMessageType.PlayerMessage);
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }


    [PunRPC]
    void SendChat(string message, ChatMessageType type)
    {
        GameObject newMessage = Instantiate(messagePrefab, content.transform);
        TextMeshProUGUI newMessageText = newMessage.GetComponent<TextMeshProUGUI>();

        newMessageText.text = message;
        newMessageText.color = messageColor;

        switch(type)
        {
            default:
            case ChatMessageType.PlayerMessage:
                newMessageText.color = messageColor;
                break;
            case ChatMessageType.NotificationMessage:
                newMessageText.color = notificationColor;
                break;
        }

        if (!IsDisplayingChat)
        {
            StartCoroutine(nameof(ChatDisplayTimer));
            ActivateChat();
        }
        else if(!IsChatting)
        {
            StopCoroutine(nameof(ChatDisplayTimer));
            StartCoroutine(nameof(ChatDisplayTimer));
        }
        StartCoroutine(PushToBottom());
    }

    IEnumerator PushToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scrollRect.transform);
    }

    private void ActivateChatInput()
    {
        StopCoroutine(nameof(ChatDisplayTimer));
        canvasGroup.interactable = true;
        inputField.gameObject.SetActive(true);
        sendButton.gameObject.SetActive(true);
        inputField.ActivateInputField();
        IsChatting = true;
    }

    private void ActivateChat()
    {
        StopCoroutine(nameof(FadeIn));
        StopCoroutine(nameof(FadeOut));
        StartCoroutine(nameof(FadeIn));
        IsDisplayingChat = true;
    }

    private void DeactivateChatInput()
    { 
        canvasGroup.interactable = false;
        inputField.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
        inputField.DeactivateInputField();
        IsChatting = false;
    }

    private void DeactivateChat()
    {
        StopCoroutine(nameof(FadeIn));
        StopCoroutine(nameof(FadeOut));
        StartCoroutine(nameof(FadeOut));
        IsDisplayingChat = false;
    }

    public bool IsChatting { get; private set; } = false;
    public bool IsDisplayingChat { get; private set; } = false;

}
