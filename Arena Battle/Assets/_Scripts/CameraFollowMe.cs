using Com.Enceladus.PiratesArena;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraFollowMe : MonoBehaviourPun
{
    Transform selfieStickTransform;
    float rotateSpeed = 60f;
    Vector3 selfieStickOffset;

    // Start is called before the first frame update
    void Start()
    {
        selfieStickOffset = new Vector3(0f, 6f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        if (selfieStickTransform == null) selfieStickTransform = Camera.main.transform.parent;

        selfieStickTransform.position = transform.position + selfieStickOffset;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            selfieStickTransform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            selfieStickTransform.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f, Space.World);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            selfieStickTransform.Rotate(rotateSpeed * Time.deltaTime, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            selfieStickTransform.Rotate(-rotateSpeed * Time.deltaTime, 0f, 0f);
        }
    }
}
