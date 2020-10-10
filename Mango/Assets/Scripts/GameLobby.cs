using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using MangoVersioning;

public class GameLobby : MonoBehaviourPunCallbacks
{
    public int maximosJugadores = 4;

    string playerName = "Player";

    // Los jugadores seran separados por version. Es importante cambiar la version cuando se cambia el codigo para no tener errores en instancias por diferente codigo.
    string gameVersion = "0.0.0";

    List<RoomInfo> createdRooms = new List<RoomInfo>();

    string roomName = "Lobby 1";
    Vector2 roomListScroll = Vector2.zero;
    bool joiningRoom = false;



    // Start is called before the first frame update
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
        //////////////////////////////////////////

        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Error de conexion a Photon. Codigo de Error: " + cause.ToString() + " Conexion de Servidor: " + PhotonNetwork.ServerAddress);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conexion a servidor exitoso");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Se obtuvo la lista de lobbies");
        createdRooms = roomList;
    }

    void OnGUI()
    {
        GUI.Window(0, new Rect(Screen.width / 2 - 450, Screen.height / 2 - 200, 900, 400), LobbyWindow, "Lobby");
    }

    void LobbyWindow(int index)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Estado: " + PhotonNetwork.NetworkClientState);

        if (joiningRoom || !PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
        {
            GUI.enabled = false;
        }

        GUILayout.FlexibleSpace();

        roomName = GUILayout.TextField(roomName, GUILayout.Width(250));

        if (GUILayout.Button("Crear Partida", GUILayout.Width(125)))
        {
            if (roomName != "" && playerName != "")
            {
                joiningRoom = true;

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.IsOpen = true;
                roomOptions.IsVisible = true;
                roomOptions.MaxPlayers = (byte)maximosJugadores;

                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
            }
        }

        GUILayout.EndHorizontal();

        roomListScroll = GUILayout.BeginScrollView(roomListScroll, true, true);

        if (createdRooms.Count == 0)
        {
            GUILayout.Label("No se encontraron partidas disponibles.");
        }
        else
        {
            for (int i = 0; i < createdRooms.Count; i++)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(createdRooms[i].Name, GUILayout.Width(400));
                GUILayout.Label(createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Entrar a partida"))
                {
                    joiningRoom = true;
                    PhotonNetwork.NickName = playerName;
                    PhotonNetwork.JoinRoom(createdRooms[i].Name);
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Nombre de jugador: ", GUILayout.Width(85));
        playerName = GUILayout.TextField(playerName, GUILayout.Width(250));

 
        GUILayout.Label("Version: " + gameVersion, GUILayout.Width(85));


        GUILayout.FlexibleSpace();

        GUI.enabled = (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby || PhotonNetwork.NetworkClientState == ClientState.Disconnected) && !joiningRoom;
        if (GUILayout.Button("Actualizar", GUILayout.Width(100)))
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby(TypedLobby.Default);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        GUILayout.EndHorizontal();

        if (joiningRoom)
        {
            GUI.enabled = true;
            GUI.Label(new Rect(900 / 2 - 50, 400 / 2 - 10, 100, 20), "Conectando...");
        }
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Ocurrio un error al crear la partida. Esto puede pasar si existe una partida con el mismo nombre. Intente con otro nombre.");
        joiningRoom = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Ocurrio un error al unirse a la partida. Esto puede pasar si la partida se cerro o se lleno antes de entrar.");
        joiningRoom = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Ocurrio un error al unirse a la partida. Esto puede pasar si la partida se cerro o se lleno antes de entrar.");
        joiningRoom = false;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Se creo la partida exitosamente");
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.LoadLevel("Level");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Se unio a la partida exitosamente");
    }
}
