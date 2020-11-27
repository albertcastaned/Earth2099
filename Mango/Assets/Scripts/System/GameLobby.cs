using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviourPunCallbacks
{
    int maximosJugadores;
    string maximosJugadoresInput;
    string playerName = "Player";

    // Los jugadores seran separados por version. Es importante cambiar la version cuando se cambia el codigo para no tener errores en instancias por diferente codigo.
    string gameVersion;

    List<RoomInfo> createdRooms = new List<RoomInfo>();

    string roomName = "Lobby 1";
    Vector2 roomListScroll = Vector2.zero;
    bool joiningRoom = false;
    bool joiningDebugRoom = false;

    public GUISkin skin;
    float margin = 5f;

    // Start is called before the first frame update
    void Start()
    {
        gameVersion = Application.version;
        maximosJugadoresInput = "4";

        Debug.Log("Version: " + gameVersion);
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
        GUI.skin = skin;
        GUI.Window(0, new Rect(margin, margin, Screen.width - margin * 2, Screen.height - margin * 2), LobbyWindow, "Lobby");
    }

    bool RoomNameAvailable()
    {
        foreach(RoomInfo room in createdRooms)
        {
            if (room.Name == roomName)
                return false;
        }
        return true;
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

        roomName = GUILayout.TextField(roomName, GUILayout.Width(200));
        GUILayout.Label("Max players: ");

        maximosJugadoresInput = GUILayout.TextField(maximosJugadoresInput, GUILayout.Width(100));

        if (GUILayout.Button("Crear Partida", GUILayout.Width(125)))
        {
            if (roomName != "" && playerName != "" && RoomNameAvailable() && int.TryParse(maximosJugadoresInput, out maximosJugadores))
            {
                joiningRoom = true;

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.IsOpen = true;
                roomOptions.IsVisible = true;
                roomOptions.MaxPlayers = (byte)maximosJugadores;

                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
            }
        }
        /*
        # if UNITY_EDITOR
        if (GUILayout.Button("Entrar a debug"))
        {

                joiningRoom = true;
                joiningDebugRoom = true;

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.IsOpen = true;
                roomOptions.IsVisible = true;
                roomOptions.MaxPlayers = (byte)maximosJugadores;
                
                PhotonNetwork.JoinOrCreateRoom("DEBUG-" + CreateRandomString(), roomOptions, TypedLobby.Default);
            
        }
        #endif
        */
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
                if (createdRooms[i].MaxPlayers <= 0)
                    continue;
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
        if (GUILayout.Button("Costumizar personaje"))
        {
            SceneManager.LoadScene("ChangeColor");
        }

        GUILayout.Label("Version: " + gameVersion + "\nRegion: " + PhotonNetwork.CloudRegion, GUILayout.Width(85));


        GUILayout.FlexibleSpace();

        GUI.enabled = (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby || PhotonNetwork.NetworkClientState == ClientState.Disconnected) && !joiningRoom;
        if (GUILayout.Button("Cerrar Sesion", GUILayout.Width(100)))
        {
            Firebase.SignOut();
        }
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
        if (joiningDebugRoom)
            PhotonNetwork.LoadLevel("DebugLevel");
        else
            PhotonNetwork.LoadLevel("Level");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Se unio a la partida exitosamente");
    }
    private string CreateRandomString(int stringLength = 10)
    {
        int _stringLength = stringLength - 1;
        string randomString = "";
        string[] characters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        for (int i = 0; i <= _stringLength; i++)
        {
            randomString = randomString + characters[Random.Range(0, characters.Length)];
        }
        return randomString;
    }
}
