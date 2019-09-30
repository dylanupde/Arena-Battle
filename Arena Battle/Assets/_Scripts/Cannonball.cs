using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cannonball : MonoBehaviourPun
{
    [SerializeField] GameObject explosionPrefab;

    void OnCollisionEnter(Collision collision)
    {
        GameObject newExplosion = PhotonNetwork.Instantiate(explosionPrefab.name, collision.transform.position, Quaternion.identity, 0);
        newExplosion.SetActive(true);
        ////PhotonNetwork.Destroy(gameObject);
        //gameObject.SetActive(false);
    }
}
