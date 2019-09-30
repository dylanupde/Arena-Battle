using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Com.Enceladus.PiratesArena
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("The prefab to use for representing the player")] public GameObject playerPrefab;

        public static GameManager Instance;
        [HideInInspector] public List<GameObject> gameObjectsToDestroy = new List<GameObject>();

        private ExitGames.Client.Photon.Hashtable playerScoresHashtable = new ExitGames.Client.Photon.Hashtable();

        PlayerManagerShip[] playerManagerShips;

        void Start()
        {
            Instance = this;
            playerManagerShips = new PlayerManagerShip[4];

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManagerShip.localPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 2f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        void Update()
        {

        }


        #region Photon Callbacks
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }




        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion

        #region Private Methods

        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("OI! PhotonNetwork: Trying to Load a level but we are not the master client :(");
            }
            Debug.LogFormat("Yo, PhotonNetwork: Loading level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                PhotonNetwork.LoadLevel("Room for 2");
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                PhotonNetwork.LoadLevel("Room for 3");
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount >= 4)
            {
                PhotonNetwork.LoadLevel("Room for 4");
            }
        }

        #endregion



        public void AddPlayerToHashTable(string inputName)
        {
            playerScoresHashtable[inputName] = 5;
        }
    }
}