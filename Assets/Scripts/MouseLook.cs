using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    // sensitivity za hitrost rotacije
    public float xSensitivity = 5000f;
    public float ySensitivity = 5000f;

    public Transform player;

    float xRotation = 0f;

    void Start() {
        // zaklenemo misko in jo skrijemo
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        // dobimo input od miske
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;

        // GorDol rotacija (moramo clampati da ne pogledamo v nazaj)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotiramo kamero z pridobljeno GorDol rotacijo
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // rotiramo playerja z LevoDesno rotacijo
        player.Rotate(Vector3.up * mouseX);
    }
}
