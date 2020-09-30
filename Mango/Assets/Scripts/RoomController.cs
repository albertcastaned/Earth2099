using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomController : MonoBehaviourPunCallbacks
{
    // Objeto de jugador
    public GameObject playerPrefab;

    // Donde aparece el jugador
    public Transform spawnPoint;

    private string gameVersion = "0.0.0";

    // Use this for initialization
    void Start()
    {
        // Recupera ultima tag de Git como version
        try
        {
            gameVersion = Git.BuildVersion;
        }
        catch (GitException)
        {
            gameVersion = "0.0.0";

        }

        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Ocurrio un error al cargar la escena, volviendo a lobby.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby");
            return;
        }

        // Iniciar al jugador sincronizadamente
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);
    }

    void OnGUI()
    {
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
