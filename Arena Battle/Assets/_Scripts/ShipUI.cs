using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipUI : MonoBehaviour
{
    PlayerManagerShip playerManagerShip;
    TextMeshPro textComponent;
    Slider healthSlider;
    Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerManagerShip = transform.parent.GetComponent<PlayerManagerShip>();
        textComponent = transform.GetChild(0).GetComponent<TextMeshPro>();
        healthSlider = transform.GetChild(1).GetComponent<Slider>();
        cameraTransform = Camera.main.transform;
        textComponent.text = playerManagerShip.photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        transform.forward = transform.position - cameraTransform.position;

        if (healthSlider != null)
        {
            healthSlider.value = playerManagerShip.health;
        }
        else
        {
            Debug.Log("Hey I don't have a health slider.", gameObject);
        }
    }
}
