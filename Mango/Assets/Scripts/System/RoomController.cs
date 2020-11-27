using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine.UI;

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

        public int currentEnemies = 0;
        
        public int score = 0;
        private Text _scoreLabel;
        private GameObject _playerHolder;


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
                Debug.Log("Ocurrió un error al cargar la escena, volviendo a lobby.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Launcher");
                return;
            }


            // Iniciar al jugador sincronizadamente
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity, 0);
            player.name = PhotonNetwork.NickName;
            _playerHolder = player.transform.Find("PlayerHolder").gameObject;
            _scoreLabel = _playerHolder.GetComponent<Player>().scoreLabel;

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

        public void SetLoading(bool value)
        {
            isLoading = value;
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

        
        public void IncreaseCurrentEnemiesCount()
        {
            currentEnemies++;
        }

        public void DecreaseCurrentEnemiesCount()
        {
            currentEnemies--;
        }

        public GameObject Spawn(string prefabName, Vector3 position)
        {
            return PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
        }

        public void IncreaseScore(int points)
        {
            if (_playerHolder.GetPhotonView().IsMine)
            {
                score += points;  
            }
            if (_scoreLabel != null)
            {
                _scoreLabel.text = $"Score: {score:000}";   
            }
        }
    }
}

