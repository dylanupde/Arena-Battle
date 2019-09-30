using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Com.Enceladus.PiratesArena;

public class Ship : MonoBehaviourPun
{
    [SerializeField] GameObject cannonBallPrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float bouyantForceMin = 40000f;
    [SerializeField] float bouyantForceMax = 60000f;
    [SerializeField] float speed = 10f;
    [SerializeField] float turnSpeed = 10000f;
    [SerializeField] float straightenTorqueSide = 1;
    [SerializeField] float straightenTorqueFront = 20;
    [SerializeField] float airDrag = 0.1f;
    [SerializeField] float tiltSpeed = 3f;
    [SerializeField] float cannonLaunchSpeed = 20f;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] float explosiveForce = 1f;
    [SerializeField] float damageTakenPerHit = 0.2f;
    float bouyantForceCurrent;

    Transform[] cannonTransforms;

    Rigidbody rigidBody;
    Transform inWaterCheckerTransform;
    Transform drownedChecker;
    GameManager gameManager;
    PlayerManagerShip playerManagerShip;
    string reloadMethodString = "Reload";
    float xRotToAdd = 0f;
    bool reloaded = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        inWaterCheckerTransform = transform.GetChild(0);
        drownedChecker = transform.GetChild(1);
        playerManagerShip = GetComponent<PlayerManagerShip>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        cannonTransforms = new Transform[transform.GetChild(2).childCount];
        for (int i = 0; i < cannonTransforms.Length; i++)
        {
            cannonTransforms[i] = transform.GetChild(2).GetChild(i);
        }

        rigidBody.centerOfMass = new Vector3(-0.4f, 2f, 0);
    }

    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true) return;

        if (Input.GetKey(KeyCode.Space) && reloaded)
        {
            foreach (Transform thisCannon in cannonTransforms)
            {
                GameObject newCannonball = PhotonNetwork.Instantiate(cannonBallPrefab.name, thisCannon.position, Quaternion.identity, 0);
                newCannonball.SetActive(true);
                newCannonball.GetComponent<Rigidbody>().velocity = thisCannon.forward * cannonLaunchSpeed;

                reloaded = false;
                Invoke(reloadMethodString, reloadTime);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true) return;

        float xRotConverted = transform.localEulerAngles.x;
        if (xRotConverted > 180) xRotConverted -= 360;
        float zRotConverted = transform.localEulerAngles.z;
        if (zRotConverted > 180) zRotConverted -= 360;


        if (inWaterCheckerTransform.position.y < 0f)
        {
            rigidBody.drag = 1f;

            float t = Mathf.InverseLerp(inWaterCheckerTransform.position.y, drownedChecker.position.y, 0f);
            bouyantForceCurrent = Mathf.Lerp(bouyantForceMin, bouyantForceMax, t);

            rigidBody.AddForce(0f, bouyantForceCurrent, 0f);
        }
        else
        {
            // Turn off drag so we fall normally
            rigidBody.drag = airDrag;
            //rigidBody.velocity += Vector3.up * -9.81f * Time.fixedDeltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            rigidBody.AddForce(transform.right * speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.AddTorque(-Vector3.up * turnSpeed);

            // To lean, we'll just pretend the x-rotation is different than it is. Hehehe, stupid Unity.
            xRotToAdd -= tiltSpeed * Time.fixedDeltaTime;
            xRotToAdd = Mathf.Clamp(xRotToAdd, -7f, 7f);
            xRotConverted += xRotToAdd;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidBody.AddTorque(Vector3.up * turnSpeed);

            // To lean, we'll just pretend the x-rotation is different than it is. Hehehe, stupid Unity.
            xRotToAdd += tiltSpeed * Time.fixedDeltaTime;
            xRotToAdd = Mathf.Clamp(xRotToAdd, -7f, 7f);
            xRotConverted += xRotToAdd;
        }
        else
        {
            if (xRotToAdd > 0f) xRotToAdd = Mathf.Clamp(xRotToAdd - (tiltSpeed * Time.fixedDeltaTime), 0f, 7f);
            if (xRotToAdd < 0f) xRotToAdd = Mathf.Clamp(xRotToAdd + (tiltSpeed * Time.fixedDeltaTime), 0f, -7f);
        }

        if (Input.GetKey(KeyCode.S))
        {
            rigidBody.AddForce(-transform.right * speed * 0.5f);
        }

        rigidBody.AddTorque(-transform.right * xRotConverted * straightenTorqueSide);
        rigidBody.AddTorque(-transform.forward * zRotConverted * straightenTorqueFront);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true) return;

        print(collision.gameObject.name + " hit me");

        // If it's a cannonball...
        if (collision.transform.GetComponent<Cannonball>())
        {
            print("registered that I was hit by a ball");
            rigidBody.AddForceAtPosition(collision.transform.GetComponent<Rigidbody>().velocity.normalized * explosiveForce, collision.transform.position);
            GameObject newExplosion = PhotonNetwork.Instantiate(explosionPrefab.name, collision.transform.position, Quaternion.identity, 0);
            newExplosion.SetActive(true);
            gameManager.gameObjectsToDestroy.Add(collision.transform.gameObject);
            collision.transform.position = Vector3.down * 1000f;

            playerManagerShip.health -= damageTakenPerHit;
        }
    }



    private void Reload()
    {
        reloaded = true;
    }
}
