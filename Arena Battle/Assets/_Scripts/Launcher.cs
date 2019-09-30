using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")] [SerializeField] private byte maxPlayersPerRoom = 4;
    [Tooltip("The Ui Panel to let the user enter name, connect and play")] [SerializeField] private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")] [SerializeField] private GameObject progressLabel;

    #endregion

    bool isConnecting;
    bool waitingForOtherPlayer;
    string gameVersion = "1"; // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).


    #region MonoBehaviour CallBacks
    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }


    void Update()
    {
        if (waitingForOtherPlayer && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // Critical: Load the Room Level.
            PhotonNetwork.LoadLevel("Room for 2");
        }
    }

    #endregion


    #region Public Methods
    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isConnecting = true;

        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // we check if we are connected or not. We join if we are, else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. 
            // If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion


    #region MonobehaviourPunCallbacks Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Yo dawg, PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

        if (isConnecting)
        {
            // #Critical: The first thing we try to do is to join a potential existing room. 
            // If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
        }
        
    }


    public override void OnDisconnected(DisconnectCause inputCause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("Oi! PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", inputCause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Yo! PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("Ey! PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        
        // Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We wait for another");
            waitingForOtherPlayer = true;
            progressLabel.GetComponent<Text>().text = "Waiting for other player...";
        }
    }

    #endregion

}
