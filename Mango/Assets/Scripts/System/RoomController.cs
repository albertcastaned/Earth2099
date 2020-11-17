using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

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

        public BakeRuntime bakeRuntime;


        private string gameVersion;
        public bool isLoading = true;


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
            if (PhotonNetwork.PlayerList.Length > 1)
                CheckDuplicateName();

            // Iniciar al jugador sincronizadamente
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity, 0);
            player.name = PhotonNetwork.NickName;

            if (!PhotonNetwork.CurrentRoom.Name.StartsWith("DEBUG-"))
                StartCoroutine(nameof(Load));
            else
                isLoading = false;
        }

        IEnumerator Load()
        {
            isLoading = true;
            yield return new WaitUntil(() => bakeRuntime.loaded);
            isLoading = false;
        }


        private void CheckDuplicateName()
        {

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName)
                    PhotonNetwork.NickName += i;

            }
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
                string isMasterClient = (PhotonNetwork.PlayerList[i].IsMasterClient ? ": Host" : "");
                GUI.Label(new Rect(5, 35 + 30 * i, 200, 25), PhotonNetwork.PlayerList[i].NickName + isMasterClient);

            }

        }
        public override void OnLeftRoom()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Launcher");
        }
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            PhotonView.Find(1).RPC("SendChat", RpcTarget.All, $"<b>{otherPlayer.NickName}</b> left the game.", ChatManager.ChatMessageType.NotificationMessage);

            base.OnPlayerLeftRoom(otherPlayer);
        }


        public void Spawn(string prefabName, Vector3 position)
        {

            PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
        }

    }
}

