using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Mango.Game
{

public class RoomController : MonoBehaviourPunCallbacks
{

    private static RoomController instance;

    public static RoomController Instance { get { return instance; } }

    // Objeto de jugador
    public GameObject playerPrefab;

    // Donde aparece el jugador
    public Vector3 spawnPoint = Vector3.zero;

    private string gameVersion;
        private bool isLoading;

    void Awake()
    {
        //Singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        gameVersion = Application.version;

        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Ocurrio un error al cargar la escena, volviendo a lobby.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Launcher");
            return;
        }

            // Iniciar al jugador sincronizadamente
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity, 0);

        }

    public void SetLoading(bool value)
    {
        isLoading = value;
    }

    void OnGUI()
    {
        if (isLoading)
            return;
        if (PhotonNetwork.CurrentRoom == null)
            return;

        if (GUI.Button(new Rect(5, 5, 125, 25), "Abandonar Partida"))
        {
            PhotonNetwork.LeaveRoom();
        }

        GUI.Label(new Rect(135, 5, 200, 25), PhotonNetwork.CurrentRoom.Name);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            string isMasterClient = (PhotonNetwork.PlayerList[i].IsMasterClient ? ": MasterClient" : "");
            GUI.Label(new Rect(5, 35 + 30 * i, 200, 25), PhotonNetwork.PlayerList[i].NickName + isMasterClient);
        }

        GUI.Box(new Rect(0, Screen.height - 50, 100, 50), "Version: " + gameVersion);


    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Launcher");
    }
}
}
