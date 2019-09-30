using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerManagerShip : MonoBehaviourPunCallbacks, IPunObservable
{
    GameManager gameManager;
    public float health = 1f;

    [HideInInspector] public static GameObject localPlayerInstance;

    // Start is called before the first frame update
    void Awake()
    {
        if (photonView.IsMine)
        {
            localPlayerInstance = gameObject;
        }

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            // Respawn and add to score
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(health);
        }
        else
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
        }
    }
}
